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
    /// Validates LAG and LEAD window function generation, offset parameterization
    /// and independent window ordering across supported database providers.
    ///
    /// SQL Server:
    /// SELECT [o].[order_id] AS [OrderId], [o].[user_id], [o].[total], LAG([o].[total], @p0) OVER (PARTITION BY [o].[user_id]
    /// ORDER BY [o].[total] ASC) AS [PreviousOrderTotal], LEAD([o].[total], @p1) OVER (PARTITION BY [o].[user_id]
    /// ORDER BY [o].[order_id] ASC) AS [NextOrderTotal]
    /// FROM [orders] AS [o]
    ///
    /// PostgreSQL:
    /// SELECT "o"."order_id" AS "OrderId", "o"."user_id", "o"."total", LAG("o"."total", @p0) OVER (PARTITION BY "o"."user_id"
    /// ORDER BY "o"."total" ASC) AS "PreviousOrderTotal", LEAD("o"."total", @p1) OVER (PARTITION BY "o"."user_id"
    /// ORDER BY "o"."order_id" ASC) AS "NextOrderTotal"
    /// FROM "orders" AS "o"
    ///
    /// MySQL:
    /// SELECT `o`.`order_id` AS `OrderId`, `o`.`user_id`, `o`.`total`, LAG(`o`.`total`, @p0) OVER (PARTITION BY `o`.`user_id`
    /// ORDER BY `o`.`total` ASC) AS `PreviousOrderTotal`, LEAD(`o`.`total`, @p1) OVER (PARTITION BY `o`.`user_id`
    /// ORDER BY `o`.`order_id` ASC) AS `NextOrderTotal`
    /// FROM `orders` AS `o`
    ///
    /// Expected parameters:
    /// @p0 = 1
    /// @p1 = 1
    /// </summary>
    public static class LagLeadWindowFunctionQueryValidator
    {
        public static void Run()
        {
            var metadataResolver = ProviderMetadataFactory.CreateJoinMetadataResolver();

            ProviderQueryPrinter.Print(
                "SQL Server LAG / LEAD Window Functions",
                BuildQuery(ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL LAG / LEAD Window Functions",
                BuildQuery(ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL LAG / LEAD Window Functions",
                BuildQuery(ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));
        }

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
                .SelectLag(
                    (JoinOrder o) => o.Total,
                    alias: "PreviousOrderTotal",
                    windowBuilder: window => window
                        .PartitionBy<JoinOrder>(o => o.UserId)
                        .OrderBy<JoinOrder>(o => o.Total))
                .SelectLead(
                    (JoinOrder o) => o.Total,
                    alias: "NextOrderTotal",
                    windowBuilder: window => window
                        .PartitionBy<JoinOrder>(o => o.UserId)
                        .OrderBy<JoinOrder>(o => o.Id))
                .Build();
        }
    }
}
