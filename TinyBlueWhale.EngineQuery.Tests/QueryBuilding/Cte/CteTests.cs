namespace TinyBlueWhale.EngineQuery.Tests.QueryBuilding.CTE
{
    ///// <summary>
    ///// Validates provider-independent common table expression behavior.
    ///// </summary>
    //[TestFixtureSource(typeof(QueryTestProviderSource), nameof(QueryTestProviderSource.GetProviders))]
    //internal sealed class CteTests(IQueryTestProvider provider)
    //{
    //    private readonly IQueryTestProvider _provider = provider ?? throw new ArgumentNullException(nameof(provider));

    //    /// <summary>
    //    /// Validates common table expression generation.
    //    /// </summary>
    //    [Test]
    //    public void Build_WhenCteIsConfigured_ShouldGenerateCte()
    //    {
    //        var query = _provider
    //            .CreateQueryBuilder()
    //            .With<OrderSummary, JoinOrder>(
    //                "order_summary",
    //                cte => cte
    //                    .From<JoinOrder>(alias: "o")
    //                    .Select<JoinOrder>(order => new
    //                    {
    //                        UserId = order.UserId
    //                    })
    //                    .SelectAggregate<JoinOrder>(
    //                        QueryAggregateFunction.Sum,
    //                        order => order.Total,
    //                        "TotalAmount")
    //                    .SelectAggregate<JoinOrder>(
    //                        QueryAggregateFunction.Count,
    //                        order => order.Id,
    //                        "OrderCount")
    //                    .GroupBy<JoinOrder>(order => order.UserId))
    //            .FromCte<OrderSummary>("order_summary")
    //            .Select<OrderSummary>(summary => new
    //            {
    //                summary.UserId,
    //                summary.TotalAmount,
    //                summary.OrderCount
    //            })
    //            .Build();

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(query.CommandText, Does.Contain("WITH"));
    //            Assert.That(query.CommandText, Does.Contain("order_summary"));
    //            Assert.That(query.CommandText, Does.Contain("SELECT"));
    //            Assert.That(query.CommandText, Does.Contain("TotalAmount"));
    //            Assert.That(query.CommandText, Does.Contain("OrderCount"));
    //        });
    //    }

    //    /// <summary>
    //    /// Validates recursive common table expression generation.
    //    /// </summary>
    //    [Test]
    //    public void Build_WhenRecursiveCteIsConfigured_ShouldGenerateRecursiveCte()
    //    {
    //        var query = _provider
    //            .CreateQueryBuilder()
    //            .WithRecursive<CategoryTree, Category, Category>(
    //                name: "category_tree",
    //                baseQueryBuilder: baseQuery => baseQuery
    //                    .From<Category>(alias: "c")
    //                    .Select<Category>(category => new
    //                    {
    //                        category.Id,
    //                        category.ParentId,
    //                        category.Name
    //                    })
    //                    .Where<Category>(category => category.ParentId == null),
    //                recursiveQueryBuilder: recursiveQuery => recursiveQuery
    //                    .From<Category>(alias: "c")
    //                    .InnerJoin<Category, CategoryTree>(
    //                        alias: "ct",
    //                        on: (category, tree) => category.ParentId == tree.Id)
    //                    .Select<Category>(category => new
    //                    {
    //                        category.Id,
    //                        category.ParentId,
    //                        category.Name
    //                    }))
    //            .FromCte<CategoryTree>("category_tree")
    //            .Select<CategoryTree>(tree => new
    //            {
    //                tree.Id,
    //                tree.ParentId,
    //                tree.Name
    //            })
    //            .Build();

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(query.CommandText, Does.Contain("WITH"));
    //            Assert.That(query.CommandText, Does.Contain("category_tree"));
    //            Assert.That(query.CommandText, Does.Contain("INNER JOIN"));
    //            Assert.That(query.CommandText, Does.Contain("ParentId"));
    //        });
    //    }

    //    /// <summary>
    //    /// Validates that common table expression definitions do not leak
    //    /// between queries created from the same query builder instance.
    //    /// </summary>
    //    [Test]
    //    public void Build_WhenQueriesReuseBuilder_ShouldNotLeakPreviousCteDefinitions()
    //    {
    //        var queryBuilder = _provider.CreateQueryBuilder();

    //        var firstQuery = queryBuilder
    //            .With<OrderSummary, JoinOrder>(
    //                "first_summary",
    //                cte => cte
    //                    .From<JoinOrder>(alias: "o")
    //                    .Select<JoinOrder>(order => new
    //                    {
    //                        UserId = order.UserId
    //                    }))
    //            .FromCte<OrderSummary>("first_summary")
    //            .Select<OrderSummary>(summary => new
    //            {
    //                summary.UserId
    //            })
    //            .Build();

    //        var secondQuery = queryBuilder
    //            .With<OrderSummary, JoinOrder>(
    //                "second_summary",
    //                cte => cte
    //                    .From<JoinOrder>(alias: "o")
    //                    .Select<JoinOrder>(order => new
    //                    {
    //                        UserId = order.UserId
    //                    }))
    //            .FromCte<OrderSummary>("second_summary")
    //            .Select<OrderSummary>(summary => new
    //            {
    //                summary.UserId
    //            })
    //            .Build();

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(firstQuery.CommandText, Does.Contain("first_summary"));
    //            Assert.That(secondQuery.CommandText, Does.Contain("second_summary"));
    //            Assert.That(secondQuery.CommandText, Does.Not.Contain("first_summary"));
    //        });
    //    }

    //    /// <summary>
    //    /// Validates parameter preservation between CTE and outer query scopes.
    //    /// </summary>
    //    [Test]
    //    public void Build_WhenCteContainsParameters_ShouldPreserveAllParameters()
    //    {
    //        var query = _provider
    //            .CreateQueryBuilder()
    //            .With<OrderSummary, JoinOrder>(
    //                "order_summary",
    //                cte => cte
    //                    .From<JoinOrder>(alias: "o")
    //                    .Select<JoinOrder>(order => new
    //                    {
    //                        UserId = order.UserId
    //                    })
    //                    .SelectAggregate<JoinOrder>(
    //                        QueryAggregateFunction.Sum,
    //                        order => order.Total,
    //                        "TotalAmount")
    //                    .SelectAggregate<JoinOrder>(
    //                        QueryAggregateFunction.Count,
    //                        order => order.Id,
    //                        "OrderCount")
    //                    .Where<JoinOrder>(order => order.Total > 100)
    //                    .GroupBy<JoinOrder>(order => order.UserId))
    //            .FromCte<OrderSummary>("order_summary")
    //            .Select<OrderSummary>(summary => new
    //            {
    //                summary.UserId,
    //                summary.TotalAmount,
    //                summary.OrderCount
    //            })
    //            .WhereComputed<OrderSummary>(summary =>
    //                summary.TotalAmount > 500m)
    //            .Build();

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(query.CommandText, Does.Contain("order_summary"));
    //            Assert.That(query.Parameters, Has.Count.EqualTo(2));
    //            Assert.That(query.Parameters.Any(parameter => Equals(parameter.Value, 100m)), Is.True);
    //            Assert.That(query.Parameters.Any(parameter => Equals(parameter.Value, 500m)), Is.True);
    //        });
    //    }

    //    /// <summary>
    //    /// Validates that recursive common table expressions cannot be compiled
    //    /// when the current provider does not support them.
    //    /// </summary>
    //    [Test]
    //    public void Build_WhenRecursiveCteIsNotSupported_ShouldThrow()
    //    {
    //        var builder = _provider.CreateQueryBuilder(
    //            new UnsupportedRecursiveCteCapabilities());

    //        var exception = Assert.Throws<NotSupportedException>(() =>
    //            builder
    //                .WithRecursive<CategoryTree, Category, Category>(
    //                    name: "category_tree",
    //                    baseQueryBuilder: baseQuery => baseQuery
    //                        .From<Category>(alias: "c")
    //                        .Select<Category>(category => new
    //                        {
    //                            category.Id,
    //                            category.ParentId,
    //                            category.Name
    //                        })
    //                        .Where<Category>(category => category.ParentId == null),
    //                    recursiveQueryBuilder: recursiveQuery => recursiveQuery
    //                        .From<Category>(alias: "c")
    //                        .InnerJoin<Category, CategoryTree>(
    //                            alias: "ct",
    //                            on: (category, tree) => category.ParentId == tree.Id)
    //                        .Select<Category>(category => new
    //                        {
    //                            category.Id,
    //                            category.ParentId,
    //                            category.Name
    //                        }))
    //                .FromCte<CategoryTree>("category_tree")
    //                .Build());

    //        Assert.That(
    //            exception!.Message,
    //            Is.EqualTo("Recursive common table expressions are not supported by the current provider."));
    //    }
    //}
}
