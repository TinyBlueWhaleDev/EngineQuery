using TinyBlueWhale.EngineQuery.Abstractions.Extensions;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.Metadata
{
    /// <summary>
    /// Validates fluent metadata mapping across supported database providers.
    /// </summary>
    public static class FluentMetadataQueryValidator
    {
        /// <summary>
        /// Runs fluent metadata validation scenarios across supported database providers.
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
        /// Runs fluent metadata scenarios for a database provider.
        /// </summary>
        /// <typeparam name="TProfile">Database provider profile.</typeparam>
        /// <param name="providerName">Display name of the database provider.</param>
        /// <param name="queryBuilder">Query builder used to construct the scenarios.</param>
        private static void RunProvider<TProfile>(
            string providerName,
            Abstractions.Interfaces.IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            ProviderQueryPrinter.Print(
                $"{providerName} Fluent Metadata Select",
                BuildSelectQuery(queryBuilder));

            ProviderQueryPrinter.Print(
                $"{providerName} Fluent Metadata Insert",
                BuildInsertQuery(queryBuilder));

            ProviderQueryPrinter.Print(
                $"{providerName} Fluent Metadata Update",
                BuildUpdateQuery(queryBuilder));

            ProviderQueryPrinter.Print(
                $"{providerName} Fluent Metadata Delete",
                BuildDeleteQuery(queryBuilder));
        }

        /// <summary>
        /// Builds a SELECT command using fluent table and column mappings.
        /// </summary>
        /// <typeparam name="TProfile">Database provider profile.</typeparam>
        /// <param name="queryBuilder">Query builder used to construct the query.</param>
        /// <returns>Generated SQL query.</returns>
        private static GeneratedSqlQuery BuildSelectQuery<TProfile>(
            Abstractions.Interfaces.IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(user => new
                {
                    user.Id,
                    user.Email,
                    user.IsActive
                })
                .Where<JoinUser>(user => user.IsActive)
                .OrderBy<JoinUser>(user => user.Id)
                .Build();
        }

        /// <summary>
        /// Builds an INSERT command using fluent table and column mappings.
        /// </summary>
        /// <typeparam name="TProfile">Database provider profile.</typeparam>
        /// <param name="queryBuilder">Query builder used to construct the command.</param>
        /// <returns>Generated SQL query.</returns>
        private static GeneratedSqlQuery BuildInsertQuery<TProfile>(
            Abstractions.Interfaces.IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .InsertInto<JoinUser>()
                .Set(user => user.Email, "metadata@test.com")
                .Set(user => user.IsActive, true)
                .Build();
        }

        /// <summary>
        /// Builds an UPDATE command using fluent table and column mappings.
        /// </summary>
        /// <typeparam name="TProfile">Database provider profile.</typeparam>
        /// <param name="queryBuilder">Query builder used to construct the command.</param>
        /// <returns>Generated SQL query.</returns>
        private static GeneratedSqlQuery BuildUpdateQuery<TProfile>(
            Abstractions.Interfaces.IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .Update<JoinUser>()
                .Set(user => user.Email, "updated@test.com")
                .Where(user => user.Id == 10)
                .Build();
        }

        /// <summary>
        /// Builds a DELETE command using fluent table and column mappings.
        /// </summary>
        /// <typeparam name="TProfile">Database provider profile.</typeparam>
        /// <param name="queryBuilder">Query builder used to construct the command.</param>
        /// <returns>Generated SQL query.</returns>
        private static GeneratedSqlQuery BuildDeleteQuery<TProfile>(
            Abstractions.Interfaces.IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .DeleteFrom<JoinUser>()
                .Where(user => user.Id == 10)
                .Build();
        }
    }
}
