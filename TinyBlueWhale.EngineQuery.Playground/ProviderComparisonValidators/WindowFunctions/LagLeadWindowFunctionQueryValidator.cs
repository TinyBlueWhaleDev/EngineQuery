using TinyBlueWhale.EngineQuery.Abstractions.Extensions;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Features;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.WindowFunctions
{
    /// <summary>
    /// Validates LAG and LEAD window function generation across providers.
    /// </summary>
    public static class LagLeadWindowFunctionQueryValidator
    {
        /// <summary>
        /// Runs the validator.
        /// </summary>
        public static void Run()
        {
            var metadataResolver = ProviderMetadataFactory.CreateJoinMetadataResolver();

            ProviderQueryPrinter.Print(
                "SQL Server LAG / LEAD Window Functions",
                BuildQuery(ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL LAG / LEAD Window Functions",
                BuildQuery(ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL LAG / LEAD Window Functions",
                BuildQuery(ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));
        }

        /// <summary>
        /// Builds a query using LAG and LEAD window functions.
        /// </summary>
        /// <typeparam name="TProfile">
        /// Database provider profile type.
        /// </typeparam>
        /// <param name="queryBuilder">
        /// Query builder configured with a profile that supports window functions.
        /// </param>
        /// <returns>
        /// Generated SQL query.
        /// </returns>
        private static GeneratedSqlQuery BuildQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile, IWindowFunctionFeature
        {
            return queryBuilder
                .From<JoinOrder>(alias: "o")
                .Select<JoinOrder>(o => new
                {
                    OrderId = o.Id,
                    o.UserId,
                    o.Total
                })
                .SelectLag(
                    (JoinOrder o) => o.Total,
                    alias: "OrderRank",
                    windowBuilder: window => window
                        .PartitionBy<JoinOrder>(o => o.UserId)
                        .OrderBy<JoinOrder>(o => o.Total))
                .SelectLead(
                    (JoinOrder o) => o.Total,
                    alias: "NextOrderTotal",
                    windowBuilder: window => window
                        .PartitionBy<JoinOrder>(o => o.UserId)
                        .OrderBy<JoinOrder>(o => o.Id))
                .Build();
        }
    }
}
