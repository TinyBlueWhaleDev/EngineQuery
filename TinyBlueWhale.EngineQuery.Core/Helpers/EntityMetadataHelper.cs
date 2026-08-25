using TinyBlueWhale.EngineQuery.Metadata.Interfaces;
using TinyBlueWhale.EngineQuery.Metadata.Models;

namespace TinyBlueWhale.EngineQuery.Core.Helpers
{
    /// <summary>
    /// Provides shared metadata resolution and property-to-column mapping helpers
    /// used by query command builders.
    /// </summary>
    internal static class EntityMetadataHelper
    {
        /// <summary>
        /// Resolves metadata for the specified entity type.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type whose metadata is resolved.
        /// </typeparam>
        /// <param name="metadataResolver">
        /// Metadata resolver used to resolve the entity metadata.
        /// </param>
        /// <returns>
        /// Resolved entity metadata.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="metadataResolver"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when metadata cannot be resolved for the specified entity type.
        /// </exception>
        public static EntityMetadata Resolve<TEntity>(IEntityMetadataResolver metadataResolver)
        {
            ArgumentNullException.ThrowIfNull(metadataResolver);

            if (!metadataResolver.TryResolve<TEntity>(out var metadata))
                throw new InvalidOperationException(
                    $"Metadata for entity type '{typeof(TEntity).Name}' could not be resolved.");

            return metadata!;
        }

        /// <summary>
        /// Creates property-to-column mappings from the specified entity metadata.
        /// </summary>
        /// <param name="metadata">
        /// Entity metadata used to create the property-to-column mappings.
        /// </param>
        /// <returns>
        /// Property-to-column mappings associated with the entity.
        /// </returns>
        public static Dictionary<string, string> CreateColumnMappings(EntityMetadata metadata)
        {
            ArgumentNullException.ThrowIfNull(metadata);

            return metadata.Properties.ToDictionary(
                property => property.Key,
                property => property.Value.ColumnName);
        }
    }
}
