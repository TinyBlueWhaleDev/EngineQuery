using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.Predicates
{
    /// <summary>
    /// Validates IN and NOT IN collection predicates across SELECT, INSERT SELECT,
    /// UPDATE and DELETE commands for all supported database providers.
    ///
    /// SQL Server SELECT:
    /// SELECT *
    /// FROM [users] AS [u]
    /// WHERE [u].[user_id] IN (@p0, @p1, @p2) AND [u].[email] NOT IN (@p3, @p4)
    ///
    /// Expected parameters:
    /// @p0 = 10
    /// @p1 = 20
    /// @p2 = 30
    /// @p3 = blocked@test.com
    /// @p4 = deleted@test.com
    ///
    /// SQL Server INSERT SELECT:
    /// INSERT INTO [orders] ([user_id])
    /// SELECT [u].[user_id] AS [UserId]
    /// FROM [users] AS [u]
    /// WHERE [u].[user_id] IN (@p0, @p1, @p2)
    ///
    /// Expected parameters:
    /// @p0 = 10
    /// @p1 = 20
    /// @p2 = 30
    ///
    /// SQL Server UPDATE:
    /// UPDATE [users]
    /// SET [is_active] = @p0
    /// WHERE [user_id] IN (@p1, @p2, @p3)
    ///
    /// Expected parameters:
    /// @p0 = False
    /// @p1 = 10
    /// @p2 = 20
    /// @p3 = 30
    ///
    /// SQL Server DELETE:
    /// DELETE
    /// FROM [users]
    /// WHERE [user_id] NOT IN (@p0, @p1, @p2)
    ///
    /// Expected parameters:
    /// @p0 = 10
    /// @p1 = 20
    /// @p2 = 30
    ///
    /// PostgreSQL SELECT:
    /// SELECT *
    /// FROM "users" AS "u"
    /// WHERE "u"."user_id" IN (@p0, @p1, @p2) AND "u"."email" NOT IN (@p3, @p4)
    ///
    /// Expected parameters:
    /// @p0 = 10
    /// @p1 = 20
    /// @p2 = 30
    /// @p3 = blocked@test.com
    /// @p4 = deleted@test.com
    ///
    /// PostgreSQL INSERT SELECT:
    /// INSERT INTO "orders" ("user_id")
    /// SELECT "u"."user_id" AS "UserId"
    /// FROM "users" AS "u"
    /// WHERE "u"."user_id" IN (@p0, @p1, @p2)
    ///
    /// Expected parameters:
    /// @p0 = 10
    /// @p1 = 20
    /// @p2 = 30
    ///
    /// PostgreSQL UPDATE:
    /// UPDATE "users"
    /// SET "is_active" = @p0
    /// WHERE "user_id" IN (@p1, @p2, @p3)
    ///
    /// Expected parameters:
    /// @p0 = False
    /// @p1 = 10
    /// @p2 = 20
    /// @p3 = 30
    ///
    /// PostgreSQL DELETE:
    /// DELETE
    /// FROM "users"
    /// WHERE "user_id" NOT IN (@p0, @p1, @p2)
    ///
    /// Expected parameters:
    /// @p0 = 10
    /// @p1 = 20
    /// @p2 = 30
    ///
    /// MySQL SELECT:
    /// SELECT *
    /// FROM `users` AS `u`
    /// WHERE `u`.`user_id` IN (@p0, @p1, @p2) AND `u`.`email` NOT IN (@p3, @p4)
    ///
    /// Expected parameters:
    /// @p0 = 10
    /// @p1 = 20
    /// @p2 = 30
    /// @p3 = blocked@test.com
    /// @p4 = deleted@test.com
    ///
    /// MySQL INSERT SELECT:
    /// INSERT INTO `orders` (`user_id`)
    /// SELECT `u`.`user_id` AS `UserId`
    /// FROM `users` AS `u`
    /// WHERE `u`.`user_id` IN (@p0, @p1, @p2)
    ///
    /// Expected parameters:
    /// @p0 = 10
    /// @p1 = 20
    /// @p2 = 30
    ///
    /// MySQL UPDATE:
    /// UPDATE `users`
    /// SET `is_active` = @p0
    /// WHERE `user_id` IN (@p1, @p2, @p3)
    ///
    /// Expected parameters:
    /// @p0 = False
    /// @p1 = 10
    /// @p2 = 20
    /// @p3 = 30
    ///
    /// MySQL DELETE:
    /// DELETE
    /// FROM `users`
    /// WHERE `user_id` NOT IN (@p0, @p1, @p2)
    ///
    /// Expected parameters:
    /// @p0 = 10
    /// @p1 = 20
    /// @p2 = 30
    /// </summary>
    public static class WhereCollectionQueryValidator
    {        
        public static void Run()
        {
            var metadataResolver =
                ProviderMetadataFactory.CreateJoinMetadataResolver();

            RunProvider(
                "SQL Server",
                ProviderQueryBuilderFactory.CreateSqlServer(
                    metadataResolver));

            RunProvider(
                "PostgreSQL",
                ProviderQueryBuilderFactory.CreatePostgreSql(
                    metadataResolver));

            RunProvider(
                "MySQL",
                ProviderQueryBuilderFactory.CreateMySql(
                    metadataResolver));
        }

        // Runs all collection filtering scenarios for the specified provider.
        private static void RunProvider<TProfile>(
            string providerName,
            IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            ProviderQueryPrinter.Print(
                $"{providerName} Where IN and NOT IN Collections",
                BuildSelectQuery(queryBuilder));

            ProviderQueryPrinter.Print(
                $"{providerName} Insert Select Where IN Collection",
                BuildInsertSelectQuery(queryBuilder));

            ProviderQueryPrinter.Print(
                $"{providerName} Update Where IN Collection",
                BuildUpdateQuery(queryBuilder));

            ProviderQueryPrinter.Print(
                $"{providerName} Delete Where NOT IN Collection",
                BuildDeleteQuery(queryBuilder));
        }

        // Builds a SELECT command using IN and NOT IN collection conditions.
        private static GeneratedSqlQuery BuildSelectQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .WhereIn(
                    user => user.Id,
                    [10, 20, 30])
                .WhereNotIn(
                    user => user.Email,
                    [
                        "blocked@test.com",
                        "deleted@test.com"
                    ])
                .Build();
        }

        // Builds an INSERT SELECT command using an IN collection condition.
        private static GeneratedSqlQuery BuildInsertSelectQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .InsertInto<JoinOrder>()
                .Columns(order => new
                {
                    order.UserId
                })
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(user => new
                {
                    UserId = user.Id
                })
                .WhereIn<JoinUser, int>(
                    user => user.Id,
                    [10, 20, 30])
                .Build();
        }

        // Builds an UPDATE command using an IN collection condition.
        private static GeneratedSqlQuery BuildUpdateQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .Update<JoinUser>()
                .Set(user => user.IsActive, false)
                .WhereIn(
                    user => user.Id,
                    [10, 20, 30])
                .Build();
        }

        // Builds a DELETE command using a NOT IN collection condition.
        private static GeneratedSqlQuery BuildDeleteQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .DeleteFrom<JoinUser>()
                .WhereNotIn(
                    user => user.Id,
                    [10, 20, 30])
                .Build();
        }
    }
}
