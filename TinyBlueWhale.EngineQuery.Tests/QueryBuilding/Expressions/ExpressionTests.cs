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

namespace TinyBlueWhale.EngineQuery.Tests.QueryBuilding.Expressions
{
    /// <summary>
    /// Validates provider-independent scalar, computed and conditional expression behavior.
    /// </summary>
    [TestFixture]
    internal sealed class ExpressionTests
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
        public void Build_WhenScalarFunctionsAreConfigured_ShouldGenerateExpectedFunctions()
        {
            AssertScalarFunctions(_sqlServer);
            AssertScalarFunctions(_postgreSql);
            AssertScalarFunctions(_mySql);
        }

        [Test]
        public void Build_WhenComputedMultiplicationIsConfigured_ShouldGenerateExpectedExpression()
        {
            AssertComputedMultiplication(_sqlServer);
            AssertComputedMultiplication(_postgreSql);
            AssertComputedMultiplication(_mySql);
        }

        [Test]
        public void Build_WhenComputedArithmeticOperatorsAreConfigured_ShouldGenerateAllOperators()
        {
            AssertArithmeticOperators(_sqlServer);
            AssertArithmeticOperators(_postgreSql);
            AssertArithmeticOperators(_mySql);
        }

        [Test]
        public void Build_WhenComputedExpressionContainsNestedArithmetic_ShouldPreserveExpressionStructure()
        {
            AssertNestedArithmetic(_sqlServer);
            AssertNestedArithmetic(_postgreSql);
            AssertNestedArithmetic(_mySql);
        }

        [Test]
        public void Build_WhenComputedPredicateIsConfigured_ShouldGenerateExpectedPredicateAndParameterOrder()
        {
            AssertComputedPredicate(_sqlServer);
            AssertComputedPredicate(_postgreSql);
            AssertComputedPredicate(_mySql);
        }

        [Test]
        public void Build_WhenCaseWhenIsConfigured_ShouldGenerateCaseExpressionAndParametersInOrder()
        {
            AssertCaseWhen(_sqlServer);
            AssertCaseWhen(_postgreSql);
            AssertCaseWhen(_mySql);
        }

        [Test]
        public void Build_WhenCoalesceExpressionIsConfigured_ShouldGenerateComputedExpression()
        {
            AssertCoalesceExpression(_sqlServer);
            AssertCoalesceExpression(_postgreSql);
            AssertCoalesceExpression(_mySql);
        }

        [Test]
        public void Build_WhenCaseConditionContainsOr_ShouldGenerateCompoundCondition()
        {
            AssertCaseWhenWithOr(_sqlServer);
            AssertCaseWhenWithOr(_postgreSql);
            AssertCaseWhenWithOr(_mySql);
        }

        [Test]
        public void Build_WhenCaseConditionContainsAnd_ShouldGenerateCompoundCondition()
        {
            AssertCaseWhenWithAnd(_sqlServer);
            AssertCaseWhenWithAnd(_postgreSql);
            AssertCaseWhenWithAnd(_mySql);
        }

        [Test]
        public void Build_WhenScalarFunctionPredicateIsConfigured_ShouldGenerateExpectedPredicate()
        {
            AssertScalarFunctionPredicate(_sqlServer);
            AssertScalarFunctionPredicate(_postgreSql);
            AssertScalarFunctionPredicate(_mySql);
        }

        private static void AssertScalarFunctions<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(user => new
                {
                    UserId = user.Id
                })
                .SelectScalarFunction<JoinUser>(QueryScalarFunction.Upper, user => user.Email, "NormalizedEmail")
                .SelectScalarFunction<JoinUser>(QueryScalarFunction.Length, user => user.Email, "EmailLength")
                .SelectScalarFunction<JoinUser>(QueryScalarFunction.Trim, user => user.Email, "TrimmedEmail")
                .Build();

            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain("NormalizedEmail"));
                Assert.That(sql, Does.Contain("EmailLength"));
                Assert.That(sql, Does.Contain("TrimmedEmail"));
                Assert.That(sql, Does.Contain("email"));
                Assert.That(query.Parameters, Is.Empty);
            });
        }

        private static void AssertComputedMultiplication<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<JoinOrder>(alias: "o")
                .SelectComputed<JoinOrder>(order => order.Total * 1.16m, "TotalWithTax")
                .Build();

            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain("o.total"));
                Assert.That(sql, Does.Contain("* @p0"));
                Assert.That(sql, Does.Contain("AS TotalWithTax"));
                Assert.That(query.Parameters, Has.Count.EqualTo(1));
                QueryAssertionHelper.AssertParameter(query.Parameters, 0, 1.16m);
            });
        }

        private static void AssertArithmeticOperators<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<JoinOrder>(alias: "o")
                .SelectComputed<JoinOrder>(order => order.Total + 10m, "Added")
                .SelectComputed<JoinOrder>(order => order.Total - 20m, "Subtracted")
                .SelectComputed<JoinOrder>(order => order.Total * 2m, "Multiplied")
                .SelectComputed<JoinOrder>(order => order.Total / 4m, "Divided")
                .SelectComputed<JoinOrder>(order => order.Id % 3, "Remainder")
                .Build();

            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain("o.total + @p0"));
                Assert.That(sql, Does.Contain("o.total - @p1"));
                Assert.That(sql, Does.Contain("o.total * @p2"));
                Assert.That(sql, Does.Contain("o.total / @p3"));
                Assert.That(sql, Does.Contain("o.order_id % @p4"));
                Assert.That(query.Parameters, Has.Count.EqualTo(5));
                QueryAssertionHelper.AssertParameter(query.Parameters, 0, 10m);
                QueryAssertionHelper.AssertParameter(query.Parameters, 1, 20m);
                QueryAssertionHelper.AssertParameter(query.Parameters, 2, 2m);
                QueryAssertionHelper.AssertParameter(query.Parameters, 3, 4m);
                QueryAssertionHelper.AssertParameter(query.Parameters, 4, 3);
            });
        }

        private static void AssertNestedArithmetic<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<JoinOrder>(alias: "o")
                .SelectComputed<JoinOrder>(order => (order.Total + 100m) * 2m, "AdjustedTotal")
                .Build();

            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain("o.total"));
                Assert.That(sql, Does.Contain("+ @p0"));
                Assert.That(sql, Does.Contain("* @p1"));
                Assert.That(sql, Does.Contain("AS AdjustedTotal"));
                Assert.That(query.Parameters, Has.Count.EqualTo(2));
                QueryAssertionHelper.AssertParameter(query.Parameters, 0, 100m);
                QueryAssertionHelper.AssertParameter(query.Parameters, 1, 2m);
            });
        }

        private static void AssertComputedPredicate<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<JoinOrder>(alias: "o")
                .SelectComputed<JoinOrder>(order => order.Total * 1.16m, "TotalWithTax")
                .WhereComputed<JoinOrder>(order => order.Total * 1.16m > 1000m)
                .Build();

            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain("o.total * @p0"));
                Assert.That(sql, Does.Contain("WHERE"));
                Assert.That(sql, Does.Contain("o.total * @p1"));
                Assert.That(sql, Does.Contain("> @p2"));
                Assert.That(query.Parameters, Has.Count.EqualTo(3));
                QueryAssertionHelper.AssertParameter(query.Parameters, 0, 1.16m);
                QueryAssertionHelper.AssertParameter(query.Parameters, 1, 1.16m);
                QueryAssertionHelper.AssertParameter(query.Parameters, 2, 1000m);
            });
        }

        private static void AssertCaseWhen<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<JoinOrder>(alias: "o")
                .SelectCaseWhen<JoinOrder>(
                    condition: order => order.Total > 1000m,
                    whenTrue: "VIP",
                    whenFalse: "STANDARD",
                    alias: "CustomerType")
                .Build();

            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain("CASE"));
                Assert.That(sql, Does.Contain("WHEN"));
                Assert.That(sql, Does.Contain("o.total > @p0"));
                Assert.That(sql, Does.Contain("THEN @p1"));
                Assert.That(sql, Does.Contain("ELSE @p2"));
                Assert.That(sql, Does.Contain("AS CustomerType"));
                Assert.That(query.Parameters, Has.Count.EqualTo(3));
                QueryAssertionHelper.AssertParameter(query.Parameters, 0, 1000m);
                QueryAssertionHelper.AssertParameter(query.Parameters, 1, "VIP");
                QueryAssertionHelper.AssertParameter(query.Parameters, 2, "STANDARD");
            });
        }

        private static void AssertCoalesceExpression<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<User>(alias: "u")
                .SelectComputed<User>(user => user.Email ?? string.Empty, "EmailValue")
                .Build();

            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain("Email"));
                Assert.That(sql, Does.Contain("EmailValue"));
                Assert.That(query.Parameters, Has.Count.EqualTo(1));
                QueryAssertionHelper.AssertParameter(query.Parameters, 0, string.Empty);
            });
        }

        private static void AssertCaseWhenWithOr<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<JoinOrder>(alias: "o")
                .SelectCaseWhen<JoinOrder>(
                    condition: order => order.Total <= 0m || order.Total > 10000m,
                    whenTrue: "REVIEW",
                    whenFalse: "NORMAL",
                    alias: "RiskStatus")
                .Build();

            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain(" OR "));
                Assert.That(sql, Does.Contain("AS RiskStatus"));
                Assert.That(query.Parameters, Has.Count.EqualTo(4));
                QueryAssertionHelper.AssertParameter(query.Parameters, 0, 0m);
                QueryAssertionHelper.AssertParameter(query.Parameters, 1, 10000m);
                QueryAssertionHelper.AssertParameter(query.Parameters, 2, "REVIEW");
                QueryAssertionHelper.AssertParameter(query.Parameters, 3, "NORMAL");
            });
        }

        private static void AssertCaseWhenWithAnd<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<JoinOrder>(alias: "o")
                .SelectCaseWhen<JoinOrder>(
                    condition: order => order.Total > 1000m && order.Total < 5000m,
                    whenTrue: "VIP",
                    whenFalse: "STANDARD",
                    alias: "CustomerType")
                .Build();

            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain(" AND "));
                Assert.That(sql, Does.Contain("AS CustomerType"));
                Assert.That(query.Parameters, Has.Count.EqualTo(4));
                QueryAssertionHelper.AssertParameter(query.Parameters, 0, 1000m);
                QueryAssertionHelper.AssertParameter(query.Parameters, 1, 5000m);
                QueryAssertionHelper.AssertParameter(query.Parameters, 2, "VIP");
                QueryAssertionHelper.AssertParameter(query.Parameters, 3, "STANDARD");
            });
        }

        private static void AssertScalarFunctionPredicate<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(user => new
                {
                    UserId = user.Id,
                    user.Email
                })
                .WhereScalarFunction<JoinUser>(
                    QueryScalarFunction.Lower,
                    user => user.Email,
                    QueryComparisonOperator.Equal,
                    "admin@test.com")
                .WhereScalarFunction<JoinUser>(
                    QueryScalarFunction.Length,
                    user => user.Email,
                    QueryComparisonOperator.GreaterThan,
                    10)
                .Build();

            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain("WHERE"));
                Assert.That(sql, Does.Contain("email"));
                Assert.That(query.Parameters, Has.Count.EqualTo(2));
                QueryAssertionHelper.AssertParameter(query.Parameters, 0, "admin@test.com");
                QueryAssertionHelper.AssertParameter(query.Parameters, 1, 10);
            });
        }
    }
}
