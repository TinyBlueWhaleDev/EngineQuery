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
    /// Validates aggregate, grouping and having argument contracts.
    /// </summary>
    [TestFixture]
    internal sealed class AggregateValidationTests
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
        public void SelectAggregate_WhenSelectorIsNull_ShouldThrowArgumentNullException()
        {
            AssertAggregateNullSelector(_sqlServer);
            AssertAggregateNullSelector(_postgreSql);
            AssertAggregateNullSelector(_mySql);
        }

        [Test]
        public void GroupBy_WhenSelectorIsNull_ShouldThrowArgumentNullException()
        {
            AssertGroupByNullSelector(_sqlServer);
            AssertGroupByNullSelector(_postgreSql);
            AssertGroupByNullSelector(_mySql);
        }

        [Test]
        public void HavingAggregate_WhenSelectorIsNull_ShouldThrowArgumentNullException()
        {
            AssertHavingNullSelector(_sqlServer);
            AssertHavingNullSelector(_postgreSql);
            AssertHavingNullSelector(_mySql);
        }

        [Test]
        public void HavingAggregate_WhenValueIsNull_ShouldPreserveNullParameter()
        {
            AssertHavingNullValue(_sqlServer);
            AssertHavingNullValue(_postgreSql);
            AssertHavingNullValue(_mySql);
        }

        private static void AssertAggregateNullSelector<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var exception = Assert.Throws<ArgumentNullException>(() =>
                queryBuilder
                    .From<JoinOrder>(alias: "o")
                    .SelectAggregate<JoinOrder>(QueryAggregateFunction.Sum, null!, alias: "TotalAmount"));

            Assert.That(exception!.ParamName, Is.EqualTo("selector"));
        }

        private static void AssertGroupByNullSelector<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var exception = Assert.Throws<ArgumentNullException>(() =>
                queryBuilder
                    .From<JoinOrder>(alias: "o")
                    .GroupBy<JoinOrder>(null!));

            Assert.That(exception!.ParamName, Is.EqualTo("selector"));
        }

        private static void AssertHavingNullSelector<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var exception = Assert.Throws<ArgumentNullException>(() =>
                queryBuilder
                    .From<JoinOrder>(alias: "o")
                    .HavingAggregate<JoinOrder>(QueryAggregateFunction.Sum, null!, QueryComparisonOperator.GreaterThan, 100));

            Assert.That(exception!.ParamName, Is.EqualTo("selector"));
        }

        private static void AssertHavingNullValue<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<JoinOrder>(alias: "o")
                .SelectAggregate<JoinOrder>(QueryAggregateFunction.Count, o => o.Id, alias: "OrderCount")
                .HavingAggregate<JoinOrder>(QueryAggregateFunction.Count, o => o.Id, QueryComparisonOperator.Equal, null)
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(query.Parameters, Has.Count.EqualTo(1));
                QueryAssertionHelper.AssertParameter(query.Parameters, 0, null!);
            });
        }
    }
}
