using TinyBlueWhale.EngineQuery.Tests.Models;
using TinyBlueWhale.EngineQuery.Tests.Providers;

namespace TinyBlueWhale.EngineQuery.Tests.QueryBuilding.Select
{
    /// <summary>
    /// Validates provider-independent SELECT query behavior.
    /// </summary>
    //[TestFixtureSource(typeof(QueryTestProviderSource), nameof(QueryTestProviderSource.GetProviders))]
    //internal sealed class SelectTests(IQueryTestProvider provider)
    //{
    //    private readonly IQueryTestProvider _provider = provider ?? throw new ArgumentNullException(nameof(provider));

    //    /// <summary>
    //    /// Validates SELECT generation when no explicit projection is configured.
    //    /// </summary>
    //    [Test]
    //    public void Build_WhenProjectionIsNotConfigured_ShouldGenerateSelectAll()
    //    {
    //        var query = _provider
    //            .CreateQueryBuilder()
    //            .From<User>("Users")
    //            .Build();

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(query.CommandText, Does.Contain("SELECT"));
    //            Assert.That(query.CommandText, Does.Contain("*"));
    //            Assert.That(query.CommandText, Does.Contain("FROM"));
    //            Assert.That(query.CommandText, Does.Contain("Users"));
    //            Assert.That(query.Parameters, Is.Empty);
    //        });
    //    }

    //    /// <summary>
    //    /// Validates SELECT generation using an explicit property projection.
    //    /// </summary>
    //    [Test]
    //    public void Build_WhenProjectionIsConfigured_ShouldGenerateSelectedColumns()
    //    {
    //        var query = _provider
    //            .CreateQueryBuilder()
    //            .From<User>("Users")
    //            .Select<User>(user => new
    //            {
    //                user.Id,
    //                user.Email
    //            })
    //            .Build();

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(query.CommandText, Does.Contain("SELECT"));
    //            Assert.That(query.CommandText, Does.Contain("Id"));
    //            Assert.That(query.CommandText, Does.Contain("Email"));
    //            Assert.That(query.CommandText, Does.Contain("FROM"));
    //            Assert.That(query.CommandText, Does.Contain("Users"));

    //            Assert.That(query.CommandText, Does.Not.Contain("Age"));
    //            Assert.That(query.CommandText, Does.Not.Contain("IsActive"));

    //            Assert.That(query.Parameters, Is.Empty);
    //        });
    //    }
    //}
}
