using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Abstractions.Models;

namespace TinyBlueWhale.EngineQuery.Sql.Profiles
{
    /// <summary>
    /// Provides the base contract for database provider version profiles.
    /// </summary>
    /// <remarks>
    /// Derived profiles identify a concrete database engine version and may implement
    /// additional provider contracts to expose version-specific query functionality.
    /// </remarks>
    public abstract class DatabaseProviderProfile : IDatabaseProviderProfile
    {
        /// <inheritdoc />
        public abstract DatabaseProviderVersion Version { get; }
    }
}
