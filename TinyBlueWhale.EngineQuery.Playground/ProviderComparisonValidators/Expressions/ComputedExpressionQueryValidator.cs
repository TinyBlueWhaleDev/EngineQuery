using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.Expressions
{
    /// <summary>
    /// Validates arithmetic computed projections and parameter ordering across supported database providers.
    ///
    /// SQL Server:
    /// SELECT [o].[order_id] AS [OrderId], [o].[total], ([o].[total] * @p0) AS [TotalWithTax], (([o].[total] * @p1) - @p2) AS [FinalAmount]
    /// FROM [orders] AS [o]
    ///
    /// PostgreSQL:
    /// SELECT "o"."order_id" AS "OrderId", "o"."total", ("o"."total" * @p0) AS "TotalWithTax", (("o"."total" * @p1) - @p2) AS "FinalAmount"
    /// FROM "orders" AS "o"
    ///
    /// MySQL:
    /// SELECT `o`.`order_id` AS `OrderId`, `o`.`total`, (`o`.`total` * @p0) AS `TotalWithTax`, ((`o`.`total` * @p1) - @p2) AS `FinalAmount`
    /// FROM `orders` AS `o`
    ///
    /// Expected parameters:
    /// @p0 = 1.16
    /// @p1 = 1.16
    /// @p2 = 100
    /// </summary>
    public static class ComputedExpressionQueryValidator
    {
        public static void Run()
        {
            var metadataResolver = ProviderMetadataFactory.CreateJoinMetadataResolver();

            ProviderQueryPrinter.Print(
                "SQL Server Computed Expressions",
                BuildQuery(ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL Computed Expressions",
                BuildQuery(ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL Computed Expressions",
                BuildQuery(ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));
        }

        private static GeneratedSqlQuery BuildQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .From<JoinOrder>(alias: "o")
                .Select<JoinOrder>(o => new
                {
                    OrderId = o.Id,
                    o.Total
                })
                .SelectComputed<JoinOrder>(
                    o => o.Total * 1.16m,
                    alias: "TotalWithTax")
                .SelectComputed<JoinOrder>(
                    o => (o.Total * 1.16m) - 100,
                    alias: "FinalAmount")
                .Build();
        }
    }
}
