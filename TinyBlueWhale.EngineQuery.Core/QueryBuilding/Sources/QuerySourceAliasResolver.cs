using TinyBlueWhale.EngineQuery.Core.Helpers;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Context;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;

namespace TinyBlueWhale.EngineQuery.Core.QueryBuilding.Sources
{
    /// <summary>
    /// Resolves and assigns deterministic aliases to query sources when
    /// source qualification is required by the current query scope.
    /// </summary>
    internal sealed class QuerySourceAliasResolver(QueryCommandBuilderContext context)
    {
        private readonly QueryCommandBuilderContext _context = context ?? throw new ArgumentNullException(nameof(context));

        /// <summary>
        /// Ensures that the specified query source has an alias.
        /// </summary>
        /// <typeparam name="TSource">
        /// Entity type associated with the query source.
        /// </typeparam>
        /// <param name="sourceDefinition">
        /// Query source definition whose alias is resolved.
        /// </param>
        /// <returns>
        /// Original source when an alias already exists; otherwise,
        /// a new source definition containing a generated deterministic alias.
        /// </returns>
        public QuerySourceDefinition EnsureAlias<TSource>(QuerySourceDefinition sourceDefinition)
        {
            ArgumentNullException.ThrowIfNull(sourceDefinition);

            if (!string.IsNullOrWhiteSpace(sourceDefinition.TableAlias))
                return sourceDefinition;

            var resolvedAlias = QueryAliasGeneratorHelper.Generate(_context.AliasRegistry.Count);

            var aliasedSource = sourceDefinition with
            {
                TableAlias = resolvedAlias
            };

            _context.QueryDefinition.SourceDefinitions[typeof(TSource)] =
                aliasedSource;

            if (_context.QueryDefinition.EntityType == typeof(TSource))
            {
                _context.QueryDefinition.TableAlias =
                    resolvedAlias;
            }

            _context.AliasRegistry.Register(resolvedAlias);

            return aliasedSource;
        }
    }
}
