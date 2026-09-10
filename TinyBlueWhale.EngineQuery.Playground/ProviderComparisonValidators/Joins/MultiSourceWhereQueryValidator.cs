using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.Joins
{
    /// <summary>
    /// Validates WHERE predicates targeting multiple joined sources and their
    /// parameter ordering across supported database providers.
    ///
    /// SQL Server:
    /// SELECT *
    /// FROM [users] AS [u]
    /// INNER JOIN [orders] AS [o] ON ([u].[user_id] = [o].[user_id])
    /// LEFT JOIN [order_items] AS [oi] ON ([o].[order_id] = [oi].[order_id])
    /// WHERE ([u].[is_active] = @p0) AND ([o].[total] &gt; @p1) AND ([oi].[quantity] &gt; @p2)
    ///
    /// PostgreSQL:
    /// SELECT *
    /// FROM "users" AS "u"
    /// INNER JOIN "orders" AS "o" ON ("u"."user_id" = "o"."user_id")
    /// LEFT JOIN "order_items" AS "oi" ON ("o"."order_id" = "oi"."order_id")
    /// WHERE ("u"."is_active" = @p0) AND ("o"."total" &gt; @p1) AND ("oi"."quantity" &gt; @p2)
    ///
    /// MySQL:
    /// SELECT *
    /// FROM `users` AS `u`
    /// INNER JOIN `orders` AS `o` ON (`u`.`user_id` = `o`.`user_id`)
    /// LEFT JOIN `order_items` AS `oi` ON (`o`.`order_id` = `oi`.`order_id`)
    /// WHERE (`u`.`is_active` = @p0) AND (`o`.`total` &gt; @p1) AND (`oi`.`quantity` &gt; @p2)
    ///
    /// Expected parameters:
    /// @p0 = True
    /// @p1 = 100
    /// @p2 = 2
    /// </summary>
    public static class MultiSourceWhereQueryValidator
    {
        public static void Run()
        {
            var metadataResolver = ProviderMetadataFactory.CreateJoinMetadataResolver();

            ProviderQueryPrinter.Print("SQL Server Multi-Source Where", BuildQuery(ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));
            ProviderQueryPrinter.Print("PostgreSQL Multi-Source Where", BuildQuery(ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));
            ProviderQueryPrinter.Print("MySQL Multi-Source Where", BuildQuery(ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));
        }

        // Builds a joined query with WHERE predicates from multiple sources.
        private static GeneratedSqlQuery BuildQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .InnerJoin<JoinUser, JoinOrder>(alias: "o", on: (u, o) => u.Id == o.UserId)
                .LeftJoin<JoinOrder, JoinOrderItem>(alias: "oi", on: (o, oi) => o.Id == oi.OrderId)
                .Where<JoinUser>(u => u.IsActive)
                .Where<JoinOrder>(o => o.Total > 100)
                .Where<JoinOrderItem>(oi => oi.Quantity > 2)
                .Build();
        }
    }
}
