using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.Expressions
{

    /// <summary>
    /// Validates CASE WHEN projections with logical predicates and parameterized result values
    /// across supported database providers.
    ///
    /// SQL Server:
    /// SELECT [o].[order_id] AS [OrderId], [o].[total], CASE WHEN (([o].[total] &gt; @p0) AND ([o].[total] &lt; @p1)) THEN @p2 ELSE @p3 END AS [CustomerType], CASE WHEN ([o].[total] &lt;= @p4) THEN @p5 ELSE @p6 END AS [OrderStatus]
    /// FROM [orders] AS [o]
    ///
    /// PostgreSQL:
    /// SELECT "o"."order_id" AS "OrderId", "o"."total", CASE WHEN (("o"."total" &gt; @p0) AND ("o"."total" &lt; @p1)) THEN @p2 ELSE @p3 END AS "CustomerType", CASE WHEN ("o"."total" &lt;= @p4) THEN @p5 ELSE @p6 END AS "OrderStatus"
    /// FROM "orders" AS "o"
    ///
    /// MySQL:
    /// SELECT `o`.`order_id` AS `OrderId`, `o`.`total`, CASE WHEN ((`o`.`total` &gt; @p0) AND (`o`.`total` &lt; @p1)) THEN @p2 ELSE @p3 END AS `CustomerType`, CASE WHEN (`o`.`total` &lt;= @p4) THEN @p5 ELSE @p6 END AS `OrderStatus`
    /// FROM `orders` AS `o`
    ///
    /// Expected parameters:
    /// @p0 = 1000
    /// @p1 = 5000
    /// @p2 = VIP
    /// @p3 = STANDARD
    /// @p4 = 0
    /// @p5 = INVALID
    /// @p6 = VALID
    /// </summary>
    public static class CaseWhenQueryValidator
    {
        public static void Run()
        {
            var metadataResolver = ProviderMetadataFactory.CreateJoinMetadataResolver();

            ProviderQueryPrinter.Print(
                "SQL Server CASE WHEN",
                BuildQuery(ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL CASE WHEN",
                BuildQuery(ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL CASE WHEN",
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
                .SelectCaseWhen<JoinOrder>(
                    o => o.Total > 1000 && o.Total < 5000,
                    whenTrue: "VIP",
                    whenFalse: "STANDARD",
                    alias: "CustomerType")
                .SelectCaseWhen<JoinOrder>(
                    o => o.Total <= 0,
                    whenTrue: "INVALID",
                    whenFalse: "VALID",
                    alias: "OrderStatus")
                .Build();
        }
    }
}
