using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.SetOperations
{

    /// <summary>
    /// Validates UNION generation, parameter propagation and projection arity
    /// validation across supported database providers.
    ///
    /// SQL Server:
    /// SELECT [u].[user_id], [u].[email]
    /// FROM [users] AS [u]
    /// WHERE ([u].[user_id] &gt; @p0)
    /// UNION
    /// SELECT [a].[archived_user_id], [a].[email]
    /// FROM [archived_users] AS [a]
    /// WHERE ([a].[archived_user_id] &gt; @p1)
    ///
    /// PostgreSQL:
    /// SELECT "u"."user_id", "u"."email"
    /// FROM "users" AS "u"
    /// WHERE ("u"."user_id" &gt; @p0)
    /// UNION
    /// SELECT "a"."archived_user_id", "a"."email"
    /// FROM "archived_users" AS "a"
    /// WHERE ("a"."archived_user_id" &gt; @p1)
    ///
    /// MySQL:
    /// SELECT `u`.`user_id`, `u`.`email`
    /// FROM `users` AS `u`
    /// WHERE (`u`.`user_id` &gt; @p0)
    /// UNION
    /// SELECT `a`.`archived_user_id`, `a`.`email`
    /// FROM `archived_users` AS `a`
    /// WHERE (`a`.`archived_user_id` &gt; @p1)
    ///
    /// Expected parameters:
    /// @p0 = 10
    /// @p1 = 100
    ///
    /// Expected arity validation:
    /// Set operation 'Union' requires matching projection arity.
    /// The left query projects 2 column(s) and the right query projects 1 column(s).
    /// </summary>
    public static class UnionQueryValidator
    {
        public static void Run()
        {
            var metadataResolver = ProviderMetadataFactory.CreateJoinMetadataResolver();

            ProviderQueryPrinter.Print(
                "SQL Server UNION",
                BuildQuery(ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL UNION",
                BuildQuery(ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL UNION",
                BuildQuery(ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));

            ValidateMismatch(
                "SQL Server UNION Arity Mismatch",
                () => BuildMismatchQuery(
                    ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ValidateMismatch(
                "PostgreSQL UNION Arity Mismatch",
                () => BuildMismatchQuery(
                    ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ValidateMismatch(
                "MySQL UNION Arity Mismatch",
                () => BuildMismatchQuery(
                    ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));
        }

        // Builds a UNION query.
        private static GeneratedSqlQuery BuildQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .From<ActiveUser>(alias: "u")
                .Select<ActiveUser>(u => new
                {
                    u.Id,
                    u.Email
                })
                .Where<ActiveUser>(u => u.Id > 10)
                .Union(union => union
                    .From<ArchivedUser>(alias: "a")
                    .Select<ArchivedUser>(a => new
                    {
                        a.Id,
                        a.Email
                    })
                    .Where<ArchivedUser>(a => a.Id > 100))
                .Build();
        }

        // Builds an invalid UNION query with incompatible projection arity.
        private static GeneratedSqlQuery BuildMismatchQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .From<ActiveUser>(alias: "u")
                .Select<ActiveUser>(u => new
                {
                    u.Id,
                    u.Email
                })
                .Union(union => union
                    .From<ArchivedUser>(alias: "a")
                    .Select<ArchivedUser>(a => new
                    {
                        a.Email
                    }))
                .Build();
        }

        // Validates that incompatible projection arity is rejected.
        private static void ValidateMismatch(string title, Func<GeneratedSqlQuery> queryFactory)
        {
            Console.WriteLine($"--- {title} ---");

            try
            {
                queryFactory();

                Console.WriteLine("FAILED: Expected projection arity validation.");
            }
            catch (InvalidOperationException exception)
            {
                Console.WriteLine($"PASS: {exception.Message}");
            }

            Console.WriteLine();
        }
    }
}
