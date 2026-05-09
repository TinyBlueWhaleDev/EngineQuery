using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.Abstractions.Paging
{
    /// <summary>
    /// Represents a paginated query result.
    /// </summary>
    /// <typeparam name="T">
    /// Type of the returned query items.
    /// </typeparam>
    public sealed record QueryPagedResult<T>
    {
        /// <summary>
        /// Gets the collection of items returned for the current page.
        /// </summary>
        public required IReadOnlyList<T> Items { get; init; }

        /// <summary>
        /// Gets pagination metadata associated with the current result.
        /// </summary>
        public required QueryPaginationMetadata Pagination { get; init; }

        /// <summary>
        /// Gets a value indicating whether the current result contains items.
        /// </summary>
        public bool HasItems =>
            Items.Count > 0;
    }
}
