using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;
using TinyBlueWhale.EngineQuery.Sql.Compilation;
using TinyBlueWhale.EngineQuery.Sql.Helpers;
using TinyBlueWhale.EngineQuery.Sql.Interfaces;

namespace TinyBlueWhale.EngineQuery.Sql.Clauses
{
    /// <summary>
    /// Builds SQL HAVING clauses from aggregate filter definitions.
    /// </summary>
    /// <remarks>
    /// This builder emits aggregate conditions that are applied after GROUP BY processing.
    /// </remarks>
    /// <remarks>
    /// Initializes a new instance of the <see cref="HavingClauseBuilder"/> class.
    /// </remarks>
    /// <param name="columnReferenceBuilder">
    /// SQL column reference builder used to resolve aggregate column references.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="columnReferenceBuilder"/> is <see langword="null"/>.
    /// </exception>
    public sealed class HavingClauseBuilder(SqlColumnReferenceBuilder columnReferenceBuilder) : IOptionalSqlClauseBuilder
    {
        private readonly SqlColumnReferenceBuilder _columnReferenceBuilder = columnReferenceBuilder ?? throw new ArgumentNullException(nameof(columnReferenceBuilder));

        /// <summary>
        /// Determines whether a HAVING clause should be built.
        /// </summary>
        /// <param name="queryDefinition">
        /// Query definition to inspect.
        /// </param>
        /// <returns>
        /// <see langword="true"/> when aggregate filter definitions are configured; otherwise, <see langword="false"/>.
        /// </returns>
        public bool CanBuild(CompiledQueryDefinition queryDefinition)
        {
            ArgumentNullException.ThrowIfNull(queryDefinition);

            return queryDefinition.HavingAggregateDefinitions.Count > 0;
        }

        /// <summary>
        /// Builds the SQL HAVING clause.
        /// </summary>
        /// <param name="queryDefinition">
        /// Query definition that contains aggregate filter metadata.
        /// </param>
        /// <param name="context">
        /// Current SQL compilation context.
        /// </param>
        /// <returns>
        /// SQL HAVING clause.
        /// </returns>
        public string Build(CompiledQueryDefinition queryDefinition, QueryCompilationContext context)
        {
            ArgumentNullException.ThrowIfNull(queryDefinition);
            ArgumentNullException.ThrowIfNull(context);

            var havingConditions = queryDefinition.HavingAggregateDefinitions
                .Select(havingDefinition => BuildHavingAggregateCondition(havingDefinition, context));

            return "HAVING " + string.Join(" AND ", havingConditions);
        }

        private string BuildHavingAggregateCondition(QueryHavingAggregateDefinition havingDefinition, QueryCompilationContext context)
        {
            var columnReference = _columnReferenceBuilder.Build(
                havingDefinition.Source,
                havingDefinition.PropertyName);

            var parameterName = context.AddParameter(havingDefinition.Value);

            var functionName = SqlFunctionNameResolver.ResolveAggregateFunctionName(
                havingDefinition.Function);

            var comparisonOperator = SqlComparisonOperatorResolver.Resolve(
                havingDefinition.ComparisonOperator);

            return $"{functionName}({columnReference}) {comparisonOperator} {parameterName}";
        }
    }
}
