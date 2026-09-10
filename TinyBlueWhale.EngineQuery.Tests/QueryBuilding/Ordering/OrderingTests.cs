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

namespace TinyBlueWhale.EngineQuery.Tests.QueryBuilding.Ordering
{
    /// <summary>
    /// Validates provider-independent ordering behavior.
    /// </summary>
    [TestFixture]
    internal sealed class OrderingTests
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
        public void Build_WhenOrderByIsConfigured_ShouldGenerateAscendingOrdering()
        {
            AssertAscendingOrdering(_sqlServer);
            AssertAscendingOrdering(_postgreSql);
            AssertAscendingOrdering(_mySql);
        }

        [Test]
        public void Build_WhenMultipleOrderingsAreConfigured_ShouldPreserveOrderingSequence()
        {
            AssertMultipleOrderings(_sqlServer);
            AssertMultipleOrderings(_postgreSql);
            AssertMultipleOrderings(_mySql);
        }

        [Test]
        public void Build_WhenOrderingTargetsJoinedEntity_ShouldUseJoinedSource()
        {
            AssertJoinedSourceOrdering(_sqlServer);
            AssertJoinedSourceOrdering(_postgreSql);
            AssertJoinedSourceOrdering(_mySql);
        }

        [Test]
        public void Build_WhenOrderingFollowsWhere_ShouldGenerateClausesInStructuralOrder()
        {
            AssertClauseOrder(_sqlServer);
            AssertClauseOrder(_postgreSql);
            AssertClauseOrder(_mySql);
        }

        [Test]
        public void Build_WhenSameTypeJoinUsesMatchingAlias_ShouldOrderByJoinedSource()
        {
            AssertSameTypeJoinedSourceOrdering(_sqlServer);
            AssertSameTypeJoinedSourceOrdering(_postgreSql);
            AssertSameTypeJoinedSourceOrdering(_mySql);
        }

        private static void AssertAscendingOrdering<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<User>(alias: "u")
                .OrderBy<User>(user => user.Email)
                .Build();

            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain("ORDER BY"));
                Assert.That(sql, Does.Contain("Email"));
                Assert.That(sql, Does.Not.Contain("DESC"));
                Assert.That(query.Parameters, Is.Empty);
            });
        }

        private static void AssertMultipleOrderings<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<User>(alias: "u")
                .OrderBy<User>(user => user.Email)
                .ThenByDescending<User>(user => user.CreatedAt)
                .Build();

            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);
            var emailIndex = sql.IndexOf("Email", StringComparison.Ordinal);
            var createdAtIndex = sql.IndexOf("CreatedAt", StringComparison.Ordinal);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain("ORDER BY"));
                Assert.That(sql, Does.Contain("Email"));
                Assert.That(sql, Does.Contain("CreatedAt"));
                Assert.That(sql, Does.Contain("DESC"));
                Assert.That(emailIndex, Is.GreaterThanOrEqualTo(0));
                Assert.That(createdAtIndex, Is.GreaterThan(emailIndex));
                Assert.That(query.Parameters, Is.Empty);
            });
        }

        private static void AssertJoinedSourceOrdering<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<JoinUser>(alias: "u")
                .InnerJoin<JoinUser, JoinOrder>(alias: "o", on: (user, order) => user.Id == order.UserId)
                .OrderBy<JoinOrder>(order => order.Total)
                .Build();

            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain("ORDER BY"));
                Assert.That(sql, Does.Contain("o.total"));
                Assert.That(query.Parameters, Is.Empty);
            });
        }

        private static void AssertClauseOrder<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<JoinUser>(alias: "u")
                .Where<JoinUser>(user => user.IsActive)
                .OrderBy<JoinUser>(user => user.Email)
                .Build();

            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);
            var whereIndex = sql.IndexOf("WHERE", StringComparison.Ordinal);
            var orderByIndex = sql.IndexOf("ORDER BY", StringComparison.Ordinal);

            Assert.Multiple(() =>
            {
                Assert.That(whereIndex, Is.GreaterThanOrEqualTo(0));
                Assert.That(orderByIndex, Is.GreaterThan(whereIndex));
                QueryAssertionHelper.AssertParameter(query.Parameters, 0, true);
            });
        }

        private static void AssertSameTypeJoinedSourceOrdering<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<Category>(alias: "parent")
                .InnerJoin<Category, Category>(alias: "child", on: (parent, child) => parent.Id == child.ParentId)
                .OrderBy<Category>(child => child.Id)
                .Build();

            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain("ORDER BY"));
                Assert.That(sql, Does.Contain("child"));
                Assert.That(query.Parameters, Is.Empty);
            });
        }
    }
}
