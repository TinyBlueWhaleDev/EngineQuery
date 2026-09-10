using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.Aggregates
{
    /// <summary>
    /// Validates GROUP BY generation over columns from multiple query sources.
    ///
    /// SQL Server:
    /// SELECT [u].[user_id] AS [UserId], [u].[email], [o].[user_id]
    /// FROM [users] AS [u]
    /// INNER JOIN [orders] AS [o] ON ([u].[user_id] = [o].[user_id])
    /// GROUP BY [u].[user_id], [u].[email], [o].[user_id]
    ///
    /// PostgreSQL:
    /// SELECT "u"."user_id" AS "UserId", "u"."email", "o"."user_id"
    /// FROM "users" AS "u"
    /// INNER JOIN "orders" AS "o" ON ("u"."user_id" = "o"."user_id")
    /// GROUP BY "u"."user_id", "u"."email", "o"."user_id"
    ///
    /// MySQL:
    /// SELECT `u`.`user_id` AS `UserId`, `u`.`email`, `o`.`user_id`
    /// FROM `users` AS `u`
    /// INNER JOIN `orders` AS `o` ON (`u`.`user_id` = `o`.`user_id`)
    /// GROUP BY `u`.`user_id`, `u`.`email`, `o`.`user_id`
    /// </summary>
    public static class GroupByQueryValidator
    {
        public static void Run()
        {
            var metadataResolver = ProviderMetadataFactory.CreateJoinMetadataResolver();

            ProviderQueryPrinter.Print("SQL Server GroupBy", BuildQuery(ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));
            ProviderQueryPrinter.Print("PostgreSQL GroupBy", BuildQuery(ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));
            ProviderQueryPrinter.Print("MySQL GroupBy", BuildQuery(ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));
        }

        private static GeneratedSqlQuery BuildQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .InnerJoin<JoinUser, JoinOrder>(alias: "o", on: (u, o) => u.Id == o.UserId)
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .Select<JoinOrder>(o => new
                {
                    o.UserId
                })
                .GroupBy<JoinUser>(u => new
                {
                    u.Id,
                    u.Email
                })
                .GroupBy<JoinOrder>(o => o.UserId)
                .Build();
        }
    }
}
