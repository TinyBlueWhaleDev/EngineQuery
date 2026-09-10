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

namespace TinyBlueWhale.EngineQuery.Tests.QueryBuilding.Join
{
    /// <summary>
    /// Validates provider-independent JOIN query behavior.
    /// </summary>
    [TestFixture]
    internal sealed class JoinTests
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
        public void Build_WhenInnerJoinIsConfigured_ShouldGenerateInnerJoin()
        {
            AssertInnerJoin(_sqlServer);
            AssertInnerJoin(_postgreSql);
            AssertInnerJoin(_mySql);
        }

        [Test]
        public void Build_WhenLeftJoinIsConfigured_ShouldGenerateLeftJoin()
        {
            AssertLeftJoin(_sqlServer);
            AssertLeftJoin(_postgreSql);
            AssertLeftJoin(_mySql);
        }

        [Test]
        public void Build_WhenExplicitJoinTableIsConfigured_ShouldUseTableName()
        {
            AssertExplicitJoinTable(_sqlServer);
            AssertExplicitJoinTable(_postgreSql);
            AssertExplicitJoinTable(_mySql);
        }

        [Test]
        public void Build_WhenJoinContainsAndPredicate_ShouldGenerateCompoundCondition()
        {
            AssertAndPredicate(_sqlServer);
            AssertAndPredicate(_postgreSql);
            AssertAndPredicate(_mySql);
        }

        [Test]
        public void Build_WhenJoinContainsOrPredicate_ShouldGenerateCompoundCondition()
        {
            AssertOrPredicate(_sqlServer);
            AssertOrPredicate(_postgreSql);
            AssertOrPredicate(_mySql);
        }

        [Test]
        public void Build_WhenJoinUsesNullableProperty_ShouldGenerateValidComparison()
        {
            AssertNullableProperty(_sqlServer);
            AssertNullableProperty(_postgreSql);
            AssertNullableProperty(_mySql);
        }

        private static void AssertInnerJoin<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<JoinUser>(alias: "u")
                .InnerJoin<JoinUser, JoinOrder>(alias: "o", on: (user, order) => user.Id == order.UserId)
                .Select<JoinUser>(user => new
                {
                    UserId = user.Id,
                    user.Email
                })
                .Build();

            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain("INNER JOIN"));
                Assert.That(sql, Does.Contain("users"));
                Assert.That(sql, Does.Contain("orders"));
                Assert.That(sql, Does.Contain("user_id"));
                Assert.That(query.Parameters, Is.Empty);
            });
        }

        private static void AssertLeftJoin<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<JoinUser>(alias: "u")
                .InnerJoin<JoinUser, JoinOrder>(alias: "o", on: (user, order) => user.Id == order.UserId)
                .LeftJoin<JoinOrder, JoinOrderItem>(alias: "oi", on: (order, item) => order.Id == item.OrderId)
                .Select<JoinUser>(user => new
                {
                    UserId = user.Id,
                    user.Email
                })
                .Build();

            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain("INNER JOIN"));
                Assert.That(sql, Does.Contain("LEFT JOIN"));
                Assert.That(sql, Does.Contain("orders"));
                Assert.That(sql, Does.Contain("order_items"));
                Assert.That(sql, Does.Contain("order_id"));
                Assert.That(query.Parameters, Is.Empty);
            });
        }

        private static void AssertExplicitJoinTable<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<JoinUser>(alias: "u")
                .InnerJoinTable<JoinUser, JoinOrder>(tableName: "custom_orders", schemaName: null, alias: "o", on: (user, order) => user.Id == order.UserId)
                .Select<JoinUser>(user => new
                {
                    user.Id
                })
                .Build();

            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain("INNER JOIN"));
                Assert.That(sql, Does.Contain("custom_orders"));
                Assert.That(query.Parameters, Is.Empty);
            });
        }

        private static void AssertAndPredicate<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<JoinOrder>(alias: "o")
                .LeftJoin<JoinOrder, JoinUser>(alias: "u", on: (order, user) => order.UserId == user.Id && order.TenantId == user.TenantId)
                .Select<JoinOrder>(order => new
                {
                    order.Id,
                    order.UserId
                })
                .Select<JoinUser>(user => new
                {
                    UserEmail = user.Email
                })
                .Build();

            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain("LEFT JOIN"));
                Assert.That(sql, Does.Contain(" AND "));
                Assert.That(sql, Does.Contain("user_id"));
                Assert.That(sql, Does.Contain("TenantId"));
                Assert.That(query.Parameters, Is.Empty);
            });
        }

        private static void AssertOrPredicate<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<JoinOrder>(alias: "o")
                .LeftJoin<JoinOrder, JoinUser>(alias: "u", on: (order, user) => order.UserId == user.Id || order.ApproverUserId == user.Id)
                .Select<JoinOrder>(order => new
                {
                    order.Id,
                    order.UserId
                })
                .Select<JoinUser>(user => new
                {
                    UserEmail = user.Email
                })
                .Build();

            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain("LEFT JOIN"));
                Assert.That(sql, Does.Contain(" OR "));
                Assert.That(sql, Does.Contain("user_id"));
                Assert.That(query.Parameters, Is.Empty);
            });
        }

        private static void AssertNullableProperty<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<Category>(alias: "c")
                .InnerJoin<Category, CategoryTree>(alias: "ct", on: (category, tree) => category.ParentId == tree.Id)
                .Select<Category>(category => new
                {
                    category.Id,
                    category.ParentId
                })
                .Build();

            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain("INNER JOIN category_tree AS ct"));
                Assert.That(sql, Does.Contain("c.category_id"));
                Assert.That(sql, Does.Contain("c.parent_category_id"));
                Assert.That(sql, Does.Contain("c.parent_category_id = ct.Id"));
                Assert.That(query.Parameters, Is.Empty);
            });
        }
    }
}
