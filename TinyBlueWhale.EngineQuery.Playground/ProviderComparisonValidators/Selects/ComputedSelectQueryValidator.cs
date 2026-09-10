using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.Selects
{
    /// <summary>
    /// Validates regular and arithmetic computed SELECT projections, including
    /// expression grouping, aliases and parameter ordering across supported database providers.
    ///
    /// SQL Server:
    /// SELECT [o].[order_id] AS [OrderId], [o].[total], ([o].[total] * @p0) AS [TotalWithTax], (([o].[total] * @p1) - @p2) AS [FinalAmount]
    /// FROM [orders] AS [o]
    ///
    /// PostgreSQL:
    /// SELECT "o"."order_id" AS "OrderId", "o"."total", ("o"."total" * @p0) AS "TotalWithTax", (("o"."total" * @p1) - @p2) AS "FinalAmount"
    /// FROM "orders" AS "o"
    ///
    /// MySQL:
    /// SELECT `o`.`order_id` AS `OrderId`, `o`.`total`, (`o`.`total` * @p0) AS `TotalWithTax`, ((`o`.`total` * @p1) - @p2) AS `FinalAmount`
    /// FROM `orders` AS `o`
    ///
    /// Expected parameters:
    /// @p0 = 1.16
    /// @p1 = 1.16
    /// @p2 = 100
    /// </summary>
    public static class ComputedSelectQueryValidator
    {
        /// <summary>
        /// Runs computed SELECT validation scenarios across supported database providers.
        /// </summary>
        public static void Run()
        {
            var metadataResolver = ProviderMetadataFactory.CreateJoinMetadataResolver();

            RunProvider(
                "SQL Server",
                ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver));

            RunProvider(
                "PostgreSQL",
                ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver));

            RunProvider(
                "MySQL",
                ProviderQueryBuilderFactory.CreateMySql(metadataResolver));
        }

        // Runs the computed SELECT scenario for one database provider.
        private static void RunProvider<TProfile>(string providerName, IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            ProviderQueryPrinter.Print(
                $"{providerName} Computed Select",
                BuildComputedSelectQuery(queryBuilder));
        }

        // Builds a query containing regular and computed projections.
        private static GeneratedSqlQuery BuildComputedSelectQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .From<JoinOrder>(alias: "o")
                .Select<JoinOrder>(order => new
                {
                    OrderId = order.Id,
                    order.Total
                })
                .SelectComputed<JoinOrder>(
                    order => order.Total * 1.16m,
                    alias: "TotalWithTax")
                .SelectComputed<JoinOrder>(
                    order => (order.Total * 1.16m) - 100m,
                    alias: "FinalAmount")
                .Build();
        }
    }
}
