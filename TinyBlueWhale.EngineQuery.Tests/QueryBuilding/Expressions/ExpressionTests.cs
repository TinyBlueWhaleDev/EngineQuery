using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Tests.Models;
using TinyBlueWhale.EngineQuery.Tests.Providers;

namespace TinyBlueWhale.EngineQuery.Tests.QueryBuilding.Expressions
{
    /// <summary>
    /// Validates provider-independent scalar, computed and conditional expression behavior.
    /// </summary>
    [TestFixtureSource(typeof(QueryTestProviderSource), nameof(QueryTestProviderSource.GetProviders))]
    internal sealed class ExpressionTests(IQueryTestProvider provider)
    {
        private readonly IQueryTestProvider _provider = provider ?? throw new ArgumentNullException(nameof(provider));

        /// <summary>
        /// Validates scalar function projection generation.
        /// </summary>
        [Test]
        public void Build_WhenScalarFunctionsAreConfigured_ShouldGenerateExpectedFunctions()
        {
            var query = _provider
                .CreateQueryBuilder()
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(user => new
                {
                    UserId = user.Id
                })
                .SelectScalarFunction<JoinUser>(
                    QueryScalarFunction.Upper,
                    user => user.Email,
                    "NormalizedEmail")
                .SelectScalarFunction<JoinUser>(
                    QueryScalarFunction.Length,
                    user => user.Email,
                    "EmailLength")
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(query.CommandText, Does.Contain("NormalizedEmail"));
                Assert.That(query.CommandText, Does.Contain("EmailLength"));
                Assert.That(query.CommandText, Does.Contain("email"));
            });
        }

        /// <summary>
        /// Validates computed expression projection and filtering behavior.
        /// </summary>
        [Test]
        public void Build_WhenComputedExpressionsAreConfigured_ShouldGenerateExpectedSql()
        {
            var query = _provider
                .CreateQueryBuilder()
                .From<JoinOrder>(alias: "o")
                .Select<JoinOrder>(order => new
                {
                    OrderId = order.Id,
                    order.Total
                })
                .SelectComputed<JoinOrder>(
                    order => order.Total * 1.16m,
                    "TotalWithTax")
                .WhereComputed<JoinOrder>(
                    order => order.Total * 1.16m > 1000)
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(query.CommandText, Does.Contain("TotalWithTax"));
                Assert.That(query.CommandText, Does.Contain("total"));
                Assert.That(query.CommandText, Does.Contain("WHERE"));

                Assert.That(query.Parameters, Has.Count.GreaterThanOrEqualTo(1));
                Assert.That(query.Parameters.Any(parameter => Equals(parameter.Value, 1000m)), Is.True);
            });
        }

        /// <summary>
        /// Validates CASE WHEN projection generation.
        /// </summary>
        [Test]
        public void Build_WhenCaseWhenIsConfigured_ShouldGenerateCaseExpression()
        {
            var query = _provider
                .CreateQueryBuilder()
                .From<JoinOrder>(alias: "o")
                .Select<JoinOrder>(order => new
                {
                    OrderId = order.Id,
                    order.Total
                })
                .SelectCaseWhen<JoinOrder>(
                    condition: order => order.Total > 1000,
                    whenTrue: "VIP",
                    whenFalse: "STANDARD",
                    alias: "CustomerType")
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(query.CommandText, Does.Contain("CASE"));
                Assert.That(query.CommandText, Does.Contain("WHEN"));
                Assert.That(query.CommandText, Does.Contain("CustomerType"));

                Assert.That(query.Parameters.Any(parameter => Equals(parameter.Value, 1000m)), Is.True);
                Assert.That(query.Parameters.Any(parameter => Equals(parameter.Value, "VIP")), Is.True);
                Assert.That(query.Parameters.Any(parameter => Equals(parameter.Value, "STANDARD")), Is.True);
            });
        }

        /// <summary>
        /// Validates null-coalescing computed expression generation.
        /// </summary>
        [Test]
        public void Build_WhenCoalesceExpressionIsConfigured_ShouldGenerateComputedExpression()
        {
            var query = _provider
                .CreateQueryBuilder()
                .From<User>(alias: "u")
                .SelectComputed<User>(
                    user => user.Email ?? string.Empty,
                    "Nombre")
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(query.CommandText, Does.Contain("Nombre"));
                Assert.That(query.CommandText, Does.Contain("Email"));
            });
        }

        /// <summary>
        /// Validates CASE WHEN generation using a logical OR condition.
        /// </summary>
        [Test]
        public void Build_WhenCaseConditionContainsOr_ShouldGenerateCompoundCondition()
        {
            var query = _provider
                .CreateQueryBuilder()
                .From<JoinOrder>(alias: "o")
                .Select<JoinOrder>(order => new
                {
                    OrderId = order.Id,
                    order.Total
                })
                .SelectCaseWhen<JoinOrder>(
                    condition: order =>
                        order.Total <= 0 ||
                        order.Total > 10000,
                    whenTrue: "REVIEW",
                    whenFalse: "NORMAL",
                    alias: "RiskStatus")
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(query.CommandText, Does.Contain("CASE"));
                Assert.That(query.CommandText, Does.Contain(" OR "));
                Assert.That(query.CommandText, Does.Contain("RiskStatus"));
            });
        }
    }
}
