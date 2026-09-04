using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Metadata.Resolvers;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.Metadata
{
    /// <summary>
    /// Validates attribute-based metadata mapping across supported database providers.
    /// </summary>
    public static class AttributeMetadataQueryValidator
    {
        /// <summary>
        /// Runs attribute metadata validation scenarios across supported database providers.
        /// </summary>
        public static void Run()
        {
            var metadataResolver = new AttributeEntityMetadataResolver();

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
        /// Runs attribute metadata scenarios for a database provider.
        /// </summary>
        /// <typeparam name="TProfile">Database provider profile.</typeparam>
        /// <param name="providerName">Display name of the database provider.</param>
        /// <param name="queryBuilder">Query builder used to construct the scenarios.</param>
        private static void RunProvider<TProfile>(string providerName, IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            ProviderQueryPrinter.Print(
                $"{providerName} Attribute Metadata Select",
                BuildSelectQuery(queryBuilder));

            ProviderQueryPrinter.Print(
                $"{providerName} Attribute Metadata Insert",
                BuildInsertQuery(queryBuilder));

            ProviderQueryPrinter.Print(
                $"{providerName} Attribute Metadata Update",
                BuildUpdateQuery(queryBuilder));

            ProviderQueryPrinter.Print(
                $"{providerName} Attribute Metadata Delete",
                BuildDeleteQuery(queryBuilder));
        }

        /// <summary>
        /// Builds a SELECT command using attribute-based table and column mappings.
        /// </summary>
        /// <typeparam name="TProfile">Database provider profile.</typeparam>
        /// <param name="queryBuilder">Query builder used to construct the query.</param>
        /// <returns>Generated SQL query.</returns>
        private static GeneratedSqlQuery BuildSelectQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .From<AttributeMappedUser>(alias: "u")
                .Select<AttributeMappedUser>(user => new
                {
                    user.Id,
                    user.Email,
                    user.IsActive
                })
                .Where<AttributeMappedUser>(user => user.IsActive)
                .OrderBy<AttributeMappedUser>(user => user.Id)
                .Build();
        }

        /// <summary>
        /// Builds an INSERT command using attribute-based table and column mappings.
        /// </summary>
        /// <typeparam name="TProfile">Database provider profile.</typeparam>
        /// <param name="queryBuilder">Query builder used to construct the command.</param>
        /// <returns>Generated SQL query.</returns>
        private static GeneratedSqlQuery BuildInsertQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .InsertInto<AttributeMappedUser>()
                .Set(user => user.Email, "attribute@test.com")
                .Set(user => user.IsActive, true)
                .Build();
        }

        /// <summary>
        /// Builds an UPDATE command using attribute-based table and column mappings.
        /// </summary>
        /// <typeparam name="TProfile">Database provider profile.</typeparam>
        /// <param name="queryBuilder">Query builder used to construct the command.</param>
        /// <returns>Generated SQL query.</returns>
        private static GeneratedSqlQuery BuildUpdateQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .Update<AttributeMappedUser>()
                .Set(user => user.Email, "updated-attribute@test.com")
                .Where(user => user.Id == 25)
                .Build();
        }

        /// <summary>
        /// Builds a DELETE command using attribute-based table and column mappings.
        /// </summary>
        /// <typeparam name="TProfile">Database provider profile.</typeparam>
        /// <param name="queryBuilder">Query builder used to construct the command.</param>
        /// <returns>Generated SQL query.</returns>
        private static GeneratedSqlQuery BuildDeleteQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .DeleteFrom<AttributeMappedUser>()
                .Where(user => user.Id == 25)
                .Build();
        }
    }
}
