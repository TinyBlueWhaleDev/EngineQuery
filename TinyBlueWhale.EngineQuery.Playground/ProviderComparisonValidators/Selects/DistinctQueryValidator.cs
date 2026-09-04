using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.Selects
{

    /// <summary>
    /// Validates DISTINCT query generation across providers.
    /// </summary>
    public static class DistinctQueryValidator
    {
        /// <summary>
        /// Runs the validator.
        /// </summary>
        public static void Run()
        {
            var metadataResolver = ProviderMetadataFactory.CreateJoinMetadataResolver();

            ProviderQueryPrinter.Print(
                "SQL Server DISTINCT",
                BuildDistinctCompositionQuery(ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL DISTINCT",
                BuildDistinctCompositionQuery(ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL DISTINCT",
                BuildDistinctCompositionQuery(ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));
        }

        // Builds a DISTINCT query.
        private static GeneratedSqlQuery BuildDistinctCompositionQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(user => new
                {
                    user.Email
                })
                .Distinct()
                .Where<JoinUser>(user => user.IsActive)
                .OrderBy<JoinUser>(user => user.Email)
                .Build();
        }
    }
}
