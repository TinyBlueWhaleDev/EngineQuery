using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding;
using TinyBlueWhale.EngineQuery.Tests.Models;

namespace TinyBlueWhale.EngineQuery.Tests.Infrastructure
{
    /// <summary>
    /// Provides provider-shared negative tests for SQL generation validation.
    /// </summary>
    public abstract class QueryCompilerNegativeTestBase : QueryCompilerProviderTestBase
    {
        /// <summary>
        /// Creates a provider-specific query builder without window function support.
        /// </summary>
        protected abstract QueryBuilder CreateQueryBuilderWithoutWindowFunctions();

        /// <summary>
        /// Creates a provider-specific query builder without LATERAL or APPLY support.
        /// </summary>
        protected abstract QueryBuilder CreateQueryBuilderWithoutLateralJoins();

        /// <summary>
        /// Creates a provider-specific query builder without INTERSECT and EXCEPT support.
        /// </summary>
        protected abstract QueryBuilder CreateQueryBuilderWithoutSetOperations();

        /// <summary>
        /// Creates a provider-specific query builder without recursive CTE support.
        /// </summary>
        protected abstract QueryBuilder CreateQueryBuilderWithoutRecursiveCte();

        [Test]
        public void SelectNtile_Should_Throw_When_Buckets_Are_Zero()
        {
            var builder = CreateQueryBuilder()
                .From<JoinOrder>(alias: "o");

            var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
                builder.SelectNtile(
                    0,
                    "Quartile",
                    window => window.OrderBy<JoinOrder>(o => o.Id)));

            Assert.That(exception, Is.Not.Null);
        }

        [Test]
        public void SelectLag_Should_Throw_When_Offset_Is_Zero()
        {
            var builder = CreateQueryBuilder()
                .From<JoinOrder>(alias: "o");

            var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
                builder.SelectLag<JoinOrder>(
                    o => o.Total,
                    "PreviousTotal",
                    window => window.OrderBy<JoinOrder>(o => o.Id),
                    offset: 0));

            Assert.That(exception, Is.Not.Null);
        }

        [Test]
        public void SelectLead_Should_Throw_When_Offset_Is_Negative()
        {
            var builder = CreateQueryBuilder()
                .From<JoinOrder>(alias: "o");

            var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
                builder.SelectLead<JoinOrder>(
                    o => o.Total,
                    "NextTotal",
                    window => window.OrderBy<JoinOrder>(o => o.Id),
                    offset: -1));

            Assert.That(exception, Is.Not.Null);
        }

        [Test]
        public void SelectRowNumber_Should_Throw_When_Alias_Is_Whitespace()
        {
            var builder = CreateQueryBuilder()
                .From<JoinOrder>(alias: "o");

            var exception = Assert.Throws<ArgumentException>(() =>
                builder.SelectRowNumber(
                    " ",
                    window => window.OrderBy<JoinOrder>(o => o.Id)));

            Assert.That(exception, Is.Not.Null);
        }

        [Test]
        public void SelectRowNumber_Should_Throw_When_Window_Has_No_OrderBy()
        {
            var builder = CreateQueryBuilder()
                .From<JoinOrder>(alias: "o");

            var exception = Assert.Throws<InvalidOperationException>(() =>
                builder.SelectRowNumber(
                    "RowNumber",
                    window => window.PartitionBy<JoinOrder>(o => o.UserId)));

            Assert.That(exception, Is.Not.Null);
        }

        [Test]
        public void Build_Should_Throw_When_Pagination_Has_No_OrderBy()
        {
            var exception = Assert.Throws<InvalidOperationException>(() =>
                CreateQueryBuilder()
                    .From<User>("Users")
                    .Skip(10)
                    .Take(5)
                    .Build());

            Assert.That(exception, Is.Not.Null);
        }

        [Test]
        public void From_Should_Throw_When_Metadata_Is_Not_Registered()
        {
            var exception = Assert.Throws<InvalidOperationException>(() =>
                CreateQueryBuilder()
                    .From<UnmappedEntity>("x"));

            Assert.That(exception, Is.Not.Null);
        }

        [Test]
        public void Build_Should_Throw_When_Window_Functions_Are_Not_Supported()
        {
            var builder = CreateQueryBuilderWithoutWindowFunctions();

            var exception = Assert.Throws<NotSupportedException>(() =>
                builder
                    .From<JoinOrder>(alias: "o")
                    .SelectNtile(
                        4,
                        "Quartile",
                        window => window.OrderByDescending<JoinOrder>(o => o.Total))
                    .Build());

            Assert.That(exception!.Message, Is.EqualTo("Window functions are not supported by the current provider."));
        }

        [Test]
        public void Build_Should_Throw_When_Lateral_Joins_Are_Not_Supported()
        {
            var builder = CreateQueryBuilderWithoutLateralJoins();

            var exception = Assert.Throws<NotSupportedException>(() =>
                builder
                    .From<JoinUser>(alias: "u")
                    .CrossApply<JoinUser, JoinOrder>(
                        alias: "latest_order",
                        apply => apply
                            .Select<JoinOrder>(o => new
                            {
                                OrderId = o.Id
                            })
                            .WhereComputed<JoinOrder, JoinUser>((o, u) => o.UserId == u.Id))
                    .Build());

            Assert.That(exception!.Message, Is.EqualTo("APPLY or LATERAL joins are not supported by the current provider."));
        }

        [Test]
        public void Build_Should_Throw_When_Intersect_Is_Not_Supported()
        {
            var builder = CreateQueryBuilderWithoutSetOperations();

            var exception = Assert.Throws<NotSupportedException>(() =>
                builder
                    .From<ActiveUser>(alias: "u")
                    .Select<ActiveUser>(u => new
                    {
                        u.Email
                    })
                    .Intersect<ArchivedUser>(set => set
                        .From<ArchivedUser>(alias: "a")
                        .Select<ArchivedUser>(a => new
                        {
                            a.Email
                        }))
                    .Build());

            Assert.That(exception!.Message, Is.EqualTo("INTERSECT set operations are not supported by the current provider."));
        }

        [Test]
        public void Build_Should_Throw_When_Except_Is_Not_Supported()
        {
            var builder = CreateQueryBuilderWithoutSetOperations();

            var exception = Assert.Throws<NotSupportedException>(() =>
                builder
                    .From<ActiveUser>(alias: "u")
                    .Select<ActiveUser>(u => new
                    {
                        u.Email
                    })
                    .Except<ArchivedUser>(set => set
                        .From<ArchivedUser>(alias: "a")
                        .Select<ArchivedUser>(a => new
                        {
                            a.Email
                        }))
                    .Build());

            Assert.That(exception!.Message, Is.EqualTo("EXCEPT set operations are not supported by the current provider."));
        }

        [Test]
        public void Build_Should_Throw_When_Recursive_Cte_Is_Not_Supported()
        {
            var builder = CreateQueryBuilderWithoutRecursiveCte();

            var exception = Assert.Throws<NotSupportedException>(() =>
                builder
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
                    .Build());

            Assert.That(exception!.Message, Is.EqualTo("Recursive common table expressions are not supported by the current provider."));
        }

        private sealed class UnmappedEntity
        {
            public int Id { get; set; }
        }
    }
}

