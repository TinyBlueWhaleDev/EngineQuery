using TinyBlueWhale.EngineQuery.Abstractions.Extensions;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Features;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.SetOperations
{

    /// <summary>
    /// Validates INTERSECT and EXCEPT generation, parameter propagation and
    /// projection arity validation across supported database providers.
    ///
    /// SQL Server INTERSECT:
    /// SELECT [u].[email]
    /// FROM [users] AS [u]
    /// WHERE ([u].[user_id] &gt; @p0)
    /// INTERSECT
    /// SELECT [a].[email]
    /// FROM [archived_users] AS [a]
    /// WHERE ([a].[archived_user_id] &gt; @p1)
    ///
    /// PostgreSQL INTERSECT:
    /// SELECT "u"."email"
    /// FROM "users" AS "u"
    /// WHERE ("u"."user_id" &gt; @p0)
    /// INTERSECT
    /// SELECT "a"."email"
    /// FROM "archived_users" AS "a"
    /// WHERE ("a"."archived_user_id" &gt; @p1)
    ///
    /// MySQL INTERSECT:
    /// SELECT `u`.`email`
    /// FROM `users` AS `u`
    /// WHERE (`u`.`user_id` &gt; @p0)
    /// INTERSECT
    /// SELECT `a`.`email`
    /// FROM `archived_users` AS `a`
    /// WHERE (`a`.`archived_user_id` &gt; @p1)
    ///
    /// SQL Server EXCEPT:
    /// SELECT [u].[email]
    /// FROM [users] AS [u]
    /// WHERE ([u].[user_id] &gt; @p0)
    /// EXCEPT
    /// SELECT [a].[email]
    /// FROM [archived_users] AS [a]
    /// WHERE ([a].[archived_user_id] &gt; @p1)
    ///
    /// PostgreSQL EXCEPT:
    /// SELECT "u"."email"
    /// FROM "users" AS "u"
    /// WHERE ("u"."user_id" &gt; @p0)
    /// EXCEPT
    /// SELECT "a"."email"
    /// FROM "archived_users" AS "a"
    /// WHERE ("a"."archived_user_id" &gt; @p1)
    ///
    /// MySQL EXCEPT:
    /// SELECT `u`.`email`
    /// FROM `users` AS `u`
    /// WHERE (`u`.`user_id` &gt; @p0)
    /// EXCEPT
    /// SELECT `a`.`email`
    /// FROM `archived_users` AS `a`
    /// WHERE (`a`.`archived_user_id` &gt; @p1)
    ///
    /// Expected parameters for each valid set operation:
    /// @p0 = 10
    /// @p1 = 100
    ///
    /// Expected INTERSECT arity validation:
    /// Set operation 'Intersect' requires matching projection arity.
    /// The left query projects 2 column(s) and the right query projects 1 column(s).
    ///
    /// Expected EXCEPT arity validation:
    /// Set operation 'Except' requires matching projection arity.
    /// The left query projects 2 column(s) and the right query projects 1 column(s).
    /// </summary>
    public static class IntersectExceptQueryValidator
    {        
        public static void Run()
        {
            var metadataResolver = ProviderMetadataFactory.CreateJoinMetadataResolver();

            ProviderQueryPrinter.Print(
                "SQL Server INTERSECT",
                BuildIntersectQuery(ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL INTERSECT",
                BuildIntersectQuery(ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL INTERSECT",
                BuildIntersectQuery(ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "SQL Server EXCEPT",
                BuildExceptQuery(ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL EXCEPT",
                BuildExceptQuery(ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL EXCEPT",
                BuildExceptQuery(ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));

            ValidateMismatch(
                "SQL Server INTERSECT Arity Mismatch",
                () => BuildIntersectMismatchQuery(
                    ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ValidateMismatch(
                "PostgreSQL INTERSECT Arity Mismatch",
                () => BuildIntersectMismatchQuery(
                    ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ValidateMismatch(
                "MySQL INTERSECT Arity Mismatch",
                () => BuildIntersectMismatchQuery(
                    ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));

            ValidateMismatch(
                "SQL Server EXCEPT Arity Mismatch",
                () => BuildExceptMismatchQuery(
                    ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ValidateMismatch(
                "PostgreSQL EXCEPT Arity Mismatch",
                () => BuildExceptMismatchQuery(
                    ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ValidateMismatch(
                "MySQL EXCEPT Arity Mismatch",
                () => BuildExceptMismatchQuery(
                    ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));
        }

        // Builds an INTERSECT query.
        private static GeneratedSqlQuery BuildIntersectQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile, IIntersectFeature
        {
            return queryBuilder
                .From<ActiveUser>(alias: "u")
                .Select<ActiveUser>(u => new
                {
                    u.Email
                })
                .Where<ActiveUser>(u => u.Id > 10)
                .Intersect(set => set
                    .From<ArchivedUser>(alias: "a")
                    .Select<ArchivedUser>(a => new
                    {
                        a.Email
                    })
                    .Where<ArchivedUser>(a => a.Id > 100))
                .Build();
        }

        // Builds an EXCEPT query.
        private static GeneratedSqlQuery BuildExceptQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile, IExceptFeature
        {
            return queryBuilder
                .From<ActiveUser>(alias: "u")
                .Select<ActiveUser>(u => new
                {
                    u.Email
                })
                .Where<ActiveUser>(u => u.Id > 10)
                .Except(set => set
                    .From<ArchivedUser>(alias: "a")
                    .Select<ArchivedUser>(a => new
                    {
                        a.Email
                    })
                    .Where<ArchivedUser>(a => a.Id > 100))
                .Build();
        }

        // Builds an invalid INTERSECT query with incompatible projection arity.
        private static GeneratedSqlQuery BuildIntersectMismatchQuery<TProfile>(
            IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile, IIntersectFeature
        {
            return queryBuilder
                .From<ActiveUser>(alias: "u")
                .Select<ActiveUser>(u => new
                {
                    u.Id,
                    u.Email
                })
                .Intersect(set => set
                    .From<ArchivedUser>(alias: "a")
                    .Select<ArchivedUser>(a => new
                    {
                        a.Email
                    }))
                .Build();
        }

        // Builds an invalid EXCEPT query with incompatible projection arity.
        private static GeneratedSqlQuery BuildExceptMismatchQuery<TProfile>(
            IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile, IExceptFeature
        {
            return queryBuilder
                .From<ActiveUser>(alias: "u")
                .Select<ActiveUser>(u => new
                {
                    u.Id,
                    u.Email
                })
                .Except(set => set
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
