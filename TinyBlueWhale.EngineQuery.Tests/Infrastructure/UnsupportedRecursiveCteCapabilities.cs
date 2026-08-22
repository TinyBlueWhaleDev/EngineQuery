using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;

namespace TinyBlueWhale.EngineQuery.Tests.Infrastructure
{

    /// <summary>
    /// Represents provider capabilities without recursive common table expression support.
    /// </summary>
    internal sealed class UnsupportedRecursiveCteCapabilities : IDatabaseProviderCapabilities
    {
        public bool SupportsCommonTableExpressions => true;

        public bool SupportsRecursiveCommonTableExpressions => false;

        public bool SupportsWindowFunctions => true;

        public bool SupportsLateralJoins => true;

        public bool SupportsIntersect => true;

        public bool SupportsExcept => true;

        public bool SupportsOffsetFetchPagination => true;

        public bool SupportsLimitOffsetPagination => false;
    }
}
