namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators
{
    /// <summary>
    /// Validates INSERT command generation across providers.
    /// </summary>
    //public static class InsertCommandQueryValidator
    //{
    //    /// <summary>
    //    /// Runs the validator.
    //    /// </summary>
    //    public static void Run()
    //    {
    //        var metadataResolver = ProviderMetadataFactory.CreateJoinMetadataResolver();

    //        RunProvider(
    //            "SQL Server",
    //            ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver));

    //        RunProvider(
    //            "PostgreSQL",
    //            ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver));

    //        RunProvider(
    //            "MySQL",
    //            ProviderQueryBuilderFactory.CreateMySql(metadataResolver));
    //    }

    //    // Runs all INSERT validation scenarios for the specified provider.
    //    private static void RunProvider<TProfile>(string providerName, IQueryBuilder<TProfile> queryBuilder)
    //        where TProfile : IDatabaseProviderProfile
    //    {
    //        ProviderQueryPrinter.Print(
    //            $"{providerName} Insert Command",
    //            BuildInsertValuesQuery(queryBuilder));

    //        ProviderQueryPrinter.Print(
    //           $"{providerName} Insert Return Identity",
    //           BuildInsertIdentityQuery(queryBuilder, providerName));

    //        ProviderQueryPrinter.Print(
    //            $"{providerName} Insert Select",
    //            BuildInsertSelectQuery(queryBuilder));

    //        ProviderQueryPrinter.Print(
    //            $"{providerName} Insert Select Where",
    //            BuildInsertSelectWhereQuery(queryBuilder));

    //        ProviderQueryPrinter.Print(
    //            $"{providerName} Insert Select Join",
    //            BuildInsertSelectJoinQuery(queryBuilder));

    //        ProviderQueryPrinter.Print(
    //            $"{providerName} Insert Select Inferred Columns",
    //            BuildInsertSelectInferredColumnsQuery(queryBuilder));

    //        ProviderQueryPrinter.Print(
    //            $"{providerName} Insert Select Inferred Aggregate",
    //            BuildInsertSelectInferredAggregateQuery(queryBuilder));

    //        ProviderQueryPrinter.Print(
    //            $"{providerName} Insert Select Inferred Scalar Function",
    //            BuildInsertSelectInferredScalarFunctionQuery(queryBuilder));

    //        ProviderQueryPrinter.Print(
    //            $"{providerName} Insert Select Inferred Computed Expression",
    //            BuildInsertSelectInferredComputedQuery(queryBuilder));

    //        ProviderQueryPrinter.Print(
    //            $"{providerName} Insert Select Inferred Case When",
    //            BuildInsertSelectInferredCaseWhenQuery(queryBuilder));

    //        ProviderQueryPrinter.Print(
    //            $"{providerName} Insert Select Inferred Window Function",
    //            BuildInsertSelectInferredWindowQuery(queryBuilder));

    //        ProviderQueryPrinter.Print(
    //            $"{providerName} Insert Select Mixed Inferred Projection",
    //            BuildInsertSelectMixedInferredProjectionQuery(queryBuilder));
    //    }

    //    // Builds a strongly typed INSERT VALUES command.
    //    private static GeneratedSqlQuery BuildInsertValuesQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
    //        where TProfile : IDatabaseProviderProfile
    //    {
    //        return queryBuilder
    //            .InsertInto<JoinUser>()
    //            .Set(user => user.Email, "admin@test.com")
    //            .Build();
    //    }

    //    // Builds a SQL Server or MySQL INSERT command that retrieves the generated identity through a connection-scoped function.
    //    private static GeneratedSqlQuery BuildInsertIdentityQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder, string providerName)
    //        where TProfile : IDatabaseProviderProfile   
    //    {
    //        var query = queryBuilder.InsertInto<JoinUser>()
    //            .Set(user => user.Email, "admin@test.com");

    //        query = providerName.Equals("PostgreSQL") ? query.ReturnIdentity(x => x.Id) : query.ReturnIdentity();

    //        return query
    //            .Build();
    //    }

    //    // Builds an INSERT SELECT command using explicit target columns.
    //    private static GeneratedSqlQuery BuildInsertSelectQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
    //        where TProfile : IDatabaseProviderProfile
    //    {
    //        return queryBuilder
    //            .InsertInto<JoinUser>()
    //            .Columns(user => new
    //            {
    //                user.Id,
    //                user.Email
    //            })
    //            .From<JoinUser>(alias: "u")
    //            .Select<JoinUser>(user => new
    //            {
    //                user.Id,
    //                user.Email
    //            })
    //            .Build();
    //    }

    //    // Builds an INSERT SELECT command using an explicit WHERE predicate.
    //    private static GeneratedSqlQuery BuildInsertSelectWhereQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
    //        where TProfile : IDatabaseProviderProfile
    //    {
    //        return queryBuilder
    //            .InsertInto<JoinUser>()
    //            .Columns(user => new
    //            {
    //                user.Id,
    //                user.Email
    //            })
    //            .From<JoinUser>(alias: "u")
    //            .Select<JoinUser>(user => new
    //            {
    //                user.Id,
    //                user.Email
    //            })
    //            .Where<JoinUser>(user => user.IsActive)
    //            .Build();
    //    }

    //    // Builds an INSERT SELECT command using projections from multiple joined sources.
    //    private static GeneratedSqlQuery BuildInsertSelectJoinQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
    //        where TProfile : IDatabaseProviderProfile
    //    {
    //        return queryBuilder
    //            .InsertInto<JoinOrder>()
    //            .Columns(order => new
    //            {
    //                order.UserId,
    //                order.Total
    //            })
    //            .From<JoinUser>(alias: "u")
    //            .InnerJoin<JoinUser, JoinOrder>(alias: "o", on: (user, order) => user.Id == order.UserId)
    //            .Select<JoinUser>(user => new
    //            {
    //                UserId = user.Id
    //            })
    //            .Select<JoinOrder>(order => new
    //            {
    //                order.Total
    //            })
    //            .Where<JoinUser>(user => user.IsActive)
    //            .Build();
    //    }

    //    // Builds an INSERT SELECT command inferring target columns from projection aliases.
    //    private static GeneratedSqlQuery BuildInsertSelectInferredColumnsQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
    //        where TProfile : IDatabaseProviderProfile
    //    {
    //        return queryBuilder
    //            .InsertInto<JoinOrder>()
    //            .From<JoinUser>(alias: "u")
    //            .InnerJoin<JoinUser, JoinOrder>(alias: "o", on: (user, order) => user.Id == order.UserId)
    //            .Select<JoinUser>(user => new
    //            {
    //                UserId = user.Id
    //            })
    //            .Select<JoinOrder>(order => new
    //            {
    //                order.Total
    //            })
    //            .Where<JoinUser>(user => user.IsActive)
    //            .Build();
    //    }

    //    // Builds an INSERT SELECT command inferring a target column from an aggregate projection alias.
    //    private static GeneratedSqlQuery BuildInsertSelectInferredAggregateQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
    //        where TProfile : IDatabaseProviderProfile
    //    {
    //        return queryBuilder
    //            .InsertInto<JoinOrder>("projection_results")
    //            .From<JoinOrder>(alias: "o")
    //            .SelectAggregate<JoinOrder>(QueryAggregateFunction.Sum, order => order.Total, "TotalAmount")
    //            .Build();
    //    }

    //    // Builds an INSERT SELECT command inferring a target column from a scalar function projection alias.
    //    private static GeneratedSqlQuery BuildInsertSelectInferredScalarFunctionQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
    //        where TProfile : IDatabaseProviderProfile
    //    {
    //        return queryBuilder
    //            .InsertInto<JoinUser>("projection_results")
    //            .From<JoinUser>(alias: "u")
    //            .SelectScalarFunction<JoinUser>(QueryScalarFunction.Upper, user => user.Email, "NormalizedEmail")
    //            .Build();
    //    }

    //    // Builds an INSERT SELECT command inferring a target column from a computed expression alias.
    //    private static GeneratedSqlQuery BuildInsertSelectInferredComputedQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
    //        where TProfile : IDatabaseProviderProfile
    //    {
    //        return queryBuilder
    //            .InsertInto<JoinOrder>("projection_results")
    //            .From<JoinOrder>(alias: "o")
    //            .SelectComputed<JoinOrder>(order => order.Total * 1.16m, "TotalWithTax")
    //            .Build();
    //    }

    //    // Builds an INSERT SELECT command inferring a target column from a CASE WHEN projection alias.
    //    private static GeneratedSqlQuery BuildInsertSelectInferredCaseWhenQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
    //        where TProfile : IDatabaseProviderProfile
    //    {
    //        return queryBuilder
    //            .InsertInto<JoinOrder>("projection_results")
    //            .From<JoinOrder>(alias: "o")
    //            .SelectCaseWhen<JoinOrder>(order => order.Total > 1000, "VIP", "STANDARD", "CustomerType")
    //            .Build();
    //    }

    //    // Builds an INSERT SELECT command inferring a target column from a window function projection alias.
    //    private static GeneratedSqlQuery BuildInsertSelectInferredWindowQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
    //        where TProfile : IDatabaseProviderProfile
    //    {
    //        return queryBuilder
    //            .InsertInto<JoinOrder>("projection_results")
    //            .From<JoinOrder>(alias: "o")
    //            .SelectRowNumber(
    //                "RowNumber",
    //                window => window
    //                    .PartitionBy<JoinOrder>(order => order.UserId)
    //                    .OrderByDescending<JoinOrder>(order => order.Total))
    //            .Build();
    //    }

    //    // Builds an INSERT SELECT command validating inferred target column order across all supported projection types.
    //    private static GeneratedSqlQuery BuildInsertSelectMixedInferredProjectionQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
    //        where TProfile : IDatabaseProviderProfile
    //    {
    //        return queryBuilder
    //            .InsertInto<JoinOrder>("projection_results")
    //            .From<JoinUser>(alias: "u")
    //            .InnerJoin<JoinUser, JoinOrder>(alias: "o", on: (user, order) => user.Id == order.UserId)
    //            .Select<JoinUser>(user => new
    //            {
    //                UserId = user.Id
    //            })
    //            .SelectAggregate<JoinOrder>(QueryAggregateFunction.Sum, order => order.Total, "TotalAmount")
    //            .SelectScalarFunction<JoinUser>(QueryScalarFunction.Upper, user => user.Email, "NormalizedEmail")
    //            .SelectComputed<JoinOrder>(order => order.Total * 1.16m, "TotalWithTax")
    //            .SelectCaseWhen<JoinOrder>(order => order.Total > 1000, "VIP", "STANDARD", "CustomerType")
    //            .SelectRowNumber(
    //                "RowNumber",
    //                window => window
    //                    .PartitionBy<JoinOrder>(order => order.UserId)
    //                    .OrderByDescending<JoinOrder>(order => order.Total))
    //            .GroupBy<JoinUser>(user => new
    //            {
    //                user.Id,
    //                user.Email
    //            })
    //            .GroupBy<JoinOrder>(order => new
    //            {
    //                order.UserId,
    //                order.Total
    //            })
    //            .Build();
    //    }
    //}
}

