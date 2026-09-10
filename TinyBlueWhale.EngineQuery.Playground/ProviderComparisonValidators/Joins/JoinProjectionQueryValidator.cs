using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.Joins
{
    /// <summary>
    /// Validates projections from multiple joined sources with explicit projection aliases
    /// and metadata-resolved columns across supported database providers.
    ///
    /// SQL Server:
    /// SELECT [u].[user_id] AS [UserId], [u].[email], [o].[order_id] AS [OrderId], [o].[user_id] AS [OrderUserId], [o].[total], [oi].[order_item_id] AS [OrderItemId], [oi].[quantity]
    /// FROM [users] AS [u]
    /// INNER JOIN [orders] AS [o] ON ([u].[user_id] = [o].[user_id])
    /// LEFT JOIN [order_items] AS [oi] ON ([o].[order_id] = [oi].[order_id])
    ///
    /// PostgreSQL:
    /// SELECT "u"."user_id" AS "UserId", "u"."email", "o"."order_id" AS "OrderId", "o"."user_id" AS "OrderUserId", "o"."total", "oi"."order_item_id" AS "OrderItemId", "oi"."quantity"
    /// FROM "users" AS "u"
    /// INNER JOIN "orders" AS "o" ON ("u"."user_id" = "o"."user_id")
    /// LEFT JOIN "order_items" AS "oi" ON ("o"."order_id" = "oi"."order_id")
    ///
    /// MySQL:
    /// SELECT `u`.`user_id` AS `UserId`, `u`.`email`, `o`.`order_id` AS `OrderId`, `o`.`user_id` AS `OrderUserId`, `o`.`total`, `oi`.`order_item_id` AS `OrderItemId`, `oi`.`quantity`
    /// FROM `users` AS `u`
    /// INNER JOIN `orders` AS `o` ON (`u`.`user_id` = `o`.`user_id`)
    /// LEFT JOIN `order_items` AS `oi` ON (`o`.`order_id` = `oi`.`order_id`)
    /// </summary>
    public static class JoinProjectionQueryValidator
    {
        public static void Run()
        {
            var metadataResolver = ProviderMetadataFactory.CreateJoinMetadataResolver();

            ProviderQueryPrinter.Print("SQL Server Join Projection", BuildQuery(ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));
            ProviderQueryPrinter.Print("PostgreSQL Join Projection", BuildQuery(ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));
            ProviderQueryPrinter.Print("MySQL Join Projection", BuildQuery(ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));
        }

        // Builds a joined query with projections from multiple sources.
        private static GeneratedSqlQuery BuildQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .InnerJoin<JoinUser, JoinOrder>(alias: "o", on: (u, o) => u.Id == o.UserId)
                .LeftJoin<JoinOrder, JoinOrderItem>(alias: "oi", on: (o, oi) => o.Id == oi.OrderId)
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .Select<JoinOrder>(o => new
                {
                    OrderId = o.Id,
                    OrderUserId = o.UserId,
                    o.Total
                })
                .Select<JoinOrderItem>(oi => new
                {
                    OrderItemId = oi.Id,
                    oi.Quantity
                })
                .Build();
        }
    }
}
