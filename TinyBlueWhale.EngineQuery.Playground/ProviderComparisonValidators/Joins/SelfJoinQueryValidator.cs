using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.Joins
{
    /// <summary>
    /// Validates same-type source identity, alias-based resolution, sequential projection
    /// resolution, joined-source ordering and automatic alias uniqueness across providers.
    ///
    /// Alias-matched and sequential projection SQL Server:
    /// SELECT [category].[category_id] AS [CategoryId], [category].[name] AS [CategoryName], [parent].[category_id] AS [ParentId], [parent].[name] AS [ParentName]
    /// FROM [categories] AS [category]
    /// LEFT JOIN [categories] AS [parent] ON ([category].[parent_category_id] = [parent].[category_id])
    /// ORDER BY [category].[category_id] ASC
    ///
    /// Alias-matched and sequential projection PostgreSQL:
    /// SELECT "category"."category_id" AS "CategoryId", "category"."name" AS "CategoryName", "parent"."category_id" AS "ParentId", "parent"."name" AS "ParentName"
    /// FROM "categories" AS "category"
    /// LEFT JOIN "categories" AS "parent" ON ("category"."parent_category_id" = "parent"."category_id")
    /// ORDER BY "category"."category_id" ASC
    ///
    /// Alias-matched and sequential projection MySQL:
    /// SELECT `category`.`category_id` AS `CategoryId`, `category`.`name` AS `CategoryName`, `parent`.`category_id` AS `ParentId`, `parent`.`name` AS `ParentName`
    /// FROM `categories` AS `category`
    /// LEFT JOIN `categories` AS `parent` ON (`category`.`parent_category_id` = `parent`.`category_id`)
    /// ORDER BY `category`.`category_id` ASC
    ///
    /// Joined-source ordering SQL Server:
    /// SELECT [category].[category_id] AS [CategoryId], [category].[name] AS [CategoryName], [parent].[category_id] AS [ParentId], [parent].[name] AS [ParentName]
    /// FROM [categories] AS [category]
    /// LEFT JOIN [categories] AS [parent] ON ([category].[parent_category_id] = [parent].[category_id])
    /// ORDER BY [parent].[category_id] ASC
    ///
    /// Joined-source ordering PostgreSQL:
    /// SELECT "category"."category_id" AS "CategoryId", "category"."name" AS "CategoryName", "parent"."category_id" AS "ParentId", "parent"."name" AS "ParentName"
    /// FROM "categories" AS "category"
    /// LEFT JOIN "categories" AS "parent" ON ("category"."parent_category_id" = "parent"."category_id")
    /// ORDER BY "parent"."category_id" ASC
    ///
    /// Joined-source ordering MySQL:
    /// SELECT `category`.`category_id` AS `CategoryId`, `category`.`name` AS `CategoryName`, `parent`.`category_id` AS `ParentId`, `parent`.`name` AS `ParentName`
    /// FROM `categories` AS `category`
    /// LEFT JOIN `categories` AS `parent` ON (`category`.`parent_category_id` = `parent`.`category_id`)
    /// ORDER BY `parent`.`category_id` ASC
    ///
    /// Multiple same-type joins SQL Server:
    /// SELECT [category].[category_id] AS [CategoryId], [category].[name] AS [CategoryName], [parent].[category_id], [parent1].[category_id], [parent2].[category_id]
    /// FROM [categories] AS [category]
    /// LEFT JOIN [categories] AS [parent] ON ([category].[parent_category_id] = [parent].[category_id])
    /// LEFT JOIN [categories] AS [parent1] ON ([category].[parent_category_id] = [parent1].[category_id])
    /// LEFT JOIN [categories] AS [parent2] ON ([category].[parent_category_id] = [parent2].[category_id])
    /// ORDER BY [category].[category_id] ASC
    ///
    /// Multiple same-type joins PostgreSQL:
    /// SELECT "category"."category_id" AS "CategoryId", "category"."name" AS "CategoryName", "parent"."category_id", "parent1"."category_id", "parent2"."category_id"
    /// FROM "categories" AS "category"
    /// LEFT JOIN "categories" AS "parent" ON ("category"."parent_category_id" = "parent"."category_id")
    /// LEFT JOIN "categories" AS "parent1" ON ("category"."parent_category_id" = "parent1"."category_id")
    /// LEFT JOIN "categories" AS "parent2" ON ("category"."parent_category_id" = "parent2"."category_id")
    /// ORDER BY "category"."category_id" ASC
    ///
    /// Multiple same-type joins MySQL:
    /// SELECT `category`.`category_id` AS `CategoryId`, `category`.`name` AS `CategoryName`, `parent`.`category_id`, `parent1`.`category_id`, `parent2`.`category_id`
    /// FROM `categories` AS `category`
    /// LEFT JOIN `categories` AS `parent` ON (`category`.`parent_category_id` = `parent`.`category_id`)
    /// LEFT JOIN `categories` AS `parent1` ON (`category`.`parent_category_id` = `parent1`.`category_id`)
    /// LEFT JOIN `categories` AS `parent2` ON (`category`.`parent_category_id` = `parent2`.`category_id`)
    /// ORDER BY `category`.`category_id` ASC
    /// </summary>
    public static class SelfJoinQueryValidator
    {        
        public static void Run()
        {
            var metadataResolver = ProviderMetadataFactory.CreateJoinMetadataResolver();

            #region Alias-matched self join

            ProviderQueryPrinter.Print(
                "SQL Server Self Join - Alias Matched",
                BuildAliasMatchedQuery(ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL Self Join - Alias Matched",
                BuildAliasMatchedQuery(ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL Self Join - Alias Matched",
                BuildAliasMatchedQuery(ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));

            #endregion

            #region Sequential projection self join

            ProviderQueryPrinter.Print(
                "SQL Server Self Join - Sequential Projection",
                BuildSequentialProjectionQuery(ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL Self Join - Sequential Projection",
                BuildSequentialProjectionQuery(ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL Self Join - Sequential Projection",
                BuildSequentialProjectionQuery(ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));

            #endregion

            #region Joined source ordering

            ProviderQueryPrinter.Print(
                "SQL Server Self Join - Joined Source Ordering",
                BuildJoinedSourceOrderingQuery(ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL Self Join - Joined Source Ordering",
                BuildJoinedSourceOrderingQuery(ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL Self Join - Joined Source Ordering",
                BuildJoinedSourceOrderingQuery(ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));

            #endregion

            #region Multiple same-type joins

            ProviderQueryPrinter.Print(
                "SQL Server Self Join - Multiple Same-Type Joins",
                BuildMultipleSameTypeJoinQuery(ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL Self Join - Multiple Same-Type Joins",
                BuildMultipleSameTypeJoinQuery(ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL Self Join - Multiple Same-Type Joins",
                BuildMultipleSameTypeJoinQuery(ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));

            #endregion
        }

        // Builds a self-join whose expression parameter names match source aliases.
        private static GeneratedSqlQuery BuildAliasMatchedQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .From<Category>(alias: "category")
                .LeftJoin<Category, Category>(alias: "parent", on: (category, parent) => category.ParentId == parent.Id)
                .Select<Category>(category => new
                {
                    CategoryId = category.Id,
                    CategoryName = category.Name
                })
                .Select<Category>(parent => new
                {
                    ParentId = parent.Id,
                    ParentName = parent.Name
                })
                .OrderBy<Category>(category => category.Id)
                .Build();
        }

        // Builds a self-join using sequential same-type projection resolution.
        private static GeneratedSqlQuery BuildSequentialProjectionQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .From<Category>(alias: "category")
                .LeftJoin<Category, Category>(alias: "parent", on: (category, parent) => category.ParentId == parent.Id)
                .Select<Category>(c => new
                {
                    CategoryId = c.Id,
                    CategoryName = c.Name
                })
                .Select<Category>(p => new
                {
                    ParentId = p.Id,
                    ParentName = p.Name
                })
                .OrderBy<Category>(x => x.Id)
                .Build();
        }

        // Builds a self-join ordered explicitly by the joined source.
        private static GeneratedSqlQuery BuildJoinedSourceOrderingQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .From<Category>(alias: "category")
                .LeftJoin<Category, Category>(alias: "parent", on: (category, parent) => category.ParentId == parent.Id)
                .Select<Category>(category => new
                {
                    CategoryId = category.Id,
                    CategoryName = category.Name
                })
                .Select<Category>(parent => new
                {
                    ParentId = parent.Id,
                    ParentName = parent.Name
                })
                .OrderBy<Category>(parent => parent.Id)
                .Build();
        }

        // Builds multiple same-type joins with automatically unique aliases.
        private static GeneratedSqlQuery BuildMultipleSameTypeJoinQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .From<Category>(alias: "category")
                .LeftJoin<Category, Category>(alias: "parent", on: (category, parent) => category.ParentId == parent.Id)
                .LeftJoin<Category, Category>(alias: "parent", on: (category, parent) => category.ParentId == parent.Id)
                .LeftJoin<Category, Category>(alias: "parent", on: (category, parent) => category.ParentId == parent.Id)
                .Select<Category>(category => new
                {
                    CategoryId = category.Id,
                    CategoryName = category.Name
                })
                .Select<Category>(parent => new
                {
                    parent.Id
                })
                .Select<Category>(parent => new
                {
                    parent.Id
                })
                .Select<Category>(parent => new
                {
                    parent.Id
                })
                .OrderBy<Category>(category => category.Id)
                .Build();
        }
    }
}
