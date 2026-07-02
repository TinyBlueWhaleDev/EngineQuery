using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Tests.Models;

namespace TinyBlueWhale.EngineQuery.Tests.Infrastructure
{
    /// <summary>
    /// Provides provider-shared snapshot tests for SQL edge cases.
    /// </summary>
    public abstract class QueryCompilerEdgeSnapshotTests : QueryCompilerProviderTestBase
    {
        [Test]
        public void ToSql_Should_Match_Snapshot_For_Null_Comparisons()
        {
            var sql = CreateQueryBuilder()
                .From<Category>(alias: "c")
                .Select<Category>(c => new
                {
                    c.Id,
                    c.ParentId
                })
                .Where<Category>(c => c.ParentId == null || c.ParentId != null)
                .Build();

            AssertSnapshot(
                "edge_null_comparisons",
                sql);
        }

        [Test]
        public void ToSql_Should_Match_Snapshot_For_Nullable_Join_Conversion()
        {
            var sql = CreateQueryBuilder()
                .From<Category>(alias: "c")
                .InnerJoin<Category, CategoryTree>(
                    alias: "ct",
                    on: (c, ct) => c.ParentId == ct.Id)
                .Select<Category>(c => new
                {
                    c.Id,
                    c.ParentId
                })
                .Build();

            AssertSnapshot(
                "edge_nullable_join_conversion",
                sql);
        }

        [Test]
        public void ToSql_Should_Match_Snapshot_For_Correlated_In_Subquery()
        {
            var sql = CreateQueryBuilder()
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .WhereIn<JoinUser, JoinOrder>(
                    u => u.Id,
                    alias: "o",
                    subquery => subquery
                        .Select<JoinOrder>(o => new
                        {
                            o.UserId
                        })
                        .WhereComputed<JoinOrder, JoinUser>((o, u) => o.UserId == u.Id && o.Total > 100))
                .Build();

            AssertSnapshot(
                "edge_correlated_in_subquery",
                sql);
        }

        [Test]
        public void ToSql_Should_Match_Snapshot_For_Outer_Apply()
        {
            var sql = CreateQueryBuilder()
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .OuterApply<JoinUser, JoinOrder>(
                    alias: "latest_order",
                    apply => apply
                        .Select<JoinOrder>(o => new
                        {
                            OrderId = o.Id,
                            o.UserId,
                            o.Total
                        })
                        .WhereComputed<JoinOrder, JoinUser>((o, u) => o.UserId == u.Id)
                        .OrderByDescending<JoinOrder>(o => o.Total)
                        .Take(1))
                .Build();

            AssertSnapshot(
                "edge_outer_apply",
                sql);
        }

        [Test]
        public void ToSql_Should_Match_Snapshot_For_WhereIf_False_After_Existing_Where()
        {
            var sql = CreateQueryBuilder()
                .From<JoinUser>(alias: "u")
                .Where<JoinUser>(u => u.IsActive)
                .WhereIf<JoinUser>(
                    false,
                    u => u.Email.Contains("@blocked.com"))
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .Build();

            AssertSnapshot(
                "edge_where_if_false_after_existing_where",
                sql);
        }

        [Test]
        public void ToSql_Should_Match_Snapshot_For_Case_When_With_Logical_Or()
        {
            var sql = CreateQueryBuilder()
                .From<JoinOrder>(alias: "o")
                .Select<JoinOrder>(o => new
                {
                    OrderId = o.Id,
                    o.Total
                })
                .SelectCaseWhen<JoinOrder>(
                    condition: o => o.Total <= 0 || o.Total > 10000,
                    whenTrue: "REVIEW",
                    whenFalse: "NORMAL",
                    alias: "RiskStatus")
                .Build();

            AssertSnapshot(
                "edge_case_when_logical_or",
                sql);
        }

        [Test]
        public void ToSql_Should_Match_Snapshot_For_Derived_Table_With_Inner_Parameters()
        {
            var sql = CreateQueryBuilder()
                .FromSubquery<OrderSummary, JoinOrder>(
                    alias: "summary",
                    subquery => subquery
                        .From<JoinOrder>(alias: "o")
                        .Select<JoinOrder>(o => new
                        {
                            UserId = o.UserId
                        })
                        .SelectAggregate<JoinOrder>(
                            Abstractions.Enums.QueryAggregateFunction.Sum,
                            o => o.Total,
                            "TotalAmount")
                        .SelectAggregate<JoinOrder>(
                            Abstractions.Enums.QueryAggregateFunction.Count,
                            o => o.Id,
                            "OrderCount")
                        .Where<JoinOrder>(o => o.Total > 100)
                        .GroupBy<JoinOrder>(o => o.UserId))
                .Select<OrderSummary>(summary => new
                {
                    summary.UserId,
                    summary.TotalAmount,
                    summary.OrderCount
                })
                .WhereComputed<OrderSummary>(summary => summary.TotalAmount > 500)
                .Build();

            AssertSnapshot(
                "edge_derived_table_with_inner_parameters",
                sql);
        }

        [Test]
        public void ToSql_Should_Match_Snapshot_For_Cte_With_Inner_Parameters()
        {
            var sql = CreateQueryBuilder()
                .With<OrderSummary, JoinOrder>(
                    "order_summary",
                    cte => cte
                        .From<JoinOrder>(alias: "o")
                        .Select<JoinOrder>(o => new
                        {
                            UserId = o.UserId
                        })
                        .SelectAggregate<JoinOrder>(
                            Abstractions.Enums.QueryAggregateFunction.Sum,
                            o => o.Total,
                            "TotalAmount")
                        .SelectAggregate<JoinOrder>(
                            Abstractions.Enums.QueryAggregateFunction.Count,
                            o => o.Id,
                            "OrderCount")
                        .Where<JoinOrder>(o => o.Total > 100)
                        .GroupBy<JoinOrder>(o => o.UserId))
                .FromCte<OrderSummary>("order_summary")
                .Select<OrderSummary>(summary => new
                {
                    summary.UserId,
                    summary.TotalAmount,
                    summary.OrderCount
                })
                .WhereComputed<OrderSummary>(summary => summary.TotalAmount > 500)
                .Build();

            AssertSnapshot(
                "edge_cte_with_inner_parameters",
                sql);
        }

        [Test]
        public void ToSql_Should_Match_Snapshot_For_Multiple_Window_Partitions()
        {
            var sql = CreateQueryBuilder()
                .From<JoinOrder>(alias: "o")
                .Select<JoinOrder>(o => new
                {
                    OrderId = o.Id,
                    o.UserId,
                    o.Total
                })
                .SelectRowNumber(
                    "UserRowNumber",
                    window => window
                        .PartitionBy<JoinOrder>(o => o.UserId)
                        .OrderByDescending<JoinOrder>(o => o.Total))
                .SelectNtile(
                    4,
                    "GlobalQuartile",
                    window => window
                        .OrderByDescending<JoinOrder>(o => o.Total))
                .Build();

            AssertSnapshot(
                "edge_multiple_window_partitions",
                sql);
        }
    }
}
