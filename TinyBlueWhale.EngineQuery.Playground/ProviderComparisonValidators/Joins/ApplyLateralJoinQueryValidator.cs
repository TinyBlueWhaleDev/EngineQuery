using TinyBlueWhale.EngineQuery.Abstractions.Extensions;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.MySql.Profiles;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;
using TinyBlueWhale.EngineQuery.PostgreSql.Profiles;
using TinyBlueWhale.EngineQuery.SqlServer.Profiles;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.Joins
{
    /// <summary>
    /// Validates correlated APPLY and LATERAL joins, provider-specific lateral syntax,
    /// inner result limiting and composition with outer filtering, ordering and pagination.
    ///
    /// SQL Server CROSS APPLY:
    /// SELECT [u].[user_id] AS [UserId], [u].[email]
    /// FROM [users] AS [u]
    /// CROSS APPLY (SELECT [latest_order].[order_id] AS [OrderId], [latest_order].[user_id] AS [UserId], [latest_order].[total] AS [Total]
    /// FROM [orders] AS [latest_order]
    /// WHERE ([latest_order].[user_id] = [u].[user_id])
    /// ORDER BY [latest_order].[total] DESC
    /// OFFSET 0 ROWS FETCH NEXT 1 ROWS ONLY) AS [latest_order]
    ///
    /// PostgreSQL LATERAL JOIN:
    /// SELECT "u"."user_id" AS "UserId", "u"."email"
    /// FROM "users" AS "u"
    /// JOIN LATERAL (SELECT "latest_order"."order_id" AS "OrderId", "latest_order"."user_id" AS "UserId", "latest_order"."total" AS "Total"
    /// FROM "orders" AS "latest_order"
    /// WHERE ("latest_order"."user_id" = "u"."user_id")
    /// ORDER BY "latest_order"."total" DESC
    /// LIMIT 1) AS "latest_order" ON TRUE
    ///
    /// MySQL LATERAL JOIN:
    /// SELECT `u`.`user_id` AS `UserId`, `u`.`email`
    /// FROM `users` AS `u`
    /// JOIN LATERAL (SELECT `latest_order`.`order_id` AS `OrderId`, `latest_order`.`user_id` AS `UserId`, `latest_order`.`total` AS `Total`
    /// FROM `orders` AS `latest_order`
    /// WHERE (`latest_order`.`user_id` = `u`.`user_id`)
    /// ORDER BY `latest_order`.`total` DESC
    /// LIMIT 1) AS `latest_order` ON TRUE
    ///
    /// SQL Server OUTER APPLY:
    /// SELECT [u].[user_id] AS [UserId], [u].[email]
    /// FROM [users] AS [u]
    /// OUTER APPLY (SELECT [latest_order].[order_id] AS [OrderId], [latest_order].[user_id] AS [UserId], [latest_order].[total] AS [Total]
    /// FROM [orders] AS [latest_order]
    /// WHERE ([latest_order].[user_id] = [u].[user_id])
    /// ORDER BY [latest_order].[total] DESC
    /// OFFSET 0 ROWS FETCH NEXT 1 ROWS ONLY) AS [latest_order]
    ///
    /// PostgreSQL LEFT LATERAL JOIN:
    /// SELECT "u"."user_id" AS "UserId", "u"."email"
    /// FROM "users" AS "u"
    /// LEFT JOIN LATERAL (SELECT "latest_order"."order_id" AS "OrderId", "latest_order"."user_id" AS "UserId", "latest_order"."total" AS "Total"
    /// FROM "orders" AS "latest_order"
    /// WHERE ("latest_order"."user_id" = "u"."user_id")
    /// ORDER BY "latest_order"."total" DESC
    /// LIMIT 1) AS "latest_order" ON TRUE
    ///
    /// MySQL LEFT LATERAL JOIN:
    /// SELECT `u`.`user_id` AS `UserId`, `u`.`email`
    /// FROM `users` AS `u`
    /// LEFT JOIN LATERAL (SELECT `latest_order`.`order_id` AS `OrderId`, `latest_order`.`user_id` AS `UserId`, `latest_order`.`total` AS `Total`
    /// FROM `orders` AS `latest_order`
    /// WHERE (`latest_order`.`user_id` = `u`.`user_id`)
    /// ORDER BY `latest_order`.`total` DESC
    /// LIMIT 1) AS `latest_order` ON TRUE
    ///
    /// SQL Server composition:
    /// SELECT [u].[user_id] AS [UserId], [u].[email]
    /// FROM [users] AS [u]
    /// CROSS APPLY (SELECT [latest_order].[order_id] AS [OrderId], [latest_order].[user_id] AS [UserId], [latest_order].[total] AS [Total]
    /// FROM [orders] AS [latest_order]
    /// WHERE ([latest_order].[user_id] = [u].[user_id])
    /// ORDER BY [latest_order].[total] DESC
    /// OFFSET 0 ROWS FETCH NEXT 1 ROWS ONLY) AS [latest_order]
    /// WHERE ([u].[is_active] = @p0)
    /// ORDER BY [u].[user_id] ASC
    /// OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY
    ///
    /// PostgreSQL composition:
    /// SELECT "u"."user_id" AS "UserId", "u"."email"
    /// FROM "users" AS "u"
    /// JOIN LATERAL (SELECT "latest_order"."order_id" AS "OrderId", "latest_order"."user_id" AS "UserId", "latest_order"."total" AS "Total"
    /// FROM "orders" AS "latest_order"
    /// WHERE ("latest_order"."user_id" = "u"."user_id")
    /// ORDER BY "latest_order"."total" DESC
    /// LIMIT 1) AS "latest_order" ON TRUE
    /// WHERE ("u"."is_active" = @p0)
    /// ORDER BY "u"."user_id" ASC
    /// LIMIT 10
    ///
    /// MySQL composition:
    /// SELECT `u`.`user_id` AS `UserId`, `u`.`email`
    /// FROM `users` AS `u`
    /// JOIN LATERAL (SELECT `latest_order`.`order_id` AS `OrderId`, `latest_order`.`user_id` AS `UserId`, `latest_order`.`total` AS `Total`
    /// FROM `orders` AS `latest_order`
    /// WHERE (`latest_order`.`user_id` = `u`.`user_id`)
    /// ORDER BY `latest_order`.`total` DESC
    /// LIMIT 1) AS `latest_order` ON TRUE
    /// WHERE (`u`.`is_active` = @p0)
    /// ORDER BY `u`.`user_id` ASC
    /// LIMIT 10
    ///
    /// Expected composition parameters:
    /// @p0 = True
    /// </summary>
    public static class ApplyLateralJoinQueryValidator
    {
        /// <summary>
        /// Runs APPLY, LATERAL join, and clause composition validation scenarios.
        /// </summary>
        public static void Run()
        {
            var metadataResolver = ProviderMetadataFactory.CreateJoinMetadataResolver();

            ProviderQueryPrinter.Print(
                "SQL Server CROSS APPLY",
                BuildSqlServerCrossApplyQuery(
                    ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL LATERAL JOIN",
                BuildPostgreSqlCrossApplyQuery(
                    ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL LATERAL JOIN",
                BuildMySqlCrossApplyQuery(
                    ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "SQL Server OUTER APPLY",
                BuildSqlServerOuterApplyQuery(
                    ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL LEFT LATERAL JOIN",
                BuildPostgreSqlOuterApplyQuery(
                    ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL LEFT LATERAL JOIN",
                BuildMySqlOuterApplyQuery(
                    ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "SQL Server Lateral Composition",
                BuildSqlServerLateralCompositionQuery(
                    ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL Lateral Composition",
                BuildPostgreSqlLateralCompositionQuery(
                    ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL Lateral Composition",
                BuildMySqlLateralCompositionQuery(
                    ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));
        }

        private static GeneratedSqlQuery BuildSqlServerCrossApplyQuery(IQueryBuilder<SqlServer2012Profile> queryBuilder)
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .CrossApply<JoinUser, JoinOrder>(
                    alias: "latest_order",
                    applyBuilder: apply => apply
                        .Select<JoinOrder>(o => new
                        {
                            OrderId = o.Id,
                            o.UserId,
                            o.Total
                        })
                        .WhereComputed<JoinOrder, JoinUser>(
                            (o, u) => o.UserId == u.Id)
                        .OrderByDescending<JoinOrder>(o => o.Total)
                        .Take(1))
                .Build();
        }

        private static GeneratedSqlQuery BuildPostgreSqlCrossApplyQuery(IQueryBuilder<PostgreSql93Profile> queryBuilder)
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .CrossApply<JoinUser, JoinOrder>(
                    alias: "latest_order",
                    applyBuilder: apply => apply
                        .Select<JoinOrder>(o => new
                        {
                            OrderId = o.Id,
                            o.UserId,
                            o.Total
                        })
                        .WhereComputed<JoinOrder, JoinUser>(
                            (o, u) => o.UserId == u.Id)
                        .OrderByDescending<JoinOrder>(o => o.Total)
                        .Take(1))
                .Build();
        }
        
        private static GeneratedSqlQuery BuildMySqlCrossApplyQuery(IQueryBuilder<MySql8031Profile> queryBuilder)
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .CrossApply<JoinUser, JoinOrder>(
                    alias: "latest_order",
                    applyBuilder: apply => apply
                        .Select<JoinOrder>(o => new
                        {
                            OrderId = o.Id,
                            o.UserId,
                            o.Total
                        })
                        .WhereComputed<JoinOrder, JoinUser>(
                            (o, u) => o.UserId == u.Id)
                        .OrderByDescending<JoinOrder>(o => o.Total)
                        .Take(1))
                .Build();
        }

        private static GeneratedSqlQuery BuildSqlServerOuterApplyQuery(IQueryBuilder<SqlServer2012Profile> queryBuilder)
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .OuterApply<JoinUser, JoinOrder>(
                    alias: "latest_order",
                    applyBuilder: apply => apply
                        .Select<JoinOrder>(o => new
                        {
                            OrderId = o.Id,
                            o.UserId,
                            o.Total
                        })
                        .WhereComputed<JoinOrder, JoinUser>(
                            (o, u) => o.UserId == u.Id)
                        .OrderByDescending<JoinOrder>(o => o.Total)
                        .Take(1))
                .Build();
        }
        
        private static GeneratedSqlQuery BuildPostgreSqlOuterApplyQuery(IQueryBuilder<PostgreSql93Profile> queryBuilder)
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .OuterApply<JoinUser, JoinOrder>(
                    alias: "latest_order",
                    applyBuilder: apply => apply
                        .Select<JoinOrder>(o => new
                        {
                            OrderId = o.Id,
                            o.UserId,
                            o.Total
                        })
                        .WhereComputed<JoinOrder, JoinUser>(
                            (o, u) => o.UserId == u.Id)
                        .OrderByDescending<JoinOrder>(o => o.Total)
                        .Take(1))
                .Build();
        }
        
        private static GeneratedSqlQuery BuildMySqlOuterApplyQuery(IQueryBuilder<MySql8031Profile> queryBuilder)
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .OuterApply<JoinUser, JoinOrder>(
                    alias: "latest_order",
                    applyBuilder: apply => apply
                        .Select<JoinOrder>(o => new
                        {
                            OrderId = o.Id,
                            o.UserId,
                            o.Total
                        })
                        .WhereComputed<JoinOrder, JoinUser>(
                            (o, u) => o.UserId == u.Id)
                        .OrderByDescending<JoinOrder>(o => o.Total)
                        .Take(1))
                .Build();
        }
        
        private static GeneratedSqlQuery BuildSqlServerLateralCompositionQuery(IQueryBuilder<SqlServer2012Profile> queryBuilder)
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .CrossApply<JoinUser, JoinOrder>(
                    alias: "latest_order",
                    applyBuilder: apply => apply
                        .Select<JoinOrder>(o => new
                        {
                            OrderId = o.Id,
                            o.UserId,
                            o.Total
                        })
                        .WhereComputed<JoinOrder, JoinUser>(
                            (o, u) => o.UserId == u.Id)
                        .OrderByDescending<JoinOrder>(o => o.Total)
                        .Take(1))
                .Where<JoinUser>(u => u.IsActive)
                .OrderBy<JoinUser>(u => u.Id)
                .Take(10)
                .Build();
        }
        
        private static GeneratedSqlQuery BuildPostgreSqlLateralCompositionQuery(IQueryBuilder<PostgreSql93Profile> queryBuilder)
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .CrossApply<JoinUser, JoinOrder>(
                    alias: "latest_order",
                    applyBuilder: apply => apply
                        .Select<JoinOrder>(o => new
                        {
                            OrderId = o.Id,
                            o.UserId,
                            o.Total
                        })
                        .WhereComputed<JoinOrder, JoinUser>(
                            (o, u) => o.UserId == u.Id)
                        .OrderByDescending<JoinOrder>(o => o.Total)
                        .Take(1))
                .Where<JoinUser>(u => u.IsActive)
                .OrderBy<JoinUser>(u => u.Id)
                .Take(10)
                .Build();
        }
        
        private static GeneratedSqlQuery BuildMySqlLateralCompositionQuery(IQueryBuilder<MySql8031Profile> queryBuilder)
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .CrossApply<JoinUser, JoinOrder>(
                    alias: "latest_order",
                    applyBuilder: apply => apply
                        .Select<JoinOrder>(o => new
                        {
                            OrderId = o.Id,
                            o.UserId,
                            o.Total
                        })
                        .WhereComputed<JoinOrder, JoinUser>(
                            (o, u) => o.UserId == u.Id)
                        .OrderByDescending<JoinOrder>(o => o.Total)
                        .Take(1))
                .Where<JoinUser>(u => u.IsActive)
                .OrderBy<JoinUser>(u => u.Id)
                .Take(10)
                .Build();
        }
    }
}
