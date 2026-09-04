using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions.Sources;

namespace TinyBlueWhale.EngineQuery.Core.QueryDefinitions.Filtering
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
        /// Gets the query source associated with the selected column.
        /// </summary>
        public required QuerySourceDefinition Source { get; init; }
    }
}
