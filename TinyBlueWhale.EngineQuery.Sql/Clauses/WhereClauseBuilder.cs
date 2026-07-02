using TinyBlueWhale.EngineQuery.Core.ExpressionScopes;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;
using TinyBlueWhale.EngineQuery.Sql.Compilation;
using TinyBlueWhale.EngineQuery.Sql.ExpressionsParsing;
using TinyBlueWhale.EngineQuery.Sql.Helpers;
using TinyBlueWhale.EngineQuery.Sql.Interfaces;

namespace TinyBlueWhale.EngineQuery.Sql.Clauses
{
    /// <summary>
    /// Builds SQL WHERE clauses from query filter definitions.
    /// </summary>
    /// <remarks>
    /// This builder supports predicate expressions, scalar function filters, computed expression filters,
    /// EXISTS conditions and IN subquery conditions.
    /// </remarks>
    /// <remarks>
    /// Initializes a new instance of the <see cref="WhereClauseBuilder"/> class.
    /// </remarks>
    /// <param name="columnReferenceBuilder">
    /// SQL column reference builder used to resolve column references.
    /// </param>
    /// <param name="subqueryCompiler">
    /// Subquery compiler used to compile nested WHERE subqueries.
    /// </param>
    public sealed class WhereClauseBuilder(SqlColumnReferenceBuilder columnReferenceBuilder, SubqueryCompiler subqueryCompiler) : IOptionalSqlClauseBuilder
    {
        private readonly SqlColumnReferenceBuilder _columnReferenceBuilder = columnReferenceBuilder ?? throw new ArgumentNullException(nameof(columnReferenceBuilder));
        private readonly SubqueryCompiler _subqueryCompiler = subqueryCompiler ?? throw new ArgumentNullException(nameof(subqueryCompiler));

        /// <summary>
        /// Determines whether a WHERE clause should be built.
        /// </summary>
        /// <param name="queryDefinition">
        /// Query definition to inspect.
        /// </param>
        /// <returns>
        /// <see langword="true"/> when filter definitions are configured; otherwise, <see langword="false"/>.
        /// </returns>
        public bool CanBuild(CompiledQueryDefinition queryDefinition)
        {
            ArgumentNullException.ThrowIfNull(queryDefinition);

            return queryDefinition.WhereDefinitions.Count > 0 ||
                   queryDefinition.WhereScalarFunctionDefinitions.Count > 0 ||
                   queryDefinition.WhereComputedExpressionDefinitions.Count > 0 ||
                   queryDefinition.ExistsDefinitions.Count > 0 ||
                   queryDefinition.InSubqueryDefinitions.Count > 0;
        }

        /// <summary>
        /// Builds the SQL WHERE clause.
        /// </summary>
        /// <param name="queryDefinition">
        /// Query definition that contains filter metadata.
        /// </param>
        /// <param name="context">
        /// Current SQL compilation context.
        /// </param>
        /// <returns>
        /// SQL WHERE clause.
        /// </returns>
        public string Build(CompiledQueryDefinition queryDefinition, QueryCompilationContext context)
        {
            ArgumentNullException.ThrowIfNull(queryDefinition);
            ArgumentNullException.ThrowIfNull(context);

            var whereConditions = queryDefinition.WhereDefinitions
                .Select(whereDefinition =>
                {
                    var parser = new QueryWhereClauseExpressionParser(
                        context.DatabaseDialect,
                        context.Parameters,
                        whereDefinition.Source.ColumnMappings ?? queryDefinition.ColumnMappings,
                        whereDefinition.Source.TableAlias ?? queryDefinition.TableAlias);

                    return parser.ParseToSqlCondition(whereDefinition.PredicateExpression.Body);
                });

            var functionConditions = queryDefinition.WhereScalarFunctionDefinitions
                .Select(functionDefinition => BuildWhereScalarFunctionCondition(functionDefinition, context));

            var computedConditions = queryDefinition.WhereComputedExpressionDefinitions
                .Select(computedDefinition => BuildWhereComputedExpressionCondition(computedDefinition, context));

            var existsConditions = queryDefinition.ExistsDefinitions
                .Select(existsDefinition => BuildExistsCondition(existsDefinition, context));

            var inConditions = queryDefinition.InSubqueryDefinitions
                .Select(inDefinition => BuildInSubqueryCondition(inDefinition, context));

            return "WHERE " + string.Join(" AND ", whereConditions
                .Concat(functionConditions)
                .Concat(computedConditions)
                .Concat(existsConditions)
                .Concat(inConditions));
        }

        private string BuildWhereScalarFunctionCondition(QueryWhereScalarFunctionDefinition functionDefinition, QueryCompilationContext context)
        {
            var columnReference = _columnReferenceBuilder.Build(functionDefinition.Source, functionDefinition.PropertyName);

            var parameterName = context.AddParameter(functionDefinition.Value);
            var functionName = SqlFunctionNameResolver.ResolveScalarFunctionName(functionDefinition.Function, context.DatabaseDialect);
            var comparisonOperator = SqlComparisonOperatorResolver.Resolve(functionDefinition.ComparisonOperator);

            return $"{functionName}({columnReference}) {comparisonOperator} {parameterName}";
        }

        private static string BuildWhereComputedExpressionCondition(
            QueryWhereComputedExpressionDefinition computedDefinition,
            QueryCompilationContext context)
        {
            var expressionScope = new QueryExpressionScope();

            foreach (var source in computedDefinition.Sources)
                expressionScope.Register(source.Key, source.Value);

            var parser = new SqlComputedExpressionParser(
                context.DatabaseDialect,
                context.Parameters,
                null,
                null,
                expressionScope);

            return parser.Parse(computedDefinition.Expression.Body);
        }

        private string BuildExistsCondition(QueryExistsDefinition existsDefinition, QueryCompilationContext context)
        {
            var commandText = _subqueryCompiler.CompileAndReindex(existsDefinition.Subquery, context);

            var existsKeyword = existsDefinition.IsNegated ? "NOT EXISTS" : "EXISTS";

            return $"{existsKeyword} ({commandText})";
        }

        private string BuildInSubqueryCondition(QueryInSubqueryDefinition inDefinition, QueryCompilationContext context)
        {
            var propertyName = ExpressionColumnSelector.ExtractSinglePropertyName(inDefinition.OuterSelector);
            var outerColumnReference = _columnReferenceBuilder.Build(inDefinition.OuterSource, propertyName);
            var commandText = _subqueryCompiler.CompileAndReindex(inDefinition.Subquery, context);

            return $"{outerColumnReference} IN ({commandText})";
        }
    }
}
