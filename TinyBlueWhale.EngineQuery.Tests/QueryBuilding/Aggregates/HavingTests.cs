using TinyBlueWhale.EngineQuery.Abstractions.Enums;
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

namespace TinyBlueWhale.EngineQuery.Tests.QueryBuilding.Aggregates
{
    /// <summary>
    /// Validates HAVING aggregate query behavior.
    /// </summary>
    [TestFixture]
    internal sealed class HavingTests
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
        public void HavingAggregate_WhenFunctionIsConfigured_ShouldGenerateExpectedFunction(QueryAggregateFunction function, string expectedFunction)
        {
            AssertAggregateFunction(_sqlServer, function, $"HAVING {expectedFunction}([o].[total]) > @p0");
            AssertAggregateFunction(_postgreSql, function, $"HAVING {expectedFunction}(\"o\".\"total\") > @p0");
            AssertAggregateFunction(_mySql, function, $"HAVING {expectedFunction}(`o`.`total`) > @p0");
        }

        [TestCase(QueryComparisonOperator.Equal, "=")]
        [TestCase(QueryComparisonOperator.NotEqual, "<>")]
        [TestCase(QueryComparisonOperator.GreaterThan, ">")]
        [TestCase(QueryComparisonOperator.GreaterThanOrEqual, ">=")]
        [TestCase(QueryComparisonOperator.LessThan, "<")]
        [TestCase(QueryComparisonOperator.LessThanOrEqual, "<=")]
        public void HavingAggregate_WhenComparisonOperatorIsConfigured_ShouldGenerateExpectedOperator(QueryComparisonOperator comparisonOperator, string expectedOperator)
        {
            AssertComparisonOperator(_sqlServer, comparisonOperator, $"HAVING SUM([o].[total]) {expectedOperator} @p0");
            AssertComparisonOperator(_postgreSql, comparisonOperator, $"HAVING SUM(\"o\".\"total\") {expectedOperator} @p0");
            AssertComparisonOperator(_mySql, comparisonOperator, $"HAVING SUM(`o`.`total`) {expectedOperator} @p0");
        }

        [Test]
        public void HavingAggregate_WhenConfigured_ShouldGenerateParameter()
        {
            AssertHavingParameter(_sqlServer);
            AssertHavingParameter(_postgreSql);
            AssertHavingParameter(_mySql);
        }

        [Test]
        public void HavingAggregate_WhenMultipleConditionsAreConfigured_ShouldJoinWithAndAndPreserveOrder()
        {
            AssertMultipleConditions(_sqlServer, "HAVING SUM([o].[total]) > @p0 AND COUNT([o].[order_id]) <= @p1");
            AssertMultipleConditions(_postgreSql, "HAVING SUM(\"o\".\"total\") > @p0 AND COUNT(\"o\".\"order_id\") <= @p1");
            AssertMultipleConditions(_mySql, "HAVING SUM(`o`.`total`) > @p0 AND COUNT(`o`.`order_id`) <= @p1");
        }

        [Test]
        public void HavingAggregate_WhenCombinedWithGroupBy_ShouldRenderAfterGroupBy()
        {
            AssertClauseOrder(_sqlServer);
            AssertClauseOrder(_postgreSql);
            AssertClauseOrder(_mySql);
        }

        [Test]
        public void HavingAggregate_WhenAppliedToJoinedSource_ShouldResolveJoinedSource()
        {
            AssertJoinedSource(_sqlServer, "HAVING SUM([o].[total]) > @p0");
            AssertJoinedSource(_postgreSql, "HAVING SUM(\"o\".\"total\") > @p0");
            AssertJoinedSource(_mySql, "HAVING SUM(`o`.`total`) > @p0");
        }

        [Test]
        public void HavingAggregate_WhenAppliedToRootSource_ShouldResolveRootSource()
        {
            AssertRootSource(_sqlServer, "HAVING SUM([o].[total]) > @p0");
            AssertRootSource(_postgreSql, "HAVING SUM(\"o\".\"total\") > @p0");
            AssertRootSource(_mySql, "HAVING SUM(`o`.`total`) > @p0");
        }

        [Test]
        public void HavingAggregate_WhenMultipleSameTypeSourcesExist_ShouldResolveSourceByLambdaAlias()
        {
            AssertSameTypeSource(_sqlServer, "HAVING COUNT([parent].[category_id]) > @p0");
            AssertSameTypeSource(_postgreSql, "HAVING COUNT(\"parent\".\"category_id\") > @p0");
            AssertSameTypeSource(_mySql, "HAVING COUNT(`parent`.`category_id`) > @p0");
        }

        [Test]
        public void HavingAggregate_WhenMultipleSameTypeSourcesAreUsed_ShouldResolveEachConditionIndependently()
        {
            AssertMultipleSameTypeSources(_sqlServer, "HAVING COUNT([category].[category_id]) > @p0 AND COUNT([parent].[category_id]) < @p1");
            AssertMultipleSameTypeSources(_postgreSql, "HAVING COUNT(\"category\".\"category_id\") > @p0 AND COUNT(\"parent\".\"category_id\") < @p1");
            AssertMultipleSameTypeSources(_mySql, "HAVING COUNT(`category`.`category_id`) > @p0 AND COUNT(`parent`.`category_id`) < @p1");
        }

        [Test]
        public void HavingAggregate_WhenSelectorContainsMultipleProperties_ShouldThrow()
        {
            AssertMultiplePropertySelectorThrows(_sqlServer);
            AssertMultiplePropertySelectorThrows(_postgreSql);
            AssertMultiplePropertySelectorThrows(_mySql);
        }

        [Test]
        public void HavingAggregate_WhenSelectorIsUnsupported_ShouldThrow()
        {
            AssertUnsupportedSelectorThrows(_sqlServer);
            AssertUnsupportedSelectorThrows(_postgreSql);
            AssertUnsupportedSelectorThrows(_mySql);
        }

        private static void AssertAggregateFunction<TProfile>(QueryBuilder<TProfile> queryBuilder, QueryAggregateFunction function, string expectedHaving) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<JoinOrder>(alias: "o")
                .SelectAggregate<JoinOrder>(function, o => o.Total, alias: "AggregateValue")
                .HavingAggregate<JoinOrder>(function, o => o.Total, QueryComparisonOperator.GreaterThan, 100)
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(GetHavingClause(query.CommandText), Is.EqualTo(expectedHaving));
                Assert.That(query.Parameters, Has.Count.EqualTo(1));
                QueryAssertionHelper.AssertParameter(query.Parameters, 0, 100);
            });
        }

        private static void AssertComparisonOperator<TProfile>(QueryBuilder<TProfile> queryBuilder, QueryComparisonOperator comparisonOperator, string expectedHaving) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<JoinOrder>(alias: "o")
                .SelectAggregate<JoinOrder>(QueryAggregateFunction.Sum, o => o.Total, alias: "TotalAmount")
                .HavingAggregate<JoinOrder>(QueryAggregateFunction.Sum, o => o.Total, comparisonOperator, 500)
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(GetHavingClause(query.CommandText), Is.EqualTo(expectedHaving));
                Assert.That(query.Parameters, Has.Count.EqualTo(1));
                QueryAssertionHelper.AssertParameter(query.Parameters, 0, 500);
            });
        }

        private static void AssertHavingParameter<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            const decimal threshold = 1250.50m;

            var query = queryBuilder
                .From<JoinOrder>(alias: "o")
                .SelectAggregate<JoinOrder>(QueryAggregateFunction.Sum, o => o.Total, alias: "TotalAmount")
                .HavingAggregate<JoinOrder>(QueryAggregateFunction.Sum, o => o.Total, QueryComparisonOperator.GreaterThanOrEqual, threshold)
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(query.Parameters, Has.Count.EqualTo(1));
                QueryAssertionHelper.AssertParameter(query.Parameters, 0, threshold);
            });
        }

        private static void AssertMultipleConditions<TProfile>(QueryBuilder<TProfile> queryBuilder, string expectedHaving) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<JoinOrder>(alias: "o")
                .SelectAggregate<JoinOrder>(QueryAggregateFunction.Sum, o => o.Total, alias: "TotalAmount")
                .SelectAggregate<JoinOrder>(QueryAggregateFunction.Count, o => o.Id, alias: "OrderCount")
                .HavingAggregate<JoinOrder>(QueryAggregateFunction.Sum, o => o.Total, QueryComparisonOperator.GreaterThan, 1000)
                .HavingAggregate<JoinOrder>(QueryAggregateFunction.Count, o => o.Id, QueryComparisonOperator.LessThanOrEqual, 50)
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(GetHavingClause(query.CommandText), Is.EqualTo(expectedHaving));
                Assert.That(query.Parameters, Has.Count.EqualTo(2));
                QueryAssertionHelper.AssertParameter(query.Parameters, 0, 1000);
                QueryAssertionHelper.AssertParameter(query.Parameters, 1, 50);
            });
        }

        private static void AssertClauseOrder<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<JoinUser>(alias: "u")
                .InnerJoin<JoinUser, JoinOrder>(alias: "o", on: (u, o) => u.Id == o.UserId)
                .Select<JoinUser>(u => u.Id)
                .SelectAggregate<JoinOrder>(QueryAggregateFunction.Sum, o => o.Total, alias: "TotalAmount")
                .GroupBy<JoinUser>(u => u.Id)
                .HavingAggregate<JoinOrder>(QueryAggregateFunction.Sum, o => o.Total, QueryComparisonOperator.GreaterThan, 1000)
                .Build();

            var groupByIndex = query.CommandText.IndexOf("GROUP BY", StringComparison.Ordinal);
            var havingIndex = query.CommandText.IndexOf("HAVING", StringComparison.Ordinal);

            Assert.Multiple(() =>
            {
                Assert.That(groupByIndex, Is.GreaterThanOrEqualTo(0));
                Assert.That(havingIndex, Is.GreaterThan(groupByIndex));
            });
        }

        private static void AssertJoinedSource<TProfile>(QueryBuilder<TProfile> queryBuilder, string expectedHaving) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<JoinUser>(alias: "u")
                .InnerJoin<JoinUser, JoinOrder>(alias: "o", on: (u, o) => u.Id == o.UserId)
                .Select<JoinUser>(u => u.Id)
                .SelectAggregate<JoinOrder>(QueryAggregateFunction.Sum, o => o.Total, alias: "TotalAmount")
                .GroupBy<JoinUser>(u => u.Id)
                .HavingAggregate<JoinOrder>(QueryAggregateFunction.Sum, o => o.Total, QueryComparisonOperator.GreaterThan, 1000)
                .Build();

            Assert.That(GetHavingClause(query.CommandText), Is.EqualTo(expectedHaving));
        }

        private static void AssertRootSource<TProfile>(QueryBuilder<TProfile> queryBuilder, string expectedHaving) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<JoinOrder>(alias: "o")
                .SelectAggregate<JoinOrder>(QueryAggregateFunction.Sum, o => o.Total, alias: "TotalAmount")
                .HavingAggregate<JoinOrder>(QueryAggregateFunction.Sum, o => o.Total, QueryComparisonOperator.GreaterThan, 1000)
                .Build();

            Assert.That(GetHavingClause(query.CommandText), Is.EqualTo(expectedHaving));
        }

        private static void AssertSameTypeSource<TProfile>(QueryBuilder<TProfile> queryBuilder, string expectedHaving) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<Category>(alias: "category")
                .LeftJoin<Category, Category>(alias: "parent", on: (category, parent) => category.ParentId == parent.Id)
                .Select<Category>(category => category.Id)
                .SelectAggregate<Category>(QueryAggregateFunction.Count, parent => parent.Id, alias: "ParentCount")
                .GroupBy<Category>(category => category.Id)
                .HavingAggregate<Category>(QueryAggregateFunction.Count, parent => parent.Id, QueryComparisonOperator.GreaterThan, 0)
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(GetHavingClause(query.CommandText), Is.EqualTo(expectedHaving));
                QueryAssertionHelper.AssertParameter(query.Parameters, 0, 0);
            });
        }

        private static void AssertMultipleSameTypeSources<TProfile>(QueryBuilder<TProfile> queryBuilder, string expectedHaving) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<Category>(alias: "category")
                .LeftJoin<Category, Category>(alias: "parent", on: (category, parent) => category.ParentId == parent.Id)
                .SelectAggregate<Category>(QueryAggregateFunction.Count, category => category.Id, alias: "CategoryCount")
                .SelectAggregate<Category>(QueryAggregateFunction.Count, parent => parent.Id, alias: "ParentCount")
                .HavingAggregate<Category>(QueryAggregateFunction.Count, category => category.Id, QueryComparisonOperator.GreaterThan, 0)
                .HavingAggregate<Category>(QueryAggregateFunction.Count, parent => parent.Id, QueryComparisonOperator.LessThan, 100)
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(GetHavingClause(query.CommandText), Is.EqualTo(expectedHaving));
                Assert.That(query.Parameters, Has.Count.EqualTo(2));
                QueryAssertionHelper.AssertParameter(query.Parameters, 0, 0);
                QueryAssertionHelper.AssertParameter(query.Parameters, 1, 100);
            });
        }

        private static void AssertMultiplePropertySelectorThrows<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            Assert.Throws<InvalidOperationException>(() =>
                queryBuilder
                    .From<JoinOrder>(alias: "o")
                    .HavingAggregate<JoinOrder>(
                        QueryAggregateFunction.Sum,
                        o => new
                        {
                            o.Id,
                            o.Total
                        },
                        QueryComparisonOperator.GreaterThan,
                        100));
        }

        private static void AssertUnsupportedSelectorThrows<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            Assert.Throws<NotSupportedException>(() =>
                queryBuilder
                    .From<JoinOrder>(alias: "o")
                    .HavingAggregate<JoinOrder>(QueryAggregateFunction.Sum, o => o.Total + 1, QueryComparisonOperator.GreaterThan, 100));
        }

        private static string GetHavingClause(string commandText)
        {
            return commandText
                .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                .Single(line => line.StartsWith("HAVING ", StringComparison.Ordinal));
        }
    }
}
