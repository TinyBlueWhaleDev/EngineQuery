using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators
{

    /// <summary>
    /// Validates ROW_NUMBER window function generation across providers.
    /// </summary>
    public static class RowNumberWindowFunctionQueryValidator
    {
        /// <summary>
        /// Runs the validator.
        /// </summary>
        public static void Run()
        {
            var metadataResolver = ProviderMetadataFactory.CreateJoinMetadataResolver();

            ProviderQueryPrinter.Print(
                "SQL Server ROW_NUMBER",
                BuildQuery(ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL ROW_NUMBER",
                BuildQuery(ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL ROW_NUMBER",
                BuildQuery(ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));
        }

        // Builds a query using ROW_NUMBER with PARTITION BY and ORDER BY.
        private static GeneratedSqlQuery BuildQuery(
            QueryBuilder queryBuilder)
        {
            return queryBuilder
                .From<JoinOrder>(alias: "o")
                .Select<JoinOrder>(o => new
                {
                    OrderId = o.Id,
                    o.UserId,
                    o.Total
                })
                .SelectRowNumber(
                    alias: "UserOrderRank",
                    window => window
                        .PartitionBy<JoinOrder>(o => o.UserId)
                        .OrderByDescending<JoinOrder>(o => o.Total))
                .Build();
        }
    }
}
