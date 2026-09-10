using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.Metadata
{
    /// <summary>
    /// Validates fluent table and column metadata resolution across SELECT,
    /// INSERT, UPDATE and DELETE commands for all supported database providers.
    ///
    /// SQL Server SELECT:
    /// SELECT [u].[user_id], [u].[email], [u].[is_active]
    /// FROM [users] AS [u]
    /// WHERE ([u].[is_active] = @p0)
    /// ORDER BY [u].[user_id] ASC
    ///
    /// Expected parameters:
    /// @p0 = True
    ///
    /// SQL Server INSERT:
    /// INSERT INTO [users] ([email], [is_active])
    /// VALUES (@p0, @p1)
    ///
    /// Expected parameters:
    /// @p0 = metadata@test.com
    /// @p1 = True
    ///
    /// SQL Server UPDATE:
    /// UPDATE [users]
    /// SET [email] = @p0
    /// WHERE ([user_id] = @p1)
    ///
    /// Expected parameters:
    /// @p0 = updated@test.com
    /// @p1 = 10
    ///
    /// SQL Server DELETE:
    /// DELETE
    /// FROM [users]
    /// WHERE ([user_id] = @p0)
    ///
    /// Expected parameters:
    /// @p0 = 10
    ///
    /// PostgreSQL SELECT:
    /// SELECT "u"."user_id", "u"."email", "u"."is_active"
    /// FROM "users" AS "u"
    /// WHERE ("u"."is_active" = @p0)
    /// ORDER BY "u"."user_id" ASC
    ///
    /// Expected parameters:
    /// @p0 = True
    ///
    /// PostgreSQL INSERT:
    /// INSERT INTO "users" ("email", "is_active")
    /// VALUES (@p0, @p1)
    ///
    /// Expected parameters:
    /// @p0 = metadata@test.com
    /// @p1 = True
    ///
    /// PostgreSQL UPDATE:
    /// UPDATE "users"
    /// SET "email" = @p0
    /// WHERE ("user_id" = @p1)
    ///
    /// Expected parameters:
    /// @p0 = updated@test.com
    /// @p1 = 10
    ///
    /// PostgreSQL DELETE:
    /// DELETE
    /// FROM "users"
    /// WHERE ("user_id" = @p0)
    ///
    /// Expected parameters:
    /// @p0 = 10
    ///
    /// MySQL SELECT:
    /// SELECT `u`.`user_id`, `u`.`email`, `u`.`is_active`
    /// FROM `users` AS `u`
    /// WHERE (`u`.`is_active` = @p0)
    /// ORDER BY `u`.`user_id` ASC
    ///
    /// Expected parameters:
    /// @p0 = True
    ///
    /// MySQL INSERT:
    /// INSERT INTO `users` (`email`, `is_active`)
    /// VALUES (@p0, @p1)
    ///
    /// Expected parameters:
    /// @p0 = metadata@test.com
    /// @p1 = True
    ///
    /// MySQL UPDATE:
    /// UPDATE `users`
    /// SET `email` = @p0
    /// WHERE (`user_id` = @p1)
    ///
    /// Expected parameters:
    /// @p0 = updated@test.com
    /// @p1 = 10
    ///
    /// MySQL DELETE:
    /// DELETE
    /// FROM `users`
    /// WHERE (`user_id` = @p0)
    ///
    /// Expected parameters:
    /// @p0 = 10
    /// </summary>
    public static class FluentMetadataQueryValidator
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

        // Runs fluent metadata scenarios for one database provider.
        private static void RunProvider<TProfile>(
            string providerName,
            Abstractions.Interfaces.IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            ProviderQueryPrinter.Print(
                $"{providerName} Fluent Metadata Select",
                BuildSelectQuery(queryBuilder));

            ProviderQueryPrinter.Print(
                $"{providerName} Fluent Metadata Insert",
                BuildInsertQuery(queryBuilder));

            ProviderQueryPrinter.Print(
                $"{providerName} Fluent Metadata Update",
                BuildUpdateQuery(queryBuilder));

            ProviderQueryPrinter.Print(
                $"{providerName} Fluent Metadata Delete",
                BuildDeleteQuery(queryBuilder));
        }

        // Builds a SELECT command using fluent metadata.
        private static GeneratedSqlQuery BuildSelectQuery<TProfile>(
            Abstractions.Interfaces.IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(user => new
                {
                    user.Id,
                    user.Email,
                    user.IsActive
                })
                .Where<JoinUser>(user => user.IsActive)
                .OrderBy<JoinUser>(user => user.Id)
                .Build();
        }

        // Builds an INSERT command using fluent metadata.
        private static GeneratedSqlQuery BuildInsertQuery<TProfile>(
            Abstractions.Interfaces.IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .InsertInto<JoinUser>()
                .Set(user => user.Email, "metadata@test.com")
                .Set(user => user.IsActive, true)
                .Build();
        }

        // Builds an UPDATE command using fluent metadata.
        private static GeneratedSqlQuery BuildUpdateQuery<TProfile>(
            Abstractions.Interfaces.IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .Update<JoinUser>()
                .Set(user => user.Email, "updated@test.com")
                .Where(user => user.Id == 10)
                .Build();
        }

        // Builds a DELETE command using fluent metadata.
        private static GeneratedSqlQuery BuildDeleteQuery<TProfile>(
            Abstractions.Interfaces.IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .DeleteFrom<JoinUser>()
                .Where(user => user.Id == 10)
                .Build();
        }
    }
}
