using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators
{

    /// <summary>
    /// Validates IN subquery generation across providers.
    /// </summary>
    public static class InSubqueryQueryValidator
    {
        /// <summary>
        /// Runs the validator.
        /// </summary>
        public static void Run()
        {
            var metadataResolver = ProviderMetadataFactory.CreateJoinMetadataResolver();

            ProviderQueryPrinter.Print(
                "SQL Server IN Subquery",
                BuildQuery(ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL IN Subquery",
                BuildQuery(ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL IN Subquery",
                BuildQuery(ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));
        }

        // Builds a query with an IN subquery.
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
                .WhereIn<JoinUser, JoinOrder>(
                    u => u.Id,
                    alias: "o",
                    subquery => subquery
                        .Select<JoinOrder>(o => new
                        {
                            o.UserId
                        })
                        .Where<JoinOrder>(o => o.Total > 100))
                .Build();
        }
    }
}
