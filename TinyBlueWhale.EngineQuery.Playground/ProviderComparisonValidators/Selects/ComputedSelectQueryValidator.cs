using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.Selects
{
    /// <summary>
    /// Validates computed SELECT projections across supported database providers.
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

        /// <summary>
        /// Runs computed SELECT scenarios for a database provider.
        /// </summary>
        /// <typeparam name="TProfile">Database provider profile.</typeparam>
        /// <param name="providerName">Display name of the database provider.</param>
        /// <param name="queryBuilder">Query builder used to construct the scenario.</param>
        private static void RunProvider<TProfile>(string providerName, IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            ProviderQueryPrinter.Print(
                $"{providerName} Computed Select",
                BuildComputedSelectQuery(queryBuilder));
        }

        /// <summary>
        /// Builds a query containing regular and computed projections.
        /// </summary>
        /// <typeparam name="TProfile">Database provider profile.</typeparam>
        /// <param name="queryBuilder">Query builder used to construct the query.</param>
        /// <returns>Generated SQL query.</returns>
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
