using TinyBlueWhale.EngineQuery.Abstractions.Enums;
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
    /// Validates aggregate projection behavior across supported providers.
    /// </summary>
    [TestFixture]
    internal sealed class AggregateProjectionTests
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

        [TestCase(QueryAggregateFunction.Count, "COUNT")]
        [TestCase(QueryAggregateFunction.Sum, "SUM")]
        [TestCase(QueryAggregateFunction.Average, "AVG")]
        [TestCase(QueryAggregateFunction.Minimum, "MIN")]
        [TestCase(QueryAggregateFunction.Maximum, "MAX")]
        public void SelectAggregate_WhenFunctionIsConfigured_ShouldGenerateExpectedAggregate(QueryAggregateFunction function, string sqlFunction)
        {
            AssertAggregateFunction(_sqlServer, function, sqlFunction, "[orders].[total]");
            AssertAggregateFunction(_postgreSql, function, sqlFunction, "\"orders\".\"total\"");
            AssertAggregateFunction(_mySql, function, sqlFunction, "`orders`.`total`");
        }

        [Test]
        public void SelectAggregate_WhenAliasIsConfigured_ShouldRenderExplicitAlias()
        {
            AssertExplicitAlias(_sqlServer, " AS [TotalAmount]");
            AssertExplicitAlias(_postgreSql, " AS \"TotalAmount\"");
            AssertExplicitAlias(_mySql, " AS `TotalAmount`");
        }

        [Test]
        public void SelectAggregate_WhenMultipleAggregatesAreConfigured_ShouldPreserveProjectionOrder()
        {
            AssertMultipleAggregates(_sqlServer);
            AssertMultipleAggregates(_postgreSql);
            AssertMultipleAggregates(_mySql);
        }

        [Test]
        public void SelectAggregate_WhenAliasIsEmpty_ShouldThrow()
        {
            AssertAggregateAliasRequired(_sqlServer);
            AssertAggregateAliasRequired(_postgreSql);
            AssertAggregateAliasRequired(_mySql);
        }

        [Test]
        public void SelectAggregate_WhenCombinedWithRegularProjection_ShouldPreserveProjectionOrder()
        {
            AssertRegularAndAggregateProjection(_sqlServer);
            AssertRegularAndAggregateProjection(_postgreSql);
            AssertRegularAndAggregateProjection(_mySql);
        }

        [Test]
        public void SelectAggregate_WhenAppliedToJoinedSource_ShouldResolveJoinedSource()
        {
            AssertJoinedSourceAggregate(_sqlServer, "[o].[total]");
            AssertJoinedSourceAggregate(_postgreSql, "\"o\".\"total\"");
            AssertJoinedSourceAggregate(_mySql, "`o`.`total`");
        }

        [Test]
        public void SelectAggregate_WhenMultipleSameTypeSourcesExist_ShouldResolveSourceByLambdaAlias()
        {
            AssertSameTypeJoinedSourceAggregate(_sqlServer, "COUNT([parent].[category_id]) AS [ParentCount]");
            AssertSameTypeJoinedSourceAggregate(_postgreSql, "COUNT(\"parent\".\"category_id\") AS \"ParentCount\"");
            AssertSameTypeJoinedSourceAggregate(_mySql, "COUNT(`parent`.`category_id`) AS `ParentCount`");
        }

        [Test]
        public void SelectAggregate_WhenNoGroupByIsConfigured_ShouldGenerateAggregateOnlyQuery()
        {
            AssertAggregateWithoutGroupBy(_sqlServer);
            AssertAggregateWithoutGroupBy(_postgreSql);
            AssertAggregateWithoutGroupBy(_mySql);
        }

        private static void AssertAggregateFunction<TProfile>(QueryBuilder<TProfile> queryBuilder, QueryAggregateFunction function, string sqlFunction, string expectedColumn) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<JoinOrder>(alias: "orders")
                .SelectAggregate<JoinOrder>(function, order => order.Total, "TotalAmount")
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(query.CommandText, Does.Contain($"{sqlFunction}({expectedColumn})"));
                Assert.That(query.Parameters, Is.Empty);
            });
        }

        private static void AssertExplicitAlias<TProfile>(QueryBuilder<TProfile> queryBuilder, string expectedAlias) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<JoinOrder>(alias: "orders")
                .SelectAggregate<JoinOrder>(QueryAggregateFunction.Sum, order => order.Total, alias: "TotalAmount")
                .Build();

            Assert.That(query.CommandText, Does.Contain(expectedAlias));
        }

        private static void AssertMultipleAggregates<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<JoinOrder>(alias: "orders")
                .SelectAggregate<JoinOrder>(QueryAggregateFunction.Count, order => order.Id, alias: "OrderCount")
                .SelectAggregate<JoinOrder>(QueryAggregateFunction.Sum, order => order.Total, alias: "TotalAmount")
                .SelectAggregate<JoinOrder>(QueryAggregateFunction.Average, order => order.Total, alias: "AverageAmount")
                .SelectAggregate<JoinOrder>(QueryAggregateFunction.Minimum, order => order.Total, alias: "MinimumAmount")
                .SelectAggregate<JoinOrder>(QueryAggregateFunction.Maximum, order => order.Total, alias: "MaximumAmount")
                .Build();

            var countIndex = query.CommandText.IndexOf("COUNT", StringComparison.Ordinal);
            var sumIndex = query.CommandText.IndexOf("SUM", StringComparison.Ordinal);
            var averageIndex = query.CommandText.IndexOf("AVG", StringComparison.Ordinal);
            var minimumIndex = query.CommandText.IndexOf("MIN", StringComparison.Ordinal);
            var maximumIndex = query.CommandText.IndexOf("MAX", StringComparison.Ordinal);

            Assert.Multiple(() =>
            {
                Assert.That(countIndex, Is.GreaterThanOrEqualTo(0));
                Assert.That(sumIndex, Is.GreaterThan(countIndex));
                Assert.That(averageIndex, Is.GreaterThan(sumIndex));
                Assert.That(minimumIndex, Is.GreaterThan(averageIndex));
                Assert.That(maximumIndex, Is.GreaterThan(minimumIndex));
                Assert.That(query.Parameters, Is.Empty);
            });
        }

        private static void AssertRegularAndAggregateProjection<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<JoinUser>(alias: "u")
                .InnerJoin<JoinUser, JoinOrder>(alias: "o", on: (u, o) => u.Id == o.UserId)
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id
                })
                .SelectAggregate<JoinOrder>(QueryAggregateFunction.Count, o => o.Id, alias: "OrderCount")
                .Build();

            var userProjectionIndex = query.CommandText.IndexOf("UserId", StringComparison.Ordinal);
            var aggregateIndex = query.CommandText.IndexOf("COUNT", StringComparison.Ordinal);

            Assert.Multiple(() =>
            {
                Assert.That(userProjectionIndex, Is.GreaterThanOrEqualTo(0));
                Assert.That(aggregateIndex, Is.GreaterThan(userProjectionIndex));
                Assert.That(query.Parameters, Is.Empty);
            });
        }

        private static void AssertJoinedSourceAggregate<TProfile>(QueryBuilder<TProfile> queryBuilder, string expectedColumn) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<JoinUser>(alias: "u")
                .InnerJoin<JoinUser, JoinOrder>(alias: "o", on: (u, o) => u.Id == o.UserId)
                .SelectAggregate<JoinOrder>(QueryAggregateFunction.Sum, o => o.Total, alias: "TotalAmount")
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(query.CommandText, Does.Contain($"SUM({expectedColumn})"));
                Assert.That(query.Parameters, Is.Empty);
            });
        }

        private static void AssertSameTypeJoinedSourceAggregate<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<Category>(alias: "category")
                .LeftJoin<Category, Category>(alias: "parent", on: (category, parent) => category.ParentId == parent.Id)
                .SelectAggregate<Category>(QueryAggregateFunction.Count, parent => parent.Id, alias: "ParentCount")
                .Build();

            Assert.That(query.Parameters, Is.Empty);
        }

        private static void AssertSameTypeJoinedSourceAggregate<TProfile>(QueryBuilder<TProfile> queryBuilder, string expectedProjection) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<Category>(alias: "category")
                .LeftJoin<Category, Category>(alias: "parent", on: (category, parent) => category.ParentId == parent.Id)
                .SelectAggregate<Category>(QueryAggregateFunction.Count, parent => parent.Id, alias: "ParentCount")
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(query.CommandText, Does.Contain(expectedProjection));
                Assert.That(query.Parameters, Is.Empty);
            });
        }

        private static void AssertAggregateWithoutGroupBy<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<JoinOrder>(alias: "orders")
                .SelectAggregate<JoinOrder>(QueryAggregateFunction.Count, order => order.Id, alias: "OrderCount")
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(query.CommandText, Does.Contain("COUNT"));
                Assert.That(query.CommandText, Does.Not.Contain("GROUP BY"));
                Assert.That(query.CommandText, Does.Not.Contain("HAVING"));
                Assert.That(query.Parameters, Is.Empty);
            });
        }

        private static void AssertAggregateAliasRequired<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var exception = Assert.Throws<ArgumentException>(() =>
                queryBuilder
                    .From<JoinOrder>(alias: "orders")
                    .SelectAggregate<JoinOrder>(QueryAggregateFunction.Sum, order => order.Total, alias: string.Empty));

            Assert.That(exception!.ParamName, Is.EqualTo("alias"));
        }
    }
}
