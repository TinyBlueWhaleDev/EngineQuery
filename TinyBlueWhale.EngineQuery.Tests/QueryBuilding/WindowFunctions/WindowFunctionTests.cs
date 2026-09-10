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

namespace TinyBlueWhale.EngineQuery.Tests.QueryBuilding.WindowFunctions
{
    /// <summary>
    /// Validates window function behavior exposed through supported provider profiles.
    /// </summary>
    [TestFixture]
    internal sealed class WindowFunctionTests
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
        public void Build_WhenRankingFunctionsAreConfigured_ShouldGenerateExpectedFunctions()
        {
            var sqlServer = _sqlServer
                .From<JoinOrder>(alias: "o")
                .Select<JoinOrder>(order => new
                {
                    OrderId = order.Id,
                    order.UserId,
                    order.Total
                })
                .SelectRowNumber(
                    "RowNumber",
                    window => window
                        .PartitionBy<JoinOrder>(order => order.UserId)
                        .OrderByDescending<JoinOrder>(order => order.Total))
                .SelectRank(
                    "OrderRank",
                    window => window
                        .PartitionBy<JoinOrder>(order => order.UserId)
                        .OrderByDescending<JoinOrder>(order => order.Total))
                .SelectDenseRank(
                    "DenseOrderRank",
                    window => window
                        .PartitionBy<JoinOrder>(order => order.UserId)
                        .OrderByDescending<JoinOrder>(order => order.Total))
                .Build();

            var postgreSql = _postgreSql
                .From<JoinOrder>(alias: "o")
                .Select<JoinOrder>(order => new
                {
                    OrderId = order.Id,
                    order.UserId,
                    order.Total
                })
                .SelectRowNumber(
                    "RowNumber",
                    window => window
                        .PartitionBy<JoinOrder>(order => order.UserId)
                        .OrderByDescending<JoinOrder>(order => order.Total))
                .SelectRank(
                    "OrderRank",
                    window => window
                        .PartitionBy<JoinOrder>(order => order.UserId)
                        .OrderByDescending<JoinOrder>(order => order.Total))
                .SelectDenseRank(
                    "DenseOrderRank",
                    window => window
                        .PartitionBy<JoinOrder>(order => order.UserId)
                        .OrderByDescending<JoinOrder>(order => order.Total))
                .Build();

            var mySql = _mySql
                .From<JoinOrder>(alias: "o")
                .Select<JoinOrder>(order => new
                {
                    OrderId = order.Id,
                    order.UserId,
                    order.Total
                })
                .SelectRowNumber(
                    "RowNumber",
                    window => window
                        .PartitionBy<JoinOrder>(order => order.UserId)
                        .OrderByDescending<JoinOrder>(order => order.Total))
                .SelectRank(
                    "OrderRank",
                    window => window
                        .PartitionBy<JoinOrder>(order => order.UserId)
                        .OrderByDescending<JoinOrder>(order => order.Total))
                .SelectDenseRank(
                    "DenseOrderRank",
                    window => window
                        .PartitionBy<JoinOrder>(order => order.UserId)
                        .OrderByDescending<JoinOrder>(order => order.Total))
                .Build();

            AssertRankingFunctions(sqlServer);
            AssertRankingFunctions(postgreSql);
            AssertRankingFunctions(mySql);
        }

        [Test]
        public void Build_WhenLagAndLeadAreConfigured_ShouldGenerateExpectedFunctions()
        {
            var sqlServer = _sqlServer
                .From<JoinOrder>(alias: "o")
                .SelectLag(
                    (JoinOrder order) => order.Total,
                    "PreviousOrderTotal",
                    window => window
                        .PartitionBy<JoinOrder>(order => order.UserId)
                        .OrderBy<JoinOrder>(order => order.Id))
                .SelectLead(
                    (JoinOrder order) => order.Total,
                    "NextOrderTotal",
                    window => window
                        .PartitionBy<JoinOrder>(order => order.UserId)
                        .OrderBy<JoinOrder>(order => order.Id))
                .Build();

            var postgreSql = _postgreSql
                .From<JoinOrder>(alias: "o")
                .SelectLag(
                     (JoinOrder order) => order.Total,
                    "PreviousOrderTotal",
                    window => window
                        .PartitionBy<JoinOrder>(order => order.UserId)
                        .OrderBy<JoinOrder>(order => order.Id))
                .SelectLead(
                     (JoinOrder order) => order.Total,
                    "NextOrderTotal",
                    window => window
                        .PartitionBy<JoinOrder>(order => order.UserId)
                        .OrderBy<JoinOrder>(order => order.Id))
                .Build();

            var mySql = _mySql
                .From<JoinOrder>(alias: "o")
                .SelectLag(
                     (JoinOrder order) => order.Total,
                    "PreviousOrderTotal",
                    window => window
                        .PartitionBy<JoinOrder>(order => order.UserId)
                        .OrderBy<JoinOrder>(order => order.Id))
                .SelectLead(
                     (JoinOrder order) => order.Total,
                    "NextOrderTotal",
                    window => window
                        .PartitionBy<JoinOrder>(order => order.UserId)
                        .OrderBy<JoinOrder>(order => order.Id))
                .Build();

            AssertLagAndLead(sqlServer);
            AssertLagAndLead(postgreSql);
            AssertLagAndLead(mySql);
        }

        [Test]
        public void Build_WhenFirstAndLastValueAreConfigured_ShouldGenerateExpectedFunctions()
        {
            var sqlServer = _sqlServer
                .From<JoinOrder>(alias: "o")
                .SelectFirstValue(
                     (JoinOrder order) => order.Total,
                    "FirstOrderTotal",
                    window => window
                        .PartitionBy<JoinOrder>(order => order.UserId)
                        .OrderBy<JoinOrder>(order => order.Id))
                .SelectLastValue(
                     (JoinOrder order) => order.Total,
                    "LastOrderTotal",
                    window => window
                        .PartitionBy<JoinOrder>(order => order.UserId)
                        .OrderBy<JoinOrder>(order => order.Id))
                .Build();

            var postgreSql = _postgreSql
                .From<JoinOrder>(alias: "o")
                .SelectFirstValue(
                    (JoinOrder order) => order.Total,
                    "FirstOrderTotal",
                    window => window
                        .PartitionBy<JoinOrder>(order => order.UserId)
                        .OrderBy<JoinOrder>(order => order.Id))
                .SelectLastValue(
                    (JoinOrder order) => order.Total,
                    "LastOrderTotal",
                    window => window
                        .PartitionBy<JoinOrder>(order => order.UserId)
                        .OrderBy<JoinOrder>(order => order.Id))
                .Build();

            var mySql = _mySql
                .From<JoinOrder>(alias: "o")
                .SelectFirstValue(
                     (JoinOrder order) => order.Total,
                    "FirstOrderTotal",
                    window => window
                        .PartitionBy<JoinOrder>(order => order.UserId)
                        .OrderBy<JoinOrder>(order => order.Id))
                .SelectLastValue(
                     (JoinOrder order) => order.Total,
                    "LastOrderTotal",
                    window => window
                        .PartitionBy<JoinOrder>(order => order.UserId)
                        .OrderBy<JoinOrder>(order => order.Id))
                .Build();

            AssertValueFunctions(sqlServer);
            AssertValueFunctions(postgreSql);
            AssertValueFunctions(mySql);
        }

        [Test]
        public void Build_WhenNtileIsConfigured_ShouldGenerateBucketFunction()
        {
            var sqlServer = _sqlServer
                .From<JoinOrder>(alias: "o")
                .SelectNtile(
                    4,
                    "OrderQuartile",
                    window => window
                        .PartitionBy<JoinOrder>(order => order.UserId)
                        .OrderByDescending<JoinOrder>(order => order.Total))
                .Build();

            var postgreSql = _postgreSql
                .From<JoinOrder>(alias: "o")
                .SelectNtile(
                    4,
                    "OrderQuartile",
                    window => window
                        .PartitionBy<JoinOrder>(order => order.UserId)
                        .OrderByDescending<JoinOrder>(order => order.Total))
                .Build();

            var mySql = _mySql
                .From<JoinOrder>(alias: "o")
                .SelectNtile(
                    4,
                    "OrderQuartile",
                    window => window
                        .PartitionBy<JoinOrder>(order => order.UserId)
                        .OrderByDescending<JoinOrder>(order => order.Total))
                .Build();

            AssertNtile(sqlServer);
            AssertNtile(postgreSql);
            AssertNtile(mySql);
        }

        [Test]
        public void Build_WhenMultipleWindowFunctionsAreConfigured_ShouldGenerateAllFunctions()
        {
            var sqlServer = _sqlServer
                .From<JoinOrder>(alias: "o")
                .SelectRowNumber("RowNumber", window => window.OrderBy<JoinOrder>(order => order.Id))
                .SelectRank("OrderRank", window => window.OrderBy<JoinOrder>(order => order.Id))
                .SelectDenseRank("DenseOrderRank", window => window.OrderBy<JoinOrder>(order => order.Id))
                .SelectLag((JoinOrder order) => order.Total, "PreviousTotal", window => window.OrderBy<JoinOrder>(order => order.Id))
                .SelectLead((JoinOrder order) => order.Total, "NextTotal", window => window.OrderBy<JoinOrder>(order => order.Id))
                .SelectFirstValue((JoinOrder order) => order.Total, "FirstTotal", window => window.OrderBy<JoinOrder>(order => order.Id))
                .SelectLastValue((JoinOrder order) => order.Total, "LastTotal", window => window.OrderBy<JoinOrder>(order => order.Id))
                .SelectNtile(4, "Quartile", window => window.OrderBy<JoinOrder>(order => order.Id))
                .Build();

            var postgreSql = _postgreSql
                .From<JoinOrder>(alias: "o")
                .SelectRowNumber("RowNumber", window => window.OrderBy<JoinOrder>(order => order.Id))
                .SelectRank("OrderRank", window => window.OrderBy<JoinOrder>(order => order.Id))
                .SelectDenseRank("DenseOrderRank", window => window.OrderBy<JoinOrder>(order => order.Id))
                .SelectLag((JoinOrder order) => order.Total, "PreviousTotal", window => window.OrderBy<JoinOrder>(order => order.Id))
                .SelectLead((JoinOrder order) => order.Total, "NextTotal", window => window.OrderBy<JoinOrder>(order => order.Id))
                .SelectFirstValue((JoinOrder order) => order.Total, "FirstTotal", window => window.OrderBy<JoinOrder>(order => order.Id))
                .SelectLastValue((JoinOrder order) => order.Total, "LastTotal", window => window.OrderBy<JoinOrder>(order => order.Id))
                .SelectNtile(4, "Quartile", window => window.OrderBy<JoinOrder>(order => order.Id))
                .Build();

            var mySql = _mySql
                .From<JoinOrder>(alias: "o")
                .SelectRowNumber("RowNumber", window => window.OrderBy<JoinOrder>(order => order.Id))
                .SelectRank("OrderRank", window => window.OrderBy<JoinOrder>(order => order.Id))
                .SelectDenseRank("DenseOrderRank", window => window.OrderBy<JoinOrder>(order => order.Id))
                .SelectLag((JoinOrder order) => order.Total, "PreviousTotal", window => window.OrderBy<JoinOrder>(order => order.Id))
                .SelectLead((JoinOrder order) => order.Total, "NextTotal", window => window.OrderBy<JoinOrder>(order => order.Id))
                .SelectFirstValue((JoinOrder order) => order.Total, "FirstTotal", window => window.OrderBy<JoinOrder>(order => order.Id))
                .SelectLastValue((JoinOrder order) => order.Total, "LastTotal", window => window.OrderBy<JoinOrder>(order => order.Id))
                .SelectNtile(4, "Quartile", window => window.OrderBy<JoinOrder>(order => order.Id))
                .Build();

            AssertAllFunctions(sqlServer);
            AssertAllFunctions(postgreSql);
            AssertAllFunctions(mySql);
        }

        [Test]
        public void Build_WhenWindowFunctionsUseDifferentPartitions_ShouldPreserveDefinitions()
        {
            var sqlServer = _sqlServer
                .From<JoinOrder>(alias: "o")
                .SelectRowNumber(
                    "UserRowNumber",
                    window => window
                        .PartitionBy<JoinOrder>(order => order.UserId)
                        .OrderByDescending<JoinOrder>(order => order.Total))
                .SelectNtile(
                    4,
                    "GlobalQuartile",
                    window => window.OrderByDescending<JoinOrder>(order => order.Total))
                .Build();

            var postgreSql = _postgreSql
                .From<JoinOrder>(alias: "o")
                .SelectRowNumber(
                    "UserRowNumber",
                    window => window
                        .PartitionBy<JoinOrder>(order => order.UserId)
                        .OrderByDescending<JoinOrder>(order => order.Total))
                .SelectNtile(
                    4,
                    "GlobalQuartile",
                    window => window.OrderByDescending<JoinOrder>(order => order.Total))
                .Build();

            var mySql = _mySql
                .From<JoinOrder>(alias: "o")
                .SelectRowNumber(
                    "UserRowNumber",
                    window => window
                        .PartitionBy<JoinOrder>(order => order.UserId)
                        .OrderByDescending<JoinOrder>(order => order.Total))
                .SelectNtile(
                    4,
                    "GlobalQuartile",
                    window => window.OrderByDescending<JoinOrder>(order => order.Total))
                .Build();

            AssertDifferentWindowDefinitions(sqlServer);
            AssertDifferentWindowDefinitions(postgreSql);
            AssertDifferentWindowDefinitions(mySql);
        }

        [Test]
        public void SelectNtile_WhenBucketsAreZero_ShouldThrowArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _sqlServer
                .From<JoinOrder>(alias: "o")
                .SelectNtile(0, "Quartile", window => window.OrderBy<JoinOrder>(order => order.Id)));

            Assert.Throws<ArgumentOutOfRangeException>(() => _postgreSql
                .From<JoinOrder>(alias: "o")
                .SelectNtile(0, "Quartile", window => window.OrderBy<JoinOrder>(order => order.Id)));

            Assert.Throws<ArgumentOutOfRangeException>(() => _mySql
                .From<JoinOrder>(alias: "o")
                .SelectNtile(0, "Quartile", window => window.OrderBy<JoinOrder>(order => order.Id)));
        }

        [Test]
        public void SelectLag_WhenOffsetIsZero_ShouldThrowArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _sqlServer
                .From<JoinOrder>(alias: "o")
                .SelectLag((JoinOrder order) => order.Total, "PreviousTotal", window => window.OrderBy<JoinOrder>(order => order.Id), offset: 0));

            Assert.Throws<ArgumentOutOfRangeException>(() => _postgreSql
                .From<JoinOrder>(alias: "o")
                .SelectLag((JoinOrder order) => order.Total, "PreviousTotal", window => window.OrderBy<JoinOrder>(order => order.Id), offset: 0));

            Assert.Throws<ArgumentOutOfRangeException>(() => _mySql
                .From<JoinOrder>(alias: "o")
                .SelectLag((JoinOrder order) => order.Total, "PreviousTotal", window => window.OrderBy<JoinOrder>(order => order.Id), offset: 0));
        }

        [Test]
        public void SelectLead_WhenOffsetIsNegative_ShouldThrowArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _sqlServer
                .From<JoinOrder>(alias: "o")
                .SelectLead((JoinOrder order) => order.Total, "NextTotal", window => window.OrderBy<JoinOrder>(order => order.Id), offset: -1));

            Assert.Throws<ArgumentOutOfRangeException>(() => _postgreSql
                .From<JoinOrder>(alias: "o")
                .SelectLead((JoinOrder order) => order.Total, "NextTotal", window => window.OrderBy<JoinOrder>(order => order.Id), offset: -1));

            Assert.Throws<ArgumentOutOfRangeException>(() => _mySql
                .From<JoinOrder>(alias: "o")
                .SelectLead((JoinOrder order) => order.Total, "NextTotal", window => window.OrderBy<JoinOrder>(order => order.Id), offset: -1));
        }

        [Test]
        public void SelectRowNumber_WhenAliasIsWhitespace_ShouldThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _sqlServer
                .From<JoinOrder>(alias: "o")
                .SelectRowNumber(" ", window => window.OrderBy<JoinOrder>(order => order.Id)));

            Assert.Throws<ArgumentException>(() => _postgreSql
                .From<JoinOrder>(alias: "o")
                .SelectRowNumber(" ", window => window.OrderBy<JoinOrder>(order => order.Id)));

            Assert.Throws<ArgumentException>(() => _mySql
                .From<JoinOrder>(alias: "o")
                .SelectRowNumber(" ", window => window.OrderBy<JoinOrder>(order => order.Id)));
        }

        [Test]
        public void SelectRowNumber_WhenOrderByIsMissing_ShouldThrowInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => _sqlServer
                .From<JoinOrder>(alias: "o")
                .SelectRowNumber("RowNumber", window => window.PartitionBy<JoinOrder>(order => order.UserId)));

            Assert.Throws<InvalidOperationException>(() => _postgreSql
                .From<JoinOrder>(alias: "o")
                .SelectRowNumber("RowNumber", window => window.PartitionBy<JoinOrder>(order => order.UserId)));

            Assert.Throws<InvalidOperationException>(() => _mySql
                .From<JoinOrder>(alias: "o")
                .SelectRowNumber("RowNumber", window => window.PartitionBy<JoinOrder>(order => order.UserId)));
        }

        private static void AssertRankingFunctions(GeneratedSqlQuery query)
        {
            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain("ROW_NUMBER"));
                Assert.That(sql, Does.Contain("RANK"));
                Assert.That(sql, Does.Contain("DENSE_RANK"));
                Assert.That(sql, Does.Contain("RowNumber"));
                Assert.That(sql, Does.Contain("OrderRank"));
                Assert.That(sql, Does.Contain("DenseOrderRank"));
                Assert.That(sql, Does.Contain("PARTITION BY"));
                Assert.That(sql, Does.Contain("ORDER BY"));
                Assert.That(query.Parameters, Is.Empty);
            });
        }

        private static void AssertLagAndLead(GeneratedSqlQuery query)
        {
            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain("LAG(o.total, @p0)"));
                Assert.That(sql, Does.Contain("LEAD(o.total, @p1)"));
                Assert.That(sql, Does.Contain("PARTITION BY o.user_id"));
                Assert.That(sql, Does.Contain("ORDER BY o.order_id"));
                Assert.That(query.Parameters, Has.Count.EqualTo(2));
                QueryAssertionHelper.AssertParameter(query.Parameters, 0, 1);
                QueryAssertionHelper.AssertParameter(query.Parameters, 1, 1);
            });
        }

        private static void AssertValueFunctions(GeneratedSqlQuery query)
        {
            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain("FIRST_VALUE"));
                Assert.That(sql, Does.Contain("LAST_VALUE"));
                Assert.That(sql, Does.Contain("FirstOrderTotal"));
                Assert.That(sql, Does.Contain("LastOrderTotal"));
                Assert.That(query.Parameters, Is.Empty);
            });
        }

        private static void AssertNtile(GeneratedSqlQuery query)
        {
            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain("NTILE(@p0)"));
                Assert.That(sql, Does.Contain("PARTITION BY o.user_id"));
                Assert.That(sql, Does.Contain("ORDER BY o.total DESC"));
                Assert.That(sql, Does.Contain("AS OrderQuartile"));
                Assert.That(query.Parameters, Has.Count.EqualTo(1));
                QueryAssertionHelper.AssertParameter(query.Parameters, 0, 4);
            });
        }

        private static void AssertAllFunctions(GeneratedSqlQuery query)
        {
            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain("ROW_NUMBER"));
                Assert.That(sql, Does.Contain("RANK"));
                Assert.That(sql, Does.Contain("DENSE_RANK"));
                Assert.That(sql, Does.Contain("LAG"));
                Assert.That(sql, Does.Contain("LEAD"));
                Assert.That(sql, Does.Contain("FIRST_VALUE"));
                Assert.That(sql, Does.Contain("LAST_VALUE"));
                Assert.That(sql, Does.Contain("NTILE"));
                Assert.That(query.Parameters, Has.Count.EqualTo(3));
                QueryAssertionHelper.AssertParameter(query.Parameters, 0, 1);
                QueryAssertionHelper.AssertParameter(query.Parameters, 1, 1);
                QueryAssertionHelper.AssertParameter(query.Parameters, 2, 4);
            });
        }

        private static void AssertDifferentWindowDefinitions(GeneratedSqlQuery query)
        {
            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain("ROW_NUMBER() OVER (PARTITION BY o.user_id"));
                Assert.That(sql, Does.Contain("ORDER BY o.total DESC"));
                Assert.That(sql, Does.Contain("AS UserRowNumber"));

                Assert.That(sql, Does.Contain("NTILE(@p0) OVER (ORDER BY o.total DESC)"));
                Assert.That(sql, Does.Contain("AS GlobalQuartile"));

                Assert.That(query.Parameters, Has.Count.EqualTo(1));
                QueryAssertionHelper.AssertParameter(query.Parameters, 0, 4);
            });
        }
    }
}
