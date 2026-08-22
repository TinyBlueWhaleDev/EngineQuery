using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators
{
    /// <summary>
    /// Validates DELETE command generation across providers.
    /// </summary>
    public static class DeleteCommandQueryValidator
    {
        /// <summary>
        /// Runs the validator.
        /// </summary>
        public static void Run()
        {
            var metadataResolver = ProviderMetadataFactory.CreateJoinMetadataResolver();

            ProviderQueryPrinter.Print(
                "SQL Server Delete Command",
                BuildQuery(ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL Delete Command",
                BuildQuery(ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL Delete Command",
                BuildQuery(ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));
        }

        // Builds a strongly typed DELETE command.
        private static GeneratedSqlQuery BuildQuery(QueryBuilder queryBuilder)
        {
            return queryBuilder
                .DeleteFrom<JoinUser>()
                .Where(user => user.Id == 10)
                .Build();
        }
    }
}
