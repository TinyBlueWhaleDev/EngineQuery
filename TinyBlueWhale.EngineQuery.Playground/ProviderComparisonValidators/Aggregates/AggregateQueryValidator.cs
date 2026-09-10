using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.Aggregates
{
    /// <summary>
    /// Validates aggregate projections with GROUP BY across supported database providers.
    ///
    /// SQL Server:
    /// SELECT [u].[user_id] AS [UserId], [u].[email], SUM([o].[total]) AS [TotalAmount], COUNT([o].[order_id]) AS [OrderCount]
    /// FROM [users] AS [u]
    /// INNER JOIN [orders] AS [o] ON ([u].[user_id] = [o].[user_id])
    /// GROUP BY [u].[user_id], [u].[email]
    ///
    /// PostgreSQL:
    /// SELECT "u"."user_id" AS "UserId", "u"."email", SUM("o"."total") AS "TotalAmount", COUNT("o"."order_id") AS "OrderCount"
    /// FROM "users" AS "u"
    /// INNER JOIN "orders" AS "o" ON ("u"."user_id" = "o"."user_id")
    /// GROUP BY "u"."user_id", "u"."email"
    ///
    /// MySQL:
    /// SELECT `u`.`user_id` AS `UserId`, `u`.`email`, SUM(`o`.`total`) AS `TotalAmount`, COUNT(`o`.`order_id`) AS `OrderCount`
    /// FROM `users` AS `u`
    /// INNER JOIN `orders` AS `o` ON (`u`.`user_id` = `o`.`user_id`)
    /// GROUP BY `u`.`user_id`, `u`.`email`
    /// </summary>
    public static class AggregateQueryValidator
    {
        public static void Run()
        {
            var metadataResolver = ProviderMetadataFactory.CreateJoinMetadataResolver();

            ProviderQueryPrinter.Print("SQL Server Aggregate", BuildQuery(ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));
            ProviderQueryPrinter.Print("PostgreSQL Aggregate", BuildQuery(ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));
            ProviderQueryPrinter.Print("MySQL Aggregate", BuildQuery(ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));
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
                .Build();
        }
    }
}
