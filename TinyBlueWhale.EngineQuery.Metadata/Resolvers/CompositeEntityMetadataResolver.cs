using TinyBlueWhale.EngineQuery.Metadata.Interfaces;
using TinyBlueWhale.EngineQuery.Metadata.Models;

namespace TinyBlueWhale.EngineQuery.Metadata.Resolvers
{
    /// <summary>
    /// Resolves entity metadata by evaluating multiple metadata resolvers in priority order.
    /// </summary>
    /// <remarks>
    /// The first resolver that successfully resolves metadata wins.
    /// </remarks>
    public sealed class CompositeEntityMetadataResolver(IReadOnlyList<IEntityMetadataResolver> resolvers) : IEntityMetadataResolver
    {
        private readonly IReadOnlyList<IEntityMetadataResolver> _resolvers = resolvers;

        /// <summary>
        /// Attempts to resolve metadata associated with the specified entity type using the configured resolver chain.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type associated with the metadata.
        /// </typeparam>
        /// <param name="metadata">
        /// Resolved entity metadata when available.
        /// </param>
        /// <returns>
        /// True when any configured resolver can provide metadata; otherwise, false.
        /// </returns>
        public bool TryResolve<TEntity>(out EntityMetadata? metadata)
        {
            foreach (var resolver in _resolvers)
            {
                if (resolver.TryResolve<TEntity>(out metadata))
                    return true;
            }

            metadata = null;
            return false;
        }
    }
}
