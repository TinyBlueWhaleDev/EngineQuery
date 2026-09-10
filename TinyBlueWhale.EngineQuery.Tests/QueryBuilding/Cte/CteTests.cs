using TinyBlueWhale.EngineQuery.Abstractions.Enums;
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

namespace TinyBlueWhale.EngineQuery.Tests.QueryBuilding.CTE
{
    /// <summary>
    /// Validates common table expression behavior exposed through supported provider profiles.
    /// </summary>
    [TestFixture]
    internal sealed class CteTests
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
        public void Build_WhenCteIsConfigured_ShouldGenerateCte()
        {
            var sqlServer = BuildCte(_sqlServer);
            var postgreSql = BuildCte(_postgreSql);
            var mySql = BuildCte(_mySql);

            AssertCte(sqlServer);
            AssertCte(postgreSql);
            AssertCte(mySql);
        }

        [Test]
        public void Build_WhenRecursiveCteIsConfigured_ShouldGenerateRecursiveCte()
        {
            var sqlServer = _sqlServer
                .WithRecursive<CategoryTree, Category, Category>(
                    name: "category_tree",
                    baseQueryBuilder: baseQuery => baseQuery
                        .From<Category>(alias: "c")
                        .Select<Category>(category => new
                        {
                            category.Id,
                            category.ParentId,
                            category.Name
                        })
                        .Where<Category>(category => category.ParentId == null),
                    recursiveQueryBuilder: recursiveQuery => recursiveQuery
                        .From<Category>(alias: "c")
                        .InnerJoin<Category, CategoryTree>(
                            alias: "ct",
                            on: (category, tree) => category.ParentId == tree.Id)
                        .Select<Category>(category => new
                        {
                            category.Id,
                            category.ParentId,
                            category.Name
                        }))
                .FromCte<CategoryTree>("category_tree")
                .Select<CategoryTree>(tree => new
                {
                    tree.Id,
                    tree.ParentId,
                    tree.Name
                })
                .Build();

            var postgreSql = _postgreSql
                .WithRecursive<CategoryTree, Category, Category>(
                    name: "category_tree",
                    baseQueryBuilder: baseQuery => baseQuery
                        .From<Category>(alias: "c")
                        .Select<Category>(category => new
                        {
                            category.Id,
                            category.ParentId,
                            category.Name
                        })
                        .Where<Category>(category => category.ParentId == null),
                    recursiveQueryBuilder: recursiveQuery => recursiveQuery
                        .From<Category>(alias: "c")
                        .InnerJoin<Category, CategoryTree>(
                            alias: "ct",
                            on: (category, tree) => category.ParentId == tree.Id)
                        .Select<Category>(category => new
                        {
                            category.Id,
                            category.ParentId,
                            category.Name
                        }))
                .FromCte<CategoryTree>("category_tree")
                .Select<CategoryTree>(tree => new
                {
                    tree.Id,
                    tree.ParentId,
                    tree.Name
                })
                .Build();

            var mySql = _mySql
                .WithRecursive<CategoryTree, Category, Category>(
                    name: "category_tree",
                    baseQueryBuilder: baseQuery => baseQuery
                        .From<Category>(alias: "c")
                        .Select<Category>(category => new
                        {
                            category.Id,
                            category.ParentId,
                            category.Name
                        })
                        .Where<Category>(category => category.ParentId == null),
                    recursiveQueryBuilder: recursiveQuery => recursiveQuery
                        .From<Category>(alias: "c")
                        .InnerJoin<Category, CategoryTree>(
                            alias: "ct",
                            on: (category, tree) => category.ParentId == tree.Id)
                        .Select<Category>(category => new
                        {
                            category.Id,
                            category.ParentId,
                            category.Name
                        }))
                .FromCte<CategoryTree>("category_tree")
                .Select<CategoryTree>(tree => new
                {
                    tree.Id,
                    tree.ParentId,
                    tree.Name
                })
                .Build();

            AssertRecursiveCte(sqlServer);
            AssertRecursiveCte(postgreSql);
            AssertRecursiveCte(mySql);
        }

        [Test]
        public void Build_WhenQueriesReuseBuilder_ShouldNotLeakPreviousCteDefinitions()
        {
            AssertCteDoesNotLeak(_sqlServer);
            AssertCteDoesNotLeak(_postgreSql);
            AssertCteDoesNotLeak(_mySql);
        }

        [Test]
        public void Build_WhenCteAndOuterQueryContainParameters_ShouldPreserveParameterOrder()
        {
            var sqlServer = BuildParameterizedCte(_sqlServer);
            var postgreSql = BuildParameterizedCte(_postgreSql);
            var mySql = BuildParameterizedCte(_mySql);

            AssertCteParameters(sqlServer);
            AssertCteParameters(postgreSql);
            AssertCteParameters(mySql);
        }

        [Test]
        public void With_WhenNameIsWhitespace_ShouldThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _sqlServer
                .With<OrderSummary, JoinOrder>(
                    " ",
                    cte => cte
                        .From<JoinOrder>()
                        .Select<JoinOrder>(order => order.UserId)));

            Assert.Throws<ArgumentException>(() => _postgreSql
                .With<OrderSummary, JoinOrder>(
                    " ",
                    cte => cte
                        .From<JoinOrder>()
                        .Select<JoinOrder>(order => order.UserId)));

            Assert.Throws<ArgumentException>(() => _mySql
                .With<OrderSummary, JoinOrder>(
                    " ",
                    cte => cte
                        .From<JoinOrder>()
                        .Select<JoinOrder>(order => order.UserId)));
        }

        [Test]
        public void FromCte_WhenNameIsWhitespace_ShouldThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _sqlServer
                .With<OrderSummary, JoinOrder>(
                    "order_summary",
                    cte => cte
                        .From<JoinOrder>()
                        .Select<JoinOrder>(order => new
                        {
                            UserId = order.UserId
                        }))
                .FromCte<OrderSummary>(" "));

            Assert.Throws<ArgumentException>(() => _postgreSql
                .With<OrderSummary, JoinOrder>(
                    "order_summary",
                    cte => cte
                        .From<JoinOrder>()
                        .Select<JoinOrder>(order => new
                        {
                            UserId = order.UserId
                        }))
                .FromCte<OrderSummary>(" "));

            Assert.Throws<ArgumentException>(() => _mySql
                .With<OrderSummary, JoinOrder>(
                    "order_summary",
                    cte => cte
                        .From<JoinOrder>()
                        .Select<JoinOrder>(order => new
                        {
                            UserId = order.UserId
                        }))
                .FromCte<OrderSummary>(" "));
        }

        [Test]
        public void WithRecursive_WhenNameIsWhitespace_ShouldThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _sqlServer
                .WithRecursive<CategoryTree, Category, Category>(
                    name: " ",
                    baseQueryBuilder: query => query
                        .From<Category>()
                        .Select<Category>(category => new
                        {
                            category.Id,
                            category.ParentId,
                            category.Name
                        }),
                    recursiveQueryBuilder: query => query
                        .From<Category>()
                        .Select<Category>(category => new
                        {
                            category.Id,
                            category.ParentId,
                            category.Name
                        })));

            Assert.Throws<ArgumentException>(() => _postgreSql
                .WithRecursive<CategoryTree, Category, Category>(
                    name: " ",
                    baseQueryBuilder: query => query
                        .From<Category>()
                        .Select<Category>(category => new
                        {
                            category.Id,
                            category.ParentId,
                            category.Name
                        }),
                    recursiveQueryBuilder: query => query
                        .From<Category>()
                        .Select<Category>(category => new
                        {
                            category.Id,
                            category.ParentId,
                            category.Name
                        })));

            Assert.Throws<ArgumentException>(() => _mySql
                .WithRecursive<CategoryTree, Category, Category>(
                    name: " ",
                    baseQueryBuilder: query => query
                        .From<Category>()
                        .Select<Category>(category => new
                        {
                            category.Id,
                            category.ParentId,
                            category.Name
                        }),
                    recursiveQueryBuilder: query => query
                        .From<Category>()
                        .Select<Category>(category => new
                        {
                            category.Id,
                            category.ParentId,
                            category.Name
                        })));
        }

        private static GeneratedSqlQuery BuildCte(QueryBuilder<SqlServer2012Profile> queryBuilder)
        {
            return queryBuilder
                .With<OrderSummary, JoinOrder>(
                    "order_summary",
                    cte => cte
                        .From<JoinOrder>(alias: "o")
                        .Select<JoinOrder>(order => new
                        {
                            UserId = order.UserId
                        })
                        .SelectAggregate<JoinOrder>(QueryAggregateFunction.Sum, order => order.Total, "TotalAmount")
                        .SelectAggregate<JoinOrder>(QueryAggregateFunction.Count, order => order.Id, "OrderCount")
                        .GroupBy<JoinOrder>(order => order.UserId))
                .FromCte<OrderSummary>("order_summary")
                .Select<OrderSummary>(summary => new
                {
                    summary.UserId,
                    summary.TotalAmount,
                    summary.OrderCount
                })
                .Build();
        }

        private static GeneratedSqlQuery BuildCte(QueryBuilder<PostgreSql93Profile> queryBuilder)
        {
            return queryBuilder
                .With<OrderSummary, JoinOrder>(
                    "order_summary",
                    cte => cte
                        .From<JoinOrder>(alias: "o")
                        .Select<JoinOrder>(order => new
                        {
                            UserId = order.UserId
                        })
                        .SelectAggregate<JoinOrder>(QueryAggregateFunction.Sum, order => order.Total, "TotalAmount")
                        .SelectAggregate<JoinOrder>(QueryAggregateFunction.Count, order => order.Id, "OrderCount")
                        .GroupBy<JoinOrder>(order => order.UserId))
                .FromCte<OrderSummary>("order_summary")
                .Select<OrderSummary>(summary => new
                {
                    summary.UserId,
                    summary.TotalAmount,
                    summary.OrderCount
                })
                .Build();
        }

        private static GeneratedSqlQuery BuildCte(QueryBuilder<MySql8031Profile> queryBuilder)
        {
            return queryBuilder
                .With<OrderSummary, JoinOrder>(
                    "order_summary",
                    cte => cte
                        .From<JoinOrder>(alias: "o")
                        .Select<JoinOrder>(order => new
                        {
                            UserId = order.UserId
                        })
                        .SelectAggregate<JoinOrder>(QueryAggregateFunction.Sum, order => order.Total, "TotalAmount")
                        .SelectAggregate<JoinOrder>(QueryAggregateFunction.Count, order => order.Id, "OrderCount")
                        .GroupBy<JoinOrder>(order => order.UserId))
                .FromCte<OrderSummary>("order_summary")
                .Select<OrderSummary>(summary => new
                {
                    summary.UserId,
                    summary.TotalAmount,
                    summary.OrderCount
                })
                .Build();
        }

        private static GeneratedSqlQuery BuildParameterizedCte(QueryBuilder<SqlServer2012Profile> queryBuilder)
        {
            return queryBuilder
                .With<OrderSummary, JoinOrder>(
                    "order_summary",
                    cte => cte
                        .From<JoinOrder>(alias: "o")
                        .Select<JoinOrder>(order => new
                        {
                            UserId = order.UserId
                        })
                        .SelectAggregate<JoinOrder>(QueryAggregateFunction.Sum, order => order.Total, "TotalAmount")
                        .SelectAggregate<JoinOrder>(QueryAggregateFunction.Count, order => order.Id, "OrderCount")
                        .Where<JoinOrder>(order => order.Total > 100m)
                        .GroupBy<JoinOrder>(order => order.UserId))
                .FromCte<OrderSummary>("order_summary")
                .Select<OrderSummary>(summary => new
                {
                    summary.UserId,
                    summary.TotalAmount,
                    summary.OrderCount
                })
                .WhereComputed<OrderSummary>(summary => summary.TotalAmount > 500m)
                .Build();
        }

        private static GeneratedSqlQuery BuildParameterizedCte(QueryBuilder<PostgreSql93Profile> queryBuilder)
        {
            return queryBuilder
                .With<OrderSummary, JoinOrder>(
                    "order_summary",
                    cte => cte
                        .From<JoinOrder>(alias: "o")
                        .Select<JoinOrder>(order => new
                        {
                            UserId = order.UserId
                        })
                        .SelectAggregate<JoinOrder>(QueryAggregateFunction.Sum, order => order.Total, "TotalAmount")
                        .SelectAggregate<JoinOrder>(QueryAggregateFunction.Count, order => order.Id, "OrderCount")
                        .Where<JoinOrder>(order => order.Total > 100m)
                        .GroupBy<JoinOrder>(order => order.UserId))
                .FromCte<OrderSummary>("order_summary")
                .Select<OrderSummary>(summary => new
                {
                    summary.UserId,
                    summary.TotalAmount,
                    summary.OrderCount
                })
                .WhereComputed<OrderSummary>(summary => summary.TotalAmount > 500m)
                .Build();
        }

        private static GeneratedSqlQuery BuildParameterizedCte(QueryBuilder<MySql8031Profile> queryBuilder)
        {
            return queryBuilder
                .With<OrderSummary, JoinOrder>(
                    "order_summary",
                    cte => cte
                        .From<JoinOrder>(alias: "o")
                        .Select<JoinOrder>(order => new
                        {
                            UserId = order.UserId
                        })
                        .SelectAggregate<JoinOrder>(QueryAggregateFunction.Sum, order => order.Total, "TotalAmount")
                        .SelectAggregate<JoinOrder>(QueryAggregateFunction.Count, order => order.Id, "OrderCount")
                        .Where<JoinOrder>(order => order.Total > 100m)
                        .GroupBy<JoinOrder>(order => order.UserId))
                .FromCte<OrderSummary>("order_summary")
                .Select<OrderSummary>(summary => new
                {
                    summary.UserId,
                    summary.TotalAmount,
                    summary.OrderCount
                })
                .WhereComputed<OrderSummary>(summary => summary.TotalAmount > 500m)
                .Build();
        }

        private static void AssertCteDoesNotLeak(QueryBuilder<SqlServer2012Profile> queryBuilder)
        {
            var firstQuery = queryBuilder
                .With<OrderSummary, JoinOrder>(
                    "first_summary",
                    cte => cte
                        .From<JoinOrder>()
                        .Select<JoinOrder>(order => new
                        {
                            UserId = order.UserId
                        }))
                .FromCte<OrderSummary>("first_summary")
                .Build();

            var secondQuery = queryBuilder
                .With<OrderSummary, JoinOrder>(
                    "second_summary",
                    cte => cte
                        .From<JoinOrder>()
                        .Select<JoinOrder>(order => new
                        {
                            UserId = order.UserId
                        }))
                .FromCte<OrderSummary>("second_summary")
                .Build();

            AssertNoCteLeak(firstQuery, secondQuery);
        }

        private static void AssertCteDoesNotLeak(QueryBuilder<PostgreSql93Profile> queryBuilder)
        {
            var firstQuery = queryBuilder
                .With<OrderSummary, JoinOrder>(
                    "first_summary",
                    cte => cte
                        .From<JoinOrder>()
                        .Select<JoinOrder>(order => new
                        {
                            UserId = order.UserId
                        }))
                .FromCte<OrderSummary>("first_summary")
                .Build();

            var secondQuery = queryBuilder
                .With<OrderSummary, JoinOrder>(
                    "second_summary",
                    cte => cte
                        .From<JoinOrder>()
                        .Select<JoinOrder>(order => new
                        {
                            UserId = order.UserId
                        }))
                .FromCte<OrderSummary>("second_summary")
                .Build();

            AssertNoCteLeak(firstQuery, secondQuery);
        }

        private static void AssertCteDoesNotLeak(QueryBuilder<MySql8031Profile> queryBuilder)
        {
            var firstQuery = queryBuilder
                .With<OrderSummary, JoinOrder>(
                    "first_summary",
                    cte => cte
                        .From<JoinOrder>()
                        .Select<JoinOrder>(order => new
                        {
                            UserId = order.UserId
                        }))
                .FromCte<OrderSummary>("first_summary")
                .Build();

            var secondQuery = queryBuilder
                .With<OrderSummary, JoinOrder>(
                    "second_summary",
                    cte => cte
                        .From<JoinOrder>()
                        .Select<JoinOrder>(order => new
                        {
                            UserId = order.UserId
                        }))
                .FromCte<OrderSummary>("second_summary")
                .Build();

            AssertNoCteLeak(firstQuery, secondQuery);
        }

        private static void AssertCte(GeneratedSqlQuery query)
        {
            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain("WITH"));
                Assert.That(sql, Does.Contain("order_summary"));
                Assert.That(sql, Does.Contain("TotalAmount"));
                Assert.That(sql, Does.Contain("OrderCount"));
                Assert.That(sql, Does.Contain("GROUP BY"));
                Assert.That(query.Parameters, Is.Empty);
            });
        }

        private static void AssertRecursiveCte(GeneratedSqlQuery query)
        {
            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain("WITH"));
                Assert.That(sql, Does.Contain("category_tree"));
                Assert.That(sql, Does.Contain("UNION ALL"));
                Assert.That(sql, Does.Contain("INNER JOIN"));
                Assert.That(query.Parameters, Is.Empty);
            });
        }

        private static void AssertCteParameters(GeneratedSqlQuery query)
        {
            Assert.Multiple(() =>
            {
                Assert.That(query.Parameters, Has.Count.EqualTo(2));
                QueryAssertionHelper.AssertParameter(query.Parameters, 0, 500m);
                QueryAssertionHelper.AssertParameter(query.Parameters, 1, 100m);
            });
        }

        private static void AssertNoCteLeak(GeneratedSqlQuery firstQuery, GeneratedSqlQuery secondQuery)
        {
            Assert.Multiple(() =>
            {
                Assert.That(firstQuery.CommandText, Does.Contain("first_summary"));
                Assert.That(secondQuery.CommandText, Does.Contain("second_summary"));
                Assert.That(secondQuery.CommandText, Does.Not.Contain("first_summary"));
            });
        }
    }
}
