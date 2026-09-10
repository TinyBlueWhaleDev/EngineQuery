using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Metadata.Resolvers;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.Metadata
{
    /// <summary>
    /// Validates attribute-based table and column metadata resolution across SELECT,
    /// INSERT, UPDATE and DELETE commands for all supported database providers.
    ///
    /// SQL Server SELECT:
    /// SELECT [u].[attribute_user_id], [u].[email_address], [u].[active_flag]
    /// FROM [attribute_users] AS [u]
    /// WHERE ([u].[active_flag] = @p0)
    /// ORDER BY [u].[attribute_user_id] ASC
    ///
    /// Expected parameters:
    /// @p0 = True
    ///
    /// SQL Server INSERT:
    /// INSERT INTO [attribute_users] ([email_address], [active_flag])
    /// VALUES (@p0, @p1)
    ///
    /// Expected parameters:
    /// @p0 = attribute@test.com
    /// @p1 = True
    ///
    /// SQL Server UPDATE:
    /// UPDATE [attribute_users]
    /// SET [email_address] = @p0
    /// WHERE ([attribute_user_id] = @p1)
    ///
    /// Expected parameters:
    /// @p0 = updated-attribute@test.com
    /// @p1 = 25
    ///
    /// SQL Server DELETE:
    /// DELETE
    /// FROM [attribute_users]
    /// WHERE ([attribute_user_id] = @p0)
    ///
    /// Expected parameters:
    /// @p0 = 25
    ///
    /// PostgreSQL SELECT:
    /// SELECT "u"."attribute_user_id", "u"."email_address", "u"."active_flag"
    /// FROM "attribute_users" AS "u"
    /// WHERE ("u"."active_flag" = @p0)
    /// ORDER BY "u"."attribute_user_id" ASC
    ///
    /// Expected parameters:
    /// @p0 = True
    ///
    /// PostgreSQL INSERT:
    /// INSERT INTO "attribute_users" ("email_address", "active_flag")
    /// VALUES (@p0, @p1)
    ///
    /// Expected parameters:
    /// @p0 = attribute@test.com
    /// @p1 = True
    ///
    /// PostgreSQL UPDATE:
    /// UPDATE "attribute_users"
    /// SET "email_address" = @p0
    /// WHERE ("attribute_user_id" = @p1)
    ///
    /// Expected parameters:
    /// @p0 = updated-attribute@test.com
    /// @p1 = 25
    ///
    /// PostgreSQL DELETE:
    /// DELETE
    /// FROM "attribute_users"
    /// WHERE ("attribute_user_id" = @p0)
    ///
    /// Expected parameters:
    /// @p0 = 25
    ///
    /// MySQL SELECT:
    /// SELECT `u`.`attribute_user_id`, `u`.`email_address`, `u`.`active_flag`
    /// FROM `attribute_users` AS `u`
    /// WHERE (`u`.`active_flag` = @p0)
    /// ORDER BY `u`.`attribute_user_id` ASC
    ///
    /// Expected parameters:
    /// @p0 = True
    ///
    /// MySQL INSERT:
    /// INSERT INTO `attribute_users` (`email_address`, `active_flag`)
    /// VALUES (@p0, @p1)
    ///
    /// Expected parameters:
    /// @p0 = attribute@test.com
    /// @p1 = True
    ///
    /// MySQL UPDATE:
    /// UPDATE `attribute_users`
    /// SET `email_address` = @p0
    /// WHERE (`attribute_user_id` = @p1)
    ///
    /// Expected parameters:
    /// @p0 = updated-attribute@test.com
    /// @p1 = 25
    ///
    /// MySQL DELETE:
    /// DELETE
    /// FROM `attribute_users`
    /// WHERE (`attribute_user_id` = @p0)
    ///
    /// Expected parameters:
    /// @p0 = 25
    /// </summary>
    public static class AttributeMetadataQueryValidator
    {
        public static void Run()
        {
            var metadataResolver = new AttributeEntityMetadataResolver();

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

        // Runs attribute metadata scenarios for one database provider.
        private static void RunProvider<TProfile>(string providerName, IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            ProviderQueryPrinter.Print(
                $"{providerName} Attribute Metadata Select",
                BuildSelectQuery(queryBuilder));

            ProviderQueryPrinter.Print(
                $"{providerName} Attribute Metadata Insert",
                BuildInsertQuery(queryBuilder));

            ProviderQueryPrinter.Print(
                $"{providerName} Attribute Metadata Update",
                BuildUpdateQuery(queryBuilder));

            ProviderQueryPrinter.Print(
                $"{providerName} Attribute Metadata Delete",
                BuildDeleteQuery(queryBuilder));
        }

        // Builds a SELECT command using attribute metadata.
        private static GeneratedSqlQuery BuildSelectQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .From<AttributeMappedUser>(alias: "u")
                .Select<AttributeMappedUser>(user => new
                {
                    user.Id,
                    user.Email,
                    user.IsActive
                })
                .Where<AttributeMappedUser>(user => user.IsActive)
                .OrderBy<AttributeMappedUser>(user => user.Id)
                .Build();
        }

        // Builds an INSERT command using attribute metadata.
        private static GeneratedSqlQuery BuildInsertQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .InsertInto<AttributeMappedUser>()
                .Set(user => user.Email, "attribute@test.com")
                .Set(user => user.IsActive, true)
                .Build();
        }

        // Builds an UPDATE command using attribute metadata.
        private static GeneratedSqlQuery BuildUpdateQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .Update<AttributeMappedUser>()
                .Set(user => user.Email, "updated-attribute@test.com")
                .Where(user => user.Id == 25)
                .Build();
        }

        // Builds a DELETE command using attribute metadata.
        private static GeneratedSqlQuery BuildDeleteQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .DeleteFrom<AttributeMappedUser>()
                .Where(user => user.Id == 25)
                .Build();
        }
    }
}
