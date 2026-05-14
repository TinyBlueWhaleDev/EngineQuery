using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Abstractions.Enums;

namespace TinyBlueWhale.EngineQuery.Core.QueryDefinitions
{
    /// <summary>
    /// Represents a scalar SQL function projection.
    /// </summary>
    public sealed record QueryScalarFunctionDefinition
    {
        /// <summary>
        /// Gets the scalar SQL function applied to the selected column.
        /// </summary>
        public required QueryScalarFunction Function { get; init; }

        /// <summary>
        /// Gets the entity property name used by the scalar function.
        /// </summary>
        public string? PropertyName { get; init; }

        /// <summary>
        /// Gets the scalar function arguments.
        /// </summary>
        public IReadOnlyList<QueryScalarFunctionArgumentDefinition> Arguments { get; init; } = [];

        /// <summary>
        /// Gets the SQL alias assigned to the function result.
        /// </summary>
        public required string Alias { get; init; }

        /// <summary>
        /// Gets the CLR entity type that owns the selected property.
        /// </summary>
        public Type? SourceType { get; init; }

        /// <summary>
        /// Gets the table alias associated with the scalar function source.
        /// </summary>
        public string? SourceAlias { get; init; }

        /// <summary>
        /// Gets the property-to-column mappings associated with the scalar function source.
        /// </summary>
        public IReadOnlyDictionary<string, string>? SourceColumnMappings { get; init; }
    }
}
