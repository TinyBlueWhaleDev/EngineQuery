using TinyBlueWhale.EngineQuery.Tests.Models;
using TinyBlueWhale.EngineQuery.Tests.Providers;

namespace TinyBlueWhale.EngineQuery.Tests.QueryBuilding.Insert
{
    /// <summary>
    /// Validates provider-independent INSERT VALUES behavior.
    /// </summary>
    //[TestFixtureSource(typeof(QueryTestProviderSource), nameof(QueryTestProviderSource.GetProviders))]
    //internal sealed class InsertTests(IQueryTestProvider provider)
    //{
    //    private readonly IQueryTestProvider _provider =
    //        provider ?? throw new ArgumentNullException(nameof(provider));

    //    /// <summary>
    //    /// Validates SQL generation for a strongly typed INSERT VALUES command.
    //    /// </summary>
    //    [Test]
    //    public void Build_WhenInsertIsValid_ShouldGenerateExpectedSql()
    //    {
    //        var query = _provider
    //            .CreateQueryBuilder()
    //            .InsertInto<User>()
    //            .Set(user => user.Email, "admin@test.com")
    //            .Set(user => user.Age, 35)
    //            .Set(user => user.IsActive, true)
    //            .Build();

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(query.CommandText, Does.Contain("INSERT"));
    //            Assert.That(query.CommandText, Does.Contain("Users"));
    //            Assert.That(query.CommandText, Does.Contain("Email"));
    //            Assert.That(query.CommandText, Does.Contain("Age"));
    //            Assert.That(query.CommandText, Does.Contain("IsActive"));
    //            Assert.That(query.CommandText, Does.Contain("VALUES"));

    //            Assert.That(query.Parameters, Has.Count.EqualTo(3));

    //            Assert.That(
    //                query.Parameters[0].Value,
    //                Is.EqualTo("admin@test.com"));

    //            Assert.That(
    //                query.Parameters[1].Value,
    //                Is.EqualTo(35));

    //            Assert.That(
    //                query.Parameters[2].Value,
    //                Is.EqualTo(true));
    //        });
    //    }

    //    /// <summary>
    //    /// Validates INSERT parameter generation in assignment order.
    //    /// </summary>
    //    [Test]
    //    public void Build_WhenMultipleValuesAreConfigured_ShouldGenerateParametersInAssignmentOrder()
    //    {
    //        var query = _provider
    //            .CreateQueryBuilder()
    //            .InsertInto<User>()
    //            .Set(user => user.Email, "admin@test.com")
    //            .Set(user => user.Age, 35)
    //            .Set(user => user.IsActive, true)
    //            .Build();

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(query.Parameters, Has.Count.EqualTo(3));

    //            Assert.That(
    //                query.Parameters[0].Name,
    //                Is.EqualTo("@p0"));

    //            Assert.That(
    //                query.Parameters[0].Value,
    //                Is.EqualTo("admin@test.com"));

    //            Assert.That(
    //                query.Parameters[1].Name,
    //                Is.EqualTo("@p1"));

    //            Assert.That(
    //                query.Parameters[1].Value,
    //                Is.EqualTo(35));

    //            Assert.That(
    //                query.Parameters[2].Name,
    //                Is.EqualTo("@p2"));

    //            Assert.That(
    //                query.Parameters[2].Value,
    //                Is.EqualTo(true));
    //        });
    //    }

    //    /// <summary>
    //    /// Validates INSERT parameter generation when a nullable value
    //    /// is explicitly assigned as null.
    //    /// </summary>
    //    [Test]
    //    public void Build_WhenValueIsNull_ShouldGenerateNullParameter()
    //    {
    //        var query = _provider
    //            .CreateQueryBuilder()
    //            .InsertInto<Category>()
    //            .Set(category => category.ParentId, null)
    //            .Build();

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(query.Parameters, Has.Count.EqualTo(1));
    //            Assert.That(query.Parameters[0].Name, Is.EqualTo("@p0"));
    //            Assert.That(query.Parameters[0].Value, Is.Null);
    //        });
    //    }

    //    /// <summary>
    //    /// Validates that INSERT assignments only accept direct
    //    /// entity property selectors.
    //    /// </summary>
    //    [Test]
    //    public void Set_WhenSelectorIsNotDirectProperty_ShouldThrow()
    //    {
    //        var commandBuilder = _provider
    //            .CreateQueryBuilder()
    //            .InsertInto<User>();

    //        Assert.Throws<ArgumentException>(() => commandBuilder.Set(user => user.Email.ToLower(), "admin@test.com"));
    //    }

    //    /// <summary>
    //    /// Validates that the same INSERT property cannot receive
    //    /// multiple value assignments.
    //    /// </summary>
    //    [Test]
    //    public void Set_WhenPropertyIsAssignedMoreThanOnce_ShouldThrow()
    //    {
    //        var commandBuilder = _provider
    //            .CreateQueryBuilder()
    //            .InsertInto<User>()
    //            .Set(user => user.Email, "first@test.com");

    //        Assert.Throws<InvalidOperationException>(() => commandBuilder.Set(user => user.Email, "second@test.com"));
    //    }

    //    /// <summary>
    //    /// Validates INSERT generation using an explicitly configured table name.
    //    /// </summary>
    //    [Test]
    //    public void Build_WhenExplicitTableNameIsProvided_ShouldUseTableName()
    //    {
    //        var query = _provider
    //            .CreateQueryBuilder()
    //            .InsertInto<User>("CustomUsers")
    //            .Set(user => user.Email, "admin@test.com")
    //            .Build();

    //        Assert.That(query.CommandText, Does.Contain("CustomUsers"));
    //    }
    //}
}
