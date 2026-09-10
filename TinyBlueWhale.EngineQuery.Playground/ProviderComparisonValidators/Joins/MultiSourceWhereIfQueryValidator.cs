using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.Joins
{
    /// <summary>
    /// Validates conditional WHERE predicates across multiple joined sources,
    /// including exclusion of predicates whose condition evaluates to false.
    ///
    /// SQL Server:
    /// SELECT *
    /// FROM [users] AS [u]
    /// INNER JOIN [orders] AS [o] ON ([u].[user_id] = [o].[user_id])
    /// LEFT JOIN [order_items] AS [oi] ON ([o].[order_id] = [oi].[order_id])
    /// WHERE ([u].[is_active] = @p0) AND ([o].[total] &gt; @p1)
    ///
    /// PostgreSQL:
    /// SELECT *
    /// FROM "users" AS "u"
    /// INNER JOIN "orders" AS "o" ON ("u"."user_id" = "o"."user_id")
    /// LEFT JOIN "order_items" AS "oi" ON ("o"."order_id" = "oi"."order_id")
    /// WHERE ("u"."is_active" = @p0) AND ("o"."total" &gt; @p1)
    ///
    /// MySQL:
    /// SELECT *
    /// FROM `users` AS `u`
    /// INNER JOIN `orders` AS `o` ON (`u`.`user_id` = `o`.`user_id`)
    /// LEFT JOIN `order_items` AS `oi` ON (`o`.`order_id` = `oi`.`order_id`)
    /// WHERE (`u`.`is_active` = @p0) AND (`o`.`total` &gt; @p1)
    ///
    /// Expected parameters:
    /// @p0 = True
    /// @p1 = 100
    /// </summary>
    public static class MultiSourceWhereIfQueryValidator
    {
        public static void Run()
        {
            var metadataResolver = ProviderMetadataFactory.CreateJoinMetadataResolver();

            ProviderQueryPrinter.Print("SQL Server Multi-Source WhereIf", BuildQuery(ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));
            ProviderQueryPrinter.Print("PostgreSQL Multi-Source WhereIf", BuildQuery(ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));
            ProviderQueryPrinter.Print("MySQL Multi-Source WhereIf", BuildQuery(ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));
        }

        // Builds a joined query with conditional WHERE predicates from multiple sources.
        private static GeneratedSqlQuery BuildQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .InnerJoin<JoinUser, JoinOrder>(alias: "o", on: (u, o) => u.Id == o.UserId)
                .LeftJoin<JoinOrder, JoinOrderItem>(alias: "oi", on: (o, oi) => o.Id == oi.OrderId)
                .WhereIf<JoinUser>(true, u => u.IsActive)
                .WhereIf<JoinOrder>(true, o => o.Total > 100)
                .WhereIf<JoinOrderItem>(false, oi => oi.Quantity > 2)
                .Build();
        }
    }
}
