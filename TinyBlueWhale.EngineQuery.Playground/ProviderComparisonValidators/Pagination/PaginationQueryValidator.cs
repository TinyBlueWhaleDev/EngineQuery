using TinyBlueWhale.EngineQuery.Abstractions.Extensions;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Features;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.Pagination
{
    /// <summary>
    /// Validates provider-specific pagination using Take, Skip and their combined
    /// composition over an explicitly ordered query.
    ///
    /// SQL Server Take only:
    /// SELECT [u].[user_id] AS [UserId], [u].[email]
    /// FROM [users] AS [u]
    /// ORDER BY [u].[user_id] ASC
    /// OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY
    ///
    /// SQL Server Skip only:
    /// SELECT [u].[user_id] AS [UserId], [u].[email]
    /// FROM [users] AS [u]
    /// ORDER BY [u].[user_id] ASC
    /// OFFSET 20 ROWS
    ///
    /// SQL Server Skip and Take:
    /// SELECT [u].[user_id] AS [UserId], [u].[email]
    /// FROM [users] AS [u]
    /// ORDER BY [u].[user_id] ASC
    /// OFFSET 20 ROWS FETCH NEXT 10 ROWS ONLY
    ///
    /// PostgreSQL Take only:
    /// SELECT "u"."user_id" AS "UserId", "u"."email"
    /// FROM "users" AS "u"
    /// ORDER BY "u"."user_id" ASC
    /// LIMIT 10
    ///
    /// PostgreSQL Skip only:
    /// SELECT "u"."user_id" AS "UserId", "u"."email"
    /// FROM "users" AS "u"
    /// ORDER BY "u"."user_id" ASC
    /// OFFSET 20
    ///
    /// PostgreSQL Skip and Take:
    /// SELECT "u"."user_id" AS "UserId", "u"."email"
    /// FROM "users" AS "u"
    /// ORDER BY "u"."user_id" ASC
    /// LIMIT 10 OFFSET 20
    ///
    /// MySQL Take only:
    /// SELECT `u`.`user_id` AS `UserId`, `u`.`email`
    /// FROM `users` AS `u`
    /// ORDER BY `u`.`user_id` ASC
    /// LIMIT 10
    ///
    /// MySQL Skip only:
    /// SELECT `u`.`user_id` AS `UserId`, `u`.`email`
    /// FROM `users` AS `u`
    /// ORDER BY `u`.`user_id` ASC
    /// LIMIT 18446744073709551615 OFFSET 20
    ///
    /// MySQL Skip and Take:
    /// SELECT `u`.`user_id` AS `UserId`, `u`.`email`
    /// FROM `users` AS `u`
    /// ORDER BY `u`.`user_id` ASC
    /// LIMIT 10 OFFSET 20
    /// </summary>
    public static class PaginationQueryValidator
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

        // Runs all pagination scenarios for one database provider.
        private static void RunProvider<TProfile>(string providerName, IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile, IPaginationFeature
        {
            ProviderQueryPrinter.Print(
                $"{providerName} Pagination - Take Only",
                BuildTakeOnlyQuery(queryBuilder));

            ProviderQueryPrinter.Print(
                $"{providerName} Pagination - Skip Only",
                BuildSkipOnlyQuery(queryBuilder));

            ProviderQueryPrinter.Print(
                $"{providerName} Pagination - Skip And Take",
                BuildSkipAndTakeQuery(queryBuilder));
        }

        // Builds an ordered query using only Take.
        private static GeneratedSqlQuery BuildTakeOnlyQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile, IPaginationFeature
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .OrderBy<JoinUser>(u => u.Id)
                .Take(10)
                .Build();
        }

        // Builds an ordered query using only Skip.
        private static GeneratedSqlQuery BuildSkipOnlyQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile, IPaginationFeature
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .OrderBy<JoinUser>(u => u.Id)
                .Skip(20)
                .Build();
        }

        // Builds an ordered query using Skip and Take.
        private static GeneratedSqlQuery BuildSkipAndTakeQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile, IPaginationFeature
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .OrderBy<JoinUser>(u => u.Id)
                .Skip(20)
                .Take(10)
                .Build();
        }
    }
}
