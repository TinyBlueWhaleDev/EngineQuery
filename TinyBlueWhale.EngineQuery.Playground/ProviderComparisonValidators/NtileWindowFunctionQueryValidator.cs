using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators
{

    /// <summary>
    /// Validates NTILE window function generation across providers.
    /// </summary>
    public static class NtileWindowFunctionQueryValidator
    {
        /// <summary>
        /// Runs the validator.
        /// </summary>
        public static void Run()
        {
            var metadataResolver = ProviderMetadataFactory.CreateJoinMetadataResolver();

            ProviderQueryPrinter.Print(
                "SQL Server NTILE Window Function",
                BuildQuery(ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL NTILE Window Function",
                BuildQuery(ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL NTILE Window Function",
                BuildQuery(ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));
        }

        // Builds an NTILE window function query.
        private static GeneratedSqlQuery BuildQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .From<JoinOrder>(alias: "o")
                .Select<JoinOrder>(o => new
                {
                    OrderId = o.Id,
                    o.UserId,
                    o.Total
                })
                .SelectNtile(
                    buckets: 4,
                    alias: "OrderQuartile",
                    windowBuilder: window => window
                        .PartitionBy<JoinOrder>(o => o.UserId)
                        .OrderByDescending<JoinOrder>(o => o.Total))
                .Build();
        }
    }
}
