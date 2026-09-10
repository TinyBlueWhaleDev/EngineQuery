using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding;
using TinyBlueWhale.EngineQuery.MySql.Compilation;
using TinyBlueWhale.EngineQuery.MySql.Profiles;
using TinyBlueWhale.EngineQuery.PostgreSql.Compilation;
using TinyBlueWhale.EngineQuery.PostgreSql.Profiles;
using TinyBlueWhale.EngineQuery.SqlServer.Compilation;
using TinyBlueWhale.EngineQuery.SqlServer.Profiles;
using TinyBlueWhale.EngineQuery.Tests.Infrastructure;
using TinyBlueWhale.EngineQuery.Tests.Models;

namespace TinyBlueWhale.EngineQuery.Tests.QueryBuilding.Deterministic
{
    /// <summary>
    /// Validates deterministic SQL generation behavior.
    /// </summary>
    [TestFixture]
    internal sealed class DeterminismTests
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
        public void Build_WhenInvokedMultipleTimes_ShouldProduceDeterministicResult()
        {
            AssertRepeatedBuild(_sqlServer);
            AssertRepeatedBuild(_postgreSql);
            AssertRepeatedBuild(_mySql);
        }

        [Test]
        public void Build_WhenEquivalentQueriesAreCreatedIndependently_ShouldProduceEquivalentResult()
        {
            AssertIndependentEquivalentQueries(_sqlServer);
            AssertIndependentEquivalentQueries(_postgreSql);
            AssertIndependentEquivalentQueries(_mySql);
        }

        [Test]
        public void Build_WhenMultipleParametersAreConfigured_ShouldPreserveDeterministicParameterOrder()
        {
            AssertParameterOrder(_sqlServer);
            AssertParameterOrder(_postgreSql);
            AssertParameterOrder(_mySql);
        }

        private static void AssertRepeatedBuild<TProfile>(QueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            var command = queryBuilder
                .From<User>("Users")
                .Where<User>(user => user.IsActive);

            var firstQuery = command.Build();
            var secondQuery = command.Build();

            AssertEquivalentQueries(firstQuery, secondQuery);
        }

        private static void AssertIndependentEquivalentQueries<TProfile>(QueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            var firstQuery = queryBuilder
                .From<User>("Users")
                .Select<User>(user => new
                {
                    user.Id,
                    user.Email
                })
                .Where<User>(user => user.IsActive)
                .Build();

            var secondQuery = queryBuilder
                .From<User>("Users")
                .Select<User>(user => new
                {
                    user.Id,
                    user.Email
                })
                .Where<User>(user => user.IsActive)
                .Build();

            AssertEquivalentQueries(firstQuery, secondQuery);
        }

        private static void AssertParameterOrder<TProfile>(QueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            var firstQuery = queryBuilder
                .From<User>("Users")
                .Where<User>(user => user.Id == 10)
                .Where<User>(user => user.Age >= 18)
                .Build();

            var secondQuery = queryBuilder
                .From<User>("Users")
                .Where<User>(user => user.Id == 10)
                .Where<User>(user => user.Age >= 18)
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(firstQuery.CommandText, Is.EqualTo(secondQuery.CommandText));
                Assert.That(firstQuery.Parameters, Has.Count.EqualTo(2));
                Assert.That(secondQuery.Parameters, Has.Count.EqualTo(2));

                AssertParameter(firstQuery.Parameters, 0, "@p0", 10);
                AssertParameter(firstQuery.Parameters, 1, "@p1", 18);
                AssertParameter(secondQuery.Parameters, 0, "@p0", 10);
                AssertParameter(secondQuery.Parameters, 1, "@p1", 18);
            });
        }

        private static void AssertEquivalentQueries(GeneratedSqlQuery firstQuery, GeneratedSqlQuery secondQuery)
        {
            Assert.Multiple(() =>
            {
                Assert.That(firstQuery.CommandText, Is.EqualTo(secondQuery.CommandText));
                Assert.That(firstQuery.Parameters, Has.Count.EqualTo(secondQuery.Parameters.Count));

                for (var index = 0; index < firstQuery.Parameters.Count; index++)
                {
                    Assert.That(firstQuery.Parameters[index].Name, Is.EqualTo(secondQuery.Parameters[index].Name));
                    Assert.That(firstQuery.Parameters[index].Value, Is.EqualTo(secondQuery.Parameters[index].Value));
                }
            });
        }

        private static void AssertParameter(IReadOnlyList<QuerySqlParameter> parameters, int index, string name, object expectedValue)
        {
            Assert.That(parameters, Has.Count.GreaterThan(index));

            Assert.Multiple(() =>
            {
                Assert.That(parameters[index].Name, Is.EqualTo(name));
                Assert.That(parameters[index].Value, Is.EqualTo(expectedValue));
            });
        }
    }
}
