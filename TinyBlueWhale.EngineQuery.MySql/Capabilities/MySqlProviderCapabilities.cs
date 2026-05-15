using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;

namespace TinyBlueWhale.EngineQuery.MySqlServer.Capabilities
{
    /// <summary>
    /// Defines MySQL provider capability support.
    /// </summary>
    /// <remarks>
    /// This capability profile assumes a modern MySQL version with support for common table expressions,
    /// recursive common table expressions, window functions and SQL set operations.
    /// </remarks>
    public sealed class MySqlProviderCapabilities : IDatabaseProviderCapabilities
    {
        /// <inheritdoc />
        public bool SupportsCommonTableExpressions => true;

        /// <inheritdoc />
        public bool SupportsRecursiveCommonTableExpressions => true;

        /// <inheritdoc />
        public bool SupportsWindowFunctions => true;

        /// <inheritdoc />
        public bool SupportsLateralJoins => true;

        /// <inheritdoc />
        public bool SupportsIntersect => true;

        /// <inheritdoc />
        public bool SupportsExcept => true;

        /// <inheritdoc />
        public bool SupportsOffsetFetchPagination => false;

        /// <inheritdoc />
        public bool SupportsLimitOffsetPagination => true;
    }
}
