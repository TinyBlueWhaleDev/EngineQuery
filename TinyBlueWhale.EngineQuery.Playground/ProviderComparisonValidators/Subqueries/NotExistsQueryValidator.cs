using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.Subqueries
{
    /// <summary>
    /// Validates correlated NOT EXISTS subqueries and outer source resolution
    /// across supported database providers.
    ///
    /// SQL Server:
    /// SELECT [u].[user_id] AS [UserId], [u].[email]
    /// FROM [users] AS [u]
    /// WHERE NOT EXISTS (SELECT 1
    /// FROM [orders] AS [o]
    /// WHERE (([o].[user_id] = [u].[user_id]) AND ([o].[total] &gt; @p0)))
    ///
    /// PostgreSQL:
    /// SELECT "u"."user_id" AS "UserId", "u"."email"
    /// FROM "users" AS "u"
    /// WHERE NOT EXISTS (SELECT 1
    /// FROM "orders" AS "o"
    /// WHERE (("o"."user_id" = "u"."user_id") AND ("o"."total" &gt; @p0)))
    ///
    /// MySQL:
    /// SELECT `u`.`user_id` AS `UserId`, `u`.`email`
    /// FROM `users` AS `u`
    /// WHERE NOT EXISTS (SELECT 1
    /// FROM `orders` AS `o`
    /// WHERE ((`o`.`user_id` = `u`.`user_id`) AND (`o`.`total` &gt; @p0)))
    ///
    /// Expected parameters:
    /// @p0 = 100
    /// </summary>
    public static class NotExistsQueryValidator
    {
        public static void Run()
        {
            var metadataResolver = ProviderMetadataFactory.CreateJoinMetadataResolver();

            ProviderQueryPrinter.Print(
                "SQL Server NOT EXISTS",
                BuildQuery(ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL NOT EXISTS",
                BuildQuery(ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL NOT EXISTS",
                BuildQuery(ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));
        }

        // Builds a query with a correlated NOT EXISTS subquery.
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
                .WhereNotExists<JoinUser, JoinOrder>(
                    alias: "o",
                    subquery => subquery
                        .WhereComputed<JoinOrder, JoinUser>(
                            (o, u) => o.UserId == u.Id && o.Total > 100))
                .Build();
        }
    }
}
