using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.Core.QueryDefinitions
{
    /// <summary>
    /// Represents a GROUP BY definition used to generate SQL grouping clauses.
    /// </summary>
    public sealed record QueryGroupByDefinition
    {
        /// <summary>
        /// Gets the grouped columns included in this grouping definition.
        /// </summary>
        public required IReadOnlyList<QueryColumnDefinition> Columns { get; init; }

        /// <summary>
        /// Gets the CLR entity type that owns the grouped properties.
        /// </summary>
        public Type? SourceType { get; init; }

        /// <summary>
        /// Gets the table alias associated with the grouped source.
        /// </summary>
        public string? SourceAlias { get; init; }

        /// <summary>
        /// Gets the property-to-column mappings associated with the grouped source.
        /// </summary>
        public IReadOnlyDictionary<string, string>? SourceColumnMappings { get; init; }
    }   
}
