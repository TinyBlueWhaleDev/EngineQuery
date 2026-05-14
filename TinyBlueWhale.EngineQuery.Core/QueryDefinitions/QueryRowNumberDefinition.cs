using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.Core.QueryDefinitions
{
    /// <summary>
    /// Represents a ROW_NUMBER window function projection.
    /// </summary>
    public sealed record QueryRowNumberDefinition
    {
        /// <summary>
        /// Gets the SQL alias assigned to the ROW_NUMBER result.
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
    }
}
