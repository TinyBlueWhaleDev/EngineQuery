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

namespace TinyBlueWhale.EngineQuery.Tests.QueryBuilding.Insert
{
    /// <summary>
    /// Validates provider-independent INSERT VALUES behavior.
    /// </summary>
    [TestFixture]
    internal sealed class InsertTests
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
        public void Build_WhenInsertIsValid_ShouldGenerateExpectedSql()
        {
            AssertValidInsert(_sqlServer);
            AssertValidInsert(_postgreSql);
            AssertValidInsert(_mySql);
        }

        [Test]
        public void Build_WhenMultipleValuesAreConfigured_ShouldPreserveAssignmentOrder()
        {
            AssertAssignmentOrder(_sqlServer);
            AssertAssignmentOrder(_postgreSql);
            AssertAssignmentOrder(_mySql);
        }

        [Test]
        public void Build_WhenNullableValueIsNull_ShouldGenerateNullParameter()
        {
            AssertNullValue(_sqlServer);
            AssertNullValue(_postgreSql);
            AssertNullValue(_mySql);
        }

        [Test]
        public void Build_WhenExplicitTableNameIsProvided_ShouldUseTableName()
        {
            AssertExplicitTableName(_sqlServer);
            AssertExplicitTableName(_postgreSql);
            AssertExplicitTableName(_mySql);
        }

        [Test]
        public void InsertInto_WhenTableNameIsNull_ShouldThrowArgumentNullException()
        {
            AssertNullTableName(_sqlServer);
            AssertNullTableName(_postgreSql);
            AssertNullTableName(_mySql);
        }

        [Test]
        public void InsertInto_WhenTableNameIsEmptyOrWhitespace_ShouldThrowArgumentException()
        {
            AssertInvalidTableName(_sqlServer);
            AssertInvalidTableName(_postgreSql);
            AssertInvalidTableName(_mySql);
        }

        [Test]
        public void Set_WhenSelectorIsNull_ShouldThrowArgumentNullException()
        {
            AssertNullSelector(_sqlServer);
            AssertNullSelector(_postgreSql);
            AssertNullSelector(_mySql);
        }

        [Test]
        public void Set_WhenSelectorIsNotDirectProperty_ShouldThrowArgumentException()
        {
            AssertInvalidSelector(_sqlServer);
            AssertInvalidSelector(_postgreSql);
            AssertInvalidSelector(_mySql);
        }

        [Test]
        public void Set_WhenPropertyIsAssignedMoreThanOnce_ShouldThrowInvalidOperationException()
        {
            AssertDuplicateAssignment(_sqlServer);
            AssertDuplicateAssignment(_postgreSql);
            AssertDuplicateAssignment(_mySql);
        }

        private static void AssertValidInsert<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .InsertInto<User>()
                .Set(user => user.Email, "admin@test.com")
                .Set(user => user.Age, 35)
                .Set(user => user.IsActive, true)
                .Build();

            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain("INSERT"));
                Assert.That(sql, Does.Contain("Users"));
                Assert.That(sql, Does.Contain("Email"));
                Assert.That(sql, Does.Contain("Age"));
                Assert.That(sql, Does.Contain("IsActive"));
                Assert.That(sql, Does.Contain("VALUES"));
                Assert.That(query.Parameters, Has.Count.EqualTo(3));
                QueryAssertionHelper.AssertParameter(query.Parameters, 0, "admin@test.com");
                QueryAssertionHelper.AssertParameter(query.Parameters, 1, 35);
                QueryAssertionHelper.AssertParameter(query.Parameters, 2, true);
            });
        }

        private static void AssertAssignmentOrder<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .InsertInto<User>()
                .Set(user => user.Email, "admin@test.com")
                .Set(user => user.Age, 35)
                .Set(user => user.IsActive, true)
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(query.Parameters, Has.Count.EqualTo(3));
                QueryAssertionHelper.AssertParameter(query.Parameters, 0, "@p0", "admin@test.com");
                QueryAssertionHelper.AssertParameter(query.Parameters, 1, "@p1", 35);
                QueryAssertionHelper.AssertParameter(query.Parameters, 2, "@p2", true);
            });
        }

        private static void AssertNullValue<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .InsertInto<Category>()
                .Set(category => category.ParentId, null)
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(query.Parameters, Has.Count.EqualTo(1));
                Assert.That(query.Parameters[0].Name, Is.EqualTo("@p0"));
                Assert.That(query.Parameters[0].Value, Is.Null);
            });
        }

        private static void AssertExplicitTableName<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .InsertInto<User>("CustomUsers")
                .Set(user => user.Email, "admin@test.com")
                .Build();

            Assert.That(QueryAssertionHelper.NormalizeSql(query.CommandText), Does.Contain("CustomUsers"));
        }

        private static void AssertNullTableName<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var exception = Assert.Throws<ArgumentNullException>(() => queryBuilder.InsertInto<User>(null!));

            Assert.That(exception!.ParamName, Is.EqualTo("tableName"));
        }

        private static void AssertInvalidTableName<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var emptyException = Assert.Throws<ArgumentException>(() => queryBuilder.InsertInto<User>(string.Empty));
            var whitespaceException = Assert.Throws<ArgumentException>(() => queryBuilder.InsertInto<User>(" "));

            Assert.Multiple(() =>
            {
                Assert.That(emptyException!.ParamName, Is.EqualTo("tableName"));
                Assert.That(whitespaceException!.ParamName, Is.EqualTo("tableName"));
            });
        }

        private static void AssertNullSelector<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            Expression<Func<User, string?>> selector = null!;

            var command = queryBuilder.InsertInto<User>();
            var exception = Assert.Throws<ArgumentNullException>(() => command.Set(selector, "admin@test.com"));

            Assert.That(exception!.ParamName, Is.EqualTo("selector"));
        }

        private static void AssertInvalidSelector<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var command = queryBuilder.InsertInto<User>();

            var exception = Assert.Throws<ArgumentException>(() => command.Set(user => user.Email!.ToLower(), "admin@test.com"));

            Assert.That(exception!.ParamName, Is.EqualTo("selector"));
        }

        private static void AssertDuplicateAssignment<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var command = queryBuilder
                .InsertInto<User>()
                .Set(user => user.Email, "first@test.com");

            var exception = Assert.Throws<InvalidOperationException>(() => command.Set(user => user.Email, "second@test.com"));

            Assert.That(exception, Is.Not.Null);
        }
    }
}
