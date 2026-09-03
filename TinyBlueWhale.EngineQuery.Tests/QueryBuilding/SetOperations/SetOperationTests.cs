namespace TinyBlueWhale.EngineQuery.Tests.QueryBuilding.SetOperations
{
    /// <summary>
    /// Validates provider-independent set operation behavior.
    /// </summary>
    //[TestFixtureSource(typeof(QueryTestProviderSource), nameof(QueryTestProviderSource.GetProviders))]
    //internal sealed class SetOperationTests(IQueryTestProvider provider)
    //{
    //    private readonly IQueryTestProvider _provider = provider ?? throw new ArgumentNullException(nameof(provider));

    //    /// <summary>
    //    /// Validates UNION ALL generation between compatible projections.
    //    /// </summary>
    //    [Test]
    //    public void Build_WhenUnionAllIsConfigured_ShouldGenerateUnionAll()
    //    {
    //        var query = _provider
    //            .CreateQueryBuilder()
    //            .From<ActiveUser>(alias: "u")
    //            .Select<ActiveUser>(user => new
    //            {
    //                user.Email
    //            })
    //            .UnionAll<ArchivedUser>(set => set
    //                .From<ArchivedUser>(alias: "a")
    //                .Select<ArchivedUser>(user => new
    //                {
    //                    user.Email
    //                }))
    //            .Build();

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(query.CommandText, Does.Contain("UNION ALL"));
    //            Assert.That(query.CommandText, Does.Contain("users"));
    //            Assert.That(query.CommandText, Does.Contain("archived_users"));
    //            Assert.That(query.CommandText, Does.Contain("email"));
    //        });
    //    }

    //    /// <summary>
    //    /// Validates INTERSECT generation between compatible projections.
    //    /// </summary>
    //    [Test]
    //    public void Build_WhenIntersectIsConfigured_ShouldGenerateIntersect()
    //    {
    //        var query = _provider
    //            .CreateQueryBuilder()
    //            .From<ActiveUser>(alias: "u")
    //            .Select<ActiveUser>(user => new
    //            {
    //                user.Email
    //            })
    //            .Intersect<ArchivedUser>(set => set
    //                .From<ArchivedUser>(alias: "a")
    //                .Select<ArchivedUser>(user => new
    //                {
    //                    user.Email
    //                }))
    //            .Build();

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(query.CommandText, Does.Contain("INTERSECT"));
    //            Assert.That(query.CommandText, Does.Contain("users"));
    //            Assert.That(query.CommandText, Does.Contain("archived_users"));
    //        });
    //    }

    //    /// <summary>
    //    /// Validates EXCEPT generation between compatible projections.
    //    /// </summary>
    //    [Test]
    //    public void Build_WhenExceptIsConfigured_ShouldGenerateExcept()
    //    {
    //        var query = _provider
    //            .CreateQueryBuilder()
    //            .From<ActiveUser>(alias: "u")
    //            .Select<ActiveUser>(user => new
    //            {
    //                user.Email
    //            })
    //            .Except<ArchivedUser>(set => set
    //                .From<ArchivedUser>(alias: "a")
    //                .Select<ArchivedUser>(user => new
    //                {
    //                    user.Email
    //                }))
    //            .Build();

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(query.CommandText, Does.Contain("EXCEPT"));
    //            Assert.That(query.CommandText, Does.Contain("users"));
    //            Assert.That(query.CommandText, Does.Contain("archived_users"));
    //        });
    //    }

    //    /// <summary>
    //    /// Validates multiple set operations composed in a single query.
    //    /// </summary>
    //    [Test]
    //    public void Build_WhenMultipleSetOperationsAreConfigured_ShouldGenerateAllOperations()
    //    {
    //        var query = _provider
    //            .CreateQueryBuilder()
    //            .From<ActiveUser>(alias: "u")
    //            .Select<ActiveUser>(user => new
    //            {
    //                user.Email
    //            })
    //            .UnionAll<ArchivedUser>(set => set
    //                .From<ArchivedUser>(alias: "a")
    //                .Select<ArchivedUser>(user => new
    //                {
    //                    user.Email
    //                }))
    //            .Intersect<ArchivedUser>(set => set
    //                .From<ArchivedUser>(alias: "a2")
    //                .Select<ArchivedUser>(user => new
    //                {
    //                    user.Email
    //                }))
    //            .Except<ArchivedUser>(set => set
    //                .From<ArchivedUser>(alias: "a3")
    //                .Select<ArchivedUser>(user => new
    //                {
    //                    user.Email
    //                }))
    //            .Build();

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(query.CommandText, Does.Contain("UNION ALL"));
    //            Assert.That(query.CommandText, Does.Contain("INTERSECT"));
    //            Assert.That(query.CommandText, Does.Contain("EXCEPT"));
    //        });
    //    }

    //    /// <summary>
    //    /// Validates that INTERSECT cannot be compiled when
    //    /// the current provider does not support set operations.
    //    /// </summary>
    //    [Test]
    //    public void Build_WhenIntersectIsNotSupported_ShouldThrow()
    //    {
    //        var builder = _provider.CreateQueryBuilder(
    //            new UnsupportedSetOperationCapabilities());

    //        var exception = Assert.Throws<NotSupportedException>(() =>
    //            builder
    //                .From<ActiveUser>(alias: "u")
    //                .Select<ActiveUser>(user => new
    //                {
    //                    user.Email
    //                })
    //                .Intersect<ArchivedUser>(set => set
    //                    .From<ArchivedUser>(alias: "a")
    //                    .Select<ArchivedUser>(user => new
    //                    {
    //                        user.Email
    //                    }))
    //                .Build());

    //        Assert.That(
    //            exception!.Message,
    //            Is.EqualTo("INTERSECT set operations are not supported by the current provider."));
    //    }

    //    /// <summary>
    //    /// Validates that EXCEPT cannot be compiled when
    //    /// the current provider does not support set operations.
    //    /// </summary>
    //    [Test]
    //    public void Build_WhenExceptIsNotSupported_ShouldThrow()
    //    {
    //        var builder = _provider.CreateQueryBuilder(
    //            new UnsupportedSetOperationCapabilities());

    //        var exception = Assert.Throws<NotSupportedException>(() =>
    //            builder
    //                .From<ActiveUser>(alias: "u")
    //                .Select<ActiveUser>(user => new
    //                {
    //                    user.Email
    //                })
    //                .Except<ArchivedUser>(set => set
    //                    .From<ArchivedUser>(alias: "a")
    //                    .Select<ArchivedUser>(user => new
    //                    {
    //                        user.Email
    //                    }))
    //                .Build());

    //        Assert.That(
    //            exception!.Message,
    //            Is.EqualTo("EXCEPT set operations are not supported by the current provider."));
    //    }
    //}
}
