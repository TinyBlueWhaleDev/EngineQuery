using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Context;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions.Sources;
using TinyBlueWhale.EngineQuery.Metadata.Models;

namespace TinyBlueWhale.EngineQuery.Core.QueryBuilding.Sources
{

    /// <summary>
    /// Resolves query sources and metadata for query command builders.
    /// </summary>
    internal sealed class QuerySourceResolver(QueryCommandBuilderContext context)
    {
        private readonly QueryCommandBuilderContext _context = context ?? throw new ArgumentNullException(nameof(context));

        /// <summary>
        /// Resolves an unambiguous query source by entity type.
        /// </summary>
        /// <param name="entityType">
        /// CLR entity type associated with the query source.
        /// </param>
        /// <returns>
        /// Resolved query source.
        /// </returns>
        public QuerySourceDefinition Resolve(Type entityType)
        {
            ArgumentNullException.ThrowIfNull(entityType);

            var currentSources = _context.QueryDefinition.Sources
                .Where(source => source.EntityType == entityType)
                .ToList();

            if (currentSources.Count == 1)
                return currentSources[0];

            if (currentSources.Count > 1)
                throw new InvalidOperationException(
                    $"Multiple query sources are registered for entity type '{entityType.Name}'. Resolve the source through the current expression scope.");

            var outerSources = _context.QueryDefinition.OuterSources
                .Where(source => source.EntityType == entityType)
                .ToList();

            if (outerSources.Count == 1)
                return outerSources[0];

            if (outerSources.Count > 1)
                throw new InvalidOperationException(
                    $"Multiple outer query sources are registered for entity type '{entityType.Name}'. Resolve the source through the current expression scope.");

            throw new InvalidOperationException(
                $"Entity type '{entityType.Name}' is not available in the current query scope.");
        }

        /// <summary>
        /// Resolves an unambiguous query source by entity type.
        /// </summary>
        /// <typeparam name="TEntity">
        /// CLR entity type associated with the query source.
        /// </typeparam>
        /// <returns>
        /// Resolved query source.
        /// </returns>
        public QuerySourceDefinition Resolve<TEntity>()
        {
            return Resolve(typeof(TEntity));
        }

        /// <summary>
        /// Resolves entity metadata for the specified entity type.
        /// </summary>
        /// <typeparam name="TEntity">
        /// CLR entity type whose metadata should be resolved.
        /// </typeparam>
        /// <returns>
        /// Resolved entity metadata.
        /// </returns>
        public EntityMetadata ResolveMetadata<TEntity>()
        {
            if (_context.MetadataResolver is null)
                throw new InvalidOperationException("No entity metadata resolver is configured.");

            if (!_context.MetadataResolver.TryResolve<TEntity>(out var metadata))
                throw new InvalidOperationException($"Metadata for entity type '{typeof(TEntity).Name}' could not be resolved.");

            return metadata!;
        }

        /// <summary>
        /// Builds property-to-column mappings from entity metadata.
        /// </summary>
        /// <param name="metadata">
        /// Entity metadata used to build property mappings.
        /// </param>
        /// <returns>
        /// Property-to-column mappings.
        /// </returns>
        public static IReadOnlyDictionary<string, string> BuildColumnMappings(EntityMetadata metadata)
        {
            ArgumentNullException.ThrowIfNull(metadata);

            return metadata.Properties.ToDictionary(
                property => property.Key,
                property => property.Value.ColumnName);
        }
    }
}
