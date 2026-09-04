using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Sql.Interfaces.Strategies;

namespace TinyBlueWhale.EngineQuery.Sql.Composition
{
    /// <summary>
    /// Creates SQL feature compositions from database provider profiles.
    /// </summary>
    public static class QueryFeatureCompositionFactory
    {
        /// <summary>
        /// Creates the SQL feature composition associated with the specified provider profile.
        /// </summary>
        /// <param name="profile">
        /// Database provider profile used to resolve SQL feature strategies.
        /// </param>
        /// <returns>
        /// SQL feature composition resolved from the provider profile.
        /// </returns>
        public static QueryFeatureComposition Create(IDatabaseProviderProfile profile)
        {
            ArgumentNullException.ThrowIfNull(profile);

            return new QueryFeatureComposition
            {
                PaginationStrategy = profile is IPaginationStrategyProvider paginationStrategyProvider
                    ? paginationStrategyProvider.CreatePaginationStrategy()
                    : null,

                CteStrategy = profile is ICTEStrategyProvider cteStrategyProvider
                    ? cteStrategyProvider.CreateCteStrategy()
                    : null,

                LateralJoinStrategy = profile is ILateralJoinStrategyProvider lateralJoinStrategyProvider
                    ? lateralJoinStrategyProvider.CreateLateralJoinStrategy()
                    : null,

                InsertIdentityRetrievalStrategy = profile is IInsertIdentityRetrievalStrategyProvider insertIdentityRetrievalStrategyProvider
                    ? insertIdentityRetrievalStrategyProvider.CreateInsertIdentityRetrievalStrategy()
                    : null
            };
        }
    }
}
