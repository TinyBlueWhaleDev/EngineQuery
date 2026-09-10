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
        /// Ensures that the specified query source has a unique alias
        /// within the current query scope.
        /// </summary>
        /// <param name="sourceDefinition">
        /// Query source definition whose alias is resolved.
        /// </param>
        /// <returns>
        /// Query source definition containing the resolved alias.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="sourceDefinition"/> is null.
        /// </exception>
        public QuerySourceDefinition EnsureAlias(QuerySourceDefinition sourceDefinition)
        {
            ArgumentNullException.ThrowIfNull(sourceDefinition);

            if (IsRegisteredSource(sourceDefinition))
            {
                if (!string.IsNullOrWhiteSpace(sourceDefinition.TableAlias))
                    return sourceDefinition;

                var generatedAlias = ResolveGeneratedAlias();

                sourceDefinition.TableAlias = generatedAlias;
                _context.AliasRegistry.Register(generatedAlias);

                return sourceDefinition;
            }

            var requestedAlias = sourceDefinition.TableAlias;

            if (string.IsNullOrWhiteSpace(requestedAlias))
                requestedAlias = ResolveGeneratedAlias();

            var resolvedAlias = ResolveUniqueAlias(requestedAlias);

            sourceDefinition.TableAlias = resolvedAlias;
            _context.AliasRegistry.Register(resolvedAlias);

            return sourceDefinition;
        }

        // Determines whether the exact source instance is already registered.
        private bool IsRegisteredSource(QuerySourceDefinition sourceDefinition)
        {
            return _context.QueryDefinition.Sources.Any(source => ReferenceEquals(source, sourceDefinition));
        }

        // Resolves the next deterministic generated alias.
        private string ResolveGeneratedAlias()
        {
            var index = _context.AliasRegistry.Count;
            var alias = QueryAliasGeneratorHelper.Generate(index);

            while (_context.AliasRegistry.Contains(alias))
            {
                index++;
                alias = QueryAliasGeneratorHelper.Generate(index);
            }

            return alias;
        }

        // Resolves a unique alias from the requested alias.
        private string ResolveUniqueAlias(string requestedAlias)
        {
            if (!_context.AliasRegistry.Contains(requestedAlias))
                return requestedAlias;

            var suffix = 1;

            while (_context.AliasRegistry.Contains($"{requestedAlias}{suffix}"))
                suffix++;

            return $"{requestedAlias}{suffix}";
        }
    }
}
