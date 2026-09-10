using TinyBlueWhale.EngineQuery.Abstractions.Models;

namespace TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers
{
    /// <summary>
    /// Defines the identity of a database provider profile.
    /// </summary>
    /// <remarks>
    /// A provider profile represents a concrete database engine version and acts
    /// as the compile-time identity used to resolve supported query features and
    /// provider-specific behavior.
    /// </remarks>
    public interface IDatabaseProviderProfile
    {
        /// <summary>
        /// Gets the database engine version represented by the provider profile.
        /// </summary>
        DatabaseProviderVersion Version { get; }
    }
}
