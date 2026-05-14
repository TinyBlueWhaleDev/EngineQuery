using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Core.ExpressionScopes;

namespace TinyBlueWhale.EngineQuery.Core.QueryDefinitions
{
    /// <summary>
    /// Represents an EXISTS subquery condition.
    /// </summary>
    public sealed record QueryExistsDefinition
    {
        /// <summary>
        /// Gets the compiled subquery definition.
        /// </summary>
        public required CompiledQueryDefinition Subquery { get; init; }
    }
}
