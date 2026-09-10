using System.Linq.Expressions;
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

namespace TinyBlueWhale.EngineQuery.Tests.QueryBuilding.Select
{
    /// <summary>
    /// Validates provider-independent SELECT projection behavior.
    /// </summary>
    [TestFixture]
    internal sealed class SelectTests
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
        public void Build_WhenProjectionIsNotConfigured_ShouldGenerateSelectAll()
        {
            AssertSelectAll(_sqlServer);
            AssertSelectAll(_postgreSql);
            AssertSelectAll(_mySql);
        }

        [Test]
        public void Build_WhenSinglePropertyIsProjected_ShouldGenerateSelectedColumn()
        {
            AssertSingleProperty(_sqlServer);
            AssertSingleProperty(_postgreSql);
            AssertSingleProperty(_mySql);
        }

        [Test]
        public void Build_WhenMultiplePropertiesAreProjected_ShouldPreserveProjectionOrder()
        {
            AssertMultipleProperties(_sqlServer);
            AssertMultipleProperties(_postgreSql);
            AssertMultipleProperties(_mySql);
        }

        [Test]
        public void Build_WhenProjectionDefinesExplicitAlias_ShouldGenerateAlias()
        {
            AssertExplicitAlias(_sqlServer);
            AssertExplicitAlias(_postgreSql);
            AssertExplicitAlias(_mySql);
        }

        [Test]
        public void Build_WhenProjectionUsesPropertyName_ShouldNotGenerateRedundantAlias()
        {
            AssertImplicitPropertyName(_sqlServer);
            AssertImplicitPropertyName(_postgreSql);
            AssertImplicitPropertyName(_mySql);
        }

        [Test]
        public void Build_WhenSelectIsCalledMultipleTimes_ShouldAppendProjections()
        {
            AssertMultipleSelectCalls(_sqlServer);
            AssertMultipleSelectCalls(_postgreSql);
            AssertMultipleSelectCalls(_mySql);
        }

        [Test]
        public void Build_WhenProjectionTargetsJoinedEntity_ShouldUseJoinedSource()
        {
            AssertJoinedProjection(_sqlServer);
            AssertJoinedProjection(_postgreSql);
            AssertJoinedProjection(_mySql);
        }

        [Test]
        public void Build_WhenSameTypeSourcesUseAliasMatchedParameters_ShouldResolveExactSources()
        {
            AssertSameTypeAliasMatchedProjection(_sqlServer);
            AssertSameTypeAliasMatchedProjection(_postgreSql);
            AssertSameTypeAliasMatchedProjection(_mySql);
        }

        [Test]
        public void Build_WhenSameTypeSourcesUseSequentialProjection_ShouldAdvanceSources()
        {
            AssertSameTypeSequentialProjection(_sqlServer);
            AssertSameTypeSequentialProjection(_postgreSql);
            AssertSameTypeSequentialProjection(_mySql);
        }

        [Test]
        public void Select_WhenSelectorIsNull_ShouldThrowArgumentNullException()
        {
            AssertNullSelector(_sqlServer);
            AssertNullSelector(_postgreSql);
            AssertNullSelector(_mySql);
        }

        [Test]
        public void Select_WhenProjectionContainsUnsupportedExpression_ShouldThrowNotSupportedException()
        {
            AssertUnsupportedProjection(_sqlServer);
            AssertUnsupportedProjection(_postgreSql);
            AssertUnsupportedProjection(_mySql);
        }

        private static void AssertSelectAll<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<JoinUser>(alias: "u")
                .Build();

            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain("SELECT"));
                Assert.That(sql, Does.Contain("*"));
                Assert.That(sql, Does.Contain("FROM"));
                Assert.That(sql, Does.Contain("users"));
                Assert.That(query.Parameters, Is.Empty);
            });
        }

        private static void AssertSingleProperty<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(user => user.Email)
                .Build();

            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain("u.email"));
                Assert.That(sql, Does.Not.Contain("user_id"));
                Assert.That(query.Parameters, Is.Empty);
            });
        }

        private static void AssertMultipleProperties<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(user => new
                {
                    user.Id,
                    user.Email
                })
                .Build();

            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);
            var idIndex = sql.IndexOf("user_id", StringComparison.Ordinal);
            var emailIndex = sql.IndexOf("email", StringComparison.Ordinal);

            Assert.Multiple(() =>
            {
                Assert.That(idIndex, Is.GreaterThanOrEqualTo(0));
                Assert.That(emailIndex, Is.GreaterThan(idIndex));
                Assert.That(query.Parameters, Is.Empty);
            });
        }

        private static void AssertExplicitAlias<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(user => new
                {
                    UserId = user.Id
                })
                .Build();

            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain("u.user_id"));
                Assert.That(sql, Does.Contain("AS UserId"));
                Assert.That(query.Parameters, Is.Empty);
            });
        }

        private static void AssertImplicitPropertyName<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(user => new
                {
                    user.Id
                })
                .Build();

            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain("u.user_id"));
                Assert.That(sql, Does.Not.Contain("AS Id"));
                Assert.That(query.Parameters, Is.Empty);
            });
        }

        private static void AssertMultipleSelectCalls<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(user => user.Id)
                .Select<JoinUser>(user => user.Email)
                .Build();

            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);
            var idIndex = sql.IndexOf("user_id", StringComparison.Ordinal);
            var emailIndex = sql.IndexOf("email", StringComparison.Ordinal);

            Assert.Multiple(() =>
            {
                Assert.That(idIndex, Is.GreaterThanOrEqualTo(0));
                Assert.That(emailIndex, Is.GreaterThan(idIndex));
                Assert.That(query.Parameters, Is.Empty);
            });
        }

        private static void AssertJoinedProjection<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<JoinUser>(alias: "u")
                .InnerJoin<JoinUser, JoinOrder>(alias: "o", on: (user, order) => user.Id == order.UserId)
                .Select<JoinUser>(user => new
                {
                    UserId = user.Id
                })
                .Select<JoinOrder>(order => new
                {
                    OrderId = order.Id,
                    order.Total
                })
                .Build();

            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain("u.user_id"));
                Assert.That(sql, Does.Contain("o.order_id"));
                Assert.That(sql, Does.Contain("o.total"));
                Assert.That(query.Parameters, Is.Empty);
            });
        }

        private static void AssertSameTypeAliasMatchedProjection<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<Category>(alias: "parent")
                .InnerJoin<Category, Category>(alias: "child", on: (parent, child) => parent.Id == child.ParentId)
                .Select<Category>(parent => new
                {
                    ParentId = parent.Id
                })
                .Select<Category>(child => new
                {
                    ChildId = child.Id
                })
                .Build();

            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain("parent."));
                Assert.That(sql, Does.Contain("child."));
                Assert.That(sql, Does.Contain("AS ParentId"));
                Assert.That(sql, Does.Contain("AS ChildId"));
                Assert.That(query.Parameters, Is.Empty);
            });
        }

        private static void AssertSameTypeSequentialProjection<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<Category>(alias: "parent")
                .InnerJoin<Category, Category>(alias: "child", on: (parent, child) => parent.Id == child.ParentId)
                .Select<Category>(category => new
                {
                    FirstId = category.Id
                })
                .Select<Category>(category => new
                {
                    SecondId = category.Id
                })
                .Build();

            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);
            var parentIndex = sql.IndexOf("parent.", StringComparison.Ordinal);
            var childIndex = sql.IndexOf("child.", StringComparison.Ordinal);

            Assert.Multiple(() =>
            {
                Assert.That(parentIndex, Is.GreaterThanOrEqualTo(0));
                Assert.That(childIndex, Is.GreaterThan(parentIndex));
                Assert.That(sql, Does.Contain("AS FirstId"));
                Assert.That(sql, Does.Contain("AS SecondId"));
                Assert.That(query.Parameters, Is.Empty);
            });
        }

        private static void AssertNullSelector<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            Expression<Func<JoinUser, object>> selector = null!;

            var command = queryBuilder.From<JoinUser>(alias: "u");
            var exception = Assert.Throws<ArgumentNullException>(() => command.Select(selector));

            Assert.That(exception!.ParamName, Is.EqualTo("selector"));
        }

        private static void AssertUnsupportedProjection<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var command = queryBuilder.From<JoinUser>(alias: "u");

            Assert.Throws<NotSupportedException>(() => command.Select<JoinUser>(user => user.Email!.ToLower()));
        }
    }
}
