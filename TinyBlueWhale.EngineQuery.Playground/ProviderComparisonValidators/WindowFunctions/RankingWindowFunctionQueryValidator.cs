using TinyBlueWhale.EngineQuery.Abstractions.Extensions;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Features;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.WindowFunctions
{

    /// <summary>
    /// Validates ROW_NUMBER, RANK and DENSE_RANK window function generation
    /// across supported database providers.
    ///
    /// SQL Server:
    /// SELECT [o].[order_id] AS [OrderId], [o].[user_id], [o].[total], ROW_NUMBER() OVER (PARTITION BY [o].[user_id]
    /// ORDER BY [o].[total] DESC) AS [RowNumber], RANK() OVER (PARTITION BY [o].[user_id]
    /// ORDER BY [o].[total] DESC) AS [OrderRank], DENSE_RANK() OVER (PARTITION BY [o].[user_id]
    /// ORDER BY [o].[total] DESC) AS [DenseOrderRank]
    /// FROM [orders] AS [o]
    ///
    /// PostgreSQL:
    /// SELECT "o"."order_id" AS "OrderId", "o"."user_id", "o"."total", ROW_NUMBER() OVER (PARTITION BY "o"."user_id"
    /// ORDER BY "o"."total" DESC) AS "RowNumber", RANK() OVER (PARTITION BY "o"."user_id"
    /// ORDER BY "o"."total" DESC) AS "OrderRank", DENSE_RANK() OVER (PARTITION BY "o"."user_id"
    /// ORDER BY "o"."total" DESC) AS "DenseOrderRank"
    /// FROM "orders" AS "o"
    ///
    /// MySQL:
    /// SELECT `o`.`order_id` AS `OrderId`, `o`.`user_id`, `o`.`total`, ROW_NUMBER() OVER (PARTITION BY `o`.`user_id`
    /// ORDER BY `o`.`total` DESC) AS `RowNumber`, RANK() OVER (PARTITION BY `o`.`user_id`
    /// ORDER BY `o`.`total` DESC) AS `OrderRank`, DENSE_RANK() OVER (PARTITION BY `o`.`user_id`
    /// ORDER BY `o`.`total` DESC) AS `DenseOrderRank`
    /// FROM `orders` AS `o`
    /// </summary>
    public static class RankingWindowFunctionQueryValidator
    {        
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
            where TProfile : IDatabaseProviderProfile, IWindowFunctionFeature
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
