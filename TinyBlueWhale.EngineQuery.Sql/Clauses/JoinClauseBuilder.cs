using System.Linq.Expressions;
using TinyBlueWhale.EngineQuery.Core.Enums;
using TinyBlueWhale.EngineQuery.Core.ExpressionScopes;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions.Sources;
using TinyBlueWhale.EngineQuery.Sql.Compilation;
using TinyBlueWhale.EngineQuery.Sql.ExpressionsParsing;
using TinyBlueWhale.EngineQuery.Sql.Helpers;
using TinyBlueWhale.EngineQuery.Sql.Interfaces;

namespace TinyBlueWhale.EngineQuery.Sql.Clauses
{
    /// <summary>
    /// Builds SQL JOIN clauses from query join definitions.
    /// </summary>
    /// <remarks>
    /// This builder supports inner and left joins using equality-based join expressions.
    /// </remarks>
    public sealed class JoinClauseBuilder : IOptionalSqlClauseBuilder
    {
        /// <summary>
        /// Determines whether JOIN clauses should be built.
        /// </summary>
        /// <param name="queryDefinition">
        /// Query definition to inspect.
        /// </param>
        /// <returns>
        /// <see langword="true"/> when join definitions are configured; otherwise, <see langword="false"/>.
        /// </returns>
        public bool CanBuild(CompiledQueryDefinition queryDefinition)
        {
            ArgumentNullException.ThrowIfNull(queryDefinition);

            return queryDefinition.JoinDefinitions.Count > 0;
        }

        /// <summary>
        /// Builds all SQL JOIN clauses configured in the query definition.
        /// </summary>
        /// <param name="queryDefinition">
        /// Query definition that contains join metadata.
        /// </param>
        /// <param name="context">
        /// Current SQL compilation context.
        /// </param>
        /// <returns>
        /// SQL JOIN clause text.
        /// </returns>
        public string Build(CompiledQueryDefinition queryDefinition, QueryCompilationContext context)
        {
            ArgumentNullException.ThrowIfNull(queryDefinition);
            ArgumentNullException.ThrowIfNull(context);

            var joinClauses = queryDefinition.JoinDefinitions
                .Select(joinDefinition => BuildJoinClause(joinDefinition, context));

            return string.Join(Environment.NewLine, joinClauses);
        }

        // Builds a single JOIN clause.
        private static string BuildJoinClause(QueryJoinDefinition joinDefinition, QueryCompilationContext context)
        {
            var joinKeyword = joinDefinition.JoinType switch
            {
                QueryJoinType.Inner => "INNER JOIN",
                QueryJoinType.Left => "LEFT JOIN",
                _ => throw new NotSupportedException($"Join type '{joinDefinition.JoinType}' is not supported.")
            };

            var joinSource = joinDefinition.JoinSource;

            if (string.IsNullOrWhiteSpace(joinSource.TableName))
                throw new InvalidOperationException("JOIN source does not define a table name.");

            if (string.IsNullOrWhiteSpace(joinSource.TableAlias))
                throw new InvalidOperationException("JOIN source does not define a table alias.");

            var tableReference = SqlIdentifierHelper.BuildTableReference(context.DatabaseDialect, joinSource.TableName, joinSource.SchemaName);
            var tableAlias = context.DatabaseDialect.EscapeIdentifier(joinSource.TableAlias);
            var joinCondition = BuildJoinCondition(joinDefinition, context);

            return $"{joinKeyword} {tableReference} AS {tableAlias} ON {joinCondition}";
        }

        // Builds the complete JOIN condition.
        private static string BuildJoinCondition(QueryJoinDefinition joinDefinition, QueryCompilationContext context)
        {
            var expressionScope = CreateExpressionScope(joinDefinition);

            return BuildJoinExpression(joinDefinition.JoinExpression.Body, expressionScope, context);
        }

        // Creates the expression scope associated with the JOIN lambda parameters.
        private static QueryExpressionScope CreateExpressionScope(QueryJoinDefinition joinDefinition)
        {
            var parameters = joinDefinition.JoinExpression.Parameters;

            if (parameters.Count != 2)
                throw new InvalidOperationException("JOIN expressions must define exactly two source parameters.");

            var expressionScope = new QueryExpressionScope();

            expressionScope.Register(parameters[0], joinDefinition.Source);
            expressionScope.Register(parameters[1], joinDefinition.JoinSource);

            return expressionScope;
        }

        // Builds a JOIN boolean expression.
        private static string BuildJoinExpression(Expression expression, QueryExpressionScope expressionScope, QueryCompilationContext context)
        {
            expression = SqlComputedExpressionParser.UnwrapConvertExpression(expression);

            if (expression is not BinaryExpression binaryExpression)
                throw new NotSupportedException($"Join expression '{expression}' is not supported.");

            if (binaryExpression.NodeType is ExpressionType.AndAlso or ExpressionType.OrElse)
            {
                var left = BuildJoinExpression(binaryExpression.Left, expressionScope, context);
                var right = BuildJoinExpression(binaryExpression.Right, expressionScope, context);

                var sqlOperator = binaryExpression.NodeType == ExpressionType.AndAlso
                    ? "AND"
                    : "OR";

                return $"({left} {sqlOperator} {right})";
            }

            var comparisonOperator = ResolveJoinComparisonOperator(binaryExpression.NodeType);
            var leftColumn = BuildJoinColumnReference(binaryExpression.Left, expressionScope, context);
            var rightColumn = BuildJoinColumnReference(binaryExpression.Right, expressionScope, context);

            return $"({leftColumn} {comparisonOperator} {rightColumn})";
        }

        // Resolves the SQL comparison operator associated with a JOIN expression.
        private static string ResolveJoinComparisonOperator(ExpressionType expressionType)
        {
            return expressionType switch
            {
                ExpressionType.Equal => "=",
                ExpressionType.NotEqual => "<>",
                ExpressionType.GreaterThan => ">",
                ExpressionType.GreaterThanOrEqual => ">=",
                ExpressionType.LessThan => "<",
                ExpressionType.LessThanOrEqual => "<=",
                _ => throw new NotSupportedException($"Join operator '{expressionType}' is not supported.")
            };
        }

        // Builds a qualified column reference from the source bound to the expression parameter.
        private static string BuildJoinColumnReference(Expression expression, QueryExpressionScope expressionScope, QueryCompilationContext context)
        {
            expression = SqlComputedExpressionParser.UnwrapConvertExpression(expression);

            if (expression is not MemberExpression memberExpression)
                throw new NotSupportedException($"Join expression member '{expression}' is not supported.");

            if (memberExpression.Expression is not ParameterExpression parameterExpression)
                throw new NotSupportedException($"Join expression member '{expression}' is not supported.");

            var source = expressionScope.Resolve(parameterExpression);
            var propertyName = memberExpression.Member.Name;
            var columnName = SqlColumnReferenceBuilder.ResolveMappedColumnName(source.ColumnMappings, propertyName);

            if (string.IsNullOrWhiteSpace(source.TableAlias))
                throw new InvalidOperationException($"Query source '{source.EntityType.Name}' does not define an alias required by the JOIN expression.");

            return context.DatabaseDialect.BuildQualifiedIdentifier(source.TableAlias, columnName);
        }
    }
}
