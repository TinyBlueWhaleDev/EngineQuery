using TinyBlueWhale.EngineQuery.Abstractions.Extensions;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding;
using TinyBlueWhale.EngineQuery.MySql.Profiles;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;
using TinyBlueWhale.EngineQuery.PostgreSql.Profiles;
using TinyBlueWhale.EngineQuery.SqlServer.Profiles;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.CommonTableExpressions
{

    /// <summary>
    /// Validates recursive common table expression generation using a base query
    /// and a recursive UNION ALL branch across supported database providers.
    ///
    /// SQL Server:
    /// WITH [category_tree] AS (SELECT [c].[category_id] AS [Id], [c].[parent_category_id] AS [ParentId], [c].[name] AS [Name]
    /// FROM [categories] AS [c]
    /// WHERE ([c].[parent_category_id] IS NULL)
    /// UNION ALL
    /// SELECT [c].[category_id] AS [Id], [c].[parent_category_id] AS [ParentId], [c].[name] AS [Name]
    /// FROM [categories] AS [c]
    /// INNER JOIN [category_tree] AS [ct] ON ([c].[parent_category_id] = [ct].[Id]))
    /// SELECT [Id], [ParentId], [Name]
    /// FROM [category_tree]
    ///
    /// PostgreSQL:
    /// WITH RECURSIVE "category_tree" AS (SELECT "c"."category_id" AS "Id", "c"."parent_category_id" AS "ParentId", "c"."name" AS "Name"
    /// FROM "categories" AS "c"
    /// WHERE ("c"."parent_category_id" IS NULL)
    /// UNION ALL
    /// SELECT "c"."category_id" AS "Id", "c"."parent_category_id" AS "ParentId", "c"."name" AS "Name"
    /// FROM "categories" AS "c"
    /// INNER JOIN "category_tree" AS "ct" ON ("c"."parent_category_id" = "ct"."Id"))
    /// SELECT "Id", "ParentId", "Name"
    /// FROM "category_tree"
    ///
    /// MySQL:
    /// WITH RECURSIVE `category_tree` AS (SELECT `c`.`category_id` AS `Id`, `c`.`parent_category_id` AS `ParentId`, `c`.`name` AS `Name`
    /// FROM `categories` AS `c`
    /// WHERE (`c`.`parent_category_id` IS NULL)
    /// UNION ALL
    /// SELECT `c`.`category_id` AS `Id`, `c`.`parent_category_id` AS `ParentId`, `c`.`name` AS `Name`
    /// FROM `categories` AS `c`
    /// INNER JOIN `category_tree` AS `ct` ON (`c`.`parent_category_id` = `ct`.`Id`))
    /// SELECT `Id`, `ParentId`, `Name`
    /// FROM `category_tree`
    /// </summary>
    public static class RecursiveCommonTableExpressionQueryValidator
    {
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
