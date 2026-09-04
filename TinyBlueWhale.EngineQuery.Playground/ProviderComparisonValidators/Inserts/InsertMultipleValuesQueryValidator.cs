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
    /// <summary>
    /// Validates multi-column INSERT VALUES generation across supported database providers.
    /// </summary>
    public static class InsertMultipleValuesQueryValidator
    {
        /// <summary>
        /// Runs multi-column INSERT VALUES validation scenarios.
        /// </summary>
        public static void Run()
        {
            var metadataResolver = ProviderMetadataFactory.CreateJoinMetadataResolver();

            ProviderQueryPrinter.Print(
                "SQL Server Insert Multiple Values",
                BuildQuery(ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL Insert Multiple Values",
                BuildQuery(ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL Insert Multiple Values",
                BuildQuery(ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));
        }

        /// <summary>
        /// Builds a multi-column INSERT VALUES command.
        /// </summary>
        /// <typeparam name="TProfile">Database provider profile.</typeparam>
        /// <param name="queryBuilder">Query builder used to construct the command.</param>
        /// <returns>Generated SQL query.</returns>
        private static GeneratedSqlQuery BuildQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .InsertInto<JoinUser>()
                .Set(user => user.Email, "admin@test.com")
                .Set(user => user.IsActive, true)
                .Build();
        }
    }
}
