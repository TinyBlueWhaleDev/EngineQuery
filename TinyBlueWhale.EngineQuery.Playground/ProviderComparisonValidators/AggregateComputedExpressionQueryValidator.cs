using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators
{
    /// <summary>
    /// Validates aggregate projections using computed expressions.
    /// </summary>
    public static class AggregateComputedExpressionQueryValidator
    {
        /// <summary>
        /// Runs the validator.
        /// </summary>
        public static void Run()
        {
            var metadataResolver = ProviderMetadataFactory.CreateJoinMetadataResolver();

            ProviderQueryPrinter.Print(
                "SQL Server Aggregate Computed Expression",
                BuildQuery(ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL Aggregate Computed Expression",
                BuildQuery(ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL Aggregate Computed Expression",
                BuildQuery(ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));

            ValidateUnsupportedCountComputedExpression();
        }

        /// <summary>
        /// Builds a grouped query with aggregate computed expression projections.
        /// </summary>
        /// <param name="queryBuilder">
        /// Query builder.
        /// </param>
        /// <returns>
        /// Generated SQL query.
        /// </returns>
        private static GeneratedSqlQuery BuildQuery(QueryBuilder queryBuilder)
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .InnerJoin<JoinUser, JoinOrder>(
                    alias: "o",
                    on: (u, o) => u.Id == o.UserId)
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .SelectAggregate<JoinOrder>(
                    QueryAggregateFunction.Sum,
                    o => o.Total * 1.16m,
                    alias: "TotalWithTax")
                .SelectAggregate<JoinOrder>(
                    QueryAggregateFunction.Average,
                    o => (o.Total * 1.16m) - 100,
                    alias: "AverageFinalAmount")
                .SelectAggregate<JoinOrder>(
                    QueryAggregateFunction.Minimum,
                    o => o.Total * 1.16m,
                    alias: "MinimumTotalWithTax")
                .SelectAggregate<JoinOrder>(
                    QueryAggregateFunction.Maximum,
                    o => (o.Total * 1.16m) - 100,
                    alias: "MaximumFinalAmount")
                .GroupBy<JoinUser>(u => new
                {
                    u.Id,
                    u.Email
                })
                .Build();
        }

        /// <summary>
        /// Validates that COUNT does not support computed aggregate expressions.
        /// </summary>
        private static void ValidateUnsupportedCountComputedExpression()
        {
            var metadataResolver = ProviderMetadataFactory.CreateJoinMetadataResolver();

            try
            {
                BuildUnsupportedCountQuery(
                    ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver));

                throw new InvalidOperationException(
                    "Expected COUNT computed aggregate expression exception was not thrown.");
            }
            catch (NotSupportedException exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        /// <summary>
        /// Builds an unsupported COUNT computed aggregate expression query.
        /// </summary>
        /// <param name="queryBuilder">
        /// Query builder.
        /// </param>
        /// <returns>
        /// Generated SQL query.
        /// </returns>
        private static GeneratedSqlQuery BuildUnsupportedCountQuery(QueryBuilder queryBuilder)
        {
            return queryBuilder
                .From<JoinOrder>(alias: "o")
                .SelectAggregate<JoinOrder>(
                    QueryAggregateFunction.Count,
                    o => o.Total * 1.16m,
                    alias: "InvalidCount")
                .Build();
        }
    }
}
