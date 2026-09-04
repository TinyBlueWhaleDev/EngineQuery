using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Features;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.PostgreSql.Clauses.Strategies.InsertIdentityRetrieval;
using TinyBlueWhale.EngineQuery.PostgreSql.Profiles.Interfaces;
using TinyBlueWhale.EngineQuery.Sql.Clauses.Cte;
using TinyBlueWhale.EngineQuery.Sql.Clauses.Pagination;
using TinyBlueWhale.EngineQuery.Sql.Interfaces.Strategies;
using TinyBlueWhale.EngineQuery.Sql.Profiles;

namespace TinyBlueWhale.EngineQuery.PostgreSql.Profiles
{
    /// <summary>
    /// Represents the EngineQuery provider profile for PostgreSQL 8.4.
    /// </summary>
    /// <remarks>
    /// This profile represents the minimum supported PostgreSQL version and acts
    /// as the base profile for later compatible PostgreSQL versions.
    /// </remarks>
    public class PostgreSql84Profile : DatabaseProviderProfile,
        IPostgreSqlProfile,
        ICTEFeature,
        IRecursiveCTEFeature,
        IWindowFunctionFeature,
        IIntersectFeature,
        IExceptFeature,
        ILimitOffsetPaginationFeature,
        IPaginationStrategyProvider,
        ICTEStrategyProvider,
        IInsertIdentityRetrievalStrategyProvider
    {
        /// <inheritdoc />
        public override DatabaseProviderVersion Version { get; } = DatabaseProviderVersion.Create(8, 4);

        public ICTEStrategy CreateCteStrategy()
        {
            return new CteStrategy();
        }

        public IInsertIdentityRetrievalStrategy CreateInsertIdentityRetrievalStrategy()
        {
            return new PostgreSql84InsertIdentityRetrievalStrategy();
        }

        public IPaginationStrategy CreatePaginationStrategy()
        {
            return new PaginationStrategy();
        }
    }
}
