using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;

namespace TinyBlueWhale.EngineQuery.Tests.Infrastructure
{
    /// <summary>
    /// Represents provider capabilities without INTERSECT and EXCEPT support.
    /// </summary>
    internal sealed class UnsupportedSetOperationCapabilities : IDatabaseProviderCapabilities
    {
        public bool SupportsLateralJoins => true;

        public bool SupportsIntersect => false;

        public bool SupportsExcept => false;
    }
}
