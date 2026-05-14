using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.Core.QueryDefinitions
{
    /// <summary>
    /// Represents a CASE WHEN SELECT expression used during SQL generation.
    /// </summary>
    public sealed record QueryCaseWhenDefinition
    {
        /// <summary>
        /// Gets the condition evaluated by the CASE WHEN expression.
        /// </summary>
        public required LambdaExpression ConditionExpression { get; init; }

        /// <summary>
        /// Gets the value returned when the condition is true.
        /// </summary>
        public required object? WhenTrueValue { get; init; }

        /// <summary>
        /// Gets the value returned when the condition is false.
        /// </summary>
        public required object? WhenFalseValue { get; init; }

        /// <summary>
        /// Gets the SQL alias assigned to the CASE WHEN expression result.
        /// </summary>
        public required string Alias { get; init; }

        /// <summary>
        /// Gets the CLR entity type that owns the CASE WHEN condition source.
        /// </summary>
        public Type? SourceType { get; init; }

        /// <summary>
        /// Gets the table alias associated with the CASE WHEN condition source.
        /// </summary>
        public string? SourceAlias { get; init; }

        /// <summary>
        /// Gets the property-to-column mappings associated with the CASE WHEN condition source.
        /// </summary>
        public IReadOnlyDictionary<string, string>? SourceColumnMappings { get; init; }
    }
}
