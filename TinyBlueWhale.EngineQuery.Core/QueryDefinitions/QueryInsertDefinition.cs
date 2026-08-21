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

        /// <summary>
        /// Gets the explicitly configured target INSERT columns.
        /// </summary>
        public List<QueryInsertColumnDefinition> ColumnDefinitions { get; } = [];

        /// <summary>
        /// Gets or sets the source definition associated with an INSERT SELECT command.
        /// </summary>
        public QuerySourceDefinition? SourceDefinition { get; set; }

        /// <summary>
        /// Gets or sets the identity retrieval definition associated with a direct INSERT VALUES command.
        /// </summary>
        public QueryInsertIdentityDefinition? IdentityDefinition { get; set; }
    }
}
