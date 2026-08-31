using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Tests.Models;
using TinyBlueWhale.EngineQuery.Tests.Providers;

namespace TinyBlueWhale.EngineQuery.Tests.QueryBuilding.Aggregates
{
    ///// <summary>
    ///// Validates provider-independent aggregate query behavior.
    ///// </summary>
    //[TestFixtureSource(typeof(QueryTestProviderSource), nameof(QueryTestProviderSource.GetProviders))]
    //internal sealed class AggregateTests(IQueryTestProvider provider)
    //{
    //    private readonly IQueryTestProvider _provider = provider ?? throw new ArgumentNullException(nameof(provider));

    //    /// <summary>
    //    /// Validates GROUP BY generation using an aggregate projection.
    //    /// </summary>
    //    [Test]
    //    public void Build_WhenGroupByAndAggregateAreConfigured_ShouldGenerateExpectedSql()
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
    //            .SelectAggregate<JoinOrder>(
    //                QueryAggregateFunction.Sum,
    //                order => order.Total,
    //                "TotalAmount")
    //            .SelectAggregate<JoinOrder>(
    //                QueryAggregateFunction.Count,
    //                order => order.Id,
    //                "OrderCount")
    //            .GroupBy<JoinUser>(user => new
    //            {
    //                user.Id,
    //                user.Email
    //            })
    //            .Build();

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(query.CommandText, Does.Contain("GROUP BY"));
    //            Assert.That(query.CommandText, Does.Contain("SUM"));
    //            Assert.That(query.CommandText, Does.Contain("COUNT"));
    //            Assert.That(query.CommandText, Does.Contain("TotalAmount"));
    //            Assert.That(query.CommandText, Does.Contain("OrderCount"));
    //        });
    //    }

    //    /// <summary>
    //    /// Validates HAVING generation over an aggregate expression.
    //    /// </summary>
    //    [Test]
    //    public void Build_WhenHavingAggregateIsConfigured_ShouldGenerateHavingClause()
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
    //            .SelectAggregate<JoinOrder>(
    //                QueryAggregateFunction.Sum,
    //                order => order.Total,
    //                "TotalAmount")
    //            .GroupBy<JoinUser>(user => new
    //            {
    //                user.Id,
    //                user.Email
    //            })
    //            .HavingAggregate<JoinOrder>(
    //                QueryAggregateFunction.Sum,
    //                order => order.Total,
    //                QueryComparisonOperator.GreaterThan,
    //                1000)
    //            .Build();

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(query.CommandText, Does.Contain("GROUP BY"));
    //            Assert.That(query.CommandText, Does.Contain("HAVING"));
    //            Assert.That(query.CommandText, Does.Contain("SUM"));
    //            Assert.That(query.Parameters, Has.Count.EqualTo(1));
    //            Assert.That(query.Parameters[0].Value, Is.EqualTo(1000));
    //        });
    //    }
    //}
}
