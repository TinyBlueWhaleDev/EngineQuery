using TinyBlueWhale.EngineQuery.Tests.Models;
using TinyBlueWhale.EngineQuery.Tests.Providers;

namespace TinyBlueWhale.EngineQuery.Tests.QueryBuilding.Ordering
{
    /// <summary>
    /// Validates provider-independent ordering and pagination behavior.
    /// </summary>
    //[TestFixtureSource(typeof(QueryTestProviderSource), nameof(QueryTestProviderSource.GetProviders))]
    //internal sealed class OrderingTests(IQueryTestProvider provider)
    //{
    //    private readonly IQueryTestProvider _provider = provider ?? throw new ArgumentNullException(nameof(provider));

    //    /// <summary>
    //    /// Validates ORDER BY generation using ascending and descending columns.
    //    /// </summary>
    //    [Test]
    //    public void Build_WhenMultipleOrderingsAreConfigured_ShouldGenerateExpectedOrder()
    //    {
    //        var query = _provider
    //            .CreateQueryBuilder()
    //            .From<User>()
    //            .OrderBy<User>(user => user.Email)
    //            .ThenByDescending<User>(user => user.CreatedAt)
    //            .Build();

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(query.CommandText, Does.Contain("ORDER BY"));
    //            Assert.That(query.CommandText, Does.Contain("Email"));
    //            Assert.That(query.CommandText, Does.Contain("CreatedAt"));
    //            Assert.That(query.CommandText, Does.Contain("DESC"));
    //        });
    //    }

    //    /// <summary>
    //    /// Validates pagination generation using skip and take values.
    //    /// </summary>
    //    [Test]
    //    public void Build_WhenPaginationIsConfigured_ShouldGeneratePagination()
    //    {
    //        var query = _provider
    //            .CreateQueryBuilder()
    //            .From<User>()
    //            .OrderBy<User>(user => user.Email)
    //            .Skip(20)
    //            .Take(10)
    //            .Build();

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(query.CommandText, Does.Contain("ORDER BY"));
    //            Assert.That(query.CommandText, Does.Contain("Email"));

    //            Assert.That(query.CommandText, Does.Contain("20"));
    //            Assert.That(query.CommandText, Does.Contain("10"));

    //            Assert.That(query.Parameters, Is.Empty);
    //        });
    //    }

    //    /// <summary>
    //    /// Validates that pagination requires an ORDER BY definition.
    //    /// </summary>
    //    [Test]
    //    public void Build_WhenPaginationHasNoOrderBy_ShouldThrow()
    //    {
    //        var exception = Assert.Throws<InvalidOperationException>(() =>
    //            _provider
    //                .CreateQueryBuilder()
    //                .From<User>()
    //                .Skip(10)
    //                .Take(5)
    //                .Build());

    //        Assert.That(exception, Is.Not.Null);
    //    }
    //}
}
