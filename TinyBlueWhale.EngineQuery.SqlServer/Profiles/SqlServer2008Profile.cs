using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Features;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Sql.Clauses.LateralJoins;
using TinyBlueWhale.EngineQuery.Sql.Interfaces.Strategies;
using TinyBlueWhale.EngineQuery.Sql.Profiles;
using TinyBlueWhale.EngineQuery.SqlServer.Clauses.Strategies.Cte;
using TinyBlueWhale.EngineQuery.SqlServer.Clauses.Strategies.InsertIdentityRetrieval;
using TinyBlueWhale.EngineQuery.SqlServer.Profiles.Interfaces;

namespace TinyBlueWhale.EngineQuery.SqlServer.Profiles
{
    /// <summary>
    /// Represents the EngineQuery provider profile for SQL Server 2008.
    /// </summary>
    /// <remarks>
    /// This profile acts as the minimum supported SQL Server version profile
    /// and exposes only functionality available to that version.
    /// </remarks>
    public class SqlServer2008Profile : DatabaseProviderProfile,
        ISqlServerProfile,
        ICTEFeature,
        IRecursiveCTEFeature,
        IWindowFunctionFeature,
        ILateralJoinFeature,
        IIntersectFeature,
        IExceptFeature,
        ICTEStrategyProvider,
        ILateralJoinStrategyProvider,
        IInsertIdentityRetrievalStrategyProvider,
        IInsertScalarIdentityFeature
    {
        /// <inheritdoc />
        public override DatabaseProviderVersion Version { get; } = DatabaseProviderVersion.Create(10, 0);

        /// <summary>
        /// Creates the common table expression strategy for SQL Server 2008.
        /// </summary>
        /// <returns>
        /// The SQL Server 2008 common table expression strategy.
        /// </returns>
        public ICTEStrategy CreateCteStrategy()
        {
            return new SqlServer2008CteStrategy();
        }

        /// <summary>
        /// Creates the INSERT identity retrieval strategy for SQL Server 2008.
        /// </summary>
        /// <returns>
        /// The SQL Server 2008 identity retrieval strategy.
        /// </returns>
        public IInsertIdentityRetrievalStrategy CreateInsertIdentityRetrievalStrategy()
        {
            return new SqlServer2008InsertIdentityRetrievalStrategy();
        }

        /// <summary>
        /// Creates the lateral join strategy for SQL Server.
        /// </summary>
        /// <returns>
        /// The SQL Server lateral join strategy.
        /// </returns>
        public ILateralJoinStrategy CreateLateralJoinStrategy()
        {
            return new LateralJoinStrategy();
        }
    }
}
