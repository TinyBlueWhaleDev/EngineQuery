using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Tests.Models;

namespace TinyBlueWhale.EngineQuery.Tests.Infrastructure
{
    /// <summary>
    /// Provides provider-shared snapshot tests for successful SQL generation features.
    /// </summary>
    public abstract class QueryCompilerFeatureSnapshotTests : QueryCompilerProviderTestBase
    {
        [Test]
        public void ToSql_Should_Match_Snapshot_For_Select_All()
        {
            var sql = CreateQueryBuilder()
                .From<User>("Users")
                .Build();

            AssertSnapshot(
                "select_all",
                sql);
        }

        [Test]
        public void ToSql_Should_Match_Snapshot_For_Select_Projection()
        {
            var sql = CreateQueryBuilder()
                .From<User>("Users")
                .Select<User>(x => new
                {
                    x.Id,
                    x.Email
                })
                .Build();

            AssertSnapshot(
                "select_projection",
                sql);
        }

        [Test]
        public void ToSql_Should_Match_Snapshot_For_Where_Boolean_And_String_Methods()
        {
            var sql = CreateQueryBuilder()
                .From<User>("Users")
                .Where<User>(x =>
                    x.IsActive &&
                    x.Email.Contains("@gmail.com") &&
                    x.Age >= 18)
                .Build();

            AssertSnapshot(
                "where_boolean_string_methods",
                sql);
        }

        [Test]
        public void ToSql_Should_Match_Snapshot_For_Where_Or()
        {
            var sql = CreateQueryBuilder()
                .From<User>("Users")
                .Where<User>(x =>
                    x.Email.Contains("@gmail.com") ||
                    x.Email.Contains("@company.com"))
                .Build();

            AssertSnapshot(
                "where_or",
                sql);
        }

        [Test]
        public void ToSql_Should_Match_Snapshot_For_WhereIf()
        {
            var sql = CreateQueryBuilder()
                .From<User>("Users")
                .WhereIf<User>(true, x => x.IsActive)
                .WhereIf<User>(false, x => x.IsDeleted)
                .Build();

            AssertSnapshot(
                "where_if",
                sql);
        }

        [Test]
        public void ToSql_Should_Match_Snapshot_For_Order_And_Pagination()
        {
            var sql = CreateQueryBuilder()
                .From<User>("Users")
                .OrderBy<User>(x => x.Email)
                .ThenByDescending<User>(x => x.CreatedAt)
                .Skip(20)
                .Take(10)
                .Build();

            AssertSnapshot(
                "order_pagination",
                sql);
        }

        [Test]
        public void ToSql_Should_Match_Snapshot_For_Distinct()
        {
            var sql = CreateQueryBuilder()
                .From<JoinUser>(alias: "u")
                .Distinct()
                .Select<JoinUser>(u => new
                {
                    u.Email
                })
                .Build();

            AssertSnapshot(
                "distinct",
                sql);
        }

        [Test]
        public void ToSql_Should_Match_Snapshot_For_Joins()
        {
            var sql = CreateQueryBuilder()
                .From<JoinUser>(alias: "u")
                .InnerJoin<JoinUser, JoinOrder>(
                    alias: "o",
                    on: (u, o) => u.Id == o.UserId)
                .LeftJoin<JoinOrder, JoinOrderItem>(
                    alias: "oi",
                    on: (o, oi) => o.Id == oi.OrderId)
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .Build();

            AssertSnapshot(
                "joins",
                sql);
        }

        [Test]
        public void ToSql_Should_Match_Snapshot_For_GroupBy_Aggregates_And_Having()
        {
            var sql = CreateQueryBuilder()
                .From<JoinUser>(alias: "u")
                .InnerJoin<JoinUser, JoinOrder>(
                    alias: "o",
                    on: (u, o) => u.Id == o.UserId)
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .SelectAggregate<JoinOrder>(
                    QueryAggregateFunction.Sum,
                    o => o.Total,
                    "TotalAmount")
                .SelectAggregate<JoinOrder>(
                    QueryAggregateFunction.Count,
                    o => o.Id,
                    "OrderCount")
                .GroupBy<JoinUser>(u => new
                {
                    u.Id,
                    u.Email
                })
                .HavingAggregate<JoinOrder>(
                    QueryAggregateFunction.Sum,
                    o => o.Total,
                    QueryComparisonOperator.GreaterThan,
                    1000)
                .Build();

            AssertSnapshot(
                "groupby_aggregate_having",
                sql);
        }

        [Test]
        public void ToSql_Should_Match_Snapshot_For_Scalar_Functions()
        {
            var sql = CreateQueryBuilder()
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id
                })
                .SelectFunction<JoinUser>(
                    QueryScalarFunction.Upper,
                    u => u.Email,
                    "NormalizedEmail")
                .SelectFunction<JoinUser>(
                    QueryScalarFunction.Length,
                    u => u.Email,
                    "EmailLength")
                .Build();

            AssertSnapshot(
                "scalar_functions",
                sql);
        }

        [Test]
        public void ToSql_Should_Match_Snapshot_For_Computed_Expressions()
        {
            var sql = CreateQueryBuilder()
                .From<JoinOrder>(alias: "o")
                .Select<JoinOrder>(o => new
                {
                    OrderId = o.Id,
                    o.Total
                })
                .SelectComputed<JoinOrder>(
                    o => o.Total * 1.16m,
                    "TotalWithTax")
                .WhereComputed<JoinOrder>(
                    o => o.Total * 1.16m > 1000)
                .Build();

            AssertSnapshot(
                "computed_expressions",
                sql);
        }

        [Test]
        public void ToSql_Should_Match_Snapshot_For_Case_When()
        {
            var sql = CreateQueryBuilder()
                .From<JoinOrder>(alias: "o")
                .Select<JoinOrder>(o => new
                {
                    OrderId = o.Id,
                    o.Total
                })
                .SelectCase<JoinOrder>(
                    condition: o => o.Total > 1000,
                    whenTrue: "VIP",
                    whenFalse: "STANDARD",
                    alias: "CustomerType")
                .Build();

            AssertSnapshot(
                "case_when",
                sql);
        }

        [Test]
        public void ToSql_Should_Match_Snapshot_For_Exists_NotExists_And_In()
        {
            var sql = CreateQueryBuilder()
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .WhereExists<JoinUser, JoinOrder>(
                    alias: "o",
                    subquery => subquery
                        .WhereComputed<JoinOrder, JoinUser>((o, u) =>
                            o.UserId == u.Id &&
                            o.Total > 100))
                .WhereNotExists<JoinUser, JoinOrder>(
                    alias: "o2",
                    subquery => subquery
                        .WhereComputed<JoinOrder, JoinUser>((o, u) =>
                            o.UserId == u.Id &&
                            o.Total <= 0))
                .WhereIn<JoinUser, JoinOrder>(
                    u => u.Id,
                    alias: "oi",
                    subquery => subquery
                        .Select<JoinOrder>(o => new
                        {
                            o.UserId
                        })
                        .Where<JoinOrder>(o => o.Total > 500))
                .Build();

            AssertSnapshot(
                "exists_notexists_in",
                sql);
        }

        [Test]
        public void ToSql_Should_Match_Snapshot_For_Derived_Table()
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
                            QueryAggregateFunction.Sum,
                            o => o.Total,
                            "TotalAmount")
                        .SelectAggregate<JoinOrder>(
                            QueryAggregateFunction.Count,
                            o => o.Id,
                            "OrderCount")
                        .GroupBy<JoinOrder>(o => o.UserId))
                .Select<OrderSummary>(summary => new
                {
                    summary.UserId,
                    summary.TotalAmount,
                    summary.OrderCount
                })
                .Build();

            AssertSnapshot(
                "derived_table",
                sql);
        }

        [Test]
        public void ToSql_Should_Match_Snapshot_For_Cte()
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
                            QueryAggregateFunction.Sum,
                            o => o.Total,
                            "TotalAmount")
                        .SelectAggregate<JoinOrder>(
                            QueryAggregateFunction.Count,
                            o => o.Id,
                            "OrderCount")
                        .GroupBy<JoinOrder>(o => o.UserId))
                .FromCte<OrderSummary>("order_summary")
                .Select<OrderSummary>(summary => new
                {
                    summary.UserId,
                    summary.TotalAmount,
                    summary.OrderCount
                })
                .Build();

            AssertSnapshot(
                "cte",
                sql);
        }

        [Test]
        public void ToSql_Should_Match_Snapshot_For_Recursive_Cte()
        {
            var sql = CreateQueryBuilder()
                .WithRecursive<CategoryTree, Category, Category>(
                    name: "category_tree",
                    baseQueryBuilder: baseQuery => baseQuery
                        .From<Category>(alias: "c")
                        .Select<Category>(c => new
                        {
                            c.Id,
                            c.ParentId,
                            c.Name
                        })
                        .Where<Category>(c => c.ParentId == null),
                    recursiveQueryBuilder: recursiveQuery => recursiveQuery
                        .From<Category>(alias: "c")
                        .InnerJoin<Category, CategoryTree>(
                            alias: "ct",
                            on: (c, ct) => c.ParentId == ct.Id)
                        .Select<Category>(c => new
                        {
                            c.Id,
                            c.ParentId,
                            c.Name
                        }))
                .FromCte<CategoryTree>("category_tree")
                .Select<CategoryTree>(tree => new
                {
                    tree.Id,
                    tree.ParentId,
                    tree.Name
                })
                .Build();

            AssertSnapshot(
                "recursive_cte",
                sql);
        }

        [Test]
        public void ToSql_Should_Match_Snapshot_For_Set_Operations()
        {
            var sql = CreateQueryBuilder()
                .From<ActiveUser>(alias: "u")
                .Select<ActiveUser>(u => new
                {
                    u.Email
                })
                .UnionAll<ArchivedUser>(set => set
                    .From<ArchivedUser>(alias: "a")
                    .Select<ArchivedUser>(a => new
                    {
                        a.Email
                    }))
                .Intersect<ArchivedUser>(set => set
                    .From<ArchivedUser>(alias: "a2")
                    .Select<ArchivedUser>(a => new
                    {
                        a.Email
                    }))
                .Except<ArchivedUser>(set => set
                    .From<ArchivedUser>(alias: "a3")
                    .Select<ArchivedUser>(a => new
                    {
                        a.Email
                    }))
                .Build();

            AssertSnapshot(
                "set_operations",
                sql);
        }

        [Test]
        public void ToSql_Should_Match_Snapshot_For_Apply_Lateral()
        {
            var sql = CreateQueryBuilder()
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .CrossApply<JoinUser, JoinOrder>(
                    alias: "latest_order",
                    apply => apply
                        .Select<JoinOrder>(o => new
                        {
                            OrderId = o.Id,
                            o.UserId,
                            o.Total
                        })
                        .WhereComputed<JoinOrder, JoinUser>((o, u) =>
                            o.UserId == u.Id)
                        .OrderByDescending<JoinOrder>(o => o.Total)
                        .Take(1))
                .Build();

            AssertSnapshot(
                "apply_lateral",
                sql);
        }

        [Test]
        public void ToSql_Should_Match_Snapshot_For_Window_Functions()
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
                    "RowNumber",
                    window => window
                        .PartitionBy<JoinOrder>(o => o.UserId)
                        .OrderByDescending<JoinOrder>(o => o.Total))
                .SelectRank(
                    "OrderRank",
                    window => window
                        .PartitionBy<JoinOrder>(o => o.UserId)
                        .OrderByDescending<JoinOrder>(o => o.Total))
                .SelectDenseRank(
                    "DenseOrderRank",
                    window => window
                        .PartitionBy<JoinOrder>(o => o.UserId)
                        .OrderByDescending<JoinOrder>(o => o.Total))
                .SelectLag<JoinOrder>(
                    o => o.Total,
                    "PreviousOrderTotal",
                    window => window
                        .PartitionBy<JoinOrder>(o => o.UserId)
                        .OrderBy<JoinOrder>(o => o.Id))
                .SelectLead<JoinOrder>(
                    o => o.Total,
                    "NextOrderTotal",
                    window => window
                        .PartitionBy<JoinOrder>(o => o.UserId)
                        .OrderBy<JoinOrder>(o => o.Id))
                .SelectFirstValue<JoinOrder>(
                    o => o.Total,
                    "FirstOrderTotal",
                    window => window
                        .PartitionBy<JoinOrder>(o => o.UserId)
                        .OrderBy<JoinOrder>(o => o.Id))
                .SelectLastValue<JoinOrder>(
                    o => o.Total,
                    "LastOrderTotal",
                    window => window
                        .PartitionBy<JoinOrder>(o => o.UserId)
                        .OrderBy<JoinOrder>(o => o.Id))
                .SelectNtile(
                    4,
                    "OrderQuartile",
                    window => window
                        .PartitionBy<JoinOrder>(o => o.UserId)
                        .OrderByDescending<JoinOrder>(o => o.Total))
                .Build();

            AssertSnapshot(
                "window_functions",
                sql);
        }

        [Test]
        public void ToSql_Should_Be_Deterministic()
        {
            var query = CreateQueryBuilder()
                .From<User>("Users")
                .Where<User>(x => x.IsActive);

            var sql1 = query.Build();
            var sql2 = query.Build();

            Assert.Multiple(() =>
            {
                Assert.That(sql1.CommandText, Is.EqualTo(sql2.CommandText));
                Assert.That(sql1.Parameters.Count, Is.EqualTo(sql2.Parameters.Count));
                Assert.That(sql1.Parameters[0].Name, Is.EqualTo(sql2.Parameters[0].Name));
                Assert.That(sql1.Parameters[0].Value, Is.EqualTo(sql2.Parameters[0].Value));
            });
        }
    }
}
