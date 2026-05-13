using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Abstractions.Enums;

namespace TinyBlueWhale.EngineQuery.Core.QueryDefinitions
{
    /// <summary>
    /// Represents an aggregate SELECT expression used during SQL generation.
    /// </summary>
    public sealed record QueryAggregateDefinition
    {
        /// <summary>
        /// Gets the aggregate function applied to the selected column.
        /// </summary>
        public required QueryAggregateFunction Function { get; init; }

        /// <summary>
        /// Gets the entity property name used by the aggregate function.
        /// </summary>
        public required string PropertyName { get; init; }

        /// <summary>
        /// Gets the required SQL alias assigned to the aggregate result.
        /// </summary>
        public required string Alias { get; init; }

        /// <summary>
        /// Gets the CLR entity type that owns the aggregated property.
        /// </summary>
        public Type? SourceType { get; init; }

        /// <summary>
        /// Gets the table alias associated with the aggregate source.
        /// </summary>
        public string? SourceAlias { get; init; }

        /// <summary>
        /// Gets the property-to-column mappings associated with the aggregate source.
        /// </summary>
        public IReadOnlyDictionary<string, string>? SourceColumnMappings { get; init; }
    }
}
