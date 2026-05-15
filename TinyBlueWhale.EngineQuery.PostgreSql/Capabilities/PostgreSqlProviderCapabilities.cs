using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;

namespace TinyBlueWhale.EngineQuery.PostgreSqlServer.Capabilities
{

    /// <summary>
    /// Defines PostgreSQL provider capability support.
    /// </summary>
    public sealed class PostgreSqlProviderCapabilities : IDatabaseProviderCapabilities
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
