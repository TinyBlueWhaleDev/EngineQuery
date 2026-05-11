using TinyBlueWhale.EngineQuery.Abstractions.Enums;

namespace TinyBlueWhale.EngineQuery.Abstractions.Interfaces
{
    /// <summary>
    /// Defines a factory contract for resolving query engines by database provider.
    /// </summary>
    public interface IQueryBuilderFactory
    {
        /// <summary>
        /// Resolves a query builder configured for the specified database provider.
        /// </summary>
        /// <param name="provider">
        /// Database provider associated with the query builder.
        /// </param>
        /// <returns>
        /// Query builder configured for the requested provider.
        /// </returns>
        IQueryBuilder For(DatabaseProvider provider);
    }
}
