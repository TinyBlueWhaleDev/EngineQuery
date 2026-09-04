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

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.Inserts
{

    /// Validates INSERT SELECT generation across supported database providers.
    /// </summary>
    public static class InsertSelectQueryValidator
    {
        /// <summary>
        /// Runs INSERT SELECT validation scenarios across supported database providers.
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
        /// Runs INSERT SELECT scenarios for a database provider.
        /// </summary>
        /// <typeparam name="TProfile">Database provider profile.</typeparam>
        /// <param name="providerName">Display name of the database provider.</param>
        /// <param name="queryBuilder">Query builder used to construct the scenarios.</param>
        private static void RunProvider<TProfile>(string providerName, IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            ProviderQueryPrinter.Print(
                $"{providerName} Insert Select",
                BuildInsertSelectQuery(queryBuilder));

            ProviderQueryPrinter.Print(
                $"{providerName} Insert Select With Where",
                BuildInsertSelectWithWhereQuery(queryBuilder));
        }

        /// <summary>
        /// Builds an INSERT SELECT command using resolved source metadata.
        /// </summary>
        /// <typeparam name="TProfile">Database provider profile.</typeparam>
        /// <param name="queryBuilder">Query builder used to construct the command.</param>
        /// <returns>Generated SQL query.</returns>
        private static GeneratedSqlQuery BuildInsertSelectQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .InsertInto<JoinUser>()
                .Columns(user => new
                {
                    user.Email,
                    user.IsActive
                })
                .From<JoinUser>(alias: "source")
                .Select<JoinUser>(user => new
                {
                    user.Email,
                    user.IsActive
                })
                .Build();
        }

        /// <summary>
        /// Builds a parameterized INSERT SELECT command with source filtering.
        /// </summary>
        /// <typeparam name="TProfile">Database provider profile.</typeparam>
        /// <param name="queryBuilder">Query builder used to construct the command.</param>
        /// <returns>Generated SQL query.</returns>
        private static GeneratedSqlQuery BuildInsertSelectWithWhereQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .InsertInto<JoinUser>()
                .Columns(user => new
                {
                    user.Email,
                    user.IsActive
                })
                .From<JoinUser>(alias: "source")
                .Select<JoinUser>(user => new
                {
                    user.Email,
                    user.IsActive
                })
                .Where<JoinUser>(user => user.IsActive)
                .Build();
        }
    }
}
