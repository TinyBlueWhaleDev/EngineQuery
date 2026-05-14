using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.Core.QueryDefinitions
{
    /// <summary>
    /// Represents a UNION query definition.
    /// </summary>
    public sealed record QueryUnionDefinition
    {
        /// <summary>
        /// Gets the compiled query definition used by the UNION clause.
        /// </summary>
        public required CompiledQueryDefinition Query { get; init; }

        /// <summary>
        /// Gets whether the UNION query should preserve duplicates.
        /// </summary>
        public bool IncludeDuplicates { get; init; }
    }
}
