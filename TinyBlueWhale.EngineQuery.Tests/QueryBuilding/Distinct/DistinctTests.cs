using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding;
using TinyBlueWhale.EngineQuery.MySql.Compilation;
using TinyBlueWhale.EngineQuery.MySql.Profiles;
using TinyBlueWhale.EngineQuery.PostgreSql.Compilation;
using TinyBlueWhale.EngineQuery.PostgreSql.Profiles;
using TinyBlueWhale.EngineQuery.SqlServer.Compilation;
using TinyBlueWhale.EngineQuery.SqlServer.Profiles;
using TinyBlueWhale.EngineQuery.Tests.Infrastructure;
using TinyBlueWhale.EngineQuery.Tests.Models;

namespace TinyBlueWhale.EngineQuery.Tests.QueryBuilding.Distinct
{
    /// <summary>
    /// Validates provider-independent DISTINCT query behavior.
    /// </summary>
    [TestFixture]
    internal sealed class DistinctTests
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
        public void Build_WhenDistinctIsConfigured_ShouldGenerateDistinctSelect()
        {
            AssertDistinct(_sqlServer);
            AssertDistinct(_postgreSql);
            AssertDistinct(_mySql);
        }

        [Test]
        public void Build_WhenDistinctIsNotConfigured_ShouldGenerateRegularSelect()
        {
            AssertWithoutDistinct(_sqlServer);
            AssertWithoutDistinct(_postgreSql);
            AssertWithoutDistinct(_mySql);
        }

        [Test]
        public void Build_WhenDistinctIsConfiguredMultipleTimes_ShouldGenerateSingleDistinctModifier()
        {
            AssertRepeatedDistinct(_sqlServer);
            AssertRepeatedDistinct(_postgreSql);
            AssertRepeatedDistinct(_mySql);
        }

        private static void AssertDistinct<TProfile>(QueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<JoinUser>(alias: "u")
                .Distinct()
                .Select<JoinUser>(user => new
                {
                    user.Email
                })
                .Build();

            var sql = NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain("SELECT DISTINCT"));
                Assert.That(sql, Does.Contain("u.email"));
                Assert.That(query.Parameters, Is.Empty);
            });
        }

        private static void AssertWithoutDistinct<TProfile>(QueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(user => new
                {
                    user.Email
                })
                .Build();

            var sql = NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain("SELECT"));
                Assert.That(sql, Does.Not.Contain("SELECT DISTINCT"));
                Assert.That(sql, Does.Contain("u.email"));
                Assert.That(query.Parameters, Is.Empty);
            });
        }

        private static void AssertRepeatedDistinct<TProfile>(QueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<JoinUser>(alias: "u")
                .Distinct()
                .Distinct()
                .Select<JoinUser>(user => new
                {
                    user.Email
                })
                .Build();

            var sql = NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain("SELECT DISTINCT"));
                Assert.That(CountOccurrences(sql, "DISTINCT"), Is.EqualTo(1));
                Assert.That(query.Parameters, Is.Empty);
            });
        }

        private static int CountOccurrences(string value, string token)
        {
            return value.Split(token, StringSplitOptions.None).Length - 1;
        }

        private static string NormalizeSql(string commandText)
        {
            return commandText
                .Replace("[", string.Empty, StringComparison.Ordinal)
                .Replace("]", string.Empty, StringComparison.Ordinal)
                .Replace("\"", string.Empty, StringComparison.Ordinal)
                .Replace("`", string.Empty, StringComparison.Ordinal);
        }
    }
}
