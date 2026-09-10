using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.Aggregates
{
    /// <summary>
    /// Validates aggregate HAVING predicates across supported database providers.
    ///
    /// SQL Server:
    /// SELECT [u].[user_id] AS [UserId], [u].[email], SUM([o].[total]) AS [TotalAmount], COUNT([o].[order_id]) AS [OrderCount]
    /// FROM [users] AS [u]
    /// INNER JOIN [orders] AS [o] ON ([u].[user_id] = [o].[user_id])
    /// GROUP BY [u].[user_id], [u].[email]
    /// HAVING SUM([o].[total]) &gt; @p0 AND COUNT([o].[order_id]) &gt;= @p1
    ///
    /// PostgreSQL:
    /// SELECT "u"."user_id" AS "UserId", "u"."email", SUM("o"."total") AS "TotalAmount", COUNT("o"."order_id") AS "OrderCount"
    /// FROM "users" AS "u"
    /// INNER JOIN "orders" AS "o" ON ("u"."user_id" = "o"."user_id")
    /// GROUP BY "u"."user_id", "u"."email"
    /// HAVING SUM("o"."total") &gt; @p0 AND COUNT("o"."order_id") &gt;= @p1
    ///
    /// MySQL:
    /// SELECT `u`.`user_id` AS `UserId`, `u`.`email`, SUM(`o`.`total`) AS `TotalAmount`, COUNT(`o`.`order_id`) AS `OrderCount`
    /// FROM `users` AS `u`
    /// INNER JOIN `orders` AS `o` ON (`u`.`user_id` = `o`.`user_id`)
    /// GROUP BY `u`.`user_id`, `u`.`email`
    /// HAVING SUM(`o`.`total`) &gt; @p0 AND COUNT(`o`.`order_id`) &gt;= @p1
    ///
    /// Expected parameters:
    /// @p0 = 1000
    /// @p1 = 2
    /// </summary>
    public static class HavingQueryValidator
    {
        public static void Run()
        {
            var metadataResolver = ProviderMetadataFactory.CreateJoinMetadataResolver();

            ProviderQueryPrinter.Print("SQL Server Having", BuildQuery(ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));
            ProviderQueryPrinter.Print("PostgreSQL Having", BuildQuery(ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));
            ProviderQueryPrinter.Print("MySQL Having", BuildQuery(ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));
        }

        private static GeneratedSqlQuery BuildQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .InnerJoin<JoinUser, JoinOrder>(alias: "o", on: (u, o) => u.Id == o.UserId)
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .SelectAggregate<JoinOrder>(
                    QueryAggregateFunction.Sum,
                    o => o.Total,
                    alias: "TotalAmount")
                .SelectAggregate<JoinOrder>(
                    QueryAggregateFunction.Count,
                    o => o.Id,
                    alias: "OrderCount")
                .GroupBy<JoinUser>(u => new
                {
                    u.Id,
                    u.Email
                })
                .HavingAggregate<JoinOrder>(
                    QueryAggregateFunction.Sum,
                    o => o.Total,
                    QueryComparisonOperator.GreaterThan,
                    1000)
                .HavingAggregate<JoinOrder>(
                    QueryAggregateFunction.Count,
                    o => o.Id,
                    QueryComparisonOperator.GreaterThanOrEqual,
                    2)
                .Build();
        }
    }
}
