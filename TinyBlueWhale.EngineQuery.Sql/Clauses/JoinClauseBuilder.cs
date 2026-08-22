using System.Linq.Expressions;
using TinyBlueWhale.EngineQuery.Core.Enums;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;
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

        private static string BuildJoinClause(QueryJoinDefinition joinDefinition, QueryCompilationContext context)
        {
            var joinKeyword = joinDefinition.JoinType switch
            {
                QueryJoinType.Inner => "INNER JOIN",
                QueryJoinType.Left => "LEFT JOIN",
                _ => throw new NotSupportedException($"Join type '{joinDefinition.JoinType}' is not supported.")
            };

            var tableName = context.DatabaseDialect.EscapeIdentifier(joinDefinition.TableName);
            var tableAlias = context.DatabaseDialect.EscapeIdentifier(joinDefinition.TableAlias);
            var joinCondition = BuildJoinCondition(joinDefinition, context);

            return $"{joinKeyword} {tableName} AS {tableAlias} ON {joinCondition}";
        }

        private static string BuildJoinCondition(QueryJoinDefinition joinDefinition, QueryCompilationContext context)
        {
            return BuildJoinExpression(joinDefinition.JoinExpression.Body, joinDefinition, context);
        }

        private static string BuildJoinExpression(Expression expression, QueryJoinDefinition joinDefinition, QueryCompilationContext context)
        {
            expression = SqlComputedExpressionParser.UnwrapConvertExpression(expression);

            if (expression is not BinaryExpression binaryExpression)
                throw new NotSupportedException($"Join expression '{expression}' is not supported.");

            if (binaryExpression.NodeType is ExpressionType.AndAlso or ExpressionType.OrElse)
            {
                var left = BuildJoinExpression(binaryExpression.Left, joinDefinition, context);

                var right = BuildJoinExpression(binaryExpression.Right, joinDefinition, context);

                var sqlOperator = binaryExpression.NodeType == ExpressionType.AndAlso
                    ? "AND"
                    : "OR";

                return $"({left} {sqlOperator} {right})";
            }

            var comparisonOperator = ResolveJoinComparisonOperator(binaryExpression.NodeType);

            var leftColumn = BuildJoinColumnReference(binaryExpression.Left, joinDefinition, context);

            var rightColumn = BuildJoinColumnReference(binaryExpression.Right, joinDefinition, context);

            return $"({leftColumn} {comparisonOperator} {rightColumn})";
        }

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
                _ => throw new NotSupportedException(
                    $"Join operator '{expressionType}' is not supported.")
            };
        }

        private static string BuildJoinColumnReference(Expression expression, QueryJoinDefinition joinDefinition, QueryCompilationContext context)
        {
            expression = SqlComputedExpressionParser.UnwrapConvertExpression(expression);

            if (expression is not MemberExpression memberExpression)
                throw new NotSupportedException($"Join expression member '{expression}' is not supported.");

            if (memberExpression.Expression is not ParameterExpression parameterExpression)
                throw new NotSupportedException($"Join expression member '{expression}' is not supported.");

            var propertyName = memberExpression.Member.Name;

            if (parameterExpression.Type == joinDefinition.SourceType)
            {
                var columnName = SqlColumnReferenceBuilder.ResolveMappedColumnName(
                    joinDefinition.SourceColumnMappings,
                    propertyName);

                return context.DatabaseDialect.BuildQualifiedIdentifier(
                    joinDefinition.SourceAlias,
                    columnName);
            }

            if (parameterExpression.Type == joinDefinition.JoinTypeEntity)
            {
                var columnName = SqlColumnReferenceBuilder.ResolveMappedColumnName(
                    joinDefinition.JoinColumnMappings,
                    propertyName);

                return context.DatabaseDialect.BuildQualifiedIdentifier(joinDefinition.TableAlias, columnName);
            }

            throw new NotSupportedException($"Join expression parameter '{parameterExpression.Type.Name}' is not supported.");
        }
    }
}
