using TinyBlueWhale.EngineQuery.Abstractions.Extensions;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
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
    /// Validates provider-specific pagination behavior exposed through supported provider profiles.
    /// </summary>
    [TestFixture]
    internal sealed class PaginationTests
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
        public void Build_WhenSkipAndTakeAreConfigured_ShouldGenerateProviderSpecificPagination()
        {
            var sqlServer = _sqlServer
                .From<User>()
                .OrderBy<User>(user => user.Email)
                .Skip(20)
                .Take(10)
                .Build();

            var postgreSql = _postgreSql
                .From<User>()
                .OrderBy<User>(user => user.Email)
                .Skip(20)
                .Take(10)
                .Build();

            var mySql = _mySql
                .From<User>()
                .OrderBy<User>(user => user.Email)
                .Skip(20)
                .Take(10)
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(QueryAssertionHelper.NormalizeSql(sqlServer.CommandText), Does.Contain("ORDER BY"));
                Assert.That(sqlServer.CommandText, Does.Contain("OFFSET 20 ROWS").IgnoreCase);
                Assert.That(sqlServer.CommandText, Does.Contain("FETCH NEXT 10 ROWS ONLY").IgnoreCase);
                Assert.That(sqlServer.Parameters, Is.Empty);

                Assert.That(QueryAssertionHelper.NormalizeSql(postgreSql.CommandText), Does.Contain("ORDER BY"));
                Assert.That(postgreSql.CommandText, Does.Contain("LIMIT 10").IgnoreCase);
                Assert.That(postgreSql.CommandText, Does.Contain("OFFSET 20").IgnoreCase);
                Assert.That(postgreSql.Parameters, Is.Empty);

                Assert.That(QueryAssertionHelper.NormalizeSql(mySql.CommandText), Does.Contain("ORDER BY"));
                Assert.That(mySql.CommandText, Does.Contain("LIMIT 10").IgnoreCase);
                Assert.That(mySql.CommandText, Does.Contain("OFFSET 20").IgnoreCase);
                Assert.That(mySql.Parameters, Is.Empty);
            });
        }

        [Test]
        public void Build_WhenTakeIsConfigured_ShouldGenerateProviderSpecificLimit()
        {
            var sqlServer = _sqlServer
                .From<User>()
                .OrderBy<User>(user => user.Email)
                .Take(10)
                .Build();

            var postgreSql = _postgreSql
                .From<User>()
                .OrderBy<User>(user => user.Email)
                .Take(10)
                .Build();

            var mySql = _mySql
                .From<User>()
                .OrderBy<User>(user => user.Email)
                .Take(10)
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(sqlServer.CommandText, Does.Contain("OFFSET 0 ROWS").IgnoreCase);
                Assert.That(sqlServer.CommandText, Does.Contain("FETCH NEXT 10 ROWS ONLY").IgnoreCase);
                Assert.That(sqlServer.Parameters, Is.Empty);

                Assert.That(postgreSql.CommandText, Does.Contain("LIMIT 10").IgnoreCase);
                Assert.That(postgreSql.Parameters, Is.Empty);

                Assert.That(mySql.CommandText, Does.Contain("LIMIT 10").IgnoreCase);
                Assert.That(mySql.Parameters, Is.Empty);
            });
        }

        [Test]
        public void Build_WhenSkipIsConfigured_ShouldGenerateProviderSpecificOffset()
        {
            var sqlServer = _sqlServer
                .From<User>()
                .OrderBy<User>(user => user.Email)
                .Skip(20)
                .Build();

            var postgreSql = _postgreSql
                .From<User>()
                .OrderBy<User>(user => user.Email)
                .Skip(20)
                .Build();

            var mySql = _mySql
                .From<User>()
                .OrderBy<User>(user => user.Email)
                .Skip(20)
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(sqlServer.CommandText, Does.Contain("OFFSET 20 ROWS").IgnoreCase);
                Assert.That(sqlServer.CommandText, Does.Not.Contain("FETCH NEXT").IgnoreCase);
                Assert.That(sqlServer.Parameters, Is.Empty);

                Assert.That(postgreSql.CommandText, Does.Contain("OFFSET 20").IgnoreCase);
                Assert.That(postgreSql.Parameters, Is.Empty);

                Assert.That(mySql.CommandText, Does.Contain("OFFSET 20").IgnoreCase);
                Assert.That(mySql.Parameters, Is.Empty);
            });
        }

        [Test]
        public void Build_WhenPaginationAndWhereAreConfigured_ShouldPreserveClauseOrder()
        {
            var sqlServer = _sqlServer
                .From<User>()
                .Where<User>(user => user.IsActive)
                .OrderBy<User>(user => user.Email)
                .Skip(20)
                .Take(10)
                .Build();

            var postgreSql = _postgreSql
                .From<User>()
                .Where<User>(user => user.IsActive)
                .OrderBy<User>(user => user.Email)
                .Skip(20)
                .Take(10)
                .Build();

            var mySql = _mySql
                .From<User>()
                .Where<User>(user => user.IsActive)
                .OrderBy<User>(user => user.Email)
                .Skip(20)
                .Take(10)
                .Build();

            AssertClauseOrder(sqlServer);
            AssertClauseOrder(postgreSql);
            AssertClauseOrder(mySql);
        }

        private static void AssertClauseOrder(GeneratedSqlQuery query)
        {
            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);
            var whereIndex = sql.IndexOf("WHERE", StringComparison.Ordinal);
            var orderByIndex = sql.IndexOf("ORDER BY", StringComparison.Ordinal);
            var paginationIndex = GetPaginationIndex(sql);

            Assert.Multiple(() =>
            {
                Assert.That(whereIndex, Is.GreaterThanOrEqualTo(0));
                Assert.That(orderByIndex, Is.GreaterThan(whereIndex));
                Assert.That(paginationIndex, Is.GreaterThan(orderByIndex));
                QueryAssertionHelper.AssertParameter(query.Parameters, 0, true);
            });
        }

        private static int GetPaginationIndex(string sql)
        {
            var offsetIndex = sql.IndexOf("OFFSET", StringComparison.Ordinal);
            var limitIndex = sql.IndexOf("LIMIT", StringComparison.Ordinal);

            return offsetIndex >= 0 ? offsetIndex : limitIndex;
        }
    }
}
