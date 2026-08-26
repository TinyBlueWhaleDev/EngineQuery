using TinyBlueWhale.EngineQuery.Tests.Infrastructure;
using TinyBlueWhale.EngineQuery.Tests.Models;
using TinyBlueWhale.EngineQuery.Tests.Providers;

namespace TinyBlueWhale.EngineQuery.Tests.QueryBuilding.Apply
{
    /// <summary>
    /// Validates provider-independent APPLY and lateral query behavior.
    /// </summary>
    [TestFixtureSource(typeof(QueryTestProviderSource), nameof(QueryTestProviderSource.GetProviders))]
    internal sealed class ApplyTests(IQueryTestProvider provider)
    {
        private readonly IQueryTestProvider _provider = provider ?? throw new ArgumentNullException(nameof(provider));

        /// <summary>
        /// Validates correlated APPLY generation using a nested query source.
        /// </summary>
        [Test]
        public void Build_WhenCrossApplyIsConfigured_ShouldGenerateApplySource()
        {
            var query = _provider
                .CreateQueryBuilder()
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(user => new
                {
                    UserId = user.Id,
                    user.Email
                })
                .CrossApply<JoinUser, JoinOrder>(
                    alias: "latest_order",
                    apply => apply
                        .Select<JoinOrder>(order => new
                        {
                            OrderId = order.Id,
                            order.UserId,
                            order.Total
                        })
                        .WhereComputed<JoinOrder, JoinUser>((order, user) =>
                            order.UserId == user.Id)
                        .OrderByDescending<JoinOrder>(order => order.Total)
                        .Take(1))
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(query.CommandText, Does.Contain("latest_order"));
                Assert.That(query.CommandText, Does.Contain("orders"));
                Assert.That(query.CommandText, Does.Contain("user_id"));
                Assert.That(query.CommandText, Does.Contain("total"));
            });
        }

        /// <summary>
        /// Validates correlated OUTER APPLY generation.
        /// </summary>
        [Test]
        public void Build_WhenOuterApplyIsConfigured_ShouldGenerateApplySource()
        {
            var query = _provider
                .CreateQueryBuilder()
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(user => new
                {
                    UserId = user.Id,
                    user.Email
                })
                .OuterApply<JoinUser, JoinOrder>(
                    alias: "latest_order",
                    apply => apply
                        .Select<JoinOrder>(order => new
                        {
                            OrderId = order.Id,
                            order.UserId,
                            order.Total
                        })
                        .WhereComputed<JoinOrder, JoinUser>((order, user) =>
                            order.UserId == user.Id)
                        .OrderByDescending<JoinOrder>(order => order.Total)
                        .Take(1))
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(query.CommandText, Does.Contain("latest_order"));
                Assert.That(query.CommandText, Does.Contain("orders"));
                Assert.That(query.CommandText, Does.Contain("user_id"));
            });
        }

        /// <summary>
        /// Validates that APPLY or LATERAL joins cannot be compiled when
        /// the current provider does not support them.
        /// </summary>
        [Test]
        public void Build_WhenLateralJoinsAreNotSupported_ShouldThrow()
        {
            var builder = _provider.CreateQueryBuilder(
                new UnsupportedLateralJoinCapabilities());

            var exception = Assert.Throws<NotSupportedException>(() =>
                builder
                    .From<JoinUser>(alias: "u")
                    .CrossApply<JoinUser, JoinOrder>(
                        alias: "latest_order",
                        apply => apply
                            .Select<JoinOrder>(order => new
                            {
                                OrderId = order.Id
                            })
                            .WhereComputed<JoinOrder, JoinUser>((order, user) =>
                                order.UserId == user.Id))
                    .Build());

            Assert.That(
                exception!.Message,
                Is.EqualTo("APPLY or LATERAL joins are not supported by the current provider."));
        }
    }
}
