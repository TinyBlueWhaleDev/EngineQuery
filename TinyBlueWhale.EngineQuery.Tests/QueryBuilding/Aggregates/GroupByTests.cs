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

namespace TinyBlueWhale.EngineQuery.Tests.QueryBuilding.Aggregates
{
    /// <summary>
    /// Validates GROUP BY query behavior.
    /// </summary>
    [TestFixture]
    internal sealed class GroupByTests
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
        public void GroupBy_WhenSinglePropertyIsConfigured_ShouldGenerateExpectedClause()
        {
            AssertSingleProperty(_sqlServer, "GROUP BY [u].[user_id]");
            AssertSingleProperty(_postgreSql, "GROUP BY \"u\".\"user_id\"");
            AssertSingleProperty(_mySql, "GROUP BY `u`.`user_id`");
        }

        [Test]
        public void GroupBy_WhenMultiplePropertiesAreConfigured_ShouldPreserveColumnOrder()
        {
            AssertMultipleProperties(_sqlServer, "GROUP BY [u].[user_id], [u].[email]");
            AssertMultipleProperties(_postgreSql, "GROUP BY \"u\".\"user_id\", \"u\".\"email\"");
            AssertMultipleProperties(_mySql, "GROUP BY `u`.`user_id`, `u`.`email`");
        }

        [Test]
        public void GroupBy_WhenConfiguredMultipleTimes_ShouldAccumulateDefinitionsInOrder()
        {
            AssertMultipleDefinitions(_sqlServer, "[u].[user_id]", "[o].[user_id]");
            AssertMultipleDefinitions(_postgreSql, "\"u\".\"user_id\"", "\"o\".\"user_id\"");
            AssertMultipleDefinitions(_mySql, "`u`.`user_id`", "`o`.`user_id`");
        }

        [Test]
        public void GroupBy_WhenAppliedToJoinedSource_ShouldResolveJoinedSource()
        {
            AssertJoinedSource(_sqlServer, "GROUP BY [o].[user_id]");
            AssertJoinedSource(_postgreSql, "GROUP BY \"o\".\"user_id\"");
            AssertJoinedSource(_mySql, "GROUP BY `o`.`user_id`");
        }

        [Test]
        public void GroupBy_WhenRootAndJoinedSourcesAreConfigured_ShouldResolveEachSourceIndependently()
        {
            AssertRootAndJoinedSources(_sqlServer, "[u].[user_id]", "[o].[user_id]");
            AssertRootAndJoinedSources(_postgreSql, "\"u\".\"user_id\"", "\"o\".\"user_id\"");
            AssertRootAndJoinedSources(_mySql, "`u`.`user_id`", "`o`.`user_id`");
        }

        [Test]
        public void GroupBy_WhenMultipleSameTypeSourcesExist_ShouldResolveSourceByLambdaAlias()
        {
            AssertSameTypeSources(_sqlServer, "[category].[category_id]", "[parent].[category_id]");
            AssertSameTypeSources(_postgreSql, "\"category\".\"category_id\"", "\"parent\".\"category_id\"");
            AssertSameTypeSources(_mySql, "`category`.`category_id`", "`parent`.`category_id`");
        }

        [Test]
        public void GroupBy_WhenConfigured_ShouldNotAddParameters()
        {
            AssertNoParameters(_sqlServer);
            AssertNoParameters(_postgreSql);
            AssertNoParameters(_mySql);
        }

        [Test]
        public void GroupBy_WhenSelectorIsUnsupported_ShouldThrow()
        {
            AssertUnsupportedSelector(_sqlServer);
            AssertUnsupportedSelector(_postgreSql);
            AssertUnsupportedSelector(_mySql);
        }

        private static void AssertSingleProperty<TProfile>(QueryBuilder<TProfile> queryBuilder, string expectedGroupBy) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => u.Id)
                .GroupBy<JoinUser>(u => u.Id)
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(query.CommandText, Does.Contain(expectedGroupBy));
                Assert.That(query.Parameters, Is.Empty);
            });
        }

        private static void AssertMultipleProperties<TProfile>(QueryBuilder<TProfile> queryBuilder, string expectedGroupBy) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => new
                {
                    u.Id,
                    u.Email
                })
                .GroupBy<JoinUser>(u => new
                {
                    u.Id,
                    u.Email
                })
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(query.CommandText, Does.Contain(expectedGroupBy));
                Assert.That(query.Parameters, Is.Empty);
            });
        }

        private static void AssertMultipleDefinitions<TProfile>(QueryBuilder<TProfile> queryBuilder, string expectedRootColumn, string expectedJoinedColumn) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<JoinUser>(alias: "u")
                .InnerJoin<JoinUser, JoinOrder>(alias: "o", on: (u, o) => u.Id == o.UserId)
                .Select<JoinUser>(u => u.Id)
                .Select<JoinOrder>(o => o.UserId)
                .GroupBy<JoinUser>(u => u.Id)
                .GroupBy<JoinOrder>(o => o.UserId)
                .Build();

            var groupByIndex = query.CommandText.IndexOf("GROUP BY", StringComparison.Ordinal);
            var rootIndex = query.CommandText.IndexOf(expectedRootColumn, groupByIndex, StringComparison.Ordinal);
            var joinedIndex = query.CommandText.IndexOf(expectedJoinedColumn, groupByIndex, StringComparison.Ordinal);

            Assert.Multiple(() =>
            {
                Assert.That(groupByIndex, Is.GreaterThanOrEqualTo(0));
                Assert.That(rootIndex, Is.GreaterThan(groupByIndex));
                Assert.That(joinedIndex, Is.GreaterThan(rootIndex));
                Assert.That(query.Parameters, Is.Empty);
            });
        }

        private static void AssertJoinedSource<TProfile>(QueryBuilder<TProfile> queryBuilder, string expectedGroupBy) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<JoinUser>(alias: "u")
                .InnerJoin<JoinUser, JoinOrder>(alias: "o", on: (u, o) => u.Id == o.UserId)
                .Select<JoinOrder>(o => o.UserId)
                .GroupBy<JoinOrder>(o => o.UserId)
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(query.CommandText, Does.Contain(expectedGroupBy));
                Assert.That(query.Parameters, Is.Empty);
            });
        }

        private static void AssertRootAndJoinedSources<TProfile>(QueryBuilder<TProfile> queryBuilder, string expectedRootColumn, string expectedJoinedColumn) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<JoinUser>(alias: "u")
                .InnerJoin<JoinUser, JoinOrder>(alias: "o", on: (u, o) => u.Id == o.UserId)
                .Select<JoinUser>(u => u.Id)
                .Select<JoinOrder>(o => o.UserId)
                .GroupBy<JoinUser>(u => u.Id)
                .GroupBy<JoinOrder>(o => o.UserId)
                .Build();

            var groupByIndex = query.CommandText.IndexOf("GROUP BY", StringComparison.Ordinal);
            var rootIndex = query.CommandText.IndexOf(expectedRootColumn, groupByIndex, StringComparison.Ordinal);
            var joinedIndex = query.CommandText.IndexOf(expectedJoinedColumn, groupByIndex, StringComparison.Ordinal);

            Assert.Multiple(() =>
            {
                Assert.That(groupByIndex, Is.GreaterThanOrEqualTo(0));
                Assert.That(rootIndex, Is.GreaterThan(groupByIndex));
                Assert.That(joinedIndex, Is.GreaterThan(rootIndex));
                Assert.That(query.Parameters, Is.Empty);
            });
        }

        private static void AssertSameTypeSources<TProfile>(QueryBuilder<TProfile> queryBuilder, string expectedRootColumn, string expectedParentColumn) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<Category>(alias: "category")
                .LeftJoin<Category, Category>(alias: "parent", on: (category, parent) => category.ParentId == parent.Id)
                .Select<Category>(category => category.Id)
                .Select<Category>(parent => parent.Id)
                .GroupBy<Category>(category => category.Id)
                .GroupBy<Category>(parent => parent.Id)
                .Build();

            var groupByIndex = query.CommandText.IndexOf("GROUP BY", StringComparison.Ordinal);
            var rootIndex = query.CommandText.IndexOf(expectedRootColumn, groupByIndex, StringComparison.Ordinal);
            var parentIndex = query.CommandText.IndexOf(expectedParentColumn, groupByIndex, StringComparison.Ordinal);

            Assert.Multiple(() =>
            {
                Assert.That(groupByIndex, Is.GreaterThanOrEqualTo(0));
                Assert.That(rootIndex, Is.GreaterThan(groupByIndex));
                Assert.That(parentIndex, Is.GreaterThan(rootIndex));
                Assert.That(query.Parameters, Is.Empty);
            });
        }

        private static void AssertNoParameters<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => u.Id)
                .GroupBy<JoinUser>(u => u.Id)
                .Build();

            Assert.That(query.Parameters, Is.Empty);
        }

        private static void AssertUnsupportedSelector<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            Assert.Throws<NotSupportedException>(() =>
                queryBuilder
                    .From<JoinUser>(alias: "u")
                    .GroupBy<JoinUser>(u => u.Id + 1));
        }
    }
}
