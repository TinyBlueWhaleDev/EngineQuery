using TinyBlueWhale.EngineQuery.Tests.Infrastructure;
using TinyBlueWhale.EngineQuery.Tests.Models;
using TinyBlueWhale.EngineQuery.Tests.Providers;

namespace TinyBlueWhale.EngineQuery.Tests.QueryBuilding.WindowFunctions
{
    /// <summary>
    /// Validates provider-independent window function behavior.
    /// </summary>
    //[TestFixtureSource(typeof(QueryTestProviderSource), nameof(QueryTestProviderSource.GetProviders))]
    //internal sealed class WindowFunctionTests(IQueryTestProvider provider)
    //{
    //    private readonly IQueryTestProvider _provider = provider ?? throw new ArgumentNullException(nameof(provider));

    //    /// <summary>
    //    /// Validates ranking window function generation.
    //    /// </summary>
    //    [Test]
    //    public void Build_WhenRankingFunctionsAreConfigured_ShouldGenerateExpectedFunctions()
    //    {
    //        var query = _provider
    //            .CreateQueryBuilder()
    //            .From<JoinOrder>(alias: "o")
    //            .Select<JoinOrder>(order => new
    //            {
    //                OrderId = order.Id,
    //                order.UserId,
    //                order.Total
    //            })
    //            .SelectRowNumber(
    //                "RowNumber",
    //                window => window
    //                    .PartitionBy<JoinOrder>(order => order.UserId)
    //                    .OrderByDescending<JoinOrder>(order => order.Total))
    //            .SelectRank(
    //                "OrderRank",
    //                window => window
    //                    .PartitionBy<JoinOrder>(order => order.UserId)
    //                    .OrderByDescending<JoinOrder>(order => order.Total))
    //            .SelectDenseRank(
    //                "DenseOrderRank",
    //                window => window
    //                    .PartitionBy<JoinOrder>(order => order.UserId)
    //                    .OrderByDescending<JoinOrder>(order => order.Total))
    //            .Build();

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(query.CommandText, Does.Contain("ROW_NUMBER"));
    //            Assert.That(query.CommandText, Does.Contain("RANK"));
    //            Assert.That(query.CommandText, Does.Contain("DENSE_RANK"));

    //            Assert.That(query.CommandText, Does.Contain("RowNumber"));
    //            Assert.That(query.CommandText, Does.Contain("OrderRank"));
    //            Assert.That(query.CommandText, Does.Contain("DenseOrderRank"));

    //            Assert.That(query.CommandText, Does.Contain("PARTITION BY"));
    //            Assert.That(query.CommandText, Does.Contain("ORDER BY"));
    //        });
    //    }

    //    /// <summary>
    //    /// Validates offset window function generation.
    //    /// </summary>
    //    [Test]
    //    public void Build_WhenLagAndLeadAreConfigured_ShouldGenerateExpectedFunctions()
    //    {
    //        var query = _provider
    //            .CreateQueryBuilder()
    //            .From<JoinOrder>(alias: "o")
    //            .Select<JoinOrder>(order => new
    //            {
    //                OrderId = order.Id,
    //                order.Total
    //            })
    //            .SelectLag<JoinOrder>(
    //                order => order.Total,
    //                "PreviousOrderTotal",
    //                window => window
    //                    .PartitionBy<JoinOrder>(order => order.UserId)
    //                    .OrderBy<JoinOrder>(order => order.Id))
    //            .SelectLead<JoinOrder>(
    //                order => order.Total,
    //                "NextOrderTotal",
    //                window => window
    //                    .PartitionBy<JoinOrder>(order => order.UserId)
    //                    .OrderBy<JoinOrder>(order => order.Id))
    //            .Build();

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(query.CommandText, Does.Contain("LAG"));
    //            Assert.That(query.CommandText, Does.Contain("LEAD"));

    //            Assert.That(query.CommandText, Does.Contain("PreviousOrderTotal"));
    //            Assert.That(query.CommandText, Does.Contain("NextOrderTotal"));

    //            Assert.That(query.CommandText, Does.Contain("PARTITION BY"));
    //            Assert.That(query.CommandText, Does.Contain("ORDER BY"));
    //        });
    //    }

    //    /// <summary>
    //    /// Validates value window function generation.
    //    /// </summary>
    //    [Test]
    //    public void Build_WhenFirstAndLastValueAreConfigured_ShouldGenerateExpectedFunctions()
    //    {
    //        var query = _provider
    //            .CreateQueryBuilder()
    //            .From<JoinOrder>(alias: "o")
    //            .SelectFirstValue<JoinOrder>(
    //                order => order.Total,
    //                "FirstOrderTotal",
    //                window => window
    //                    .PartitionBy<JoinOrder>(order => order.UserId)
    //                    .OrderBy<JoinOrder>(order => order.Id))
    //            .SelectLastValue<JoinOrder>(
    //                order => order.Total,
    //                "LastOrderTotal",
    //                window => window
    //                    .PartitionBy<JoinOrder>(order => order.UserId)
    //                    .OrderBy<JoinOrder>(order => order.Id))
    //            .Build();

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(query.CommandText, Does.Contain("FIRST_VALUE"));
    //            Assert.That(query.CommandText, Does.Contain("LAST_VALUE"));

    //            Assert.That(query.CommandText, Does.Contain("FirstOrderTotal"));
    //            Assert.That(query.CommandText, Does.Contain("LastOrderTotal"));
    //        });
    //    }

    //    /// <summary>
    //    /// Validates NTILE window function generation.
    //    /// </summary>
    //    [Test]
    //    public void Build_WhenNtileIsConfigured_ShouldGenerateBucketFunction()
    //    {
    //        var query = _provider
    //            .CreateQueryBuilder()
    //            .From<JoinOrder>(alias: "o")
    //            .SelectNtile(
    //                4,
    //                "OrderQuartile",
    //                window => window
    //                    .PartitionBy<JoinOrder>(order => order.UserId)
    //                    .OrderByDescending<JoinOrder>(order => order.Total))
    //            .Build();

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(query.CommandText, Does.Contain("NTILE"));
    //            Assert.That(query.CommandText, Does.Contain("OrderQuartile"));
    //            Assert.That(query.CommandText, Does.Contain("PARTITION BY"));
    //            Assert.That(query.CommandText, Does.Contain("ORDER BY"));
    //        });
    //    }

    //    /// <summary>
    //    /// Validates that multiple window function types can coexist
    //    /// in the same query definition.
    //    /// </summary>
    //    [Test]
    //    public void Build_WhenMultipleWindowFunctionsAreConfigured_ShouldGenerateAllFunctions()
    //    {
    //        var query = _provider
    //            .CreateQueryBuilder()
    //            .From<JoinOrder>(alias: "o")
    //            .Select<JoinOrder>(order => new
    //            {
    //                OrderId = order.Id,
    //                order.UserId,
    //                order.Total
    //            })
    //            .SelectRowNumber(
    //                "RowNumber",
    //                window => window
    //                    .PartitionBy<JoinOrder>(order => order.UserId)
    //                    .OrderByDescending<JoinOrder>(order => order.Total))
    //            .SelectRank(
    //                "OrderRank",
    //                window => window
    //                    .PartitionBy<JoinOrder>(order => order.UserId)
    //                    .OrderByDescending<JoinOrder>(order => order.Total))
    //            .SelectDenseRank(
    //                "DenseOrderRank",
    //                window => window
    //                    .PartitionBy<JoinOrder>(order => order.UserId)
    //                    .OrderByDescending<JoinOrder>(order => order.Total))
    //            .SelectLag<JoinOrder>(
    //                order => order.Total,
    //                "PreviousOrderTotal",
    //                window => window
    //                    .PartitionBy<JoinOrder>(order => order.UserId)
    //                    .OrderBy<JoinOrder>(order => order.Id))
    //            .SelectLead<JoinOrder>(
    //                order => order.Total,
    //                "NextOrderTotal",
    //                window => window
    //                    .PartitionBy<JoinOrder>(order => order.UserId)
    //                    .OrderBy<JoinOrder>(order => order.Id))
    //            .SelectFirstValue<JoinOrder>(
    //                order => order.Total,
    //                "FirstOrderTotal",
    //                window => window
    //                    .PartitionBy<JoinOrder>(order => order.UserId)
    //                    .OrderBy<JoinOrder>(order => order.Id))
    //            .SelectLastValue<JoinOrder>(
    //                order => order.Total,
    //                "LastOrderTotal",
    //                window => window
    //                    .PartitionBy<JoinOrder>(order => order.UserId)
    //                    .OrderBy<JoinOrder>(order => order.Id))
    //            .SelectNtile(
    //                4,
    //                "OrderQuartile",
    //                window => window
    //                    .PartitionBy<JoinOrder>(order => order.UserId)
    //                    .OrderByDescending<JoinOrder>(order => order.Total))
    //            .Build();

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(query.CommandText, Does.Contain("ROW_NUMBER"));
    //            Assert.That(query.CommandText, Does.Contain("RANK"));
    //            Assert.That(query.CommandText, Does.Contain("DENSE_RANK"));
    //            Assert.That(query.CommandText, Does.Contain("LAG"));
    //            Assert.That(query.CommandText, Does.Contain("LEAD"));
    //            Assert.That(query.CommandText, Does.Contain("FIRST_VALUE"));
    //            Assert.That(query.CommandText, Does.Contain("LAST_VALUE"));
    //            Assert.That(query.CommandText, Does.Contain("NTILE"));
    //        });
    //    }

    //    /// <summary>
    //    /// Validates independent window definitions using different partition strategies.
    //    /// </summary>
    //    [Test]
    //    public void Build_WhenWindowFunctionsUseDifferentPartitions_ShouldPreserveDefinitions()
    //    {
    //        var query = _provider
    //            .CreateQueryBuilder()
    //            .From<JoinOrder>(alias: "o")
    //            .Select<JoinOrder>(order => new
    //            {
    //                OrderId = order.Id,
    //                order.UserId,
    //                order.Total
    //            })
    //            .SelectRowNumber(
    //                "UserRowNumber",
    //                window => window
    //                    .PartitionBy<JoinOrder>(order => order.UserId)
    //                    .OrderByDescending<JoinOrder>(order => order.Total))
    //            .SelectNtile(
    //                4,
    //                "GlobalQuartile",
    //                window => window
    //                    .OrderByDescending<JoinOrder>(order => order.Total))
    //            .Build();

    //        Assert.Multiple(() =>
    //        {
    //            Assert.That(query.CommandText, Does.Contain("ROW_NUMBER"));
    //            Assert.That(query.CommandText, Does.Contain("NTILE"));
    //            Assert.That(query.CommandText, Does.Contain("UserRowNumber"));
    //            Assert.That(query.CommandText, Does.Contain("GlobalQuartile"));
    //        });
    //    }

    //    /// <summary>
    //    /// Validates that NTILE requires a positive bucket count.
    //    /// </summary>
    //    [Test]
    //    public void SelectNtile_WhenBucketsAreZero_ShouldThrow()
    //    {
    //        var builder = _provider
    //            .CreateQueryBuilder()
    //            .From<JoinOrder>(alias: "o");

    //        Assert.Throws<ArgumentOutOfRangeException>(() =>
    //            builder.SelectNtile(
    //                0,
    //                "Quartile",
    //                window => window.OrderBy<JoinOrder>(order => order.Id)));
    //    }

    //    /// <summary>
    //    /// Validates that LAG requires a positive offset.
    //    /// </summary>
    //    [Test]
    //    public void SelectLag_WhenOffsetIsZero_ShouldThrow()
    //    {
    //        var builder = _provider
    //            .CreateQueryBuilder()
    //            .From<JoinOrder>(alias: "o");

    //        Assert.Throws<ArgumentOutOfRangeException>(() =>
    //            builder.SelectLag<JoinOrder>(
    //                order => order.Total,
    //                "PreviousTotal",
    //                window => window.OrderBy<JoinOrder>(order => order.Id),
    //                offset: 0));
    //    }

    //    /// <summary>
    //    /// Validates that LEAD requires a positive offset.
    //    /// </summary>
    //    [Test]
    //    public void SelectLead_WhenOffsetIsNegative_ShouldThrow()
    //    {
    //        var builder = _provider
    //            .CreateQueryBuilder()
    //            .From<JoinOrder>(alias: "o");

    //        Assert.Throws<ArgumentOutOfRangeException>(() =>
    //            builder.SelectLead<JoinOrder>(
    //                order => order.Total,
    //                "NextTotal",
    //                window => window.OrderBy<JoinOrder>(order => order.Id),
    //                offset: -1));
    //    }

    //    /// <summary>
    //    /// Validates that window function aliases cannot contain whitespace-only values.
    //    /// </summary>
    //    [Test]
    //    public void SelectRowNumber_WhenAliasIsWhitespace_ShouldThrow()
    //    {
    //        var builder = _provider
    //            .CreateQueryBuilder()
    //            .From<JoinOrder>(alias: "o");

    //        Assert.Throws<ArgumentException>(() =>
    //            builder.SelectRowNumber(
    //                " ",
    //                window => window.OrderBy<JoinOrder>(order => order.Id)));
    //    }

    //    /// <summary>
    //    /// Validates that ROW_NUMBER requires an ORDER BY definition.
    //    /// </summary>
    //    [Test]
    //    public void SelectRowNumber_WhenOrderByIsMissing_ShouldThrow()
    //    {
    //        var builder = _provider
    //            .CreateQueryBuilder()
    //            .From<JoinOrder>(alias: "o");

    //        Assert.Throws<InvalidOperationException>(() =>
    //            builder.SelectRowNumber(
    //                "RowNumber",
    //                window => window.PartitionBy<JoinOrder>(order => order.UserId)));
    //    }

    //    /// <summary>
    //    /// Validates that window functions cannot be compiled when
    //    /// the current provider does not support them.
    //    /// </summary>
    //    [Test]
    //    public void Build_WhenWindowFunctionsAreNotSupported_ShouldThrow()
    //    {
    //        var builder = _provider.CreateQueryBuilder(
    //            new UnsupportedWindowFunctionCapabilities());

    //        var exception = Assert.Throws<NotSupportedException>(() =>
    //            builder
    //                .From<JoinOrder>(alias: "o")
    //                .SelectNtile(
    //                    4,
    //                    "Quartile",
    //                    window => window.OrderByDescending<JoinOrder>(order => order.Total))
    //                .Build());

    //        Assert.That(
    //            exception!.Message,
    //            Is.EqualTo("Window functions are not supported by the current provider."));
    //    }
    //}
}
