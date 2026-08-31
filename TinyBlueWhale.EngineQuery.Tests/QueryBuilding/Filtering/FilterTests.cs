using TinyBlueWhale.EngineQuery.Tests.Models;
using TinyBlueWhale.EngineQuery.Tests.Providers;

namespace TinyBlueWhale.EngineQuery.Tests.QueryBuilding.Filtering
{
    /// <summary>
    /// Validates provider-independent query filtering behavior.
    ///// </summary>
    //[TestFixtureSource(typeof(QueryTestProviderSource), nameof(QueryTestProviderSource.GetProviders))]
    //internal sealed class FilterTests(IQueryTestProvider provider)
    //{
    //    private readonly IQueryTestProvider _provider = provider ?? throw new ArgumentNullException(nameof(provider));

    //    /// <summary>
    //    /// Validates WHERE generation using boolean and string expressions.
    //    /// </summary>
    //    [Test]
    //    public void Build_WhenBooleanAndStringPredicatesAreConfigured_ShouldGenerateExpectedFilters()
    //    {
    //        var query = _provider
    //            .CreateQueryBuilder()
    //            .From<User>()
    //            .Where<User>(user =>
    //                user.IsActive &&
    //                user.Email.Contains("@gmail.com") &&
    //                user.Age >= 18)
    //            .Build();

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(query.CommandText, Does.Contain("WHERE"));
    //            Assert.That(query.CommandText, Does.Contain("IsActive"));
    //            Assert.That(query.CommandText, Does.Contain("Email"));
    //            Assert.That(query.CommandText, Does.Contain("Age"));

    //            Assert.That(query.Parameters, Has.Count.EqualTo(3));
    //            Assert.That(query.Parameters[1].Value, Is.EqualTo("%@gmail.com%"));
    //            Assert.That(query.Parameters[2].Value, Is.EqualTo(18));
    //        });
    //    }

    //    /// <summary>
    //    /// Validates OR composition inside a WHERE predicate.
    //    /// </summary>
    //    [Test]
    //    public void Build_WhenPredicateContainsOr_ShouldGenerateOrOperator()
    //    {
    //        var query = _provider
    //            .CreateQueryBuilder()
    //            .From<User>("u")
    //            .Where<User>(user =>
    //                user.Email.Contains("@gmail.com") ||
    //                user.Email.Contains("@company.com"))
    //            .Build();

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(query.CommandText, Does.Contain("WHERE"));
    //            Assert.That(query.CommandText, Does.Contain(" OR "));
    //            Assert.That(query.CommandText, Does.Contain("LIKE"));

    //            Assert.That(query.Parameters, Has.Count.EqualTo(2));

    //            Assert.That(
    //                query.Parameters[0].Value,
    //                Is.EqualTo("%@gmail.com%"));

    //            Assert.That(
    //                query.Parameters[1].Value,
    //                Is.EqualTo("%@company.com%"));
    //        });
    //    }

    //    /// <summary>
    //    /// Validates conditional WHERE predicate generation.
    //    /// </summary>
    //    [Test]
    //    public void Build_WhenWhereIfConditionsDiffer_ShouldIncludeOnlyEnabledPredicate()
    //    {
    //        var query = _provider
    //            .CreateQueryBuilder()
    //            .From<User>()
    //            .WhereIf<User>(true, user => user.IsActive)
    //            .WhereIf<User>(false, user => user.IsDeleted)
    //            .Build();

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(query.CommandText, Does.Contain("IsActive"));
    //            Assert.That(query.CommandText, Does.Not.Contain("IsDeleted"));
    //        });
    //    }

    //    /// <summary>
    //    /// Validates IN collection predicate generation.
    //    /// </summary>
    //    [Test]
    //    public void Build_WhenWhereInIsConfigured_ShouldGenerateInPredicate()
    //    {
    //        var userIds = new[] { 10, 20, 30 };

    //        var query = _provider
    //            .CreateQueryBuilder()
    //            .From<User>()
    //            .WhereIn(user => user.Id, userIds)
    //            .Build();

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(query.CommandText, Does.Contain(" IN "));
    //            Assert.That(query.CommandText, Does.Contain("Id"));

    //            Assert.That(query.Parameters, Has.Count.EqualTo(3));
    //            Assert.That(query.Parameters[0].Value, Is.EqualTo(10));
    //            Assert.That(query.Parameters[1].Value, Is.EqualTo(20));
    //            Assert.That(query.Parameters[2].Value, Is.EqualTo(30));
    //        });
    //    }

    //    /// <summary>
    //    /// Validates NOT IN collection predicate generation.
    //    /// </summary>
    //    [Test]
    //    public void Build_WhenWhereNotInIsConfigured_ShouldGenerateNotInPredicate()
    //    {
    //        var excludedEmails = new[]
    //        {
    //            "blocked@test.com",
    //            "deleted@test.com"
    //        };

    //        var query = _provider
    //            .CreateQueryBuilder()
    //            .From<User>()
    //            .WhereNotIn(user => user.Email, excludedEmails)
    //            .Build();

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(query.CommandText, Does.Contain(" NOT IN "));
    //            Assert.That(query.CommandText, Does.Contain("Email"));

    //            Assert.That(query.Parameters, Has.Count.EqualTo(2));
    //            Assert.That(query.Parameters[0].Value, Is.EqualTo("blocked@test.com"));
    //            Assert.That(query.Parameters[1].Value, Is.EqualTo("deleted@test.com"));
    //        });
    //    }

    //    /// <summary>
    //    /// Validates IN and NOT IN predicates over an entity introduced by a JOIN.
    //    /// </summary>
    //    [Test]
    //    public void Build_WhenCollectionFiltersTargetJoinedEntity_ShouldUseJoinedSource()
    //    {
    //        var orderIds = new[] { 100, 200 };
    //        var excludedUserIds = new[] { 30, 40 };

    //        var query = _provider
    //            .CreateQueryBuilder()
    //            .From<JoinUser>(alias: "u")
    //            .InnerJoin<JoinUser, JoinOrder>(
    //                alias: "o",
    //                on: (user, order) => user.Id == order.UserId)
    //            .WhereIn<JoinOrder, int>(
    //                order => order.Id,
    //                orderIds)
    //            .WhereNotIn<JoinOrder, int>(
    //                order => order.UserId,
    //                excludedUserIds)
    //            .Build();

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(query.CommandText, Does.Contain("order_id"));
    //            Assert.That(query.CommandText, Does.Contain("user_id"));
    //            Assert.That(query.CommandText, Does.Contain(" IN "));
    //            Assert.That(query.CommandText, Does.Contain(" NOT IN "));

    //            Assert.That(query.Parameters, Has.Count.EqualTo(4));

    //            Assert.That(query.Parameters[0].Value, Is.EqualTo(100));
    //            Assert.That(query.Parameters[1].Value, Is.EqualTo(200));
    //            Assert.That(query.Parameters[2].Value, Is.EqualTo(30));
    //            Assert.That(query.Parameters[3].Value, Is.EqualTo(40));
    //        });
    //    }

    //    /// <summary>
    //    /// Validates null comparison generation for nullable properties.
    //    /// </summary>
    //    [Test]
    //    public void Build_WhenNullablePropertyIsComparedToNull_ShouldGenerateNullPredicates()
    //    {
    //        var query = _provider
    //            .CreateQueryBuilder()
    //            .From<Category>(alias: "c")
    //            .Select<Category>(category => new
    //            {
    //                category.Id,
    //                category.ParentId
    //            })
    //            .Where<Category>(category =>
    //                category.ParentId == null ||
    //                category.ParentId != null)
    //            .Build();

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(query.CommandText, Does.Contain("NULL"));
    //            Assert.That(query.CommandText, Does.Contain(" OR "));
    //            Assert.That(query.CommandText, Does.Contain("ParentId"));
    //        });
    //    }

    //    /// <summary>
    //    /// Validates that a disabled WhereIf predicate does not alter
    //    /// an already configured WHERE clause.
    //    /// </summary>
    //    [Test]
    //    public void Build_WhenWhereIfIsFalseAfterWhere_ShouldPreserveExistingPredicate()
    //    {
    //        var query = _provider
    //            .CreateQueryBuilder()
    //            .From<JoinUser>(alias: "u")
    //            .Where<JoinUser>(user => user.IsActive)
    //            .WhereIf<JoinUser>(
    //                false,
    //                user => user.Email.Contains("@blocked.com"))
    //            .Select<JoinUser>(user => new
    //            {
    //                UserId = user.Id,
    //                user.Email
    //            })
    //            .Build();

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(query.CommandText, Does.Contain("WHERE"));
    //            Assert.That(query.CommandText, Does.Contain("is_active"));
    //            Assert.That(query.CommandText, Does.Not.Contain("@blocked.com"));
    //            Assert.That(query.Parameters, Has.Count.EqualTo(1));
    //        });
    //    }

    //    /// <summary>
    //    /// Validates that IN and NOT IN collections cannot be null.
    //    /// </summary>
    //    /// <param name="isNegated">
    //    /// Determines whether the tested condition uses NOT IN.
    //    /// </param>
    //    [TestCase(false)]
    //    [TestCase(true)]
    //    public void WhereCollection_WhenValuesAreNull_ShouldThrow(bool isNegated)
    //    {
    //        var commandBuilder = _provider
    //            .CreateQueryBuilder()
    //            .From<User>();

    //        IEnumerable<int> values = null!;

    //        var exception = Assert.Throws<ArgumentNullException>(() =>
    //        {
    //            if (isNegated)
    //                commandBuilder.WhereNotIn(user => user.Id, values);
    //            else
    //                commandBuilder.WhereIn(user => user.Id, values);
    //        });

    //        Assert.That(exception!.ParamName, Is.EqualTo("values"));
    //    }

    //    /// <summary>
    //    /// Validates that IN and NOT IN collections cannot be empty.
    //    /// </summary>
    //    /// <param name="isNegated">
    //    /// Determines whether the tested condition uses NOT IN.
    //    /// </param>
    //    [TestCase(false)]
    //    [TestCase(true)]
    //    public void WhereCollection_WhenValuesAreEmpty_ShouldThrow(bool isNegated)
    //    {
    //        var commandBuilder = _provider
    //            .CreateQueryBuilder()
    //            .From<User>();

    //        var values = Array.Empty<int>();

    //        var exception = Assert.Throws<ArgumentException>(() =>
    //        {
    //            if (isNegated)
    //                commandBuilder.WhereNotIn(user => user.Id, values);
    //            else
    //                commandBuilder.WhereIn(user => user.Id, values);
    //        });

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(exception!.ParamName, Is.EqualTo("values"));
    //            Assert.That(exception.Message, Does.StartWith("IN and NOT IN collections must contain at least one value."));
    //        });
    //    }

    //    /// <summary>
    //    /// Validates that IN and NOT IN collections cannot contain null values.
    //    /// </summary>
    //    /// <param name="isNegated">
    //    /// Determines whether the tested condition uses NOT IN.
    //    /// </param>
    //    [TestCase(false)]
    //    [TestCase(true)]
    //    public void WhereCollection_WhenValuesContainNull_ShouldThrow(bool isNegated)
    //    {
    //        var commandBuilder = _provider
    //            .CreateQueryBuilder()
    //            .From<User>();

    //        IEnumerable<string> values =
    //        [
    //            "valid@test.com",
    //             null!
    //        ];

    //        var exception = Assert.Throws<ArgumentException>(() =>
    //        {
    //            if (isNegated)
    //                commandBuilder.WhereNotIn(user => user.Email, values);
    //            else
    //                commandBuilder.WhereIn(user => user.Email, values);
    //        });

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(exception!.ParamName, Is.EqualTo("values"));
    //            Assert.That(exception.Message, Does.StartWith("IN and NOT IN collections cannot contain null values."));
    //        });
    //    }
    //}
}
