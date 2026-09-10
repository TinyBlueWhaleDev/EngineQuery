using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.Smoke
{
    /// <summary>
    /// Validates structural query IR across root sources, repeated same-type sources,
    /// explicit source references, command targets, nested scopes and derived tables.
    ///
    /// Root source:
    /// SELECT [category].[category_id] AS [CategoryId], [category].[name] AS [CategoryName]
    /// FROM [categories] AS [category]
    /// WHERE ([category].[category_id] &gt; @p0)
    /// ORDER BY [category].[category_id] ASC
    ///
    /// Expected parameters:
    /// @p0 = 0
    ///
    /// Same-type sources:
    /// SELECT [category].[category_id] AS [CategoryId], [category].[name] AS [CategoryName], [parent].[category_id], [parent1].[category_id]
    /// FROM [categories] AS [category]
    /// LEFT JOIN [categories] AS [parent] ON ([category].[parent_category_id] = [parent].[category_id])
    /// LEFT JOIN [categories] AS [parent1] ON ([category].[parent_category_id] = [parent1].[category_id])
    /// ORDER BY [category].[category_id] ASC
    ///
    /// Source references:
    /// SELECT [category].[category_id] AS [CategoryId], [category].[name] AS [CategoryName], [parent].[category_id] AS [ParentId], [parent].[name] AS [ParentName]
    /// FROM [categories] AS [category]
    /// LEFT JOIN [categories] AS [parent] ON ([category].[parent_category_id] = [parent].[category_id])
    /// WHERE ([category].[category_id] &gt; @p0)
    /// ORDER BY [parent].[category_id] ASC
    ///
    /// Expected parameters:
    /// @p0 = 0
    ///
    /// INSERT VALUES:
    /// INSERT INTO [users] ([email])
    /// VALUES (@p0)
    ///
    /// Expected parameters:
    /// @p0 = structural@test.com
    ///
    /// INSERT SELECT:
    /// INSERT INTO [users] ([email], [is_active])
    /// SELECT [source].[email], [source].[is_active]
    /// FROM [users] AS [source]
    /// WHERE ([source].[is_active] = @p0)
    ///
    /// Expected parameters:
    /// @p0 = True
    ///
    /// UPDATE:
    /// UPDATE [users]
    /// SET [email] = @p0
    /// WHERE ([user_id] = @p1)
    ///
    /// Expected parameters:
    /// @p0 = structural-updated@test.com
    /// @p1 = 10
    ///
    /// DELETE:
    /// DELETE
    /// FROM [users]
    /// WHERE ([user_id] = @p0)
    ///
    /// Expected parameters:
    /// @p0 = 10
    ///
    /// EXISTS:
    /// SELECT [u].[user_id] AS [UserId], [u].[email]
    /// FROM [users] AS [u]
    /// WHERE EXISTS (SELECT 1
    /// FROM [orders] AS [o]
    /// WHERE ([o].[total] &gt; @p0))
    ///
    /// Expected parameters:
    /// @p0 = 100
    ///
    /// Correlated EXISTS:
    /// SELECT [u].[user_id] AS [UserId], [u].[email]
    /// FROM [users] AS [u]
    /// WHERE EXISTS (SELECT 1
    /// FROM [orders] AS [o]
    /// WHERE (([o].[user_id] = [u].[user_id]) AND ([o].[total] &gt; @p0)))
    ///
    /// Expected parameters:
    /// @p0 = 100
    ///
    /// IN subquery:
    /// SELECT [u].[user_id] AS [UserId], [u].[email]
    /// FROM [users] AS [u]
    /// WHERE [u].[user_id] IN (SELECT [o].[user_id]
    /// FROM [orders] AS [o]
    /// WHERE ([o].[total] &gt; @p0))
    ///
    /// Expected parameters:
    /// @p0 = 100
    ///
    /// Derived table:
    /// SELECT [summary].[UserId], [summary].[TotalAmount], [summary].[OrderCount]
    /// FROM (SELECT [o].[user_id] AS [UserId], SUM([o].[total]) AS [TotalAmount], COUNT([o].[order_id]) AS [OrderCount]
    /// FROM [orders] AS [o]
    /// GROUP BY [o].[user_id]) AS [summary]
    /// WHERE ([summary].[TotalAmount] &gt; @p0)
    /// ORDER BY [summary].[TotalAmount] DESC
    ///
    /// Expected parameters:
    /// @p0 = 500
    ///
    /// PostgreSQL and MySQL generate the same structural IR scenarios using
    /// their respective identifier delimiters.
    /// </summary>
    public static class StructuralIrQueryValidator
    {
        public static void Run()
        {
            var metadataResolver = ProviderMetadataFactory.CreateJoinMetadataResolver();

            ProviderQueryPrinter.Print(
                "SQL Server Structural IR Root Source",
                BuildRootSourceQuery(
                    ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL Structural IR Root Source",
                BuildRootSourceQuery(
                    ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL Structural IR Root Source",
                BuildRootSourceQuery(
                    ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "SQL Server Structural IR Same Type Sources",
                BuildSameTypeSourcesQuery(
                    ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL Structural IR Same Type Sources",
                BuildSameTypeSourcesQuery(
                    ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL Structural IR Same Type Sources",
                BuildSameTypeSourcesQuery(
                    ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "SQL Server Structural IR Source References",
                BuildSourceReferencesQuery(
                    ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL Structural IR Source References",
                BuildSourceReferencesQuery(
                    ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL Structural IR Source References",
                BuildSourceReferencesQuery(
                    ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "SQL Server Structural IR Insert Values",
                BuildInsertValuesQuery(
                    ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL Structural IR Insert Values",
                BuildInsertValuesQuery(
                    ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL Structural IR Insert Values",
                BuildInsertValuesQuery(
                    ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "SQL Server Structural IR Insert Select",
                BuildInsertSelectQuery(
                    ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL Structural IR Insert Select",
                BuildInsertSelectQuery(
                    ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL Structural IR Insert Select",
                BuildInsertSelectQuery(
                    ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "SQL Server Structural IR Update",
                BuildUpdateQuery(
                    ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL Structural IR Update",
                BuildUpdateQuery(
                    ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL Structural IR Update",
                BuildUpdateQuery(
                    ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "SQL Server Structural IR Delete",
                BuildDeleteQuery(
                    ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL Structural IR Delete",
                BuildDeleteQuery(
                    ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL Structural IR Delete",
                BuildDeleteQuery(
                    ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "SQL Server Structural IR EXISTS",
                BuildExistsQuery(
                    ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL Structural IR EXISTS",
                BuildExistsQuery(
                    ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL Structural IR EXISTS",
                BuildExistsQuery(
                    ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "SQL Server Structural IR Correlated EXISTS",
                BuildCorrelatedExistsQuery(
                    ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL Structural IR Correlated EXISTS",
                BuildCorrelatedExistsQuery(
                    ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL Structural IR Correlated EXISTS",
                BuildCorrelatedExistsQuery(
                    ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "SQL Server Structural IR IN Subquery",
                BuildInSubqueryQuery(
                    ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL Structural IR IN Subquery",
                BuildInSubqueryQuery(
                    ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL Structural IR IN Subquery",
                BuildInSubqueryQuery(
                    ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "SQL Server Structural IR Derived Table",
                BuildDerivedTableQuery(
                    ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL Structural IR Derived Table",
                BuildDerivedTableQuery(
                    ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL Structural IR Derived Table",
                BuildDerivedTableQuery(
                    ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));
        }

        // Builds a query validating root source registration and ordinary source references.
        private static GeneratedSqlQuery BuildRootSourceQuery<TProfile>(
            IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .From<Category>(alias: "category")
                .Select<Category>(category => new
                {
                    CategoryId = category.Id,
                    CategoryName = category.Name
                })
                .Where<Category>(category => category.Id > 0)
                .OrderBy<Category>(category => category.Id)
                .Build();
        }

        // Builds a query validating multiple independent SQL sources using the same CLR type.
        private static GeneratedSqlQuery BuildSameTypeSourcesQuery<TProfile>(
            IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .From<Category>(alias: "category")
                .LeftJoin<Category, Category>(
                    alias: "parent",
                    on: (category, parent) =>
                        category.ParentId == parent.Id)
                .LeftJoin<Category, Category>(
                    alias: "parent",
                    on: (category, parent) =>
                        category.ParentId == parent.Id)
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
                .OrderBy<Category>(category => category.Id)
                .Build();
        }

        // Builds a query validating explicit source references across JOIN, SELECT, WHERE and ORDER BY.
        private static GeneratedSqlQuery BuildSourceReferencesQuery<TProfile>(
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
                    CategoryId = category.Id,
                    CategoryName = category.Name
                })
                .Select<Category>(parent => new
                {
                    ParentId = parent.Id,
                    ParentName = parent.Name
                })
                .Where<Category>(category => category.Id > 0)
                .OrderBy<Category>(parent => parent.Id)
                .Build();
        }

        // Builds an INSERT VALUES command validating an isolated target source.
        private static GeneratedSqlQuery BuildInsertValuesQuery<TProfile>(
            IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .InsertInto<JoinUser>()
                .Set(user => user.Email, "structural@test.com")
                .Build();
        }

        // Builds an INSERT SELECT command validating target and source scope separation.
        private static GeneratedSqlQuery BuildInsertSelectQuery<TProfile>(
            IQueryBuilder<TProfile> queryBuilder)
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
                .Select<JoinUser>(source => new
                {
                    source.Email,
                    source.IsActive
                })
                .Where<JoinUser>(source => source.IsActive)
                .Build();
        }

        // Builds an UPDATE command validating target source registration.
        private static GeneratedSqlQuery BuildUpdateQuery<TProfile>(
            IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .Update<JoinUser>()
                .Set(user => user.Email, "structural-updated@test.com")
                .Where(user => user.Id == 10)
                .Build();
        }

        // Builds a DELETE command validating target source registration.
        private static GeneratedSqlQuery BuildDeleteQuery<TProfile>(
            IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .DeleteFrom<JoinUser>()
                .Where(user => user.Id == 10)
                .Build();
        }

        // Builds a query validating an independent nested EXISTS query scope.
        private static GeneratedSqlQuery BuildExistsQuery<TProfile>(
            IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .WhereExists<JoinOrder>(
                    subquery => subquery
                        .From<JoinOrder>(alias: "o")
                        .Where<JoinOrder>(o => o.Total > 100))
                .Build();
        }

        // Builds a query validating correlation between current and outer query scopes.
        private static GeneratedSqlQuery BuildCorrelatedExistsQuery<TProfile>(
            IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .WhereExists<JoinUser, JoinOrder>(
                    alias: "o",
                    subquery => subquery
                        .WhereComputed<JoinOrder, JoinUser>(
                            (o, u) =>
                                o.UserId == u.Id &&
                                o.Total > 100))
                .Build();
        }

        // Builds a query validating an outer source reference and nested IN subquery scope.
        private static GeneratedSqlQuery BuildInSubqueryQuery<TProfile>(
            IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .WhereIn<JoinUser, JoinOrder>(
                    u => u.Id,
                    alias: "o",
                    subquery => subquery
                        .Select<JoinOrder>(o => new
                        {
                            o.UserId
                        })
                        .Where<JoinOrder>(o => o.Total > 100))
                .Build();
        }

        // Builds a query validating a derived table as the root source.
        private static GeneratedSqlQuery BuildDerivedTableQuery<TProfile>(
            IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .FromSubquery<OrderSummary, JoinOrder>(
                    alias: "summary",
                    subquery => subquery
                        .From<JoinOrder>(alias: "o")
                        .Select<JoinOrder>(o => new
                        {
                            o.UserId
                        })
                        .SelectAggregate<JoinOrder>(
                            QueryAggregateFunction.Sum,
                            o => o.Total,
                            alias: "TotalAmount")
                        .SelectAggregate<JoinOrder>(
                            QueryAggregateFunction.Count,
                            o => o.Id,
                            alias: "OrderCount")
                        .GroupBy<JoinOrder>(o => o.UserId))
                .Select<OrderSummary>(summary => new
                {
                    summary.UserId,
                    summary.TotalAmount,
                    summary.OrderCount
                })
                .WhereComputed<OrderSummary>(
                    summary => summary.TotalAmount > 500)
                .OrderByDescending<OrderSummary>(
                    summary => summary.TotalAmount)
                .Build();
        }
    }
}
