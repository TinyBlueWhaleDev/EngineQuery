using TinyBlueWhale.EngineQuery.DependencyInjection.Enums;
using TinyBlueWhale.EngineQuery.Metadata.Models;

namespace TinyBlueWhale.EngineQuery.DependencyInjection.Interfaces
{
    /// <summary>
    /// Creates configured EngineQuery instances.
    /// </summary>
    public interface IQueryEngineFactory
    {
        /// <summary>
        /// Creates a query engine for the specified provider.
        /// </summary>
        /// <param name="provider">
        /// Query engine provider.
        /// </param>
        /// <returns>
        /// Configured query engine.
        /// </returns>
        IQueryEngine Create(QueryEngineProvider provider);

        /// <summary>
        /// Creates a query engine for the specified provider and metadata strategy.
        /// </summary>
        /// <param name="provider">
        /// Query engine provider.
        /// </param>
        /// <param name="metadataStrategy">
        /// Metadata strategy.
        /// </param>
        /// <returns>
        /// Configured query engine.
        /// </returns>
        IQueryEngine Create(QueryEngineProvider provider, MetadataStrategy metadataStrategy);
    }
}
