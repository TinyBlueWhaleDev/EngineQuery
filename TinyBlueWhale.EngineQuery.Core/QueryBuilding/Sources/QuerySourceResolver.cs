using System.Linq.Expressions;
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

            var currentSources = GetCurrentSources(entityType);

            if (currentSources.Count == 1)
                return currentSources[0];

            if (currentSources.Count > 1)
                throw new InvalidOperationException($"Multiple query sources are registered for entity type '{entityType.Name}'. Resolve the source through the current expression scope.");

            var outerSources = GetOuterSources(entityType);

            if (outerSources.Count == 1)
                return outerSources[0];

            if (outerSources.Count > 1)
                throw new InvalidOperationException($"Multiple outer query sources are registered for entity type '{entityType.Name}'. Resolve the source through the current expression scope.");

            throw new InvalidOperationException($"Entity type '{entityType.Name}' is not available in the current query scope.");
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
        /// Resolves a query source using the entity type and expression parameter context.
        /// </summary>
        /// <typeparam name="TEntity">
        /// CLR entity type associated with the query source.
        /// </typeparam>
        /// <param name="parameter">
        /// Expression parameter used to disambiguate sources of the same entity type.
        /// </param>
        /// <returns>
        /// Resolved query source.
        /// </returns>
        /// <remarks>
        /// When multiple sources use the same CLR entity type, an exact match between
        /// the expression parameter name and the source alias takes precedence.
        /// When no alias match exists in the current query scope, the root source may
        /// be used only when it has the requested entity type and is physically present
        /// in the current source collection.
        /// </remarks>
        public QuerySourceDefinition Resolve<TEntity>(ParameterExpression parameter)
        {
            ArgumentNullException.ThrowIfNull(parameter);

            return Resolve(typeof(TEntity), parameter);
        }

        /// <summary>
        /// Resolves a query source using the entity type and expression parameter context.
        /// </summary>
        /// <param name="entityType">
        /// CLR entity type associated with the query source.
        /// </param>
        /// <param name="parameter">
        /// Expression parameter used to disambiguate sources of the same entity type.
        /// </param>
        /// <returns>
        /// Resolved query source.
        /// </returns>
        public QuerySourceDefinition Resolve(Type entityType, ParameterExpression parameter)
        {
            ArgumentNullException.ThrowIfNull(entityType);
            ArgumentNullException.ThrowIfNull(parameter);

            var currentSources = GetCurrentSources(entityType);

            if (currentSources.Count == 1)
                return currentSources[0];

            if (currentSources.Count > 1)
                return ResolveContextualSource(entityType, parameter, currentSources, useRootFallback: true);

            var outerSources = GetOuterSources(entityType);

            if (outerSources.Count == 1)
                return outerSources[0];

            if (outerSources.Count > 1)
                return ResolveContextualSource(entityType, parameter, outerSources, useRootFallback: false);

            throw new InvalidOperationException($"Entity type '{entityType.Name}' is not available in the current query scope.");
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

        // Gets current query sources associated with the specified entity type.
        private List<QuerySourceDefinition> GetCurrentSources(Type entityType)
        {
            return _context.QueryDefinition.Sources
                .Where(source => source.EntityType == entityType)
                .ToList();
        }

        // Gets outer query sources associated with the specified entity type.
        private List<QuerySourceDefinition> GetOuterSources(Type entityType)
        {
            return [.. _context.QueryDefinition.OuterSources.Where(source => source.EntityType == entityType)];
        }

        // Resolves one source from an ambiguous source collection using expression context.
        private QuerySourceDefinition ResolveContextualSource(Type entityType, ParameterExpression parameter, IReadOnlyList<QuerySourceDefinition> sources, bool useRootFallback)
        {
            if (!string.IsNullOrWhiteSpace(parameter.Name))
            {
                var aliasMatches = sources
                    .Where(source => string.Equals(source.TableAlias, parameter.Name, StringComparison.Ordinal))
                    .ToList();

                if (aliasMatches.Count == 1)
                    return aliasMatches[0];

                if (aliasMatches.Count > 1)
                    throw new InvalidOperationException($"Multiple query sources for entity type '{entityType.Name}' use alias '{parameter.Name}'.");
            }

            if (useRootFallback)
            {
                var rootSource = _context.QueryDefinition.RootSource;

                if (rootSource.EntityType == entityType && sources.Any(source => ReferenceEquals(source, rootSource)))
                    return rootSource;
            }

            throw new InvalidOperationException($"Multiple query sources are registered for entity type '{entityType.Name}' and expression parameter '{parameter.Name}' does not identify a unique source.");
        }

    }
}
