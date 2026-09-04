using TinyBlueWhale.EngineQuery.Core.Helpers;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Context;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions.Sources;

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
        /// <param name="sourceDefinition">
        /// Query source definition whose alias is resolved.
        /// </param>
        /// <returns>
        /// Query source containing the resolved alias.
        /// </returns>
        public QuerySourceDefinition EnsureAlias(QuerySourceDefinition sourceDefinition)
        {
            ArgumentNullException.ThrowIfNull(sourceDefinition);

            if (!string.IsNullOrWhiteSpace(sourceDefinition.TableAlias))
                return sourceDefinition;

            var resolvedAlias = QueryAliasGeneratorHelper.Generate(_context.AliasRegistry.Count);

            sourceDefinition.TableAlias = resolvedAlias;
            _context.AliasRegistry.Register(resolvedAlias);

            return sourceDefinition;
        }
    }
}
