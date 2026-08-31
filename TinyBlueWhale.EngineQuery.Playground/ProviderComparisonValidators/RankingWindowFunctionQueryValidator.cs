using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators
{

    /// <summary>
    /// Validates ranking window function generation across providers.
    /// </summary>
    public static class RankingWindowFunctionQueryValidator
    {
        /// <summary>
        /// Runs the validator.
        /// </summary>
        public static void Run()
        {
            var metadataResolver = ProviderMetadataFactory.CreateJoinMetadataResolver();

            ProviderQueryPrinter.Print(
                "SQL Server Ranking Window Functions",
                BuildQuery(ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL Ranking Window Functions",
                BuildQuery(ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL Ranking Window Functions",
                BuildQuery(ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));
        }

        // Builds a query using ranking window functions.
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
                .SelectRowNumber(
                    alias: "RowNumber",
                    window => window
                        .PartitionBy<JoinOrder>(o => o.UserId)
                        .OrderByDescending<JoinOrder>(o => o.Total))
                .SelectRank(
                    alias: "OrderRank",
                    window => window
                        .PartitionBy<JoinOrder>(o => o.UserId)
                        .OrderByDescending<JoinOrder>(o => o.Total))
                .SelectDenseRank(
                    alias: "DenseOrderRank",
                    window => window
                        .PartitionBy<JoinOrder>(o => o.UserId)
                        .OrderByDescending<JoinOrder>(o => o.Total))
                .Build();
        }
    }
}
