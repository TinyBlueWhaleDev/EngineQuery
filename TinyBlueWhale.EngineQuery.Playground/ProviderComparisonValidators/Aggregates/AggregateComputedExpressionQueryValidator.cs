using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.Aggregates
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
        private static GeneratedSqlQuery BuildQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
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
    }
}
