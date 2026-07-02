using TinyBlueWhale.EngineQuery.Metadata.Models;

namespace TinyBlueWhale.EngineQuery.Metadata.Interfaces
{
    /// <summary>
    /// Defines a contract for resolving entity metadata used during SQL generation.
    /// </summary>
    public interface IEntityMetadataResolver
    {
        /// <summary>
        /// Attempts to resolve metadata associated with the specified entity type.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type associated with the metadata.
        /// </typeparam>
        /// <param name="metadata">
        /// Resolved entity metadata when available.
        /// </param>
        /// <returns>
        /// True when metadata could be resolved; otherwise, false.
        /// </returns>
        bool TryResolve<TEntity>(out EntityMetadata? metadata);
    }
}
