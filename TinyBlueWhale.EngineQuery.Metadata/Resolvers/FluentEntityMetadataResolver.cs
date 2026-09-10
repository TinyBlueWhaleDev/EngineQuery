using TinyBlueWhale.EngineQuery.Metadata.Fluent;
using TinyBlueWhale.EngineQuery.Metadata.Interfaces;
using TinyBlueWhale.EngineQuery.Metadata.Models;

namespace TinyBlueWhale.EngineQuery.Metadata.Resolvers
{
    /// <summary>
    /// Resolves entity metadata registered through the fluent mapping API.
    /// </summary>
    public sealed class FluentEntityMetadataResolver(EntityMetadataRegistry metadataRegistry) : IEntityMetadataResolver
    {
        private readonly EntityMetadataRegistry _metadataRegistry = metadataRegistry ?? throw new ArgumentNullException(nameof(metadataRegistry));

        /// <summary>
        /// Attempts to resolve metadata associated with the specified entity type
        /// from the fluent metadata registry.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type associated with the metadata.
        /// </typeparam>
        /// <param name="metadata">
        /// Resolved entity metadata when available.
        /// </param>
        /// <returns>
        /// <see langword="true"/> when metadata is registered;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public bool TryResolve<TEntity>(out EntityMetadata? metadata)
        {
            return _metadataRegistry.TryGet(typeof(TEntity), out metadata);
        }
    }
}
