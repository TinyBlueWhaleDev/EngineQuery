using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Core.ExpressionScopes;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions.Filtering;
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
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="queryDefinition"/> is null.
        /// </exception>
        public bool CanBuild(CompiledQueryDefinition queryDefinition)
        {
            ArgumentNullException.ThrowIfNull(queryDefinition);

            return queryDefinition.WhereDefinitions.Count > 0 ||
                   queryDefinition.WhereScalarFunctionDefinitions.Count > 0 ||
                   queryDefinition.WhereComputedExpressionDefinitions.Count > 0 ||
                   queryDefinition.ExistsDefinitions.Count > 0 ||
                   queryDefinition.InSubqueryDefinitions.Count > 0 ||
                   queryDefinition.WhereCollectionDefinitions.Count > 0;
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
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="queryDefinition"/> or
        /// <paramref name="context"/> is null.
        /// </exception>
        public string Build(CompiledQueryDefinition queryDefinition, QueryCompilationContext context)
        {
            ArgumentNullException.ThrowIfNull(queryDefinition);
            ArgumentNullException.ThrowIfNull(context);

            var predicateConditions = BuildPredicateConditions(queryDefinition, context);

            var functionConditions = queryDefinition.WhereScalarFunctionDefinitions
                .Select(functionDefinition => BuildWhereScalarFunctionCondition(functionDefinition, context));

            var computedConditions = queryDefinition.WhereComputedExpressionDefinitions
                .Select(computedDefinition => BuildWhereComputedExpressionCondition(computedDefinition, context));

            var existsConditions = queryDefinition.ExistsDefinitions
                .Select(existsDefinition => BuildExistsCondition(existsDefinition, context));

            var inConditions = queryDefinition.InSubqueryDefinitions
                .Select(inDefinition => BuildInSubqueryCondition(inDefinition, context));

            var collectionConditions = queryDefinition.WhereCollectionDefinitions
                .Select(collectionDefinition => BuildCollectionCondition(collectionDefinition, context));

            var conditions = predicateConditions
                .Concat(functionConditions)
                .Concat(computedConditions)
                .Concat(existsConditions)
                .Concat(inConditions)
                .Concat(collectionConditions);

            return "WHERE " + string.Join(" AND ", conditions);
        }

        // Compiles predicate WHERE definitions and groups consecutive logical OR operations.
        private static List<string> BuildPredicateConditions(CompiledQueryDefinition queryDefinition, QueryCompilationContext context)
        {
            var compiledConditions = queryDefinition.WhereDefinitions.Select(
                whereDefinition =>
                {
                    var parser = new QueryWhereClauseExpressionParser(
                        context.DatabaseDialect,
                        context.Parameters,
                        whereDefinition.Source.ColumnMappings,
                        whereDefinition.Source.TableAlias);

                    var sqlCondition = parser.ParseToSqlCondition(whereDefinition.PredicateExpression.Body);

                    return new CompiledPredicateCondition(sqlCondition, whereDefinition.LogicalOperator);
                }).ToList();

            return GroupOrConditions(compiledConditions);
        }

        // Groups linear OR predicate sequences while preserving SQL operator precedence.
        private static List<string> GroupOrConditions(IReadOnlyList<CompiledPredicateCondition> conditions)
        {
            if (conditions.Count == 0)
                return [];

            var groupedConditions = new List<string>();

            for (var index = 0; index < conditions.Count; index++)
            {
                var currentCondition = conditions[index];

                var nextConditionUsesOr = index + 1 < conditions.Count && conditions[index + 1].LogicalOperator == QueryLogicalOperator.Or;

                if (!nextConditionUsesOr)
                {
                    groupedConditions.Add(currentCondition.Sql);

                    continue;
                }

                var orConditions = new List<string> { currentCondition.Sql };

                while (index + 1 < conditions.Count && conditions[index + 1].LogicalOperator == QueryLogicalOperator.Or)
                {
                    index++;

                    orConditions.Add(conditions[index].Sql);
                }

                groupedConditions.Add("(" + string.Join(" OR ", orConditions) + ")");
            }

            return groupedConditions;
        }

        // Builds a scalar SQL function WHERE condition.
        private string BuildWhereScalarFunctionCondition(QueryWhereScalarFunctionDefinition functionDefinition,
            QueryCompilationContext context)
        {
            var columnReference = _columnReferenceBuilder.Build(functionDefinition.Source, functionDefinition.PropertyName);

            var parameterName = context.AddParameter(functionDefinition.Value);

            var functionName = SqlFunctionNameResolver.ResolveScalarFunctionName(functionDefinition.Function, context.DatabaseDialect);

            var comparisonOperator = SqlComparisonOperatorResolver.Resolve(functionDefinition.ComparisonOperator);

            return $"{functionName}({columnReference}) " + $"{comparisonOperator} {parameterName}";
        }

        // Builds a computed expression WHERE condition.
        private static string BuildWhereComputedExpressionCondition(QueryWhereComputedExpressionDefinition computedDefinition,
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

        // Builds an EXISTS or NOT EXISTS WHERE condition.
        private string BuildExistsCondition(QueryExistsDefinition existsDefinition, QueryCompilationContext context)
        {
            var commandText = _subqueryCompiler.CompileAndReindex(existsDefinition.Subquery, context);

            var existsKeyword = existsDefinition.IsNegated
                ? "NOT EXISTS"
                : "EXISTS";

            return $"{existsKeyword} ({commandText})";
        }

        // Builds an IN subquery WHERE condition.
        private string BuildInSubqueryCondition(QueryInSubqueryDefinition inDefinition, QueryCompilationContext context)
        {
            var propertyName = ExpressionColumnSelector.ExtractSinglePropertyName(inDefinition.OuterSelector);

            var outerColumnReference = _columnReferenceBuilder.Build(inDefinition.OuterSource, propertyName);

            var commandText = _subqueryCompiler.CompileAndReindex(inDefinition.Subquery, context);

            return $"{outerColumnReference} IN ({commandText})";
        }

        // Builds an IN or NOT IN collection WHERE condition.
        private string BuildCollectionCondition(QueryWhereCollectionDefinition collectionDefinition, QueryCompilationContext context)
        {
            var propertyName = ExpressionColumnSelector
                .ExtractSinglePropertyName(collectionDefinition.Selector);

            var columnReference = _columnReferenceBuilder.Build(
                collectionDefinition.Source,
                propertyName);

            var parameterNames = collectionDefinition.Values
                .Select(context.AddParameter);

            var collectionOperator = collectionDefinition.IsNegated
                ? "NOT IN"
                : "IN";

            return $"{columnReference} {collectionOperator} ({string.Join(", ", parameterNames)})";
        }

        /// <summary>
        /// Represents a compiled predicate condition and the logical
        /// operator that connects it with the preceding predicate.
        /// </summary>
        /// <param name="Sql">
        /// Compiled SQL predicate.
        /// </param>
        /// <param name="LogicalOperator">
        /// Logical operator associated with the predicate.
        /// </param>
        private sealed record CompiledPredicateCondition(string Sql, QueryLogicalOperator LogicalOperator);
    }

}
