using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding;
using TinyBlueWhale.EngineQuery.MySql.Compilation;
using TinyBlueWhale.EngineQuery.MySql.Profiles;
using TinyBlueWhale.EngineQuery.PostgreSql.Compilation;
using TinyBlueWhale.EngineQuery.PostgreSql.Profiles;
using TinyBlueWhale.EngineQuery.SqlServer.Compilation;
using TinyBlueWhale.EngineQuery.SqlServer.Profiles;
using TinyBlueWhale.EngineQuery.Tests.Helpers;
using TinyBlueWhale.EngineQuery.Tests.Infrastructure;
using TinyBlueWhale.EngineQuery.Tests.Models;

namespace TinyBlueWhale.EngineQuery.Tests.QueryBuilding.Subqueries
{
    /// <summary>
    /// Validates provider-independent subquery behavior.
    /// </summary>
    [TestFixture]
    internal sealed class SubqueryTests
    {
        private QueryBuilder<SqlServer2012Profile> _sqlServer = null!;
        private QueryBuilder<PostgreSql93Profile> _postgreSql = null!;
        private QueryBuilder<MySql8031Profile> _mySql = null!;

        [SetUp]
        public void SetUp()
        {
            var metadataResolver = TestMetadataFactory.CreateMetadataResolver();

            _sqlServer = SqlServerQueryCompiler.Factory
                .Create<SqlServer2012Profile>(metadataResolver);

            _postgreSql = PostgreSqlQueryCompiler.Factory
                .Create<PostgreSql93Profile>(metadataResolver);

            _mySql = MySqlQueryCompiler.Factory
                .Create<MySql8031Profile>(metadataResolver);
        }

        [Test]
        public void Build_WhenExistsIsConfigured_ShouldGenerateExistsPredicate()
        {
            AssertExists(_sqlServer);
            AssertExists(_postgreSql);
            AssertExists(_mySql);
        }

        [Test]
        public void Build_WhenNotExistsIsConfigured_ShouldGenerateNotExistsPredicate()
        {
            AssertNotExists(_sqlServer);
            AssertNotExists(_postgreSql);
            AssertNotExists(_mySql);
        }

        [Test]
        public void Build_WhenInSubqueryIsConfigured_ShouldGenerateInPredicate()
        {
            AssertInSubquery(_sqlServer);
            AssertInSubquery(_postgreSql);
            AssertInSubquery(_mySql);
        }

        [Test]
        public void Build_WhenCorrelatedInSubqueryIsConfigured_ShouldResolveOuterSource()
        {
            AssertCorrelatedInSubquery(_sqlServer);
            AssertCorrelatedInSubquery(_postgreSql);
            AssertCorrelatedInSubquery(_mySql);
        }

        [Test]
        public void Build_WhenMultipleSubqueryPredicatesAreConfigured_ShouldPreserveAllPredicatesAndParameters()
        {
            AssertMultipleSubqueries(_sqlServer);
            AssertMultipleSubqueries(_postgreSql);
            AssertMultipleSubqueries(_mySql);
        }

        [Test]
        public void Build_WhenDerivedTableIsConfigured_ShouldGenerateDerivedSource()
        {
            AssertDerivedTable(_sqlServer);
            AssertDerivedTable(_postgreSql);
            AssertDerivedTable(_mySql);
        }

        [Test]
        public void Build_WhenDerivedTableContainsParameters_ShouldPreserveParameterOrder()
        {
            AssertDerivedTableParameters(_sqlServer);
            AssertDerivedTableParameters(_postgreSql);
            AssertDerivedTableParameters(_mySql);
        }

        private static void AssertExists<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(user => new
                {
                    UserId = user.Id,
                    user.Email
                })
                .WhereExists<JoinUser, JoinOrder>(
                    alias: "o",
                    subquery => subquery
                        .WhereComputed<JoinOrder, JoinUser>((order, user) =>
                            order.UserId == user.Id &&
                            order.Total > 100))
                .Build();

            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain("EXISTS"));
                Assert.That(sql, Does.Contain("orders"));
                Assert.That(sql, Does.Contain("o.user_id"));
                Assert.That(sql, Does.Contain("u.user_id"));
                Assert.That(query.Parameters, Has.Count.EqualTo(1));
                QueryAssertionHelper.AssertParameter(query.Parameters, 0, 100m);
            });
        }

        private static void AssertNotExists<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<JoinUser>(alias: "u")
                .WhereNotExists<JoinUser, JoinOrder>(
                    alias: "o",
                    subquery => subquery
                        .WhereComputed<JoinOrder, JoinUser>((order, user) =>
                            order.UserId == user.Id &&
                            order.Total <= 0))
                .Build();

            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain("NOT EXISTS"));
                Assert.That(sql, Does.Contain("orders"));
                Assert.That(sql, Does.Contain("o.user_id"));
                Assert.That(sql, Does.Contain("u.user_id"));
                Assert.That(query.Parameters, Has.Count.EqualTo(1));
                QueryAssertionHelper.AssertParameter(query.Parameters, 0, 0m);
            });
        }

        private static void AssertInSubquery<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<JoinUser>(alias: "u")
                .WhereIn<JoinUser, JoinOrder>(
                    user => user.Id,
                    alias: "o",
                    subquery => subquery
                        .Select<JoinOrder>(order => new
                        {
                            order.UserId
                        })
                        .Where<JoinOrder>(order => order.Total > 500))
                .Build();

            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain(" IN "));
                Assert.That(sql, Does.Contain("SELECT"));
                Assert.That(sql, Does.Contain("orders"));
                Assert.That(sql, Does.Contain("user_id"));
                Assert.That(query.Parameters, Has.Count.EqualTo(1));
                QueryAssertionHelper.AssertParameter(query.Parameters, 0, 500m);
            });
        }

        private static void AssertCorrelatedInSubquery<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(user => new
                {
                    UserId = user.Id,
                    user.Email
                })
                .WhereIn<JoinUser, JoinOrder>(
                    user => user.Id,
                    alias: "o",
                    subquery => subquery
                        .Select<JoinOrder>(order => new
                        {
                            order.UserId
                        })
                        .WhereComputed<JoinOrder, JoinUser>((order, user) =>
                            order.UserId == user.Id &&
                            order.Total > 100))
                .Build();

            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain(" IN "));
                Assert.That(sql, Does.Contain("orders"));
                Assert.That(sql, Does.Contain("o.user_id"));
                Assert.That(sql, Does.Contain("u.user_id"));
                Assert.That(query.Parameters, Has.Count.EqualTo(1));
                QueryAssertionHelper.AssertParameter(query.Parameters, 0, 100m);
            });
        }

        private static void AssertMultipleSubqueries<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(user => new
                {
                    UserId = user.Id,
                    user.Email
                })
                .WhereExists<JoinUser, JoinOrder>(
                    alias: "o",
                    subquery => subquery
                        .WhereComputed<JoinOrder, JoinUser>((order, user) =>
                            order.UserId == user.Id &&
                            order.Total > 100))
                .WhereNotExists<JoinUser, JoinOrder>(
                    alias: "o2",
                    subquery => subquery
                        .WhereComputed<JoinOrder, JoinUser>((order, user) =>
                            order.UserId == user.Id &&
                            order.Total <= 0))
                .WhereIn<JoinUser, JoinOrder>(
                    user => user.Id,
                    alias: "oi",
                    subquery => subquery
                        .Select<JoinOrder>(order => new
                        {
                            order.UserId
                        })
                        .Where<JoinOrder>(order => order.Total > 500))
                .Build();

            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);
            var existsIndex = sql.IndexOf("EXISTS", StringComparison.Ordinal);
            var notExistsIndex = sql.IndexOf("NOT EXISTS", StringComparison.Ordinal);
            var inIndex = sql.LastIndexOf(" IN ", StringComparison.Ordinal);

            Assert.Multiple(() =>
            {
                Assert.That(existsIndex, Is.GreaterThanOrEqualTo(0));
                Assert.That(notExistsIndex, Is.GreaterThan(existsIndex));
                Assert.That(inIndex, Is.GreaterThan(notExistsIndex));
                Assert.That(query.Parameters, Has.Count.EqualTo(3));
                QueryAssertionHelper.AssertParameter(query.Parameters, 0, 100m);
                QueryAssertionHelper.AssertParameter(query.Parameters, 1, 0m);
                QueryAssertionHelper.AssertParameter(query.Parameters, 2, 500m);
            });
        }

        private static void AssertDerivedTable<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .FromSubquery<OrderSummary, JoinOrder>(
                    alias: "summary",
                    subquery => subquery
                        .From<JoinOrder>(alias: "o")
                        .Select<JoinOrder>(order => new
                        {
                            UserId = order.UserId
                        })
                        .SelectAggregate<JoinOrder>(QueryAggregateFunction.Sum, order => order.Total, "TotalAmount")
                        .SelectAggregate<JoinOrder>(QueryAggregateFunction.Count, order => order.Id, "OrderCount")
                        .GroupBy<JoinOrder>(order => order.UserId))
                .Select<OrderSummary>(summary => new
                {
                    summary.UserId,
                    summary.TotalAmount,
                    summary.OrderCount
                })
                .Build();

            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain("SELECT"));
                Assert.That(sql, Does.Contain("orders"));
                Assert.That(sql, Does.Contain("summary"));
                Assert.That(sql, Does.Contain("TotalAmount"));
                Assert.That(sql, Does.Contain("OrderCount"));
                Assert.That(sql, Does.Contain("GROUP BY"));
                Assert.That(query.Parameters, Is.Empty);
            });
        }

        private static void AssertDerivedTableParameters<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .FromSubquery<OrderSummary, JoinOrder>(
                    alias: "summary",
                    subquery => subquery
                        .From<JoinOrder>(alias: "o")
                        .Select<JoinOrder>(order => new
                        {
                            UserId = order.UserId
                        })
                        .SelectAggregate<JoinOrder>(QueryAggregateFunction.Sum, order => order.Total, "TotalAmount")
                        .SelectAggregate<JoinOrder>(QueryAggregateFunction.Count, order => order.Id, "OrderCount")
                        .Where<JoinOrder>(order => order.Total > 100)
                        .GroupBy<JoinOrder>(order => order.UserId))
                .Select<OrderSummary>(summary => new
                {
                    summary.UserId,
                    summary.TotalAmount,
                    summary.OrderCount
                })
                .WhereComputed<OrderSummary>(summary => summary.TotalAmount > 500)
                .Build();

            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain("summary"));
                Assert.That(sql, Does.Contain("@p0"));
                Assert.That(sql, Does.Contain("@p1"));
                Assert.That(query.Parameters, Has.Count.EqualTo(2));
                QueryAssertionHelper.AssertParameter(query.Parameters, 0, 100m);
                QueryAssertionHelper.AssertParameter(query.Parameters, 1, 500m);
            });
        }
    }
}
