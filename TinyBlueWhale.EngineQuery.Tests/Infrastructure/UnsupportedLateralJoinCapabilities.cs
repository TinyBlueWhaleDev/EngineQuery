using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;

namespace TinyBlueWhale.EngineQuery.Tests.Infrastructure
{
    /// <summary>
    /// Represents provider capabilities without LATERAL or APPLY join support.
    /// </summary>
    internal sealed class UnsupportedLateralJoinCapabilities : IDatabaseProviderCapabilities
    {
        public bool SupportsCommonTableExpressions => true;

        public bool SupportsRecursiveCommonTableExpressions => true;

        public bool SupportsWindowFunctions => true;

        public bool SupportsLateralJoins => false;

        public bool SupportsIntersect => true;

        public bool SupportsExcept => true;

        public bool SupportsOffsetFetchPagination => true;

        public bool SupportsLimitOffsetPagination => false;
    }
}
