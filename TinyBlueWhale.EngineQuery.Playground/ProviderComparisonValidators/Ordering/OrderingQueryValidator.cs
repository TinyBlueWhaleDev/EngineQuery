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

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.Ordering
{
    /// <summary>
    /// Validates ordering generation across supported database providers.
    /// </summary>
    public static class OrderingQueryValidator
    {
        /// <summary>
        /// Runs ordering validation scenarios across supported database providers.
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
        /// Runs all ordering scenarios for a database provider.
        /// </summary>
        /// <typeparam name="TProfile">Database provider profile.</typeparam>
        /// <param name="providerName">Display name of the database provider.</param>
        /// <param name="queryBuilder">Query builder used to construct the scenarios.</param>
        private static void RunProvider<TProfile>(string providerName, IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            ProviderQueryPrinter.Print(
                $"{providerName} Ordering - Ascending",
                BuildAscendingQuery(queryBuilder));

            ProviderQueryPrinter.Print(
                $"{providerName} Ordering - Descending",
                BuildDescendingQuery(queryBuilder));

            ProviderQueryPrinter.Print(
                $"{providerName} Ordering - ThenBy",
                BuildThenByQuery(queryBuilder));

            ProviderQueryPrinter.Print(
                $"{providerName} Ordering - Mixed Directions",
                BuildMixedDirectionQuery(queryBuilder));
        }

        /// <summary>
        /// Builds a query using ascending ordering.
        /// </summary>
        private static GeneratedSqlQuery BuildAscendingQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .OrderBy<JoinUser>(u => u.Email)
                .Build();
        }

        /// <summary>
        /// Builds a query using descending ordering.
        /// </summary>
        private static GeneratedSqlQuery BuildDescendingQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .OrderByDescending<JoinUser>(u => u.Email)
                .Build();
        }

        /// <summary>
        /// Builds a query using primary and secondary ascending ordering.
        /// </summary>
        private static GeneratedSqlQuery BuildThenByQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .OrderBy<JoinUser>(u => u.Email)
                .ThenBy<JoinUser>(u => u.Id)
                .Build();
        }

        /// <summary>
        /// Builds a query using mixed ordering directions.
        /// </summary>
        private static GeneratedSqlQuery BuildMixedDirectionQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .OrderBy<JoinUser>(u => u.Email)
                .ThenByDescending<JoinUser>(u => u.Id)
                .Build();
        }
    }
}
