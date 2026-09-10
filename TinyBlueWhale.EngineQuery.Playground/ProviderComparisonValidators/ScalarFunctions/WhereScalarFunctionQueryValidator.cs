using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.ScalarFunctions
{
    /// <summary>
    /// Validates scalar SQL functions used as WHERE predicates, including
    /// provider-specific function translation and parameterized comparisons.
    ///
    /// SQL Server:
    /// SELECT [u].[user_id] AS [UserId], [u].[email]
    /// FROM [users] AS [u]
    /// WHERE LOWER([u].[email]) = @p0 AND LEN([u].[email]) &gt; @p1
    ///
    /// PostgreSQL:
    /// SELECT "u"."user_id" AS "UserId", "u"."email"
    /// FROM "users" AS "u"
    /// WHERE LOWER("u"."email") = @p0 AND LENGTH("u"."email") &gt; @p1
    ///
    /// MySQL:
    /// SELECT `u`.`user_id` AS `UserId`, `u`.`email`
    /// FROM `users` AS `u`
    /// WHERE LOWER(`u`.`email`) = @p0 AND LENGTH(`u`.`email`) &gt; @p1
    ///
    /// Expected parameters:
    /// @p0 = admin@test.com
    /// @p1 = 10
    /// </summary>
    public static class WhereScalarFunctionQueryValidator
    {
        public static void Run()
        {
            var metadataResolver = ProviderMetadataFactory.CreateJoinMetadataResolver();

            ProviderQueryPrinter.Print(
                "SQL Server Where Scalar Functions",
                BuildQuery(ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL Where Scalar Functions",
                BuildQuery(ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL Where Scalar Functions",
                BuildQuery(ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));
        }

        // Builds a query with scalar SQL function predicates.
        private static GeneratedSqlQuery BuildQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .WhereScalarFunction<JoinUser>(
                    QueryScalarFunction.Lower,
                    u => u.Email,
                    QueryComparisonOperator.Equal,
                    "admin@test.com")
                .WhereScalarFunction<JoinUser>(
                    QueryScalarFunction.Length,
                    u => u.Email,
                    QueryComparisonOperator.GreaterThan,
                    10)
                .Build();
        }
    }
}
