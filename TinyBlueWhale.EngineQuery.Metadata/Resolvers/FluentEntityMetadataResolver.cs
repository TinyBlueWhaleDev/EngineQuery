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
        private readonly EntityMetadataRegistry _metadataRegistry = metadataRegistry;

        /// <summary>
        /// Resolves metadata associated with the specified entity type from the fluent metadata registry.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type associated with the metadata.
        /// </typeparam>
        /// <returns>
        /// Resolved entity metadata.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when metadata for the specified entity type is not registered.
        /// </exception>
        public bool TryResolve<TEntity>(out EntityMetadata? metadata)
        {
            return _metadataRegistry.TryGet(typeof(TEntity), out metadata);
        }
    }
}
