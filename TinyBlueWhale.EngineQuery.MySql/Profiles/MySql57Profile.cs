using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Features;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.MySql.Clauses.Strategies.InsertIdentityRetrieval;
using TinyBlueWhale.EngineQuery.MySql.Profiles.Interfaces;
using TinyBlueWhale.EngineQuery.Sql.Clauses.Pagination;
using TinyBlueWhale.EngineQuery.Sql.Interfaces.Strategies;
using TinyBlueWhale.EngineQuery.Sql.Profiles;

namespace TinyBlueWhale.EngineQuery.MySql.Profiles
{
    /// <summary>
    /// Represents the EngineQuery provider profile for MySQL 5.7.
    /// </summary>
    /// <remarks>
    /// This profile acts as the base MySQL version profile from which later
    /// compatible MySQL version profiles may inherit shared provider behavior.
    /// </remarks>
    public class MySql57Profile : DatabaseProviderProfile,
        IMySqlProfile,
        ILimitOffsetPaginationFeature,
        IPaginationStrategyProvider,
        IInsertIdentityRetrievalStrategyProvider,
        IInsertScalarIdentityFeature
    {
        /// <inheritdoc />
        public override DatabaseProviderVersion Version { get; } = DatabaseProviderVersion.Create(5, 7);

        /// <summary>
        /// Creates the pagination strategy for MySQL 5.7.
        /// </summary>
        /// <returns>
        /// The MySQL LIMIT/OFFSET pagination strategy.
        /// </returns>
        public IPaginationStrategy CreatePaginationStrategy()
        {
            return new PaginationStrategy();
        }

        /// <summary>
        /// Creates the INSERT identity retrieval strategy for MySQL 5.7.
        /// </summary>
        /// <returns>
        /// The MySQL identity retrieval strategy.
        /// </returns>
        public IInsertIdentityRetrievalStrategy CreateInsertIdentityRetrievalStrategy()
        {
            return new MySql57InsertIdentityRetrievalStrategy();
        }
    }
}
