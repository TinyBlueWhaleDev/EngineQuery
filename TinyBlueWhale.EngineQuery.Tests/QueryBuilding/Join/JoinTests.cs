using TinyBlueWhale.EngineQuery.Tests.Models;
using TinyBlueWhale.EngineQuery.Tests.Providers;

namespace TinyBlueWhale.EngineQuery.Tests.QueryBuilding.Join
{
    /// <summary>
    /// Validates provider-independent JOIN query behavior.
    /// </summary>
    //[TestFixtureSource(typeof(QueryTestProviderSource), nameof(QueryTestProviderSource.GetProviders))]
    //internal sealed class JoinTests(IQueryTestProvider provider)
    //{
    //    private readonly IQueryTestProvider _provider = provider ?? throw new ArgumentNullException(nameof(provider));

    //    /// <summary>
    //    /// Validates INNER JOIN generation between two strongly typed sources.
    //    /// </summary>
    //    [Test]
    //    public void Build_WhenInnerJoinIsConfigured_ShouldGenerateInnerJoin()
    //    {
    //        var query = _provider
    //            .CreateQueryBuilder()
    //            .From<JoinUser>(alias: "u")
    //            .InnerJoin<JoinUser, JoinOrder>(
    //                alias: "o",
    //                on: (user, order) => user.Id == order.UserId)
    //            .Select<JoinUser>(user => new
    //            {
    //                UserId = user.Id,
    //                user.Email
    //            })
    //            .Build();

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(query.CommandText, Does.Contain("INNER JOIN"));
    //            Assert.That(query.CommandText, Does.Contain("users"));
    //            Assert.That(query.CommandText, Does.Contain("orders"));
    //            Assert.That(query.CommandText, Does.Contain("user_id"));
    //        });
    //    }

    //    /// <summary>
    //    /// Validates LEFT JOIN generation between strongly typed sources.
    //    /// </summary>
    //    [Test]
    //    public void Build_WhenLeftJoinIsConfigured_ShouldGenerateLeftJoin()
    //    {
    //        var query = _provider
    //            .CreateQueryBuilder()
    //            .From<JoinUser>(alias: "u")
    //            .InnerJoin<JoinUser, JoinOrder>(
    //                alias: "o",
    //                on: (user, order) => user.Id == order.UserId)
    //            .LeftJoin<JoinOrder, JoinOrderItem>(
    //                alias: "oi",
    //                on: (order, item) => order.Id == item.OrderId)
    //            .Select<JoinUser>(user => new
    //            {
    //                UserId = user.Id,
    //                user.Email
    //            })
    //            .Build();

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(query.CommandText, Does.Contain("INNER JOIN"));
    //            Assert.That(query.CommandText, Does.Contain("LEFT JOIN"));
    //            Assert.That(query.CommandText, Does.Contain("orders"));
    //            Assert.That(query.CommandText, Does.Contain("order_items"));
    //            Assert.That(query.CommandText, Does.Contain("order_id"));
    //        });
    //    }

    //    /// <summary>
    //    /// Validates JOIN generation using an explicitly configured table source.
    //    /// </summary>
    //    [Test]
    //    public void Build_WhenExplicitJoinTableIsConfigured_ShouldUseTableName()
    //    {
    //        var query = _provider
    //            .CreateQueryBuilder()
    //            .From<JoinUser>(alias: "u")
    //            .InnerJoinTable<JoinUser, JoinOrder>(
    //                tableName: "custom_orders",
    //                schemaName: null,
    //                alias: "o",
    //                on: (user, order) => user.Id == order.UserId)
    //            .Select<JoinUser>(user => new
    //            {
    //                user.Id
    //            })
    //            .Build();

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(query.CommandText, Does.Contain("INNER JOIN"));
    //            Assert.That(query.CommandText, Does.Contain("custom_orders"));
    //        });
    //    }

    //    /// <summary>
    //    /// Validates compound JOIN predicates using logical AND composition.
    //    /// </summary>
    //    [Test]
    //    public void Build_WhenJoinContainsAndPredicate_ShouldGenerateCompoundCondition()
    //    {
    //        var query = _provider
    //            .CreateQueryBuilder()
    //            .From<JoinOrder>(alias: "o")
    //            .LeftJoin<JoinOrder, JoinUser>(
    //                alias: "u",
    //                on: (order, user) =>
    //                    order.UserId == user.Id &&
    //                    order.TenantId == user.TenantId)
    //            .Select<JoinOrder>(order => new
    //            {
    //                order.Id,
    //                order.UserId
    //            })
    //            .Select<JoinUser>(user => new
    //            {
    //                UserEmail = user.Email
    //            })
    //            .Build();

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(query.CommandText, Does.Contain("LEFT JOIN"));
    //            Assert.That(query.CommandText, Does.Contain(" AND "));
    //            Assert.That(query.CommandText, Does.Contain("user_id"));
    //            Assert.That(query.CommandText, Does.Contain("TenantId"));
    //        });
    //    }

    //    /// <summary>
    //    /// Validates compound JOIN predicates using logical OR composition.
    //    /// </summary>
    //    [Test]
    //    public void Build_WhenJoinContainsOrPredicate_ShouldGenerateCompoundCondition()
    //    {
    //        var query = _provider
    //            .CreateQueryBuilder()
    //            .From<JoinOrder>(alias: "o")
    //            .LeftJoin<JoinOrder, JoinUser>(
    //                alias: "u",
    //                on: (order, user) =>
    //                    order.UserId == user.Id ||
    //                    order.ApproverUserId == user.Id)
    //            .Select<JoinOrder>(order => new
    //            {
    //                order.Id,
    //                order.UserId
    //            })
    //            .Select<JoinUser>(user => new
    //            {
    //                UserEmail = user.Email
    //            })
    //            .Build();

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(query.CommandText, Does.Contain("LEFT JOIN"));
    //            Assert.That(query.CommandText, Does.Contain(" OR "));
    //            Assert.That(query.CommandText, Does.Contain("user_id"));
    //        });
    //    }

    //    /// <summary>
    //    /// Validates JOIN generation when nullable and non-nullable
    //    /// properties participate in the same comparison.
    //    /// </summary>
    //    [Test]
    //    public void Build_WhenJoinUsesNullableProperty_ShouldGenerateValidComparison()
    //    {
    //        var query = _provider
    //            .CreateQueryBuilder()
    //            .From<Category>(alias: "c")
    //            .InnerJoin<Category, CategoryTree>(
    //                alias: "ct",
    //                on: (category, tree) => category.ParentId == tree.Id)
    //            .Select<Category>(category => new
    //            {
    //                category.Id,
    //                category.ParentId
    //            })
    //            .Build();

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(query.CommandText, Does.Contain("INNER JOIN"));
    //            Assert.That(query.CommandText, Does.Contain("ParentId"));
    //            Assert.That(query.CommandText, Does.Contain("Id"));
    //        });
    //    }
    //}
}
