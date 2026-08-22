namespace TinyBlueWhale.EngineQuery.Metadata.EntityFramework.Resolvers
{

    /// <summary>
    /// Defines options used by the Entity Framework metadata resolver.
    /// </summary>
    public sealed class EntityFrameworkMetadataResolverOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether shadow properties should be included.
        /// </summary>
        public bool IncludeShadowProperties { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether ignored properties should be skipped.
        /// </summary>
        public bool SkipIgnoredProperties { get; set; } = true;

        /// <summary>
        /// Gets the default resolver options.
        /// </summary>
        public static EntityFrameworkMetadataResolverOptions Default => new();
    }
}
