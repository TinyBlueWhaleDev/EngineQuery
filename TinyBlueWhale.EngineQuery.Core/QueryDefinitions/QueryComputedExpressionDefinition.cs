using System.Linq.Expressions;

namespace TinyBlueWhale.EngineQuery.Core.QueryDefinitions
{
    /// <summary>
    /// Represents a computed SELECT expression used during SQL generation.
    /// </summary>
    public sealed record QueryComputedExpressionDefinition
    {
        /// <summary>
        /// Gets the computed expression.
        /// </summary>
        public required LambdaExpression Expression { get; init; }

        /// <summary>
        /// Gets the SQL alias assigned to the computed expression result.
        /// </summary>
        public required string Alias { get; init; }

        /// <summary>
        /// Gets the query source associated with the selected column.
        /// </summary>
        public required QuerySourceDefinition Source { get; init; }
    }
}
