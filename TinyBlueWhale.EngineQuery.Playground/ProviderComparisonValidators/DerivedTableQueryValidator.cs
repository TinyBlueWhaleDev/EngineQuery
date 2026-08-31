using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators
{

    /// <summary>
    /// Validates derived table generation across providers.
    /// </summary>
    public static class DerivedTableQueryValidator
    {
        /// <summary>
        /// Runs the validator.
        /// </summary>
        public static void Run()
        {
            var metadataResolver = ProviderMetadataFactory.CreateJoinMetadataResolver();

            ProviderQueryPrinter.Print(
                "SQL Server Derived Table",
                BuildQuery(ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL Derived Table",
                BuildQuery(ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL Derived Table",
                BuildQuery(ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));
        }

        // Builds a query using a derived table as the root source.
        private static GeneratedSqlQuery BuildQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .FromSubquery<OrderSummary, JoinOrder>(
                    alias: "summary",
                    subquery => subquery
                        .From<JoinOrder>(alias: "o")
                        .Select<JoinOrder>(o => new
                        {
                            o.UserId
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
