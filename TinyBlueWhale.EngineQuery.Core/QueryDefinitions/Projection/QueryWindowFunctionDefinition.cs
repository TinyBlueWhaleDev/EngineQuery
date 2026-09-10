using TinyBlueWhale.EngineQuery.Abstractions.Enums;

namespace TinyBlueWhale.EngineQuery.Core.QueryDefinitions.Projection
{
    /// <summary>
    /// Represents a SQL window function projection.
    /// </summary>
    public sealed record QueryWindowFunctionDefinition
    {
        /// <summary>
        /// Gets the SQL window function.
        /// </summary>
        public required QueryWindowFunction Function { get; init; }

        /// <summary>
        /// Gets the SQL alias assigned to the window function result.
        /// </summary>
        public required string Alias { get; init; }

        /// <summary>
        /// Gets the PARTITION BY columns used by the window function.
        /// </summary>
        public IReadOnlyList<QueryWindowPartitionDefinition> Partitions { get; init; } = [];

        /// <summary>
        /// Gets the ORDER BY columns used by the window function.
        /// </summary>
        public required IReadOnlyList<QueryWindowOrderingDefinition> Orderings { get; init; }

        /// <summary>
        /// Gets the SQL window function arguments.
        /// </summary>
        public IReadOnlyList<QueryWindowFunctionArgumentDefinition> Arguments { get; init; } = [];
    }
}
