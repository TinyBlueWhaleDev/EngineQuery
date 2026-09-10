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
    /// Validates computed aggregate projection behavior.
    /// </summary>
    [TestFixture]
    internal sealed class ComputedAggregateTests
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

        [TestCase(QueryAggregateFunction.Sum)]
        [TestCase(QueryAggregateFunction.Average)]
        [TestCase(QueryAggregateFunction.Minimum)]
        [TestCase(QueryAggregateFunction.Maximum)]
        public void SelectAggregate_WhenComputedExpressionUsesSupportedFunction_ShouldBuild(QueryAggregateFunction function)
        {
            AssertComputedAggregate(_sqlServer, function);
            AssertComputedAggregate(_postgreSql, function);
            AssertComputedAggregate(_mySql, function);
        }

        [Test]
        public void SelectAggregate_WhenComputedExpressionUsesCount_ShouldThrow()
        {
            AssertCountComputedExpressionThrows(_sqlServer);
            AssertCountComputedExpressionThrows(_postgreSql);
            AssertCountComputedExpressionThrows(_mySql);
        }

        [Test]
        public void SelectAggregate_WhenComputedExpressionContainsConstant_ShouldGenerateParameter()
        {
            AssertComputedAggregateParameter(_sqlServer);
            AssertComputedAggregateParameter(_postgreSql);
            AssertComputedAggregateParameter(_mySql);
        }

        [Test]
        public void SelectAggregate_WhenMultipleComputedAggregatesAreConfigured_ShouldPreserveOrder()
        {
            AssertMultipleComputedAggregates(_sqlServer);
            AssertMultipleComputedAggregates(_postgreSql);
            AssertMultipleComputedAggregates(_mySql);
        }

        [Test]
        public void SelectAggregate_WhenComputedAndColumnAggregatesAreCombined_ShouldPreserveOrder()
        {
            AssertComputedAndColumnAggregate(_sqlServer);
            AssertComputedAndColumnAggregate(_postgreSql);
            AssertComputedAndColumnAggregate(_mySql);
        }

        [Test]
        public void SelectAggregate_WhenAliasIsNull_ShouldThrowArgumentNullException()
        {
            AssertNullAlias(_sqlServer);
            AssertNullAlias(_postgreSql);
            AssertNullAlias(_mySql);
        }

        [TestCase("")]
        [TestCase(" ")]
        public void SelectAggregate_WhenAliasIsEmptyOrWhitespace_ShouldThrowArgumentException(string alias)
        {
            AssertInvalidAlias(_sqlServer, alias);
            AssertInvalidAlias(_postgreSql, alias);
            AssertInvalidAlias(_mySql, alias);
        }

        private static void AssertComputedAggregate<TProfile>(QueryBuilder<TProfile> queryBuilder, QueryAggregateFunction function) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<JoinOrder>(alias: "o")
                .SelectAggregate<JoinOrder>(function, order => order.Total * 2, alias: "ComputedTotal")
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(query.CommandText, Does.Contain(GetSqlFunction(function)));
                Assert.That(query.CommandText, Does.Contain("ComputedTotal"));
                Assert.That(query.Parameters, Has.Count.EqualTo(1));
                QueryAssertionHelper.AssertParameter(query.Parameters, 0, 2);
            });
        }

        private static void AssertCountComputedExpressionThrows<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var exception = Assert.Throws<NotSupportedException>(() =>
                queryBuilder
                    .From<JoinOrder>(alias: "o")
                    .SelectAggregate<JoinOrder>(QueryAggregateFunction.Count, order => order.Total * 2, alias: "ComputedCount"));

            Assert.That(exception!.Message, Is.EqualTo("Aggregate function 'Count' does not support computed expressions."));
        }

        private static void AssertComputedAggregateParameter<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            const decimal multiplier = 1.16m;

            var query = queryBuilder
                .From<JoinOrder>(alias: "o")
                .SelectAggregate<JoinOrder>(QueryAggregateFunction.Sum, order => order.Total * multiplier, alias: "TotalWithTax")
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(query.Parameters, Has.Count.EqualTo(1));
                QueryAssertionHelper.AssertParameter(query.Parameters, 0, multiplier);
            });
        }

        private static void AssertMultipleComputedAggregates<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<JoinOrder>(alias: "o")
                .SelectAggregate<JoinOrder>(QueryAggregateFunction.Sum, order => order.Total * 2, alias: "DoubleTotal")
                .SelectAggregate<JoinOrder>(QueryAggregateFunction.Average, order => order.Total + 10, alias: "AdjustedAverage")
                .SelectAggregate<JoinOrder>(QueryAggregateFunction.Minimum, order => order.Total - 5, alias: "AdjustedMinimum")
                .SelectAggregate<JoinOrder>(QueryAggregateFunction.Maximum, order => order.Total / 2, alias: "AdjustedMaximum")
                .Build();

            var sumIndex = query.CommandText.IndexOf("SUM", StringComparison.Ordinal);
            var averageIndex = query.CommandText.IndexOf("AVG", StringComparison.Ordinal);
            var minimumIndex = query.CommandText.IndexOf("MIN", StringComparison.Ordinal);
            var maximumIndex = query.CommandText.IndexOf("MAX", StringComparison.Ordinal);

            Assert.Multiple(() =>
            {
                Assert.That(sumIndex, Is.GreaterThanOrEqualTo(0));
                Assert.That(averageIndex, Is.GreaterThan(sumIndex));
                Assert.That(minimumIndex, Is.GreaterThan(averageIndex));
                Assert.That(maximumIndex, Is.GreaterThan(minimumIndex));
                Assert.That(query.Parameters, Has.Count.EqualTo(4));
                QueryAssertionHelper.AssertParameter(query.Parameters, 0, 2);
                QueryAssertionHelper.AssertParameter(query.Parameters, 1, 10);
                QueryAssertionHelper.AssertParameter(query.Parameters, 2, 5);
                QueryAssertionHelper.AssertParameter(query.Parameters, 3, 2);
            });
        }

        private static void AssertComputedAndColumnAggregate<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<JoinOrder>(alias: "o")
                .SelectAggregate<JoinOrder>(QueryAggregateFunction.Count, order => order.Id, alias: "OrderCount")
                .SelectAggregate<JoinOrder>(QueryAggregateFunction.Sum, order => order.Total * 2, alias: "DoubleTotal")
                .Build();

            var countIndex = query.CommandText.IndexOf("COUNT", StringComparison.Ordinal);
            var sumIndex = query.CommandText.IndexOf("SUM", StringComparison.Ordinal);

            Assert.Multiple(() =>
            {
                Assert.That(countIndex, Is.GreaterThanOrEqualTo(0));
                Assert.That(sumIndex, Is.GreaterThan(countIndex));
                Assert.That(query.Parameters, Has.Count.EqualTo(1));
                QueryAssertionHelper.AssertParameter(query.Parameters, 0, 2);
            });
        }

        private static void AssertNullAlias<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var exception = Assert.Throws<ArgumentNullException>(() =>
                queryBuilder
                    .From<JoinOrder>(alias: "o")
                    .SelectAggregate<JoinOrder>(QueryAggregateFunction.Sum, order => order.Total, null!));

            Assert.That(exception!.ParamName, Is.EqualTo("alias"));
        }

        private static void AssertInvalidAlias<TProfile>(QueryBuilder<TProfile> queryBuilder, string alias) where TProfile : IDatabaseProviderProfile
        {
            var exception = Assert.Throws<ArgumentException>(() =>
                queryBuilder
                    .From<JoinOrder>(alias: "o")
                    .SelectAggregate<JoinOrder>(QueryAggregateFunction.Sum, order => order.Total, alias));

            Assert.That(exception!.ParamName, Is.EqualTo("alias"));
        }

        private static string GetSqlFunction(QueryAggregateFunction function)
        {
            return function switch
            {
                QueryAggregateFunction.Sum => "SUM",
                QueryAggregateFunction.Average => "AVG",
                QueryAggregateFunction.Minimum => "MIN",
                QueryAggregateFunction.Maximum => "MAX",
                _ => throw new ArgumentOutOfRangeException(nameof(function), function, null)
            };
        }
    }
}
