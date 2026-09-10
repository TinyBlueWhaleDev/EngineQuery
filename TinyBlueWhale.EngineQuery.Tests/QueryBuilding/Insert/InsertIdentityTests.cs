using System.Linq.Expressions;
using TinyBlueWhale.EngineQuery.Abstractions.Extensions;
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
    /// Validates provider-specific INSERT identity retrieval behavior.
    /// </summary>
    [TestFixture]
    internal sealed class InsertIdentityTests
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
        public void Build_WhenIdentityRetrievalIsConfigured_ShouldGenerateProviderSpecificIdentitySql()
        {
            var sqlServer = _sqlServer
                .InsertInto<JoinUser>()
                .Set(user => user.Email, "admin@test.com")
                .ReturnIdentity()
                .Build();

            var postgreSql = _postgreSql
                .InsertInto<JoinUser>()
                .Set(user => user.Email, "admin@test.com")
                .ReturnIdentity(user => user.Id)
                .Build();

            var mySql = _mySql
                .InsertInto<JoinUser>()
                .Set(user => user.Email, "admin@test.com")
                .ReturnIdentity()
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(QueryAssertionHelper.NormalizeSql(sqlServer.CommandText), Does.Contain("INSERT"));
                Assert.That(QueryAssertionHelper.NormalizeSql(sqlServer.CommandText), Does.Contain("users"));
                Assert.That(QueryAssertionHelper.NormalizeSql(sqlServer.CommandText), Does.Contain("email"));
                Assert.That(sqlServer.CommandText, Does.Contain("IDENTITY").IgnoreCase);
                QueryAssertionHelper.AssertParameter(sqlServer.Parameters, 0, "admin@test.com");

                Assert.That(QueryAssertionHelper.NormalizeSql(postgreSql.CommandText), Does.Contain("INSERT"));
                Assert.That(QueryAssertionHelper.NormalizeSql(postgreSql.CommandText), Does.Contain("users"));
                Assert.That(QueryAssertionHelper.NormalizeSql(postgreSql.CommandText), Does.Contain("email"));
                Assert.That(postgreSql.CommandText, Does.Contain("RETURNING").IgnoreCase);
                Assert.That(QueryAssertionHelper.NormalizeSql(postgreSql.CommandText), Does.Contain("user_id"));
                QueryAssertionHelper.AssertParameter(postgreSql.Parameters, 0, "admin@test.com");

                Assert.That(QueryAssertionHelper.NormalizeSql(mySql.CommandText), Does.Contain("INSERT"));
                Assert.That(QueryAssertionHelper.NormalizeSql(mySql.CommandText), Does.Contain("users"));
                Assert.That(QueryAssertionHelper.NormalizeSql(mySql.CommandText), Does.Contain("email"));
                Assert.That(mySql.CommandText, Does.Contain("LAST_INSERT_ID").IgnoreCase);
                QueryAssertionHelper.AssertParameter(mySql.Parameters, 0, "admin@test.com");
            });
        }

        [Test]
        public void ReturnIdentity_WhenAlreadyConfigured_ShouldThrowInvalidOperationException()
        {
            var sqlServerCommand = _sqlServer
                .InsertInto<User>()
                .Set(user => user.Email, "admin@test.com")
                .ReturnIdentity();

            var postgreSqlCommand = _postgreSql
                .InsertInto<User>()
                .Set(user => user.Email, "admin@test.com")
                .ReturnIdentity(user => user.Id);

            var mySqlCommand = _mySql
                .InsertInto<User>()
                .Set(user => user.Email, "admin@test.com")
                .ReturnIdentity();

            var sqlServerException = Assert.Throws<InvalidOperationException>(
                () => sqlServerCommand.ReturnIdentity());

            var postgreSqlException = Assert.Throws<InvalidOperationException>(
                () => postgreSqlCommand.ReturnIdentity(user => user.Id));

            var mySqlException = Assert.Throws<InvalidOperationException>(
                () => mySqlCommand.ReturnIdentity());

            Assert.Multiple(() =>
            {
                Assert.That(
                    sqlServerException!.Message,
                    Is.EqualTo("Identity retrieval is already configured for the current INSERT command."));

                Assert.That(
                    postgreSqlException!.Message,
                    Is.EqualTo("Identity retrieval is already configured for the current INSERT command."));

                Assert.That(
                    mySqlException!.Message,
                    Is.EqualTo("Identity retrieval is already configured for the current INSERT command."));
            });
        }

        [Test]
        public void ReturnIdentity_WhenPostgreSqlSelectorIsNull_ShouldThrowArgumentNullException()
        {
            System.Linq.Expressions.Expression<Func<User, int>> selector = null!;

            var command = _postgreSql
                .InsertInto<User>()
                .Set(user => user.Email, "admin@test.com");

            var exception = Assert.Throws<ArgumentNullException>(
                () => command.ReturnIdentity(selector));

            Assert.That(exception!.ParamName, Is.EqualTo("identitySelector"));
        }

        [Test]
        public void ReturnIdentity_WhenPostgreSqlSelectorIsNotDirectProperty_ShouldThrowArgumentException()
        {
            var command = _postgreSql
                .InsertInto<User>()
                .Set(user => user.Email, "admin@test.com");

            var exception = Assert.Throws<ArgumentException>(
                () => command.ReturnIdentity(user => user.Email.Length));

            Assert.Multiple(() =>
            {
                Assert.That(exception!.ParamName, Is.EqualTo("identitySelector"));
                Assert.That(exception.Message, Does.StartWith("The INSERT identity selector must reference a direct entity property."));
            });
        }
    }
}
