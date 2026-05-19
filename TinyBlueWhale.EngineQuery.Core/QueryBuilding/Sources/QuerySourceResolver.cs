using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Context;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;
using TinyBlueWhale.EngineQuery.Metadata.Models;

namespace TinyBlueWhale.EngineQuery.Core.QueryBuilding.Sources
{        

    /// <summary>
    /// Resolves query sources and metadata for query command builders.
    /// </summary>
    internal sealed class QuerySourceResolver(QueryCommandBuilderContext context)
    {
        private readonly QueryCommandBuilderContext _context = context;

        /// <summary>
        /// Resolves a query source by entity type.
        /// </summary>
        public QuerySourceDefinition Resolve(Type entityType)
        {
            if (_context.QueryDefinition.SourceDefinitions.TryGetValue(entityType, out var sourceDefinition))
                return sourceDefinition;

            if (_context.QueryDefinition.OuterSourceDefinitions.TryGetValue(entityType, out var outerSourceDefinition))
                return outerSourceDefinition;

            throw new InvalidOperationException($"Entity type '{entityType.Name}' is not available in the current query scope.");
        }

        /// <summary>
        /// Resolves a query source by entity type.
        /// </summary>
        public QuerySourceDefinition Resolve<TEntity>()
        {
            return Resolve(typeof(TEntity));
        }

        /// <summary>
        /// Resolves entity metadata for the specified entity type.
        /// </summary>
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
        public static IReadOnlyDictionary<string, string> BuildColumnMappings(EntityMetadata metadata)
        {
            return metadata.Properties.ToDictionary(
                property => property.Key,
                property => property.Value.ColumnName);
        }
    }
}
