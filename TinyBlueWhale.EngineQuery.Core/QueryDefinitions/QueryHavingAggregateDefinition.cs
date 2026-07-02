using TinyBlueWhale.EngineQuery.Abstractions.Enums;

namespace TinyBlueWhale.EngineQuery.Core.QueryDefinitions
{
    /// <summary>
    /// Represents a HAVING condition based on an aggregate expression.
    /// </summary>
    public sealed record QueryHavingAggregateDefinition
    {
        /// <summary>
        /// Gets the aggregate function used by the HAVING condition.
        /// </summary>
        public required QueryAggregateFunction Function { get; init; }

        /// <summary>
        /// Gets the entity property name used by the aggregate function.
        /// </summary>
        public required string PropertyName { get; init; }

        /// <summary>
        /// Gets the comparison operator applied to the aggregate result.
        /// </summary>
        public required QueryComparisonOperator ComparisonOperator { get; init; }

        /// <summary>
        /// Gets the comparison value used by the HAVING condition.
        /// </summary>
        public required object? Value { get; init; }

        /// <summary>
        /// Gets the query source associated with the selected column.
        /// </summary>
        public required QuerySourceDefinition Source { get; init; }
    }
}
