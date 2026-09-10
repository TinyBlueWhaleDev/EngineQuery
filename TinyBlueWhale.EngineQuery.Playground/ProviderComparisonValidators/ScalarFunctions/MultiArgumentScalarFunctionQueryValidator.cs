using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.ScalarFunctions
{

    /// <summary>
    /// Validates multi-argument scalar SQL function projections with mixed
    /// column and parameter arguments across supported database providers.
    ///
    /// SQL Server:
    /// SELECT [u].[user_id] AS [UserId], COALESCE([u].[email], @p0) AS [SafeEmail], CONCAT([u].[email], @p1) AS [EmailLabel]
    /// FROM [users] AS [u]
    ///
    /// PostgreSQL:
    /// SELECT "u"."user_id" AS "UserId", COALESCE("u"."email", @p0) AS "SafeEmail", CONCAT("u"."email", @p1) AS "EmailLabel"
    /// FROM "users" AS "u"
    ///
    /// MySQL:
    /// SELECT `u`.`user_id` AS `UserId`, COALESCE(`u`.`email`, @p0) AS `SafeEmail`, CONCAT(`u`.`email`, @p1) AS `EmailLabel`
    /// FROM `users` AS `u`
    ///
    /// Expected parameters:
    /// @p0 = NO_EMAIL
    /// @p1 =  - ACTIVE
    /// </summary>
    public static class MultiArgumentScalarFunctionQueryValidator
    {
        public static void Run()
        {
            var metadataResolver = ProviderMetadataFactory.CreateJoinMetadataResolver();

            ProviderQueryPrinter.Print(
                "SQL Server Multi-Argument Scalar Functions",
                BuildQuery(ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL Multi-Argument Scalar Functions",
                BuildQuery(ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL Multi-Argument Scalar Functions",
                BuildQuery(ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));
        }

        // Builds a query with multi-argument scalar SQL function projections.        
        private static GeneratedSqlQuery BuildQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id
                })
                .SelectScalarFunction<JoinUser>(
                    QueryScalarFunction.Coalesce,
                    u => new object[]
                    {
                        u.Email,
                        "NO_EMAIL"
                    },
                    alias: "SafeEmail")
                .SelectScalarFunction<JoinUser>(
                    QueryScalarFunction.Concat,
                    u => new object[]
                    {
                        u.Email,
                        " - ACTIVE"
                    },
                    alias: "EmailLabel")
                .Build();
        }
    }
}
