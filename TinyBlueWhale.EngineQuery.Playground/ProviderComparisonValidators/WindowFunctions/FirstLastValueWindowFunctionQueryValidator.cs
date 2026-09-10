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
    /// Validates FIRST_VALUE and LAST_VALUE window function generation
    /// across supported database providers.
    ///
    /// LAST_VALUE uses the database default window frame because explicit
    /// window frame configuration is not part of the current query API.
    ///
    /// SQL Server:
    /// SELECT [o].[order_id] AS [OrderId], [o].[user_id], [o].[total], FIRST_VALUE([o].[total]) OVER (PARTITION BY [o].[user_id]
    /// ORDER BY [o].[order_id] ASC) AS [FirstOrderTotal], LAST_VALUE([o].[total]) OVER (PARTITION BY [o].[user_id]
    /// ORDER BY [o].[order_id] ASC) AS [CurrentFrameLastValue]
    /// FROM [orders] AS [o]
    ///
    /// PostgreSQL:
    /// SELECT "o"."order_id" AS "OrderId", "o"."user_id", "o"."total", FIRST_VALUE("o"."total") OVER (PARTITION BY "o"."user_id"
    /// ORDER BY "o"."order_id" ASC) AS "FirstOrderTotal", LAST_VALUE("o"."total") OVER (PARTITION BY "o"."user_id"
    /// ORDER BY "o"."order_id" ASC) AS "CurrentFrameLastValue"
    /// FROM "orders" AS "o"
    ///
    /// MySQL:
    /// SELECT `o`.`order_id` AS `OrderId`, `o`.`user_id`, `o`.`total`, FIRST_VALUE(`o`.`total`) OVER (PARTITION BY `o`.`user_id`
    /// ORDER BY `o`.`order_id` ASC) AS `FirstOrderTotal`, LAST_VALUE(`o`.`total`) OVER (PARTITION BY `o`.`user_id`
    /// ORDER BY `o`.`order_id` ASC) AS `CurrentFrameLastValue`
    /// FROM `orders` AS `o`
    /// </summary>
    public static class FirstLastValueWindowFunctionQueryValidator
    {        
        public static void Run()
        {
            var metadataResolver = ProviderMetadataFactory.CreateJoinMetadataResolver();

            ProviderQueryPrinter.Print(
                "SQL Server First Last Value Window Functions",
                BuildQuery(ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL First Last Value Window Functions",
                BuildQuery(ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL First Last Value Window Functions",
                BuildQuery(ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));
        }

        // Builds a query using FIRST_VALUE and LAST_VALUE window functions.
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
                .SelectFirstValue(
                    expression: (JoinOrder o) => o.Total,
                    alias: "FirstOrderTotal",
                    windowBuilder: window => window
                        .PartitionBy<JoinOrder>(o => o.UserId)
                        .OrderBy<JoinOrder>(o => o.Id))
                .SelectLastValue(
                    expression: (JoinOrder o) => o.Total,
                    alias: "CurrentFrameLastValue",
                    windowBuilder: window => window
                        .PartitionBy<JoinOrder>(o => o.UserId)
                        .OrderBy<JoinOrder>(o => o.Id))
                .Build();
        }
    }
}
