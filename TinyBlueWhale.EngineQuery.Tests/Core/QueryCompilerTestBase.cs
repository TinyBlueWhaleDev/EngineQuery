using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding;
using TinyBlueWhale.EngineQuery.Tests.TestModels;

namespace TinyBlueWhale.EngineQuery.Tests.Core
{
    public abstract class QueryCompilerTestBase
    {
        protected abstract QueryEngine CreateQueryEngine();
        protected abstract IQueryCompilerExpectedSyntax ExpectedSql { get; }

        [Test]
        public void ToSql_Should_Generate_Select_All_When_No_Projection_Is_Defined()
        {
            var engine = CreateQueryEngine();

            var sql = engine.Query<User>("Users").ToSql();

            Assert.Multiple(() =>
            {
                Assert.That(sql.CommandText, Is.EqualTo(ExpectedSql.SelectAllSql));
                Assert.That(sql.Parameters, Is.Empty);
            });
        }

        [Test]
        public void ToSql_Should_Generate_Select_Projection_For_Multiple_Properties()
        {
            var engine = CreateQueryEngine();

            var sql = engine.Query<User>("Users")
                .Select(x => new { x.Id, x.Email })
                .ToSql();

            Assert.Multiple(() =>
            {
                Assert.That(sql.CommandText, Is.EqualTo(ExpectedSql.SelectProjectionSql));
                Assert.That(sql.Parameters, Is.Empty);
            });
        }

        [Test]
        public void ToSql_Should_Generate_Where_Clause_For_Boolean_Property()
        {
            var engine = CreateQueryEngine();

            var sql = engine.Query<User>("Users")
                .Where(x => x.IsActive)
                .ToSql();

            Assert.Multiple(() =>
            {
                Assert.That(sql.CommandText, Is.EqualTo(ExpectedSql.BooleanWhereSql));
                Assert.That(sql.Parameters, Has.Count.EqualTo(1));
                Assert.That(sql.Parameters[0].Value, Is.EqualTo(true));
            });
        }

        [Test]
        public void ToSql_Should_Generate_Where_Clause_For_Negated_Boolean_Property()
        {
            var engine = CreateQueryEngine();

            var sql = engine.Query<User>("Users")
                .Where(x => !x.IsDeleted)
                .ToSql();

            Assert.Multiple(() =>
            {
                Assert.That(sql.CommandText, Is.EqualTo(ExpectedSql.NegatedBooleanWhereSql));
                Assert.That(sql.Parameters, Has.Count.EqualTo(1));
                Assert.That(sql.Parameters[0].Value, Is.EqualTo(false));
            });
        }

        [Test]
        public void ToSql_Should_Generate_Where_Clause_For_Closure_Value()
        {
            var engine = CreateQueryEngine();
            var minimumAge = 18;

            var sql = engine.Query<User>("Users")
                .Where(x => x.Age >= minimumAge)
                .ToSql();

            Assert.Multiple(() =>
            {
                Assert.That(sql.CommandText, Is.EqualTo(ExpectedSql.ClosureWhereSql));
                Assert.That(sql.Parameters, Has.Count.EqualTo(1));
                Assert.That(sql.Parameters[0].Value, Is.EqualTo(18));
            });
        }

        [Test]
        public void ToSql_Should_Generate_Where_Clause_For_Contains_Expression()
        {
            var engine = CreateQueryEngine();

            var sql = engine.Query<User>("Users")
                .Where(x => x.Email.Contains("@gmail.com"))
                .ToSql();

            Assert.Multiple(() =>
            {
                Assert.That(sql.CommandText, Is.EqualTo(ExpectedSql.ContainsWhereSql));
                Assert.That(sql.Parameters, Has.Count.EqualTo(1));
                Assert.That(sql.Parameters[0].Value, Is.EqualTo("%@gmail.com%"));
            });
        }

        [Test]
        public void ToSql_Should_Generate_Where_Clause_For_StartsWith_Expression()
        {
            var engine = CreateQueryEngine();

            var sql = engine.Query<User>("Users")
                .Where(x => x.Email.StartsWith("admin"))
                .ToSql();

            Assert.Multiple(() =>
            {
                Assert.That(sql.CommandText, Is.EqualTo(ExpectedSql.StartsWithWhereSql));
                Assert.That(sql.Parameters, Has.Count.EqualTo(1));
                Assert.That(sql.Parameters[0].Value, Is.EqualTo("admin%"));
            });
        }

        [Test]
        public void ToSql_Should_Generate_Where_Clause_For_EndsWith_Expression()
        {
            var engine = CreateQueryEngine();

            var sql = engine.Query<User>("Users")
                .Where(x => x.Email.EndsWith(".com"))
                .ToSql();

            Assert.Multiple(() =>
            {
                Assert.That(sql.CommandText, Is.EqualTo(ExpectedSql.EndsWithWhereSql));
                Assert.That(sql.Parameters, Has.Count.EqualTo(1));
                Assert.That(sql.Parameters[0].Value, Is.EqualTo("%.com"));
            });
        }

        [Test]
        public void ToSql_Should_Generate_Where_Clause_For_Multiple_And_Conditions()
        {
            var engine = CreateQueryEngine();

            var sql = engine.Query<User>("Users")
                .Where(x => x.IsActive && x.Age >= 18 && x.Email.Contains("@gmail.com"))
                .ToSql();

            Assert.Multiple(() =>
            {
                Assert.That(sql.CommandText, Is.EqualTo(ExpectedSql.MultipleAndWhereSql));
                Assert.That(sql.Parameters, Has.Count.EqualTo(3));
                Assert.That(sql.Parameters[0].Value, Is.EqualTo(true));
                Assert.That(sql.Parameters[1].Value, Is.EqualTo(18));
                Assert.That(sql.Parameters[2].Value, Is.EqualTo("%@gmail.com%"));
            });
        }

        [Test]
        public void ToSql_Should_Generate_Where_Clause_For_Or_Conditions()
        {
            var engine = CreateQueryEngine();

            var sql = engine.Query<User>("Users")
                .Where(x => x.Email.Contains("@gmail.com") || x.Email.Contains("@company.com"))
                .ToSql();

            Assert.Multiple(() =>
            {
                Assert.That(sql.CommandText, Is.EqualTo(ExpectedSql.OrWhereSql));
                Assert.That(sql.Parameters, Has.Count.EqualTo(2));
                Assert.That(sql.Parameters[0].Value, Is.EqualTo("%@gmail.com%"));
                Assert.That(sql.Parameters[1].Value, Is.EqualTo("%@company.com%"));
            });
        }

        [Test]
        public void ToSql_Should_Generate_Order_By_Clause()
        {
            var engine = CreateQueryEngine();

            var sql = engine.Query<User>("Users")
                .OrderBy(x => x.Email)
                .ToSql();

            Assert.That(sql.CommandText, Is.EqualTo(ExpectedSql.OrderBySql));
        }

        [Test]
        public void ToSql_Should_Generate_Order_By_Descending_Clause()
        {
            var engine = CreateQueryEngine();

            var sql = engine.Query<User>("Users")
                .OrderByDescending(x => x.CreatedAt)
                .ToSql();

            Assert.That(sql.CommandText, Is.EqualTo(ExpectedSql.OrderByDescendingSql));
        }

        [Test]
        public void ToSql_Should_Generate_Then_By_Clause()
        {
            var engine = CreateQueryEngine();

            var sql = engine.Query<User>("Users")
                .OrderBy(x => x.Email)
                .ThenByDescending(x => x.CreatedAt)
                .ToSql();

            Assert.That(sql.CommandText, Is.EqualTo(ExpectedSql.ThenBySql));
        }

        [Test]
        public void ToSql_Should_Generate_Pagination_Clause()
        {
            var engine = CreateQueryEngine();

            var sql = engine.Query<User>("Users")
                .OrderBy(x => x.Id)
                .Skip(20)
                .Take(10)
                .ToSql();

            Assert.That(sql.CommandText, Is.EqualTo(ExpectedSql.PaginationSql));
        }

        [Test]
        public void ToSql_Should_Generate_Complete_Query_With_Select_Where_Order_And_Pagination()
        {
            var engine = CreateQueryEngine();

            var sql = engine.Query<User>("Users")
                .Select(x => new { x.Id, x.Email })
                .Where(x => x.IsActive && x.Email.Contains("@gmail.com"))
                .OrderByDescending(x => x.CreatedAt)
                .Skip(20)
                .Take(10)
                .ToSql();

            Assert.Multiple(() =>
            {
                Assert.That(sql.CommandText, Is.EqualTo(ExpectedSql.CompleteQuerySql));
                Assert.That(sql.Parameters, Has.Count.EqualTo(2));
                Assert.That(sql.Parameters[0].Value, Is.EqualTo(true));
                Assert.That(sql.Parameters[1].Value, Is.EqualTo("%@gmail.com%"));
            });
        }

        [Test]
        public void ToSql_Should_Be_Deterministic()
        {
            var engine = CreateQueryEngine();

            var query = engine.Query<User>("Users")
                .Where(x => x.IsActive);

            var sql1 = query.ToSql();
            var sql2 = query.ToSql();

            Assert.Multiple(() =>
            {
                Assert.That(sql1.CommandText, Is.EqualTo(sql2.CommandText));
                Assert.That(sql1.Parameters.Count, Is.EqualTo(sql2.Parameters.Count));
                Assert.That(sql1.Parameters[0].Name, Is.EqualTo(sql2.Parameters[0].Name));
                Assert.That(sql1.Parameters[0].Value, Is.EqualTo(sql2.Parameters[0].Value));
            });
        }

        [Test]
        public void ToSql_Should_Throw_When_Pagination_Has_No_Order_By()
        {
            var engine = CreateQueryEngine();

            var exception = Assert.Throws<InvalidOperationException>(() =>
                engine.Query<User>("Users")
                    .Skip(10)
                    .Take(5)
                    .ToSql());

            Assert.That(exception, Is.Not.Null);
        }

        [Test]
        public void ToSql_Should_Apply_WhereIf_When_Condition_Is_True()
        {
            // Arrange
            var engine = CreateQueryEngine();

            // Act
            var sql = engine.Query<User>("Users")
                .WhereIf(true, x => x.IsActive)
                .ToSql();

            // Assert
            Assert.That(sql.CommandText, Is.EqualTo(ExpectedSql.BooleanWhereSql));
        }

        [Test]
        public void ToSql_Should_Not_Apply_WhereIf_When_Condition_Is_False()
        {
            // Arrange
            var engine = CreateQueryEngine();

            // Act
            var sql = engine.Query<User>("Users")
                .WhereIf(false, x => x.IsActive)
                .ToSql();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(sql.CommandText, Is.EqualTo(ExpectedSql.SelectAllSql));
                Assert.That(sql.Parameters, Is.Empty);
            });
        }
    }
}
