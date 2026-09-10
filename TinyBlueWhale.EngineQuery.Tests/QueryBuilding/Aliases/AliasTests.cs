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

namespace TinyBlueWhale.EngineQuery.Tests.QueryBuilding.Aliases
{
    /// <summary>
    /// Validates provider-independent query source alias behavior.
    /// </summary>
    [TestFixture]
    internal sealed class AliasTests
    {
        private QueryBuilder<SqlServer2012Profile> _sqlServer = null!;
        private QueryBuilder<PostgreSql93Profile> _postgreSql = null!;
        private QueryBuilder<MySql8031Profile> _mySql = null!;

        [SetUp]
        public void SetUp()
        {
            var metadataResolver = TestMetadataFactory.CreateMetadataResolver();

            _sqlServer = SqlServerQueryCompiler.Factory.Create<SqlServer2012Profile>(metadataResolver);
            _postgreSql = PostgreSqlQueryCompiler.Factory.Create<PostgreSql93Profile>(metadataResolver);
            _mySql = MySqlQueryCompiler.Factory.Create<MySql8031Profile>(metadataResolver);
        }

        [Test]
        public void From_WhenAliasIsNotProvided_ShouldNotGenerateAlias()
        {
            AssertFromWithoutAlias(_sqlServer);
            AssertFromWithoutAlias(_postgreSql);
            AssertFromWithoutAlias(_mySql);
        }

        [Test]
        public void From_WhenAliasIsProvided_ShouldUseMetadataTableAndExplicitAlias()
        {
            AssertFromWithAlias(_sqlServer);
            AssertFromWithAlias(_postgreSql);
            AssertFromWithAlias(_mySql);
        }

        [Test]
        public void From_WhenTableNameAndAliasAreProvided_ShouldPreserveBoth()
        {
            AssertFromWithTableNameAndAlias(_sqlServer);
            AssertFromWithTableNameAndAlias(_postgreSql);
            AssertFromWithTableNameAndAlias(_mySql);
        }

        [Test]
        public void InnerJoin_WhenAliasesAreNotProvided_ShouldGenerateDeterministicAliases()
        {
            AssertJoinWithoutAliases(_sqlServer);
            AssertJoinWithoutAliases(_postgreSql);
            AssertJoinWithoutAliases(_mySql);
        }

        [Test]
        public void InnerJoin_WhenAliasesAreProvided_ShouldPreserveExplicitAliases()
        {
            AssertJoinWithAliases(_sqlServer);
            AssertJoinWithAliases(_postgreSql);
            AssertJoinWithAliases(_mySql);
        }

        [Test]
        public void SelfJoin_WhenAliasesRepeat_ShouldGenerateDeterministicAliasFamily()
        {
            AssertRepeatedAliasFamily(_sqlServer);
            AssertRepeatedAliasFamily(_postgreSql);
            AssertRepeatedAliasFamily(_mySql);
        }

        [Test]
        public void SelfJoin_WhenLambdaAliasMatchesSource_ShouldResolveExactSource()
        {
            AssertExactSameTypeAliasResolution(_sqlServer);
            AssertExactSameTypeAliasResolution(_postgreSql);
            AssertExactSameTypeAliasResolution(_mySql);
        }

        [Test]
        public void WhereExists_WhenRootAliasIsNotProvided_ShouldGenerateDeterministicAlias()
        {
            AssertExistsRootAlias(_sqlServer);
            AssertExistsRootAlias(_postgreSql);
            AssertExistsRootAlias(_mySql);
        }

        [Test]
        public void WhereInSubquery_WhenRootAliasIsNotProvided_ShouldGenerateDeterministicAlias()
        {
            AssertInSubqueryRootAlias(_sqlServer);
            AssertInSubqueryRootAlias(_postgreSql);
            AssertInSubqueryRootAlias(_mySql);
        }

        [Test]
        public void Update_WhenAliasIsNotRequired_ShouldNotQualifyColumns()
        {
            AssertUpdateWithoutAlias(_sqlServer);
            AssertUpdateWithoutAlias(_postgreSql);
            AssertUpdateWithoutAlias(_mySql);
        }

        [Test]
        public void Delete_WhenAliasIsNotRequired_ShouldNotQualifyColumns()
        {
            AssertDeleteWithoutAlias(_sqlServer);
            AssertDeleteWithoutAlias(_postgreSql);
            AssertDeleteWithoutAlias(_mySql);
        }

        private static void AssertFromWithoutAlias<TProfile>(QueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<User>()
                .Select<User>(user => new
                {
                    user.Id,
                    user.Email
                })
                .Where<User>(user => user.Id == 1)
                .Build();

            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain("FROM Users"));
                Assert.That(sql, Does.Not.Contain("FROM Users AS"));
                Assert.That(sql, Does.Not.Contain("t0."));
                Assert.That(sql, Does.Not.Contain("Users.Id"));
                QueryAssertionHelper.AssertParameter(query.Parameters, 0, 1);
            });
        }

        private static void AssertFromWithAlias<TProfile>(QueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<User>(alias: "u")
                .Select<User>(u => new
                {
                    u.Id,
                    u.Email
                })
                .Where<User>(u => u.Id == 1)
                .Build();

            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain("FROM Users AS u"));
                Assert.That(sql, Does.Contain("u.Id"));
                Assert.That(sql, Does.Contain("u.Email"));
                QueryAssertionHelper.AssertParameter(query.Parameters, 0, 1);
            });
        }

        private static void AssertFromWithTableNameAndAlias<TProfile>(QueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<User>("custom_users", "u")
                .Select<User>(u => new
                {
                    u.Id,
                    u.Email
                })
                .Where<User>(u => u.Id == 1)
                .Build();

            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain("FROM custom_users AS u"));
                Assert.That(sql, Does.Contain("u.Id"));
                Assert.That(sql, Does.Contain("u.Email"));
                Assert.That(sql, Does.Not.Contain("FROM Users"));
                QueryAssertionHelper.AssertParameter(query.Parameters, 0, 1);
            });
        }

        private static void AssertJoinWithoutAliases<TProfile>(QueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<JoinUser>()
                .InnerJoin<JoinUser, JoinOrder>(
                    alias: null,
                    on: (user, order) => user.Id == order.UserId)
                .Select<JoinUser>(user => user.Id)
                .Select<JoinOrder>(order => order.Id)
                .Build();

            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain("FROM users AS t0"));
                Assert.That(sql, Does.Contain("INNER JOIN orders AS t1"));
                Assert.That(sql, Does.Contain("t0.user_id"));
                Assert.That(sql, Does.Contain("t1.order_id"));
                Assert.That(sql, Does.Contain("t0.user_id = t1.user_id"));
                Assert.That(query.Parameters, Is.Empty);
            });
        }

        private static void AssertJoinWithAliases<TProfile>(QueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<JoinUser>(alias: "u")
                .InnerJoin<JoinUser, JoinOrder>(
                    alias: "o",
                    on: (u, o) => u.Id == o.UserId)
                .Select<JoinUser>(u => u.Id)
                .Select<JoinOrder>(o => o.Id)
                .Build();

            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain("FROM users AS u"));
                Assert.That(sql, Does.Contain("INNER JOIN orders AS o"));
                Assert.That(sql, Does.Contain("u.user_id"));
                Assert.That(sql, Does.Contain("o.order_id"));
                Assert.That(sql, Does.Contain("u.user_id = o.user_id"));
                Assert.That(query.Parameters, Is.Empty);
            });
        }

        private static void AssertRepeatedAliasFamily<TProfile>(QueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<Category>(alias: "parent")
                .LeftJoin<Category, Category>(
                    alias: "parent",
                    on: (parent, parent1) => parent.ParentId == parent1.Id)
                .LeftJoin<Category, Category>(
                    alias: "parent",
                    on: (parent1, parent2) => parent1.ParentId == parent2.Id)
                .Select<Category>(parent => parent.Id)
                .Select<Category>(parent1 => parent1.Id)
                .Select<Category>(parent2 => parent2.Id)
                .Build();

            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain("AS parent"));
                Assert.That(sql, Does.Contain("AS parent1"));
                Assert.That(sql, Does.Contain("AS parent2"));
                Assert.That(sql, Does.Contain("parent.category_id"));
                Assert.That(sql, Does.Contain("parent1.category_id"));
                Assert.That(sql, Does.Contain("parent2.category_id"));
                Assert.That(query.Parameters, Is.Empty);
            });
        }

        private static void AssertExactSameTypeAliasResolution<TProfile>(QueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<Category>(alias: "category")
                .LeftJoin<Category, Category>(
                    alias: "parent",
                    on: (category, parent) => category.ParentId == parent.Id)
                .Select<Category>(parent => parent.Id)
                .Build();

            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain("parent.category_id"));
                Assert.That(sql, Does.Not.Contain("category.category_id AS"));
                Assert.That(query.Parameters, Is.Empty);
            });
        }

        private static void AssertExistsRootAlias<TProfile>(QueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<JoinUser>()
                .Select<JoinUser>(user => user.Id)
                .WhereExists<JoinUser, JoinOrder>(
                    alias: "o",
                    subquery => subquery
                        .WhereComputed<JoinOrder, JoinUser>(
                            (order, user) => order.UserId == user.Id))
                .Build();

            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain("FROM users AS t0"));
                Assert.That(sql, Does.Contain("EXISTS"));
                Assert.That(sql, Does.Contain("FROM orders AS o"));
                Assert.That(sql, Does.Contain("o.user_id = t0.user_id"));
                Assert.That(query.Parameters, Is.Empty);
            });
        }

        private static void AssertInSubqueryRootAlias<TProfile>(QueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<JoinUser>()
                .WhereIn<JoinUser, JoinOrder>(
                    user => user.Id,
                    alias: "o",
                    subquery => subquery
                        .Select<JoinOrder>(order => new
                        {
                            order.UserId
                        })
                        .WhereComputed<JoinOrder, JoinUser>(
                            (order, user) => order.UserId == user.Id))
                .Build();

            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain("FROM users AS t0"));
                Assert.That(sql, Does.Contain("t0.user_id IN"));
                Assert.That(sql, Does.Contain("FROM orders AS o"));
                Assert.That(sql, Does.Contain("o.user_id = t0.user_id"));
                Assert.That(query.Parameters, Is.Empty);
            });
        }

        private static void AssertUpdateWithoutAlias<TProfile>(QueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .Update<User>()
                .Set(user => user.Email, "updated@test.com")
                .Where(user => user.Id == 1)
                .Build();

            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain("UPDATE Users"));
                Assert.That(sql, Does.Contain("SET Email = @p0"));
                Assert.That(sql, Does.Contain("Id = @p1"));
                Assert.That(sql, Does.Not.Contain("Users.Email"));
                Assert.That(sql, Does.Not.Contain("Users.Id"));
                Assert.That(sql, Does.Not.Contain("t0."));
                QueryAssertionHelper.AssertParameter(query.Parameters, 0, "updated@test.com");
                QueryAssertionHelper.AssertParameter(query.Parameters, 1, 1);
            });
        }

        private static void AssertDeleteWithoutAlias<TProfile>(QueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .DeleteFrom<User>()
                .Where(user => user.Id == 1)
                .Build();

            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain("DELETE"));
                Assert.That(sql, Does.Contain("FROM Users"));
                Assert.That(sql, Does.Contain("Id = @p0"));
                Assert.That(sql, Does.Not.Contain("Users.Id"));
                Assert.That(sql, Does.Not.Contain("t0."));
                QueryAssertionHelper.AssertParameter(query.Parameters, 0, 1);
            });
        }
    }
}
