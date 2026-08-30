using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators
{

    /// <summary>
    /// Validates common table expression generation across providers.
    /// </summary>
    public static class CommonTableExpressionQueryValidator
    {
        /// <summary>
        /// Runs the validator.
        /// </summary>
        public static void Run()
        {
            var metadataResolver = ProviderMetadataFactory.CreateJoinMetadataResolver();

            ProviderQueryPrinter.Print(
                "SQL Server CTE",
                BuildQuery(ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL CTE",
                BuildQuery(ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL CTE",
                BuildQuery(ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));
        }

        // Builds a query using a common table expression.
        private static GeneratedSqlQuery BuildQuery(
            QueryBuilder queryBuilder)
        {
            return queryBuilder
                .With<OrderSummary, JoinOrder>(
                    "order_summary",
                    cte => cte
                        .From<JoinOrder>(alias: "o")
                        .Select<JoinOrder>(o => new
                        {
                            UserId = o.UserId
                        })
                        .SelectAggregate<JoinOrder>(
                            QueryAggregateFunction.Sum,
                            o => o.Total,
                            alias: "TotalAmount")
                        .SelectAggregate<JoinOrder>(
                            QueryAggregateFunction.Count,
                            o => o.Id,
                            alias: "OrderCount")
                        .GroupBy<JoinOrder>(o => o.UserId))
                .FromCte<OrderSummary>("order_summary")
                .Select<OrderSummary>(summary => new
                {
                    summary.UserId,
                    summary.TotalAmount,
                    summary.OrderCount
                })
                .WhereComputed<OrderSummary>(
                    summary => summary.TotalAmount > 500)
                .OrderByDescending<OrderSummary>(
                    summary => summary.TotalAmount)
                .Build();
        }
    }
}
