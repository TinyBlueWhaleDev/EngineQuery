using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Abstractions.Enums;

namespace TinyBlueWhale.EngineQuery.Core.QueryDefinitions
{
    /// <summary>
    /// Represents a WHERE condition based on a scalar SQL function expression.
    /// </summary>
    public sealed record QueryWhereScalarFunctionDefinition
    {
        /// <summary>
        /// Gets the scalar SQL function used by the WHERE condition.
        /// </summary>
        public required QueryScalarFunction Function { get; init; }

        /// <summary>
        /// Gets the entity property name used by the scalar function.
        /// </summary>
        public required string PropertyName { get; init; }

        /// <summary>
        /// Gets the comparison operator applied to the scalar function result.
        /// </summary>
        public required QueryComparisonOperator ComparisonOperator { get; init; }

        /// <summary>
        /// Gets the comparison value used by the WHERE condition.
        /// </summary>
        public required object? Value { get; init; }

        /// <summary>
        /// Gets the CLR entity type that owns the function property.
        /// </summary>
        public Type? SourceType { get; init; }

        /// <summary>
        /// Gets the table alias associated with the function source.
        /// </summary>
        public string? SourceAlias { get; init; }

        /// <summary>
        /// Gets the property-to-column mappings associated with the function source.
        /// </summary>
        public IReadOnlyDictionary<string, string>? SourceColumnMappings { get; init; }
    }
}
