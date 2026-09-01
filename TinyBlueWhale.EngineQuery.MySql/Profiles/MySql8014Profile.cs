using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Features;
using TinyBlueWhale.EngineQuery.Abstractions.Models;

namespace TinyBlueWhale.EngineQuery.MySql.Profiles
{
    /// <summary>
    /// Represents the EngineQuery provider profile for MySQL 8.0.14.
    /// </summary>
    /// <remarks>
    /// This profile represents the MySQL version boundary at which EngineQuery
    /// may expose additional provider-specific query functionality.
    /// </remarks>
    public class MySql8014Profile : MySql80Profile,
        ILateralJoinFeature
    {
        /// <inheritdoc />
        public override DatabaseProviderVersion Version { get; } = DatabaseProviderVersion.Create(8, 0, 14);
    }
}
