using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators
{

    /// <summary>
    /// Validates DISTINCT query generation across providers.
    /// </summary>
    public static class DistinctQueryValidator
    {
        /// <summary>
        /// Runs the validator.
        /// </summary>
        public static void Run()
        {
            var metadataResolver = ProviderMetadataFactory.CreateJoinMetadataResolver();

            ProviderQueryPrinter.Print(
                "SQL Server DISTINCT",
                BuildQuery(ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL DISTINCT",
                BuildQuery(ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL DISTINCT",
                BuildQuery(ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));
        }

        // Builds a DISTINCT query.
        private static GeneratedSqlQuery BuildQuery(
            QueryBuilder queryBuilder)
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .Distinct()
                .Select<JoinUser>(u => new
                {
                    u.Email
                })
                .Build();
        }
    }
}
