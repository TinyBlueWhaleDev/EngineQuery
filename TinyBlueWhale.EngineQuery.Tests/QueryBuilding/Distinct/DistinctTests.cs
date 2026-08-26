using TinyBlueWhale.EngineQuery.Tests.Models;
using TinyBlueWhale.EngineQuery.Tests.Providers;

namespace TinyBlueWhale.EngineQuery.Tests.QueryBuilding.Distinct
{
    /// <summary>
    /// Validates provider-independent DISTINCT query behavior.
    /// </summary>
    [TestFixtureSource(typeof(QueryTestProviderSource), nameof(QueryTestProviderSource.GetProviders))]
    internal sealed class DistinctTests(IQueryTestProvider provider)
    {
        private readonly IQueryTestProvider _provider = provider ?? throw new ArgumentNullException(nameof(provider));

        /// <summary>
        /// Validates DISTINCT generation for an explicit projection.
        /// </summary>
        [Test]
        public void Build_WhenDistinctIsConfigured_ShouldGenerateDistinctSelect()
        {
            var query = _provider
                .CreateQueryBuilder()
                .From<JoinUser>(alias: "u")
                .Distinct()
                .Select<JoinUser>(user => new
                {
                    user.Email
                })
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(query.CommandText, Does.Contain("SELECT"));
                Assert.That(query.CommandText, Does.Contain("DISTINCT"));
                Assert.That(query.CommandText, Does.Contain("email"));
            });
        }
    }
}
