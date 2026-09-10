using System.Linq.Expressions;
using TinyBlueWhale.EngineQuery.Core.ExpressionsParsing;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Context;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Sources;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions.Sources;

namespace TinyBlueWhale.EngineQuery.Core.QueryBuilding.Projections
{
    /// <summary>
    /// Builds SELECT projection definitions.
    /// </summary>
    internal sealed class SelectProjectionBuilder(QueryCommandBuilderContext context)
    {
        private readonly QueryCommandBuilderContext _context = context;
        private readonly QuerySourceResolver _sourceResolver = new(context);
        private readonly Dictionary<Type, int> _projectionSourceIndexes = [];
        private readonly Dictionary<(Type EntityType, string ParameterName), int> _projectionAliasIndexes = [];

        /// <summary>
        /// Adds selected properties for the query entity associated with
        /// the projection expression.
        /// </summary>
        /// <typeparam name="T">
        /// Entity type associated with the projection.
        /// </typeparam>
        /// <param name="selector">
        /// Expression that selects the projected properties.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="selector"/> is null.
        /// </exception>
        public void Add<T>(Expression<Func<T, object>> selector)
        {
            ArgumentNullException.ThrowIfNull(selector);

            AddProjection(selector);
        }

        /// <summary>
        /// Adds selected properties for an entity source available
        /// in the current query scope.
        /// </summary>
        /// <remarks>
        /// When multiple sources use the same CLR entity type, parameter names
        /// matching a source alias are resolved against that alias family.
        /// Repeated projections using the same parameter name advance through
        /// numbered aliases. Parameters without an alias match are resolved
        /// sequentially.
        /// </remarks>
        /// <typeparam name="TEntity">
        /// Entity type associated with the projection source.
        /// </typeparam>
        /// <param name="selector">
        /// Expression that selects the projected properties.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="selector"/> is null.
        /// </exception>
        public void AddForSource<TEntity>(Expression<Func<TEntity, object>> selector)
        {
            ArgumentNullException.ThrowIfNull(selector);

            AddProjection(selector);
        }

        // Adds projection definitions using the source resolved from the selector.
        private void AddProjection<TEntity>(Expression<Func<TEntity, object>> selector)
        {
            var sourceDefinition = ResolveProjectionSource<TEntity>(selector.Parameters.Single());

            var selectedColumns = SelectedPropertyExpressionExtractor
                .ExtractSelectedProperties(selector);

            foreach (var selectedColumn in selectedColumns)
            {
                _context.QueryDefinition.SelectDefinitions.Add(
                    selectedColumn with
                    {
                        Source = sourceDefinition
                    });
            }
        }

        // Resolves the source associated with the current projection.
        private QuerySourceDefinition ResolveProjectionSource<TEntity>(ParameterExpression parameter)
        {
            var entityType = typeof(TEntity);

            var sources = _context.QueryDefinition.Sources
                .Where(source => source.EntityType == entityType)
                .ToList();

            if (sources.Count == 0)
                return _sourceResolver.Resolve<TEntity>(parameter);

            if (sources.Count == 1)
                return sources[0];

            if (!string.IsNullOrWhiteSpace(parameter.Name))
            {
                var aliasFamilySource = ResolveAliasFamilySource(entityType, parameter.Name, sources);

                if (aliasFamilySource is not null)
                {
                    AdvanceProjectionSourceIndex(entityType, sources, aliasFamilySource);

                    return aliasFamilySource;
                }
            }

            return ResolveSequentialSource(entityType, sources);
        }

        // Resolves the next source belonging to the specified alias family.
        private QuerySourceDefinition? ResolveAliasFamilySource(Type entityType, string parameterName, IReadOnlyList<QuerySourceDefinition> sources)
        {
            var aliasFamily = sources
                .Where(source => IsAliasFamilyMember(source.TableAlias, parameterName))
                .ToList();

            if (aliasFamily.Count == 0)
                return null;

            var key = (entityType, parameterName);

            var familyIndex = _projectionAliasIndexes.TryGetValue(key, out var currentIndex)
                ? currentIndex
                : 0;

            if (familyIndex >= aliasFamily.Count)
                familyIndex = aliasFamily.Count - 1;

            var sourceDefinition = aliasFamily[familyIndex];

            _projectionAliasIndexes[key] = familyIndex + 1;

            return sourceDefinition;
        }

        // Resolves the next source using projection sequence.
        private QuerySourceDefinition ResolveSequentialSource(Type entityType, IReadOnlyList<QuerySourceDefinition> sources)
        {
            var sourceIndex = _projectionSourceIndexes.TryGetValue(entityType, out var currentIndex)
                ? currentIndex
                : 0;

            if (sourceIndex >= sources.Count)
                sourceIndex = sources.Count - 1;

            var sourceDefinition = sources[sourceIndex];

            _projectionSourceIndexes[entityType] = sourceIndex + 1;

            return sourceDefinition;
        }

        // Determines whether an alias belongs to the specified alias family.
        private static bool IsAliasFamilyMember(string? alias, string familyName)
        {
            if (string.IsNullOrWhiteSpace(alias))
                return false;

            if (string.Equals(alias, familyName, StringComparison.Ordinal))
                return true;

            if (!alias.StartsWith(familyName, StringComparison.Ordinal))
                return false;

            var suffix = alias[familyName.Length..];

            return suffix.Length > 0 && suffix.All(char.IsDigit);
        }

        // Advances sequential projection resolution beyond the resolved source.
        private void AdvanceProjectionSourceIndex(Type entityType, IReadOnlyList<QuerySourceDefinition> sources, QuerySourceDefinition resolvedSource)
        {
            for (var index = 0; index < sources.Count; index++)
            {
                if (!ReferenceEquals(sources[index], resolvedSource))
                    continue;

                _projectionSourceIndexes[entityType] = index + 1;

                return;
            }
        }

        /// <summary>
        /// Applies DISTINCT projection semantics.
        /// </summary>
        public void ApplyDistinct()
        {
            _context.QueryDefinition.IsDistinct = true;
        }
    }
}
