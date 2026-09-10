using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.Aggregates
{
    /// <summary>
    /// Validates aggregate projections over computed expressions across supported database providers.
    ///
    /// SQL Server:
    /// SELECT [u].[user_id] AS [UserId], [u].[email], SUM(([o].[total] * @p0)) AS [TotalWithTax], AVG((([o].[total] * @p1) - @p2)) AS [AverageFinalAmount], MIN(([o].[total] * @p3)) AS [MinimumTotalWithTax], MAX((([o].[total] * @p4) - @p5)) AS [MaximumFinalAmount]
    /// FROM [users] AS [u]
    /// INNER JOIN [orders] AS [o] ON ([u].[user_id] = [o].[user_id])
    /// GROUP BY [u].[user_id], [u].[email]
    ///
    /// PostgreSQL:
    /// SELECT "u"."user_id" AS "UserId", "u"."email", SUM(("o"."total" * @p0)) AS "TotalWithTax", AVG((("o"."total" * @p1) - @p2)) AS "AverageFinalAmount", MIN(("o"."total" * @p3)) AS "MinimumTotalWithTax", MAX((("o"."total" * @p4) - @p5)) AS "MaximumFinalAmount"
    /// FROM "users" AS "u"
    /// INNER JOIN "orders" AS "o" ON ("u"."user_id" = "o"."user_id")
    /// GROUP BY "u"."user_id", "u"."email"
    ///
    /// MySQL:
    /// SELECT `u`.`user_id` AS `UserId`, `u`.`email`, SUM((`o`.`total` * @p0)) AS `TotalWithTax`, AVG(((`o`.`total` * @p1) - @p2)) AS `AverageFinalAmount`, MIN((`o`.`total` * @p3)) AS `MinimumTotalWithTax`, MAX(((`o`.`total` * @p4) - @p5)) AS `MaximumFinalAmount`
    /// FROM `users` AS `u`
    /// INNER JOIN `orders` AS `o` ON (`u`.`user_id` = `o`.`user_id`)
    /// GROUP BY `u`.`user_id`, `u`.`email`
    ///
    /// Expected parameters:
    /// @p0 = 1.16
    /// @p1 = 1.16
    /// @p2 = 100
    /// @p3 = 1.16
    /// @p4 = 1.16
    /// @p5 = 100
    /// </summary>
    public static class AggregateComputedExpressionQueryValidator
    {
        public static void Run()
        {
            var metadataResolver = ProviderMetadataFactory.CreateJoinMetadataResolver();

            ProviderQueryPrinter.Print(
                "SQL Server Aggregate Computed Expression",
                BuildQuery(ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL Aggregate Computed Expression",
                BuildQuery(ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL Aggregate Computed Expression",
                BuildQuery(ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));
        }
        private static GeneratedSqlQuery BuildQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .InnerJoin<JoinUser, JoinOrder>(
                    alias: "o",
                    on: (u, o) => u.Id == o.UserId)
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .SelectAggregate<JoinOrder>(
                    QueryAggregateFunction.Sum,
                    o => o.Total * 1.16m,
                    alias: "TotalWithTax")
                .SelectAggregate<JoinOrder>(
                    QueryAggregateFunction.Average,
                    o => (o.Total * 1.16m) - 100,
                    alias: "AverageFinalAmount")
                .SelectAggregate<JoinOrder>(
                    QueryAggregateFunction.Minimum,
                    o => o.Total * 1.16m,
                    alias: "MinimumTotalWithTax")
                .SelectAggregate<JoinOrder>(
                    QueryAggregateFunction.Maximum,
                    o => (o.Total * 1.16m) - 100,
                    alias: "MaximumFinalAmount")
                .GroupBy<JoinUser>(u => new
                {
                    u.Id,
                    u.Email
                })
                .Build();
        }
    }
}
