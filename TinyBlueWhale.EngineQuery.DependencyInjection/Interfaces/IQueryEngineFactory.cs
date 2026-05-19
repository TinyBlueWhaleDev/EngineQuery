using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.DependencyInjection.Enums;

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
        IQueryEngine Create(QueryEngineProvider provider);

        /// <summary>
        /// Creates a query engine for the specified provider and metadata strategy.
        /// </summary>
        IQueryEngine Create(QueryEngineProvider provider, MetadataStrategy metadataStrategy);
    }
}
