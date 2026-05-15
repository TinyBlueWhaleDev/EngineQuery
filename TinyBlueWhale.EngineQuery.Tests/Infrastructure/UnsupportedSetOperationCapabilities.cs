using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;

namespace TinyBlueWhale.EngineQuery.Tests.Infrastructure
{
    /// <summary>
    /// Represents provider capabilities without INTERSECT and EXCEPT support.
    /// </summary>
    internal sealed class UnsupportedSetOperationCapabilities : IDatabaseProviderCapabilities
    {
        public bool SupportsCommonTableExpressions => true;

        public bool SupportsRecursiveCommonTableExpressions => true;

        public bool SupportsWindowFunctions => true;

        public bool SupportsLateralJoins => true;

        public bool SupportsIntersect => false;

        public bool SupportsExcept => false;

        public bool SupportsOffsetFetchPagination => true;

        public bool SupportsLimitOffsetPagination => false;
    }
}
