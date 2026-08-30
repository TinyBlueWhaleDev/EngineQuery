using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;


namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators
{
    /// <summary>
    /// Validates table alias generation across providers.
    /// </summary>
    public static class TableAliasQueryValidator
    {
        /// <summary>
        /// Runs the validator.
        /// </summary>
        public static void Run()
        {
            var metadataResolver = ProviderMetadataFactory.CreateJoinMetadataResolver();

            ProviderQueryPrinter.Print("SQL Server Table Alias", BuildQuery(ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));
            ProviderQueryPrinter.Print("PostgreSQL Table Alias", BuildQuery(ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));
            ProviderQueryPrinter.Print("MySQL Table Alias", BuildQuery(ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));
        }

        // Builds an alias-qualified query.
        private static GeneratedSqlQuery BuildQuery(QueryBuilder queryBuilder)
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .Where<JoinUser>(u => u.IsActive)
                .Build();
        }
    }
}
