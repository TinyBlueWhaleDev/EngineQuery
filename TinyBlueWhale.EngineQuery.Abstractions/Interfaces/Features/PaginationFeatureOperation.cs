using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Features
{
    /// <summary>
    /// Represents an internal operation that defines the number of rows skipped by query pagination.
    /// </summary>
    internal sealed class PaginationSkipOperation : IQueryFeatureOperation
    {
        /// <summary>
        /// Initializes a new pagination skip operation.
        /// </summary>
        /// <param name="count">
        /// Number of rows to skip.
        /// </param>
        internal PaginationSkipOperation(int count)
        {
            Count = count;
        }

        /// <summary>
        /// Gets the number of rows to skip.
        /// </summary>
        internal int Count { get; }
    }

    /// <summary>
    /// Represents an internal operation that defines the maximum number of rows returned by query pagination.
    /// </summary>
    internal sealed class PaginationTakeOperation : IQueryFeatureOperation
    {
        /// <summary>
        /// Initializes a new pagination take operation.
        /// </summary>
        /// <param name="count">
        /// Maximum number of rows to return.
        /// </param>
        internal PaginationTakeOperation(int count)
        {
            Count = count;
        }

        /// <summary>
        /// Gets the maximum number of rows to return.
        /// </summary>
        internal int Count { get; }
    }
}
