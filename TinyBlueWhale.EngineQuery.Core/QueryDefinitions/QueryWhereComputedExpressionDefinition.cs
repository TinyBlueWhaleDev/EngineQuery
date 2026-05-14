using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.Core.QueryDefinitions
{
    /// <summary>
    /// Represents a WHERE condition based on a computed SQL expression.
    /// </summary>
    public sealed record QueryWhereComputedExpressionDefinition
    {
        /// <summary>
        /// Gets the computed boolean expression used by the WHERE condition.
        /// </summary>
        public required LambdaExpression Expression { get; init; }

        /// <summary>
        /// Gets the CLR entity type that owns the computed expression source.
        /// </summary>
        public Type? SourceType { get; init; }

        /// <summary>
        /// Gets the table alias associated with the computed expression source.
        /// </summary>
        public string? SourceAlias { get; init; }

        /// <summary>
        /// Gets the property-to-column mappings associated with the computed expression source.
        /// </summary>
        public IReadOnlyDictionary<string, string>? SourceColumnMappings { get; init; }
    }
}
