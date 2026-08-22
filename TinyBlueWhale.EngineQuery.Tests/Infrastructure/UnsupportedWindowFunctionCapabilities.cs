using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;

namespace TinyBlueWhale.EngineQuery.Tests.Infrastructure
{
    /// <summary>
    /// Represents provider capabilities without window function support.
    /// </summary>
    internal sealed class UnsupportedWindowFunctionCapabilities : IDatabaseProviderCapabilities
    {
        public bool SupportsCommonTableExpressions => true;

        public bool SupportsRecursiveCommonTableExpressions => true;

        public bool SupportsWindowFunctions => false;

        public bool SupportsLateralJoins => true;

        public bool SupportsIntersect => true;

        public bool SupportsExcept => true;

        public bool SupportsOffsetFetchPagination => true;

        public bool SupportsLimitOffsetPagination => false;
    }
}
