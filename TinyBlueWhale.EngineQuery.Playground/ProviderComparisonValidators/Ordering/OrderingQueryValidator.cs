using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.Ordering
{
    /// <summary>
    /// Validates ascending, descending and chained ordering generation
    /// across supported database providers.
    ///
    /// SQL Server ascending:
    /// SELECT [u].[user_id] AS [UserId], [u].[email]
    /// FROM [users] AS [u]
    /// ORDER BY [u].[email] ASC
    ///
    /// SQL Server descending:
    /// SELECT [u].[user_id] AS [UserId], [u].[email]
    /// FROM [users] AS [u]
    /// ORDER BY [u].[email] DESC
    ///
    /// SQL Server ThenBy:
    /// SELECT [u].[user_id] AS [UserId], [u].[email]
    /// FROM [users] AS [u]
    /// ORDER BY [u].[email] ASC, [u].[user_id] ASC
    ///
    /// SQL Server mixed directions:
    /// SELECT [u].[user_id] AS [UserId], [u].[email]
    /// FROM [users] AS [u]
    /// ORDER BY [u].[email] ASC, [u].[user_id] DESC
    ///
    /// PostgreSQL ascending:
    /// SELECT "u"."user_id" AS "UserId", "u"."email"
    /// FROM "users" AS "u"
    /// ORDER BY "u"."email" ASC
    ///
    /// PostgreSQL descending:
    /// SELECT "u"."user_id" AS "UserId", "u"."email"
    /// FROM "users" AS "u"
    /// ORDER BY "u"."email" DESC
    ///
    /// PostgreSQL ThenBy:
    /// SELECT "u"."user_id" AS "UserId", "u"."email"
    /// FROM "users" AS "u"
    /// ORDER BY "u"."email" ASC, "u"."user_id" ASC
    ///
    /// PostgreSQL mixed directions:
    /// SELECT "u"."user_id" AS "UserId", "u"."email"
    /// FROM "users" AS "u"
    /// ORDER BY "u"."email" ASC, "u"."user_id" DESC
    ///
    /// MySQL ascending:
    /// SELECT `u`.`user_id` AS `UserId`, `u`.`email`
    /// FROM `users` AS `u`
    /// ORDER BY `u`.`email` ASC
    ///
    /// MySQL descending:
    /// SELECT `u`.`user_id` AS `UserId`, `u`.`email`
    /// FROM `users` AS `u`
    /// ORDER BY `u`.`email` DESC
    ///
    /// MySQL ThenBy:
    /// SELECT `u`.`user_id` AS `UserId`, `u`.`email`
    /// FROM `users` AS `u`
    /// ORDER BY `u`.`email` ASC, `u`.`user_id` ASC
    ///
    /// MySQL mixed directions:
    /// SELECT `u`.`user_id` AS `UserId`, `u`.`email`
    /// FROM `users` AS `u`
    /// ORDER BY `u`.`email` ASC, `u`.`user_id` DESC
    /// </summary>
    public static class OrderingQueryValidator
    {        
        public static void Run()
        {
            var metadataResolver = ProviderMetadataFactory.CreateJoinMetadataResolver();

            RunProvider(
                "SQL Server",
                ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver));

            RunProvider(
                "PostgreSQL",
                ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver));

            RunProvider(
                "MySQL",
                ProviderQueryBuilderFactory.CreateMySql(metadataResolver));
        }
        
        private static void RunProvider<TProfile>(string providerName, IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            ProviderQueryPrinter.Print(
                $"{providerName} Ordering - Ascending",
                BuildAscendingQuery(queryBuilder));

            ProviderQueryPrinter.Print(
                $"{providerName} Ordering - Descending",
                BuildDescendingQuery(queryBuilder));

            ProviderQueryPrinter.Print(
                $"{providerName} Ordering - ThenBy",
                BuildThenByQuery(queryBuilder));

            ProviderQueryPrinter.Print(
                $"{providerName} Ordering - Mixed Directions",
                BuildMixedDirectionQuery(queryBuilder));
        }

        // Builds a query using ascending ordering.
        private static GeneratedSqlQuery BuildAscendingQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .OrderBy<JoinUser>(u => u.Email)
                .Build();
        }

        // Builds a query using descending ordering.
        private static GeneratedSqlQuery BuildDescendingQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .OrderByDescending<JoinUser>(u => u.Email)
                .Build();
        }

        // Builds a query using primary and secondary ascending ordering.
        private static GeneratedSqlQuery BuildThenByQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .OrderBy<JoinUser>(u => u.Email)
                .ThenBy<JoinUser>(u => u.Id)
                .Build();
        }

        // Builds a query using mixed ordering directions.
        private static GeneratedSqlQuery BuildMixedDirectionQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .OrderBy<JoinUser>(u => u.Email)
                .ThenByDescending<JoinUser>(u => u.Id)
                .Build();
        }
    }
}
