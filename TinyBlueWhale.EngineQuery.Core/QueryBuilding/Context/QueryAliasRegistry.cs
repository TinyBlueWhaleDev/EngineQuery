
namespace TinyBlueWhale.EngineQuery.Core.QueryBuilding.Context
{    

    /// <summary>
    /// Tracks aliases registered within the current query scope.
    /// </summary>
    internal sealed class QueryAliasRegistry
    {
        private readonly HashSet<string> _aliases = [];

        /// <summary>
        /// Registers a query alias.
        /// </summary>
        public void Register(string? alias)
        {
            if (string.IsNullOrWhiteSpace(alias))
                return;

            if (!_aliases.Add(alias))
                throw new InvalidOperationException($"Alias '{alias}' is already registered in the current query scope.");
        }

        /// <summary>
        /// Determines whether the specified alias is already registered.
        /// </summary>
        public bool Contains(string? alias)
        {
            return !string.IsNullOrWhiteSpace(alias)
                && _aliases.Contains(alias);
        }

        public int Count => _aliases.Count;
    }
}
