using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators
{

    /// <summary>
    /// Validates UNION ALL query generation across providers.
    /// </summary>
    public static class UnionAllQueryValidator
    {
        /// <summary>
        /// Runs the validator.
        /// </summary>
        public static void Run()
        {
            var metadataResolver = ProviderMetadataFactory.CreateJoinMetadataResolver();

            ProviderQueryPrinter.Print(
                "SQL Server UNION ALL",
                BuildQuery(ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL UNION ALL",
                BuildQuery(ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL UNION ALL",
                BuildQuery(ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));
        }

        // Builds a UNION ALL query.
        private static GeneratedSqlQuery BuildQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .From<ActiveUser>(alias: "u")
                .Select<ActiveUser>(u => new
                {
                    u.Id,
                    u.Email
                })
                .Where<ActiveUser>(u => u.Id > 10)
                .UnionAll<ArchivedUser>(union => union
                    .From<ArchivedUser>(alias: "a")
                    .Select<ArchivedUser>(a => new
                    {
                        a.Id,
                        a.Email
                    })
                    .Where<ArchivedUser>(a => a.Id > 100))
                .Build();
        }
    }
}
