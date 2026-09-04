using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Features;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.PostgreSql.Clauses.Strategies.LateralJoin;
using TinyBlueWhale.EngineQuery.Sql.Interfaces.Strategies;

namespace TinyBlueWhale.EngineQuery.PostgreSql.Profiles
{
    /// <summary>
    /// Represents the EngineQuery provider profile for PostgreSQL 9.3.
    /// </summary>
    /// <remarks>
    /// PostgreSQL 9.3 introduces LATERAL query support.
    /// </remarks>
    public class PostgreSql93Profile : PostgreSql84Profile,
        ILateralJoinFeature,
        ILateralJoinStrategyProvider
    {
        /// <inheritdoc />
        public override DatabaseProviderVersion Version { get; } = DatabaseProviderVersion.Create(9, 3);

        public ILateralJoinStrategy CreateLateralJoinStrategy()
        {
            return new PostgreSql93LateralJoinStrategy();
        }
    }
}
