using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.Subqueries
{

    /// <summary>
    /// Validates derived tables used as root query sources, including aggregate
    /// projections, grouping and outer references to projected aliases.
    ///
    /// SQL Server:
    /// SELECT [summary].[UserId], [summary].[TotalAmount], [summary].[OrderCount]
    /// FROM (SELECT [o].[user_id] AS [UserId], SUM([o].[total]) AS [TotalAmount], COUNT([o].[order_id]) AS [OrderCount]
    /// FROM [orders] AS [o]
    /// GROUP BY [o].[user_id]) AS [summary]
    /// WHERE ([summary].[TotalAmount] &gt; @p0)
    /// ORDER BY [summary].[TotalAmount] DESC
    ///
    /// PostgreSQL:
    /// SELECT "summary"."UserId", "summary"."TotalAmount", "summary"."OrderCount"
    /// FROM (SELECT "o"."user_id" AS "UserId", SUM("o"."total") AS "TotalAmount", COUNT("o"."order_id") AS "OrderCount"
    /// FROM "orders" AS "o"
    /// GROUP BY "o"."user_id") AS "summary"
    /// WHERE ("summary"."TotalAmount" &gt; @p0)
    /// ORDER BY "summary"."TotalAmount" DESC
    ///
    /// MySQL:
    /// SELECT `summary`.`UserId`, `summary`.`TotalAmount`, `summary`.`OrderCount`
    /// FROM (SELECT `o`.`user_id` AS `UserId`, SUM(`o`.`total`) AS `TotalAmount`, COUNT(`o`.`order_id`) AS `OrderCount`
    /// FROM `orders` AS `o`
    /// GROUP BY `o`.`user_id`) AS `summary`
    /// WHERE (`summary`.`TotalAmount` &gt; @p0)
    /// ORDER BY `summary`.`TotalAmount` DESC
    ///
    /// Expected parameters:
    /// @p0 = 500
    /// </summary>
    public static class DerivedTableQueryValidator
    {
        public static void Run()
        {
            var metadataResolver = ProviderMetadataFactory.CreateJoinMetadataResolver();

            ProviderQueryPrinter.Print(
                "SQL Server Derived Table",
                BuildQuery(ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL Derived Table",
                BuildQuery(ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL Derived Table",
                BuildQuery(ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));
        }

        // Builds a query using a derived table as the root source.
        private static GeneratedSqlQuery BuildQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .FromSubquery<OrderSummary, JoinOrder>(
                    alias: "summary",
                    subquery => subquery
                        .From<JoinOrder>(alias: "o")
                        .Select<JoinOrder>(o => new
                        {
                            o.UserId
                        })
                        .SelectAggregate<JoinOrder>(
                            QueryAggregateFunction.Sum,
                            o => o.Total,
                            alias: "TotalAmount")
                        .SelectAggregate<JoinOrder>(
                            QueryAggregateFunction.Count,
                            o => o.Id,
                            alias: "OrderCount")
                        .GroupBy<JoinOrder>(o => o.UserId))
                .Select<OrderSummary>(summary => new
                {
                    summary.UserId,
                    summary.TotalAmount,
                    summary.OrderCount
                })
                .WhereComputed<OrderSummary>(
                    summary => summary.TotalAmount > 500)
                .OrderByDescending<OrderSummary>(
                    summary => summary.TotalAmount)
                .Build();
        }
    }
}
