using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Features;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Sql.Clauses.Cte;
using TinyBlueWhale.EngineQuery.Sql.Interfaces.Strategies;

namespace TinyBlueWhale.EngineQuery.MySql.Profiles
{
    /// <summary>
    /// Represents the EngineQuery provider profile for MySQL 8.0.
    /// </summary>
    /// <remarks>
    /// This profile extends the previous MySQL provider profile and acts as the
    /// base for MySQL 8.0 versions that introduce additional query functionality.
    /// </remarks>
    public class MySql80Profile : MySql57Profile,
        ICTEFeature,
        IRecursiveCTEFeature,
        IWindowFunctionFeature,
        ICTEStrategyProvider
    {
        /// <inheritdoc />
        public override DatabaseProviderVersion Version { get; } = DatabaseProviderVersion.Create(8, 0);

        public ICTEStrategy CreateCteStrategy()
        {
            return new CteStrategy();
        }
    }
}
