using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators
{
    /// <summary>
    /// Validates string Contains predicates across providers.
    /// </summary>
    public static class WhereStringContainsQueryValidator
    {
        /// <summary>
        /// Runs the validator.
        /// </summary>
        public static void Run()
        {
            var metadataResolver =
                ProviderMetadataFactory.CreateJoinMetadataResolver();

            ProviderQueryPrinter.Print(
                "SQL Server Where String Contains",
                BuildQuery(
                    ProviderQueryBuilderFactory.CreateSqlServer(
                        metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL Where String Contains",
                BuildQuery(
                    ProviderQueryBuilderFactory.CreatePostgreSql(
                        metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL Where String Contains",
                BuildQuery(
                    ProviderQueryBuilderFactory.CreateMySql(
                        metadataResolver)));
        }

        // Builds a query using string Contains predicates.
        private static GeneratedSqlQuery BuildQuery(
            QueryBuilder queryBuilder)
        {
            const string search = "admin";

            return queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .Where<JoinUser>(
                    u => u.Email.Contains(search))
                .Build();
        }
    }
}
