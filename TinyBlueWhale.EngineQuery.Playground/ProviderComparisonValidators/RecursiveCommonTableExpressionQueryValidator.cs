using TinyBlueWhale.EngineQuery.Abstractions.Extensions;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding;
using TinyBlueWhale.EngineQuery.MySql.Profiles;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;
using TinyBlueWhale.EngineQuery.PostgreSql.Profiles;
using TinyBlueWhale.EngineQuery.SqlServer.Profiles;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators
{

    /// <summary>
    /// Validates recursive common table expression generation across providers.
    /// </summary>
    public static class RecursiveCommonTableExpressionQueryValidator
    {
        /// <summary>
        /// Runs the validator.
        /// </summary>
        public static void Run()
        {
            var metadataResolver = ProviderMetadataFactory.CreateJoinMetadataResolver();

            ProviderQueryPrinter.Print(
                "SQL Server Recursive CTE",
                BuildSqlServerQuery(
                    ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL Recursive CTE",
                BuildPostgreSqlQuery(
                    ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL Recursive CTE",
                BuildMySqlQuery(
                    ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));
        }

        /// <summary>
        /// Builds a SQL Server recursive common table expression query.
        /// </summary>
        /// <param name="queryBuilder">
        /// SQL Server query builder configured with a profile that supports recursive common table expressions.
        /// </param>
        /// <returns>
        /// Generated SQL Server query.
        /// </returns>
        private static GeneratedSqlQuery BuildSqlServerQuery(QueryBuilder<SqlServer2012Profile> queryBuilder)
        {
            return queryBuilder
                .WithRecursive<CategoryTree, Category, Category>(
                    name: "category_tree",
                    baseQueryBuilder: baseQuery => baseQuery
                        .From<Category>(alias: "c")
                        .Select<Category>(c => new
                        {
                            c.Id,
                            c.ParentId,
                            c.Name
                        })
                        .Where<Category>(c => c.ParentId == null),
                    recursiveQueryBuilder: recursiveQuery => recursiveQuery
                        .From<Category>(alias: "c")
                        .InnerJoin<Category, CategoryTree>(
                            alias: "ct",
                            on: (c, ct) => c.ParentId == ct.Id)
                        .Select<Category>(c => new
                        {
                            c.Id,
                            c.ParentId,
                            c.Name
                        }))
                .FromCte<CategoryTree>("category_tree")
                .Select<CategoryTree>(tree => new
                {
                    tree.Id,
                    tree.ParentId,
                    tree.Name
                })
                .Build();
        }

        /// <summary>
        /// Builds a PostgreSQL recursive common table expression query.
        /// </summary>
        /// <param name="queryBuilder">
        /// PostgreSQL query builder configured with a profile that supports recursive common table expressions.
        /// </param>
        /// <returns>
        /// Generated PostgreSQL query.
        /// </returns>
        private static GeneratedSqlQuery BuildPostgreSqlQuery(QueryBuilder<PostgreSql93Profile> queryBuilder)
        {
            return queryBuilder
                .WithRecursive<CategoryTree, Category, Category>(
                    name: "category_tree",
                    baseQueryBuilder: baseQuery => baseQuery
                        .From<Category>(alias: "c")
                        .Select<Category>(c => new
                        {
                            c.Id,
                            c.ParentId,
                            c.Name
                        })
                        .Where<Category>(c => c.ParentId == null),
                    recursiveQueryBuilder: recursiveQuery => recursiveQuery
                        .From<Category>(alias: "c")
                        .InnerJoin<Category, CategoryTree>(
                            alias: "ct",
                            on: (c, ct) => c.ParentId == ct.Id)
                        .Select<Category>(c => new
                        {
                            c.Id,
                            c.ParentId,
                            c.Name
                        }))
                .FromCte<CategoryTree>("category_tree")
                .Select<CategoryTree>(tree => new
                {
                    tree.Id,
                    tree.ParentId,
                    tree.Name
                })
                .Build();
        }

        /// <summary>
        /// Builds a MySQL recursive common table expression query.
        /// </summary>
        /// <param name="queryBuilder">
        /// MySQL query builder configured with a profile that supports recursive common table expressions.
        /// </param>
        /// <returns>
        /// Generated MySQL query.
        /// </returns>
        private static GeneratedSqlQuery BuildMySqlQuery(QueryBuilder<MySql8031Profile> queryBuilder)
        {
            return queryBuilder
                .WithRecursive<CategoryTree, Category, Category>(
                    name: "category_tree",
                    baseQueryBuilder: baseQuery => baseQuery
                        .From<Category>(alias: "c")
                        .Select<Category>(c => new
                        {
                            c.Id,
                            c.ParentId,
                            c.Name
                        })
                        .Where<Category>(c => c.ParentId == null),
                    recursiveQueryBuilder: recursiveQuery => recursiveQuery
                        .From<Category>(alias: "c")
                        .InnerJoin<Category, CategoryTree>(
                            alias: "ct",
                            on: (c, ct) => c.ParentId == ct.Id)
                        .Select<Category>(c => new
                        {
                            c.Id,
                            c.ParentId,
                            c.Name
                        }))
                .FromCte<CategoryTree>("category_tree")
                .Select<CategoryTree>(tree => new
                {
                    tree.Id,
                    tree.ParentId,
                    tree.Name
                })
                .Build();
        }
    }
}
