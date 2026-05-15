using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;

namespace TinyBlueWhale.EngineQuery.SqlServer.Capabilities
{
    /// <summary>
    /// Defines SQL Server provider capability support.
    /// </summary>
    public sealed class SqlServerProviderCapabilities : IDatabaseProviderCapabilities
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
        public bool SupportsOffsetFetchPagination => true;

        /// <inheritdoc />
        public bool SupportsLimitOffsetPagination => false;
    }
}
