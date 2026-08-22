using TinyBlueWhale.EngineQuery.Metadata.Models;

namespace TinyBlueWhale.EngineQuery.Metadata.Fluent
{
    /// <summary>
    /// Stores entity metadata configured through the fluent mapping API.
    /// </summary>
    public sealed class EntityMetadataRegistry
    {
        private readonly Dictionary<Type, EntityMetadata> _metadataByEntityType = [];

        /// <summary>
        /// Creates a fluent metadata builder for the specified entity type.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type to configure.
        /// </typeparam>
        /// <returns>
        /// Fluent entity metadata builder.
        /// </returns>
        public EntityMetadataBuilder<TEntity> Entity<TEntity>()
        {
            return new EntityMetadataBuilder<TEntity>(this);
        }

        /// <summary>
        /// Registers metadata for the specified entity type.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type associated with the metadata.
        /// </typeparam>
        /// <param name="metadata">
        /// Entity metadata to register.
        /// </param>
        public void Register<TEntity>(EntityMetadata metadata)
        {
            ArgumentNullException.ThrowIfNull(metadata);

            _metadataByEntityType[typeof(TEntity)] = metadata;
        }

        /// <summary>
        /// Attempts to retrieve metadata for the specified entity type.
        /// </summary>
        /// <param name="entityType">
        /// Entity type used as the metadata key.
        /// </param>
        /// <param name="metadata">
        /// Registered entity metadata when available.
        /// </param>
        /// <returns>
        /// True when metadata exists; otherwise, false.
        /// </returns>
        public bool TryGet(Type entityType, out EntityMetadata? metadata)
        {
            return _metadataByEntityType.TryGetValue(entityType, out metadata);
        }
    }
}
