using System.Linq.Expressions;

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
        /// Gets the query sources associated with the expression parameters.
        /// </summary>
        public required IReadOnlyDictionary<ParameterExpression, QuerySourceDefinition> Sources { get; init; }
    }
}
