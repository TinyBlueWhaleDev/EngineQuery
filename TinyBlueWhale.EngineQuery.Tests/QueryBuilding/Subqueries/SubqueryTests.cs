namespace TinyBlueWhale.EngineQuery.Tests.QueryBuilding.Subqueries
{
    /// <summary>
    /// Validates provider-independent subquery behavior.
    /// </summary>
    //[TestFixtureSource(typeof(QueryTestProviderSource), nameof(QueryTestProviderSource.GetProviders))]
    //internal sealed class SubqueryTests(IQueryTestProvider provider)
    //{
    //    private readonly IQueryTestProvider _provider = provider ?? throw new ArgumentNullException(nameof(provider));

    //    /// <summary>
    //    /// Validates correlated EXISTS generation.
    //    /// </summary>
    //    [Test]
    //    public void Build_WhenExistsIsConfigured_ShouldGenerateExistsPredicate()
    //    {
    //        var query = _provider
    //            .CreateQueryBuilder()
    //            .From<JoinUser>(alias: "u")
    //            .Select<JoinUser>(user => new
    //            {
    //                UserId = user.Id,
    //                user.Email
    //            })
    //            .WhereExists<JoinUser, JoinOrder>(
    //                alias: "o",
    //                subquery => subquery
    //                    .WhereComputed<JoinOrder, JoinUser>((order, user) =>
    //                        order.UserId == user.Id &&
    //                        order.Total > 100))
    //            .Build();

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(query.CommandText, Does.Contain("EXISTS"));
    //            Assert.That(query.CommandText, Does.Contain("orders"));
    //            Assert.That(query.CommandText, Does.Contain("user_id"));
    //            Assert.That(query.Parameters.Any(parameter => Equals(parameter.Value, 100m)), Is.True);
    //        });
    //    }

    //    /// <summary>
    //    /// Validates correlated NOT EXISTS generation.
    //    /// </summary>
    //    [Test]
    //    public void Build_WhenNotExistsIsConfigured_ShouldGenerateNotExistsPredicate()
    //    {
    //        var query = _provider
    //            .CreateQueryBuilder()
    //            .From<JoinUser>(alias: "u")
    //            .WhereNotExists<JoinUser, JoinOrder>(
    //                alias: "o",
    //                subquery => subquery
    //                    .WhereComputed<JoinOrder, JoinUser>((order, user) =>
    //                        order.UserId == user.Id &&
    //                        order.Total <= 0))
    //            .Build();

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(query.CommandText, Does.Contain("NOT EXISTS"));
    //            Assert.That(query.CommandText, Does.Contain("orders"));
    //            Assert.That(query.Parameters.Any(parameter => Equals(parameter.Value, 0m)), Is.True);
    //        });
    //    }

    //    /// <summary>
    //    /// Validates IN subquery generation.
    //    /// </summary>
    //    [Test]
    //    public void Build_WhenInSubqueryIsConfigured_ShouldGenerateInPredicate()
    //    {
    //        var query = _provider
    //            .CreateQueryBuilder()
    //            .From<JoinUser>(alias: "u")
    //            .WhereIn<JoinUser, JoinOrder>(
    //                user => user.Id,
    //                alias: "o",
    //                subquery => subquery
    //                    .Select<JoinOrder>(order => new
    //                    {
    //                        order.UserId
    //                    })
    //                    .Where<JoinOrder>(order => order.Total > 500))
    //            .Build();

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(query.CommandText, Does.Contain(" IN "));
    //            Assert.That(query.CommandText, Does.Contain("SELECT"));
    //            Assert.That(query.CommandText, Does.Contain("orders"));
    //            Assert.That(query.Parameters.Any(parameter => Equals(parameter.Value, 500m)), Is.True);
    //        });
    //    }

    //    /// <summary>
    //    /// Validates multiple correlated subquery predicates in the same query.
    //    /// </summary>
    //    [Test]
    //    public void Build_WhenMultipleSubqueryPredicatesAreConfigured_ShouldGenerateAllPredicates()
    //    {
    //        var query = _provider
    //            .CreateQueryBuilder()
    //            .From<JoinUser>(alias: "u")
    //            .Select<JoinUser>(user => new
    //            {
    //                UserId = user.Id,
    //                user.Email
    //            })
    //            .WhereExists<JoinUser, JoinOrder>(
    //                alias: "o",
    //                subquery => subquery
    //                    .WhereComputed<JoinOrder, JoinUser>((order, user) =>
    //                        order.UserId == user.Id &&
    //                        order.Total > 100))
    //            .WhereNotExists<JoinUser, JoinOrder>(
    //                alias: "o2",
    //                subquery => subquery
    //                    .WhereComputed<JoinOrder, JoinUser>((order, user) =>
    //                        order.UserId == user.Id &&
    //                        order.Total <= 0))
    //            .WhereIn<JoinUser, JoinOrder>(
    //                user => user.Id,
    //                alias: "oi",
    //                subquery => subquery
    //                    .Select<JoinOrder>(order => new
    //                    {
    //                        order.UserId
    //                    })
    //                    .Where<JoinOrder>(order => order.Total > 500))
    //            .Build();

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(query.CommandText, Does.Contain("EXISTS"));
    //            Assert.That(query.CommandText, Does.Contain("NOT EXISTS"));
    //            Assert.That(query.CommandText, Does.Contain(" IN "));
    //            Assert.That(query.Parameters.Any(parameter => Equals(parameter.Value, 100m)), Is.True);
    //            Assert.That(query.Parameters.Any(parameter => Equals(parameter.Value, 0m)), Is.True);
    //            Assert.That(query.Parameters.Any(parameter => Equals(parameter.Value, 500m)), Is.True);
    //        });
    //    }

    //    /// <summary>
    //    /// Validates derived table generation using a nested query source.
    //    /// </summary>
    //    [Test]
    //    public void Build_WhenDerivedTableIsConfigured_ShouldGenerateDerivedSource()
    //    {
    //        var query = _provider
    //            .CreateQueryBuilder()
    //            .FromSubquery<OrderSummary, JoinOrder>(
    //                alias: "summary",
    //                subquery => subquery
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
    //            .Select<OrderSummary>(summary => new
    //            {
    //                summary.UserId,
    //                summary.TotalAmount,
    //                summary.OrderCount
    //            })
    //            .Build();

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(query.CommandText, Does.Contain("SELECT"));
    //            Assert.That(query.CommandText, Does.Contain("orders"));
    //            Assert.That(query.CommandText, Does.Contain("summary"));
    //            Assert.That(query.CommandText, Does.Contain("TotalAmount"));
    //            Assert.That(query.CommandText, Does.Contain("OrderCount"));
    //        });
    //    }

    //    /// <summary>
    //    /// Validates correlated IN subquery generation using outer source references.
    //    /// </summary>
    //    [Test]
    //    public void Build_WhenCorrelatedInSubqueryIsConfigured_ShouldGenerateOuterReference()
    //    {
    //        var query = _provider
    //            .CreateQueryBuilder()
    //            .From<JoinUser>(alias: "u")
    //            .Select<JoinUser>(user => new
    //            {
    //                UserId = user.Id,
    //                user.Email
    //            })
    //            .WhereIn<JoinUser, JoinOrder>(
    //                user => user.Id,
    //                alias: "o",
    //                subquery => subquery
    //                    .Select<JoinOrder>(order => new
    //                    {
    //                        order.UserId
    //                    })
    //                    .WhereComputed<JoinOrder, JoinUser>((order, user) =>
    //                        order.UserId == user.Id &&
    //                        order.Total > 100))
    //            .Build();

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(query.CommandText, Does.Contain(" IN "));
    //            Assert.That(query.CommandText, Does.Contain("orders"));
    //            Assert.That(query.CommandText, Does.Contain("user_id"));
    //            Assert.That(query.Parameters.Any(parameter => Equals(parameter.Value, 100m)), Is.True);
    //        });
    //    }

    //    /// <summary>
    //    /// Validates derived table parameter reindexing between inner
    //    /// and outer query scopes.
    //    /// </summary>
    //    [Test]
    //    public void Build_WhenDerivedTableContainsParameters_ShouldPreserveAllParameters()
    //    {
    //        var query = _provider
    //            .CreateQueryBuilder()
    //            .FromSubquery<OrderSummary, JoinOrder>(
    //                alias: "summary",
    //                subquery => subquery
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
    //            .Select<OrderSummary>(summary => new
    //            {
    //                summary.UserId,
    //                summary.TotalAmount,
    //                summary.OrderCount
    //            })
    //            .WhereComputed<OrderSummary>(summary =>
    //                summary.TotalAmount > 500)
    //            .Build();

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(query.CommandText, Does.Contain("summary"));
    //            Assert.That(query.Parameters, Has.Count.EqualTo(2));
    //            Assert.That(query.Parameters.Any(parameter => Equals(parameter.Value, 100m)), Is.True);
    //            Assert.That(query.Parameters.Any(parameter => Equals(parameter.Value, 500m)), Is.True);
    //        });
    //    }
    //}
}
