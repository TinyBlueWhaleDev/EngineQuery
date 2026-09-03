
namespace TinyBlueWhale.EngineQuery.Abstractions.Interfaces
{
    /// <summary>
    /// Defines SQL capabilities supported by a database provider.
    /// </summary>
    public interface IDatabaseProviderCapabilities
    {
        /// <summary>
        /// Gets whether the provider supports LATERAL joins or APPLY-equivalent joins.
        /// </summary>
        bool SupportsLateralJoins { get; }        

    }
}
