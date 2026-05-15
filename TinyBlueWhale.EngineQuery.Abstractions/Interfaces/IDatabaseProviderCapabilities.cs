using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.Abstractions.Interfaces
{
    /// <summary>
    /// Defines SQL capabilities supported by a database provider.
    /// </summary>
    public interface IDatabaseProviderCapabilities
    {
        /// <summary>
        /// Gets whether the provider supports common table expressions.
        /// </summary>
        bool SupportsCommonTableExpressions { get; }

        /// <summary>
        /// Gets whether the provider supports recursive common table expressions.
        /// </summary>
        bool SupportsRecursiveCommonTableExpressions { get; }

        /// <summary>
        /// Gets whether the provider supports SQL window functions.
        /// </summary>
        bool SupportsWindowFunctions { get; }

        /// <summary>
        /// Gets whether the provider supports LATERAL joins or APPLY-equivalent joins.
        /// </summary>
        bool SupportsLateralJoins { get; }

        /// <summary>
        /// Gets whether the provider supports INTERSECT set operations.
        /// </summary>
        bool SupportsIntersect { get; }

        /// <summary>
        /// Gets whether the provider supports EXCEPT set operations.
        /// </summary>
        bool SupportsExcept { get; }

        /// <summary>
        /// Gets whether the provider supports OFFSET/FETCH pagination syntax.
        /// </summary>
        bool SupportsOffsetFetchPagination { get; }

        /// <summary>
        /// Gets whether the provider supports LIMIT/OFFSET pagination syntax.
        /// </summary>
        bool SupportsLimitOffsetPagination { get; }
    }
}
