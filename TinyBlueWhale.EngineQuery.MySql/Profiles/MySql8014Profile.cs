using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Features;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.MySql.Clauses.Strategies.LateralJoin;
using TinyBlueWhale.EngineQuery.Sql.Interfaces.Strategies;

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
        ILateralJoinFeature,
        ILateralJoinStrategyProvider
    {
        /// <inheritdoc />
        public override DatabaseProviderVersion Version { get; } = DatabaseProviderVersion.Create(8, 0, 14);

        public ILateralJoinStrategy CreateLateralJoinStrategy()
        {
            return new MySql8014LateralJoinStrategy();
        }
    }
}
