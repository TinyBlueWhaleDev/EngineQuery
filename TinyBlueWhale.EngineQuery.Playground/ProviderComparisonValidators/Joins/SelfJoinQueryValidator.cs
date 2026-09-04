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

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.Joins
{
    /// <summary>
    /// Validates self-join behavior when the same CLR source type is used more than once.
    /// </summary>
    public static class SelfJoinQueryValidator
    {
        /// <summary>
        /// Runs self-join validation scenarios across supported database providers.
        /// </summary>
        public static void Run()
        {
            var metadataResolver = ProviderMetadataFactory.CreateJoinMetadataResolver();

            ProviderQueryPrinter.Print(
                "SQL Server Self Join",
                BuildQuery(ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL Self Join",
                BuildQuery(ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL Self Join",
                BuildQuery(ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));
        }

        /// <summary>
        /// Builds a category-to-parent-category self-join query.
        /// </summary>
        /// <typeparam name="TProfile">Database provider profile.</typeparam>
        /// <param name="queryBuilder">Query builder used to construct the query.</param>
        /// <returns>Generated SQL query.</returns>
        private static GeneratedSqlQuery BuildQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .From<Category>(alias: "category")
                .LeftJoin<Category, Category>(alias: "parent", on: (category, parent) => category.ParentId == parent.Id)
                .Select<Category>(category => new
                {
                    CategoryId = category.Id,
                    CategoryName = category.Name,
                })
                .Select<Category>(parent => new
                {
                    ParentId = parent.Id,
                    ParentName = parent.Name
                })
                .OrderBy<Category>(category => category.Id)
                .Build();
        }
    }
}
