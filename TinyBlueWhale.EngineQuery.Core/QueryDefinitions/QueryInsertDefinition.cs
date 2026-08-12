using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.Core.QueryDefinitions
{
    /// <summary>
    /// Represents the INSERT-specific metadata associated with a compiled command definition.
    /// </summary>
    public sealed class QueryInsertDefinition
    {
        /// <summary>
        /// Gets the values assigned to target INSERT columns.
        /// </summary>
        public List<QueryInsertValueDefinition> ValueDefinitions { get; } = [];
    }
}
