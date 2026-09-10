using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.Selects
{

    /// <summary>
    /// Validates DISTINCT composition with filtering and ordering
    /// across supported database providers.
    ///
    /// SQL Server:
    /// SELECT DISTINCT [u].[email]
    /// FROM [users] AS [u]
    /// WHERE ([u].[is_active] = @p0)
    /// ORDER BY [u].[email] ASC
    ///
    /// PostgreSQL:
    /// SELECT DISTINCT "u"."email"
    /// FROM "users" AS "u"
    /// WHERE ("u"."is_active" = @p0)
    /// ORDER BY "u"."email" ASC
    ///
    /// MySQL:
    /// SELECT DISTINCT `u`.`email`
    /// FROM `users` AS `u`
    /// WHERE (`u`.`is_active` = @p0)
    /// ORDER BY `u`.`email` ASC
    ///
    /// Expected parameters:
    /// @p0 = True
    /// </summary>
    public static class DistinctQueryValidator
    {        
        public static void Run()
        {
            var metadataResolver = ProviderMetadataFactory.CreateJoinMetadataResolver();

            ProviderQueryPrinter.Print(
                "SQL Server DISTINCT",
                BuildDistinctCompositionQuery(ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL DISTINCT",
                BuildDistinctCompositionQuery(ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL DISTINCT",
                BuildDistinctCompositionQuery(ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));
        }

        // Builds a DISTINCT query.
        private static GeneratedSqlQuery BuildDistinctCompositionQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(user => new
                {
                    user.Email
                })
                .Distinct()
                .Where<JoinUser>(user => user.IsActive)
                .OrderBy<JoinUser>(user => user.Email)
                .Build();
        }
    }
}
