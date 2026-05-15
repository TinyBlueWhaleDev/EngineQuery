using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Abstractions.Enums;

namespace TinyBlueWhale.EngineQuery.Core.QueryDefinitions
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
    }
}
