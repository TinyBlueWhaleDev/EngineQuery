using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions.Sources;

namespace TinyBlueWhale.EngineQuery.Core.QueryDefinitions.Projection
{
    /// <summary>
    /// Represents an argument used by a SQL window function.
    /// </summary>
    public sealed record QueryWindowFunctionArgumentDefinition
    {
        /// <summary>
        /// Gets the argument type.
        /// </summary>
        public required QueryWindowFunctionArgumentType ArgumentType { get; init; }

        /// <summary>
        /// Gets the column used when the argument represents a column reference.
        /// </summary>
        public QueryColumnDefinition? Column { get; init; }

        /// <summary>
        /// Gets the query source associated with the column argument.
        /// </summary>
        public QuerySourceDefinition? Source { get; init; }

        /// <summary>
        /// Gets the constant value used when the argument represents a parameterized value.
        /// </summary>
        public object? ConstantValue { get; init; }
    }
}
