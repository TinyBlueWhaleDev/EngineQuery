using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators
{
    /// <summary>
    /// Validates FIRST_VALUE and LAST_VALUE window function generation across providers.
    /// </summary>
    public static class FirstLastValueWindowFunctionQueryValidator
    {
        /// <summary>
        /// Runs the validator.
        /// </summary>
        public static void Run()
        {
            var metadataResolver = ProviderMetadataFactory.CreateJoinMetadataResolver();

            ProviderQueryPrinter.Print(
                "SQL Server First Last Value Window Functions",
                BuildQuery(ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL First Last Value Window Functions",
                BuildQuery(ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL First Last Value Window Functions",
                BuildQuery(ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));
        }

        // Builds a query using FIRST_VALUE and LAST_VALUE window functions.
        private static GeneratedSqlQuery BuildQuery(QueryBuilder queryBuilder)
        {
            return queryBuilder
                .From<JoinOrder>(alias: "o")
                .Select<JoinOrder>(o => new
                {
                    OrderId = o.Id,
                    o.UserId,
                    o.Total
                })
                .SelectFirstValue<JoinOrder>(
                    expression: o => o.Total,
                    alias: "FirstOrderTotal",
                    windowBuilder: window => window
                        .PartitionBy<JoinOrder>(o => o.UserId)
                        .OrderBy<JoinOrder>(o => o.Id))
                .SelectLastValue<JoinOrder>(
                    expression: o => o.Total,
                    alias: "LastOrderTotal",
                    windowBuilder: window => window
                        .PartitionBy<JoinOrder>(o => o.UserId)
                        .OrderBy<JoinOrder>(o => o.Id))
                .Build();
        }
    }
}
