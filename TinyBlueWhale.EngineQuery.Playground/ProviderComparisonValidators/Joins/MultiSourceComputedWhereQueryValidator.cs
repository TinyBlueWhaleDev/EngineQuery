using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.Joins
{
    /// <summary>
    /// Validates computed WHERE predicates that reference columns from multiple
    /// joined sources while preserving source qualification and logical grouping.
    ///
    /// SQL Server:
    /// SELECT [u].[user_id] AS [UserId], [u].[email]
    /// FROM [users] AS [u]
    /// INNER JOIN [orders] AS [o] ON ([u].[user_id] = [o].[user_id])
    /// LEFT JOIN [order_items] AS [oi] ON ([o].[order_id] = [oi].[order_id])
    /// WHERE (([o].[user_id] = [u].[user_id]) AND ([o].[total] &gt; @p0)) AND (([oi].[order_id] = [o].[order_id]) AND ([oi].[quantity] &lt; @p1))
    ///
    /// PostgreSQL:
    /// SELECT "u"."user_id" AS "UserId", "u"."email"
    /// FROM "users" AS "u"
    /// INNER JOIN "orders" AS "o" ON ("u"."user_id" = "o"."user_id")
    /// LEFT JOIN "order_items" AS "oi" ON ("o"."order_id" = "oi"."order_id")
    /// WHERE (("o"."user_id" = "u"."user_id") AND ("o"."total" &gt; @p0)) AND (("oi"."order_id" = "o"."order_id") AND ("oi"."quantity" &lt; @p1))
    ///
    /// MySQL:
    /// SELECT `u`.`user_id` AS `UserId`, `u`.`email`
    /// FROM `users` AS `u`
    /// INNER JOIN `orders` AS `o` ON (`u`.`user_id` = `o`.`user_id`)
    /// LEFT JOIN `order_items` AS `oi` ON (`o`.`order_id` = `oi`.`order_id`)
    /// WHERE ((`o`.`user_id` = `u`.`user_id`) AND (`o`.`total` &gt; @p0)) AND ((`oi`.`order_id` = `o`.`order_id`) AND (`oi`.`quantity` &lt; @p1))
    ///
    /// Expected parameters:
    /// @p0 = 10
    /// @p1 = 100
    /// </summary>
    public static class MultiSourceComputedWhereQueryValidator
    {
        public static void Run()
        {
            var metadataResolver = ProviderMetadataFactory.CreateJoinMetadataResolver();

            ProviderQueryPrinter.Print(
                "SQL Server Multi-Source Computed Where",
                BuildQuery(ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL Multi-Source Computed Where",
                BuildQuery(ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL Multi-Source Computed Where",
                BuildQuery(ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));
        }

        // Builds a joined query with a multi-source computed WHERE predicate.
        private static GeneratedSqlQuery BuildQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
               where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .InnerJoin<JoinUser, JoinOrder>(
                    alias: "o",
                    on: (u, o) => u.Id == o.UserId)
                .LeftJoin<JoinOrder, JoinOrderItem>(
                    alias: "oi",
                    on: (o, oi) => o.Id == oi.OrderId)
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .WhereComputed<JoinOrder, JoinUser>(
                    (o, u) => o.UserId == u.Id && o.Total > 10)
                .WhereComputed<JoinOrderItem, JoinOrder>(
                    (oi, o) => oi.OrderId == o.Id && oi.Quantity < 100)
                .Build();
        }
    }
}
