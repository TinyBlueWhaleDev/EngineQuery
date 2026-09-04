using TinyBlueWhale.EngineQuery.Abstractions.Extensions;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Features;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.Pagination
{
    /// <summary>
    /// Validates pagination generation across supported database providers.
    /// </summary>
    public static class PaginationQueryValidator
    {
        /// <summary>
        /// Runs pagination validation scenarios across supported database providers.
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
        /// Runs all pagination scenarios for a database provider.
        /// </summary>
        /// <typeparam name="TProfile">
        /// Database provider profile used to configure query features.
        /// </typeparam>
        /// <param name="providerName">
        /// Display name of the database provider.
        /// </param>
        /// <param name="queryBuilder">
        /// Query builder used to construct pagination queries.
        /// </param>
        private static void RunProvider<TProfile>(string providerName, IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile, IPaginationFeature
        {
            ProviderQueryPrinter.Print(
                $"{providerName} Pagination - Take Only",
                BuildTakeOnlyQuery(queryBuilder));

            ProviderQueryPrinter.Print(
                $"{providerName} Pagination - Skip Only",
                BuildSkipOnlyQuery(queryBuilder));

            ProviderQueryPrinter.Print(
                $"{providerName} Pagination - Skip And Take",
                BuildSkipAndTakeQuery(queryBuilder));
        }

        /// <summary>
        /// Builds an ordered query using only the take pagination operation.
        /// </summary>
        /// <typeparam name="TProfile">
        /// Database provider profile used to configure query features.
        /// </typeparam>
        /// <param name="queryBuilder">
        /// Query builder used to construct the pagination query.
        /// </param>
        /// <returns>
        /// Compiled SQL query containing provider-specific take pagination syntax.
        /// </returns>
        private static GeneratedSqlQuery BuildTakeOnlyQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile, IPaginationFeature
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .OrderBy<JoinUser>(u => u.Id)
                .Take(10)
                .Build();
        }

        /// <summary>
        /// Builds an ordered query using only the skip pagination operation.
        /// </summary>
        /// <typeparam name="TProfile">
        /// Database provider profile used to configure query features.
        /// </typeparam>
        /// <param name="queryBuilder">
        /// Query builder used to construct the pagination query.
        /// </param>
        /// <returns>
        /// Compiled SQL query containing provider-specific skip pagination syntax.
        /// </returns>
        private static GeneratedSqlQuery BuildSkipOnlyQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile, IPaginationFeature
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .OrderBy<JoinUser>(u => u.Id)
                .Skip(20)
                .Build();
        }

        /// <summary>
        /// Builds an ordered query using both skip and take pagination operations.
        /// </summary>
        /// <typeparam name="TProfile">
        /// Database provider profile used to configure query features.
        /// </typeparam>
        /// <param name="queryBuilder">
        /// Query builder used to construct the pagination query.
        /// </param>
        /// <returns>
        /// Compiled SQL query containing provider-specific pagination syntax.
        /// </returns>
        private static GeneratedSqlQuery BuildSkipAndTakeQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile, IPaginationFeature
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .OrderBy<JoinUser>(u => u.Id)
                .Skip(20)
                .Take(10)
                .Build();
        }
    }
}
