using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.Aggregates
{
    /// <summary>
    /// Validates aggregate, GROUP BY and HAVING structural scenarios across supported database providers.
    ///
    /// Basic grouped aggregate:
    /// SELECT [u].[user_id] AS [UserId], [u].[email], SUM([o].[total]) AS [TotalAmount]
    /// FROM [users] AS [u]
    /// INNER JOIN [orders] AS [o] ON ([u].[user_id] = [o].[user_id])
    /// GROUP BY [u].[user_id], [u].[email]
    ///
    /// Multiple grouped sources:
    /// SELECT [u].[user_id] AS [UserId], [o].[user_id], COUNT([o].[order_id]) AS [OrderCount]
    /// FROM [users] AS [u]
    /// INNER JOIN [orders] AS [o] ON ([u].[user_id] = [o].[user_id])
    /// GROUP BY [u].[user_id], [o].[user_id]
    ///
    /// Multiple aggregates:
    /// SELECT [u].[user_id] AS [UserId], [u].[email], SUM([o].[total]) AS [TotalAmount], COUNT([o].[order_id]) AS [OrderCount], MIN([o].[total]) AS [MinimumAmount], MAX([o].[total]) AS [MaximumAmount]
    /// FROM [users] AS [u]
    /// INNER JOIN [orders] AS [o] ON ([u].[user_id] = [o].[user_id])
    /// GROUP BY [u].[user_id], [u].[email]
    ///
    /// Aggregate HAVING predicates:
    /// SELECT [u].[user_id] AS [UserId], [u].[email], SUM([o].[total]) AS [TotalAmount], COUNT([o].[order_id]) AS [OrderCount]
    /// FROM [users] AS [u]
    /// INNER JOIN [orders] AS [o] ON ([u].[user_id] = [o].[user_id])
    /// GROUP BY [u].[user_id], [u].[email]
    /// HAVING SUM([o].[total]) &gt; @p0 AND COUNT([o].[order_id]) &gt;= @p1
    /// @p0 = 1000
    /// @p1 = 2
    ///
    /// Same CLR type sources:
    /// SELECT [category].[category_id] AS [CategoryId], [parent].[category_id] AS [ParentId], COUNT([parent].[category_id]) AS [ParentCount]
    /// FROM [categories] AS [category]
    /// LEFT JOIN [categories] AS [parent] ON ([category].[parent_category_id] = [parent].[category_id])
    /// GROUP BY [category].[category_id], [parent].[category_id]
    /// HAVING COUNT([parent].[category_id]) &gt;= @p0
    /// @p0 = 1
    ///
    /// PostgreSQL and MySQL preserve the same query structures using their provider-specific identifier delimiters.
    /// </summary>
    public static class AggregateGroupingStructuralQueryValidator
    {
        public static void Run()
        {
            var metadataResolver = ProviderMetadataFactory.CreateJoinMetadataResolver();

            ProviderQueryPrinter.Print(
                "SQL Server Aggregate Grouping Structural Basic",
                BuildBasicGroupedAggregateQuery(
                    ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL Aggregate Grouping Structural Basic",
                BuildBasicGroupedAggregateQuery(
                    ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL Aggregate Grouping Structural Basic",
                BuildBasicGroupedAggregateQuery(
                    ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "SQL Server Aggregate Grouping Structural Multiple Groups",
                BuildMultipleGroupSourcesQuery(
                    ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL Aggregate Grouping Structural Multiple Groups",
                BuildMultipleGroupSourcesQuery(
                    ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL Aggregate Grouping Structural Multiple Groups",
                BuildMultipleGroupSourcesQuery(
                    ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "SQL Server Aggregate Grouping Structural Multiple Aggregates",
                BuildMultipleAggregatesQuery(
                    ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL Aggregate Grouping Structural Multiple Aggregates",
                BuildMultipleAggregatesQuery(
                    ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL Aggregate Grouping Structural Multiple Aggregates",
                BuildMultipleAggregatesQuery(
                    ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "SQL Server Aggregate Grouping Structural Having",
                BuildHavingQuery(
                    ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL Aggregate Grouping Structural Having",
                BuildHavingQuery(
                    ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL Aggregate Grouping Structural Having",
                BuildHavingQuery(
                    ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "SQL Server Aggregate Grouping Structural Same Type Sources",
                BuildSameTypeGroupedSourcesQuery(
                    ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL Aggregate Grouping Structural Same Type Sources",
                BuildSameTypeGroupedSourcesQuery(
                    ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL Aggregate Grouping Structural Same Type Sources",
                BuildSameTypeGroupedSourcesQuery(
                    ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));
        }

        private static GeneratedSqlQuery BuildBasicGroupedAggregateQuery<TProfile>(
            IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .InnerJoin<JoinUser, JoinOrder>(
                    alias: "o",
                    on: (u, o) => u.Id == o.UserId)
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .SelectAggregate<JoinOrder>(
                    QueryAggregateFunction.Sum,
                    o => o.Total,
                    alias: "TotalAmount")
                .GroupBy<JoinUser>(u => new
                {
                    u.Id,
                    u.Email
                })
                .Build();
        }

        private static GeneratedSqlQuery BuildMultipleGroupSourcesQuery<TProfile>(
            IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .InnerJoin<JoinUser, JoinOrder>(
                    alias: "o",
                    on: (u, o) => u.Id == o.UserId)
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id
                })
                .Select<JoinOrder>(o => new
                {
                    o.UserId
                })
                .SelectAggregate<JoinOrder>(
                    QueryAggregateFunction.Count,
                    o => o.Id,
                    alias: "OrderCount")
                .GroupBy<JoinUser>(u => u.Id)
                .GroupBy<JoinOrder>(o => o.UserId)
                .Build();
        }

        private static GeneratedSqlQuery BuildMultipleAggregatesQuery<TProfile>(
            IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .InnerJoin<JoinUser, JoinOrder>(
                    alias: "o",
                    on: (u, o) => u.Id == o.UserId)
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .SelectAggregate<JoinOrder>(
                    QueryAggregateFunction.Sum,
                    o => o.Total,
                    alias: "TotalAmount")
                .SelectAggregate<JoinOrder>(
                    QueryAggregateFunction.Count,
                    o => o.Id,
                    alias: "OrderCount")
                .SelectAggregate<JoinOrder>(
                    QueryAggregateFunction.Minimum,
                    o => o.Total,
                    alias: "MinimumAmount")
                .SelectAggregate<JoinOrder>(
                    QueryAggregateFunction.Maximum,
                    o => o.Total,
                    alias: "MaximumAmount")
                .GroupBy<JoinUser>(u => new
                {
                    u.Id,
                    u.Email
                })
                .Build();
        }

        private static GeneratedSqlQuery BuildHavingQuery<TProfile>(
            IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .InnerJoin<JoinUser, JoinOrder>(
                    alias: "o",
                    on: (u, o) => u.Id == o.UserId)
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .SelectAggregate<JoinOrder>(
                    QueryAggregateFunction.Sum,
                    o => o.Total,
                    alias: "TotalAmount")
                .SelectAggregate<JoinOrder>(
                    QueryAggregateFunction.Count,
                    o => o.Id,
                    alias: "OrderCount")
                .GroupBy<JoinUser>(u => new
                {
                    u.Id,
                    u.Email
                })
                .HavingAggregate<JoinOrder>(
                    QueryAggregateFunction.Sum,
                    o => o.Total,
                    QueryComparisonOperator.GreaterThan,
                    1000)
                .HavingAggregate<JoinOrder>(
                    QueryAggregateFunction.Count,
                    o => o.Id,
                    QueryComparisonOperator.GreaterThanOrEqual,
                    2)
                .Build();
        }

        private static GeneratedSqlQuery BuildSameTypeGroupedSourcesQuery<TProfile>(
            IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .From<Category>(alias: "category")
                .LeftJoin<Category, Category>(
                    alias: "parent",
                    on: (category, parent) =>
                        category.ParentId == parent.Id)
                .Select<Category>(category => new
                {
                    CategoryId = category.Id
                })
                .Select<Category>(parent => new
                {
                    ParentId = parent.Id
                })
                .SelectAggregate<Category>(
                    QueryAggregateFunction.Count,
                    parent => parent.Id,
                    alias: "ParentCount")
                .GroupBy<Category>(category => category.Id)
                .GroupBy<Category>(parent => parent.Id)
                .HavingAggregate<Category>(
                    QueryAggregateFunction.Count,
                    parent => parent.Id,
                    QueryComparisonOperator.GreaterThanOrEqual,
                    1)
                .Build();
        }
    }
}
