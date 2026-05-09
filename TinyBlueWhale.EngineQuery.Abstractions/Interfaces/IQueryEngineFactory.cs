using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Abstractions.Enums;

namespace TinyBlueWhale.EngineQuery.Abstractions.Interfaces
{
    /// <summary>
    /// Defines a factory contract for resolving query engines by database provider.
    /// </summary>
    public interface IQueryEngineFactory
    {
        /// <summary>
        /// Resolves a query engine configured for the specified database provider.
        /// </summary>
        /// <param name="provider">
        /// Database provider associated with the query engine.
        /// </param>
        /// <returns>
        /// Query engine configured for the requested provider.
        /// </returns>
        IQueryEngine For(DatabaseProvider provider);
    }
}
