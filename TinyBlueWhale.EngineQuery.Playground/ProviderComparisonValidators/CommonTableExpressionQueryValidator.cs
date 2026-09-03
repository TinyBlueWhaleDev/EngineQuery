using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Abstractions.Extensions;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding;
using TinyBlueWhale.EngineQuery.MySql.Profiles;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;
using TinyBlueWhale.EngineQuery.PostgreSql.Profiles;
using TinyBlueWhale.EngineQuery.SqlServer.Profiles;

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
                BuildSqlServerQuery(
                    ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL CTE",
                BuildPostgreSqlQuery(
                    ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL CTE",
                BuildMySqlQuery(
                    ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));
        }

        /// <summary>
        /// Builds a SQL Server query using a common table expression.
        /// </summary>
        /// <param name="queryBuilder">
        /// SQL Server query builder configured with a profile that supports common table expressions.
        /// </param>
        /// <returns>
        /// Generated SQL Server query.
        /// </returns>
        private static GeneratedSqlQuery BuildSqlServerQuery(QueryBuilder<SqlServer2012Profile> queryBuilder)
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
                .WhereComputed<OrderSummary>(summary => summary.TotalAmount > 500)
                .OrderByDescending<OrderSummary>(summary => summary.TotalAmount)
                .Build();
        }

        /// <summary>
        /// Builds a PostgreSQL query using a common table expression.
        /// </summary>
        /// <param name="queryBuilder">
        /// PostgreSQL query builder configured with a profile that supports common table expressions.
        /// </param>
        /// <returns>
        /// Generated PostgreSQL query.
        /// </returns>
        private static GeneratedSqlQuery BuildPostgreSqlQuery(QueryBuilder<PostgreSql93Profile> queryBuilder)
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
                .WhereComputed<OrderSummary>(summary => summary.TotalAmount > 500)
                .OrderByDescending<OrderSummary>(summary => summary.TotalAmount)
                .Build();
        }

        /// <summary>
        /// Builds a MySQL query using a common table expression.
        /// </summary>
        /// <param name="queryBuilder">
        /// MySQL query builder configured with a profile that supports common table expressions.
        /// </param>
        /// <returns>
        /// Generated MySQL query.
        /// </returns>
        private static GeneratedSqlQuery BuildMySqlQuery(QueryBuilder<MySql8031Profile> queryBuilder)
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
                .WhereComputed<OrderSummary>(summary => summary.TotalAmount > 500)
                .OrderByDescending<OrderSummary>(summary => summary.TotalAmount)
                .Build();
        }       
    }
}
