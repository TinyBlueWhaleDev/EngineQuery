using System.Linq.Expressions;

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
        /// Gets the query source associated with the selected column.
        /// </summary>
        public required QuerySourceDefinition Source { get; init; }
    }
}
