namespace TinyBlueWhale.EngineQuery.Tests.Metadata
{
    ///// <summary>
    ///// Validates metadata resolution behavior used by query builders.
    ///// </summary>
    //[TestFixtureSource(typeof(QueryTestProviderSource), nameof(QueryTestProviderSource.GetProviders))]
    //internal sealed class MetadataResolutionTests(IQueryTestProvider provider)
    //{
    //    private readonly IQueryTestProvider _provider = provider ?? throw new ArgumentNullException(nameof(provider));

    //    /// <summary>
    //    /// Validates that queries cannot be created for entity types
    //    /// without registered metadata.
    //    /// </summary>
    //    [Test]
    //    public void From_WhenMetadataIsNotRegistered_ShouldThrow()
    //    {
    //        var exception = Assert.Throws<InvalidOperationException>(() =>
    //            _provider
    //                .CreateQueryBuilder()
    //                .From<UnmappedEntity>("x"));

    //        Assert.That(exception, Is.Not.Null);
    //    }

    //    /// <summary>
    //    /// Entity type intentionally excluded from test metadata registration.
    //    /// </summary>
    //    private sealed class UnmappedEntity
    //    {
    //        /// <summary>
    //        /// Gets or sets the entity identifier.
    //        /// </summary>
    //        public int Id { get; set; }
    //    }
    //}
}
