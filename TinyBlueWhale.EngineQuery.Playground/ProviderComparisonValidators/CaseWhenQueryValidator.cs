using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators
{

    /// <summary>
    /// Validates CASE WHEN SQL expression projections across providers.
    /// </summary>
    public static class CaseWhenQueryValidator
    {
        /// <summary>
        /// Runs the validator.
        /// </summary>
        public static void Run()
        {
            var metadataResolver = ProviderMetadataFactory.CreateJoinMetadataResolver();

            ProviderQueryPrinter.Print(
                "SQL Server CASE WHEN",
                BuildQuery(ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL CASE WHEN",
                BuildQuery(ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL CASE WHEN",
                BuildQuery(ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));
        }

        // Builds a query with CASE WHEN SQL projections.
        private static GeneratedSqlQuery BuildQuery(QueryBuilder queryBuilder)
        {
            return queryBuilder
                .From<JoinOrder>(alias: "o")
                .Select<JoinOrder>(o => new
                {
                    OrderId = o.Id,
                    o.Total
                })
                .SelectCaseWhen<JoinOrder>(
                    o => o.Total > 1000 && o.Total < 5000,
                    whenTrue: "VIP",
                    whenFalse: "STANDARD",
                    alias: "CustomerType")
                .SelectCaseWhen<JoinOrder>(
                    o => o.Total <= 0,
                    whenTrue: "INVALID",
                    whenFalse: "VALID",
                    alias: "OrderStatus")
                .Build();
        }
    }
}
