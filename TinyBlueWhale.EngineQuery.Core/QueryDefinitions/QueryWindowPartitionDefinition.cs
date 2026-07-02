
namespace TinyBlueWhale.EngineQuery.Core.QueryDefinitions
{
    /// <summary>
    /// Represents a PARTITION BY column used inside a SQL window function.
    /// </summary>
    public sealed record QueryWindowPartitionDefinition
    {
        /// <summary>
        /// Gets the partitioned column.
        /// </summary>
        public required QueryColumnDefinition Column { get; init; }

        /// <summary>
        /// Gets the query source associated with the partitioned column.
        /// </summary>
        public required QuerySourceDefinition Source { get; init; }
    }
}
