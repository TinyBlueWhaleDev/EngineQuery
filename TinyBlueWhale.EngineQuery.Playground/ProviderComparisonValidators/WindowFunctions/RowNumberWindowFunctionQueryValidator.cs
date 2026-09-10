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
    /// Validates ROW_NUMBER window function generation with partitioning and ordering
    /// across supported database providers.
    ///
    /// SQL Server:
    /// SELECT [o].[order_id] AS [OrderId], [o].[user_id], [o].[total], ROW_NUMBER() OVER (PARTITION BY [o].[user_id]
    /// ORDER BY [o].[total] DESC) AS [UserOrderRank]
    /// FROM [orders] AS [o]
    ///
    /// PostgreSQL:
    /// SELECT "o"."order_id" AS "OrderId", "o"."user_id", "o"."total", ROW_NUMBER() OVER (PARTITION BY "o"."user_id"
    /// ORDER BY "o"."total" DESC) AS "UserOrderRank"
    /// FROM "orders" AS "o"
    ///
    /// MySQL:
    /// SELECT `o`.`order_id` AS `OrderId`, `o`.`user_id`, `o`.`total`, ROW_NUMBER() OVER (PARTITION BY `o`.`user_id`
    /// ORDER BY `o`.`total` DESC) AS `UserOrderRank`
    /// FROM `orders` AS `o`
    /// </summary>
    public static class RowNumberWindowFunctionQueryValidator
    {
        public static void Run()
        {
            var metadataResolver = ProviderMetadataFactory.CreateJoinMetadataResolver();

            ProviderQueryPrinter.Print(
                "SQL Server ROW_NUMBER",
                BuildQuery(ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL ROW_NUMBER",
                BuildQuery(ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL ROW_NUMBER",
                BuildQuery(ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));
        }

        // Builds a query using ROW_NUMBER with PARTITION BY and ORDER BY.
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
                    alias: "UserOrderRank",
                    window => window
                        .PartitionBy<JoinOrder>(o => o.UserId)
                        .OrderByDescending<JoinOrder>(o => o.Total))
                .Build();
        }
    }
}
