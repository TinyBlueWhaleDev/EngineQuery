using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.Core.QueryDefinitions
{

    /// <summary>
    /// Represents a query source available in the current SQL generation scope.
    /// </summary>
    public sealed record QuerySourceDefinition
    {
        /// <summary>
        /// Gets the CLR entity type associated with the query source.
        /// </summary>
        public required Type EntityType { get; init; }

        /// <summary>
        /// Gets the database table name associated with the query source.
        /// </summary>
        public required string TableName { get; init; }

        /// <summary>
        /// Gets the table alias associated with the query source.
        /// </summary>
        public required string TableAlias { get; init; }

        /// <summary>
        /// Gets the property-to-column mappings associated with the query source.
        /// </summary>
        public required IReadOnlyDictionary<string, string> ColumnMappings { get; init; }
    }
}
