using TinyBlueWhale.EngineQuery.Tests.Models;
using TinyBlueWhale.EngineQuery.Tests.Providers;

namespace TinyBlueWhale.EngineQuery.Tests.QueryBuilding.Deterministic
{
    /// <summary>
    /// Validates deterministic SQL generation behavior.
    /// </summary>
    [TestFixtureSource(typeof(QueryTestProviderSource), nameof(QueryTestProviderSource.GetProviders))]
    internal sealed class DeterminismTests(IQueryTestProvider provider)
    {
        private readonly IQueryTestProvider _provider = provider ?? throw new ArgumentNullException(nameof(provider));

        /// <summary>
        /// Validates that building the same query definition repeatedly
        /// produces equivalent SQL and parameters.
        /// </summary>
        [Test]
        public void Build_WhenInvokedMultipleTimes_ShouldProduceDeterministicResult()
        {
            var queryBuilder = _provider
                .CreateQueryBuilder()
                .From<User>("Users")
                .Where<User>(user => user.IsActive);

            var firstQuery = queryBuilder.Build();
            var secondQuery = queryBuilder.Build();

            Assert.Multiple(() =>
            {
                Assert.That(firstQuery.CommandText, Is.EqualTo(secondQuery.CommandText));
                Assert.That(firstQuery.Parameters, Has.Count.EqualTo(secondQuery.Parameters.Count));

                Assert.That(firstQuery.Parameters[0].Name, Is.EqualTo(secondQuery.Parameters[0].Name));
                Assert.That(firstQuery.Parameters[0].Value, Is.EqualTo(secondQuery.Parameters[0].Value));
            });
        }
    }
}
