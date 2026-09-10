using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Abstractions.Extensions;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding;
using TinyBlueWhale.EngineQuery.MySql.Profiles;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;
using TinyBlueWhale.EngineQuery.PostgreSql.Profiles;
using TinyBlueWhale.EngineQuery.SqlServer.Profiles;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.CommonTableExpressions
{

    /// <summary>
    /// Validates common table expression generation with aggregate projections,
    /// filtering and ordering across supported database providers.
    ///
    /// SQL Server:
    /// WITH [order_summary] AS (SELECT [o].[user_id] AS [UserId], SUM([o].[total]) AS [TotalAmount], COUNT([o].[order_id]) AS [OrderCount]
    /// FROM [orders] AS [o]
    /// GROUP BY [o].[user_id])
    /// SELECT [UserId], [TotalAmount], [OrderCount]
    /// FROM [order_summary]
    /// WHERE ([TotalAmount] &gt; @p0)
    /// ORDER BY [TotalAmount] DESC
    ///
    /// PostgreSQL:
    /// WITH "order_summary" AS (SELECT "o"."user_id" AS "UserId", SUM("o"."total") AS "TotalAmount", COUNT("o"."order_id") AS "OrderCount"
    /// FROM "orders" AS "o"
    /// GROUP BY "o"."user_id")
    /// SELECT "UserId", "TotalAmount", "OrderCount"
    /// FROM "order_summary"
    /// WHERE ("TotalAmount" &gt; @p0)
    /// ORDER BY "TotalAmount" DESC
    ///
    /// MySQL:
    /// WITH `order_summary` AS (SELECT `o`.`user_id` AS `UserId`, SUM(`o`.`total`) AS `TotalAmount`, COUNT(`o`.`order_id`) AS `OrderCount`
    /// FROM `orders` AS `o`
    /// GROUP BY `o`.`user_id`)
    /// SELECT `UserId`, `TotalAmount`, `OrderCount`
    /// FROM `order_summary`
    /// WHERE (`TotalAmount` &gt; @p0)
    /// ORDER BY `TotalAmount` DESC
    ///
    /// Expected parameters:
    /// @p0 = 500
    /// </summary>
    public static class CommonTableExpressionQueryValidator
    {
        public static void Run()
        {
            var metadataResolver = ProviderMetadataFactory.CreateJoinMetadataResolver();

            ProviderQueryPrinter.Print(
                "SQL Server CTE",
                BuildSqlServerQuery(
                    ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL CTE",
                BuildPostgreSqlQuery(
                    ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL CTE",
                BuildMySqlQuery(
                    ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));
        }

        private static GeneratedSqlQuery BuildSqlServerQuery(QueryBuilder<SqlServer2012Profile> queryBuilder)
        {
            return queryBuilder
                .With<OrderSummary, JoinOrder>(
                    "order_summary",
                    cte => cte
                    .From<JoinOrder>(alias: "o")
                    .Select<JoinOrder>(o => new
                    {
                        UserId = o.UserId
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
                .FromCte<OrderSummary>("order_summary")
                .Select<OrderSummary>(summary => new
                {
                    summary.UserId,
                    summary.TotalAmount,
                    summary.OrderCount
                })
                .WhereComputed<OrderSummary>(summary => summary.TotalAmount > 500)
                .OrderByDescending<OrderSummary>(summary => summary.TotalAmount)
                .Build();
        }

        private static GeneratedSqlQuery BuildPostgreSqlQuery(QueryBuilder<PostgreSql93Profile> queryBuilder)
        {
            return queryBuilder
                .With<OrderSummary, JoinOrder>(
                    "order_summary",
                    cte => cte
                    .From<JoinOrder>(alias: "o")
                    .Select<JoinOrder>(o => new
                    {
                        UserId = o.UserId
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
                .FromCte<OrderSummary>("order_summary")
                .Select<OrderSummary>(summary => new
                {
                    summary.UserId,
                    summary.TotalAmount,
                    summary.OrderCount
                })
                .WhereComputed<OrderSummary>(summary => summary.TotalAmount > 500)
                .OrderByDescending<OrderSummary>(summary => summary.TotalAmount)
                .Build();
        }
        private static GeneratedSqlQuery BuildMySqlQuery(QueryBuilder<MySql8031Profile> queryBuilder)
        {
            return queryBuilder
                .With<OrderSummary, JoinOrder>(
                    "order_summary",
                    cte => cte
                    .From<JoinOrder>(alias: "o")
                    .Select<JoinOrder>(o => new
                    {
                        UserId = o.UserId
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
                .FromCte<OrderSummary>("order_summary")
                .Select<OrderSummary>(summary => new
                {
                    summary.UserId,
                    summary.TotalAmount,
                    summary.OrderCount
                })
                .WhereComputed<OrderSummary>(summary => summary.TotalAmount > 500)
                .OrderByDescending<OrderSummary>(summary => summary.TotalAmount)
                .Build();
        }
    }
}
