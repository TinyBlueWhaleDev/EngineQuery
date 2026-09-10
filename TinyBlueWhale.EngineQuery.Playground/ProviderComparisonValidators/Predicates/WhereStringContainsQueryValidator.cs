using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.Predicates
{
    /// <summary>
    /// Validates translation of string Contains predicates into parameterized
    /// LIKE expressions across supported database providers.
    ///
    /// SQL Server:
    /// SELECT [u].[user_id] AS [UserId], [u].[email]
    /// FROM [users] AS [u]
    /// WHERE ([u].[email] LIKE @p0)
    ///
    /// PostgreSQL:
    /// SELECT "u"."user_id" AS "UserId", "u"."email"
    /// FROM "users" AS "u"
    /// WHERE ("u"."email" LIKE @p0)
    ///
    /// MySQL:
    /// SELECT `u`.`user_id` AS `UserId`, `u`.`email`
    /// FROM `users` AS `u`
    /// WHERE (`u`.`email` LIKE @p0)
    ///
    /// Expected parameters:
    /// @p0 = %admin%
    /// </summary>
    public static class WhereStringContainsQueryValidator
    {
        public static void Run()
        {
            var metadataResolver =
                ProviderMetadataFactory.CreateJoinMetadataResolver();

            ProviderQueryPrinter.Print(
                "SQL Server Where String Contains",
                BuildQuery(
                    ProviderQueryBuilderFactory.CreateSqlServer(
                        metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL Where String Contains",
                BuildQuery(
                    ProviderQueryBuilderFactory.CreatePostgreSql(
                        metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL Where String Contains",
                BuildQuery(
                    ProviderQueryBuilderFactory.CreateMySql(
                        metadataResolver)));
        }

        // Builds a query using string Contains predicates.
        private static GeneratedSqlQuery BuildQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            const string search = "admin";

            return queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .Where<JoinUser>(
                    u => u.Email.Contains(search))
                .Build();
        }
    }
}
