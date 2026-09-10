using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.Inserts
{

    /// <summary>
    /// Validates INSERT SELECT generation using metadata-resolved target and source columns,
    /// source filtering and multiple source instances of the same CLR type.
    ///
    /// SQL Server INSERT SELECT:
    /// INSERT INTO [users] ([email], [is_active])
    /// SELECT [source].[email], [source].[is_active]
    /// FROM [users] AS [source]
    ///
    /// SQL Server INSERT SELECT with filtering:
    /// INSERT INTO [users] ([email], [is_active])
    /// SELECT [source].[email], [source].[is_active]
    /// FROM [users] AS [source]
    /// WHERE ([source].[is_active] = @p0)
    /// @p0 = True
    ///
    /// SQL Server INSERT SELECT with same-type join:
    /// INSERT INTO [users] ([email], [is_active])
    /// SELECT [source].[email], [source].[is_active]
    /// FROM [users] AS [source]
    /// LEFT JOIN [users] AS [duplicate] ON ([source].[email] = [duplicate].[email])
    /// WHERE ([source].[is_active] = @p0)
    /// @p0 = True
    ///
    /// PostgreSQL INSERT SELECT:
    /// INSERT INTO "users" ("email", "is_active")
    /// SELECT "source"."email", "source"."is_active"
    /// FROM "users" AS "source"
    ///
    /// PostgreSQL INSERT SELECT with filtering:
    /// INSERT INTO "users" ("email", "is_active")
    /// SELECT "source"."email", "source"."is_active"
    /// FROM "users" AS "source"
    /// WHERE ("source"."is_active" = @p0)
    /// @p0 = True
    ///
    /// PostgreSQL INSERT SELECT with same-type join:
    /// INSERT INTO "users" ("email", "is_active")
    /// SELECT "source"."email", "source"."is_active"
    /// FROM "users" AS "source"
    /// LEFT JOIN "users" AS "duplicate" ON ("source"."email" = "duplicate"."email")
    /// WHERE ("source"."is_active" = @p0)
    /// @p0 = True
    ///
    /// MySQL INSERT SELECT:
    /// INSERT INTO `users` (`email`, `is_active`)
    /// SELECT `source`.`email`, `source`.`is_active`
    /// FROM `users` AS `source`
    ///
    /// MySQL INSERT SELECT with filtering:
    /// INSERT INTO `users` (`email`, `is_active`)
    /// SELECT `source`.`email`, `source`.`is_active`
    /// FROM `users` AS `source`
    /// WHERE (`source`.`is_active` = @p0)
    /// @p0 = True
    ///
    /// MySQL INSERT SELECT with same-type join:
    /// INSERT INTO `users` (`email`, `is_active`)
    /// SELECT `source`.`email`, `source`.`is_active`
    /// FROM `users` AS `source`
    /// LEFT JOIN `users` AS `duplicate` ON (`source`.`email` = `duplicate`.`email`)
    /// WHERE (`source`.`is_active` = @p0)
    /// @p0 = True
    /// </summary>
    public static class InsertSelectQueryValidator
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

        // Runs INSERT SELECT scenarios for one database provider.
        private static void RunProvider<TProfile>(string providerName, IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            ProviderQueryPrinter.Print(
                $"{providerName} Insert Select",
                BuildInsertSelectQuery(queryBuilder));

            ProviderQueryPrinter.Print(
                $"{providerName} Insert Select With Where",
                BuildInsertSelectWithWhereQuery(queryBuilder));

            ProviderQueryPrinter.Print(
                $"{providerName} Insert Select With Same-Type Join",
                BuildInsertSelectWithSameTypeJoinQuery(queryBuilder));
        }

        // Builds an INSERT SELECT command from a metadata-resolved source.
        private static GeneratedSqlQuery BuildInsertSelectQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .InsertInto<JoinUser>()
                .Columns(user => new
                {
                    user.Email,
                    user.IsActive
                })
                .From<JoinUser>(alias: "source")
                .Select<JoinUser>(user => new
                {
                    user.Email,
                    user.IsActive
                })
                .Build();
        }

        // Builds an INSERT SELECT command with source filtering.
        private static GeneratedSqlQuery BuildInsertSelectWithWhereQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .InsertInto<JoinUser>()
                .Columns(user => new
                {
                    user.Email,
                    user.IsActive
                })
                .From<JoinUser>(alias: "source")
                .Select<JoinUser>(user => new
                {
                    user.Email,
                    user.IsActive
                })
                .Where<JoinUser>(user => user.IsActive)
                .Build();
        }

        // Builds an INSERT SELECT command with multiple same-type source instances.
        private static GeneratedSqlQuery BuildInsertSelectWithSameTypeJoinQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .InsertInto<JoinUser>()
                .Columns(user => new
                {
                    user.Email,
                    user.IsActive
                })
                .From<JoinUser>(alias: "source")
                .LeftJoin<JoinUser, JoinUser>(
                    alias: "duplicate",
                    on: (source, duplicate) => source.Email == duplicate.Email)
                .Select<JoinUser>(source => new
                {
                    source.Email,
                    source.IsActive
                })
                .Where<JoinUser>(source => source.IsActive)
                .Build();
        }
    }
}
