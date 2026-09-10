using System.Linq.Expressions;
using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions.Sources;

namespace TinyBlueWhale.EngineQuery.Core.QueryDefinitions.Projection
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
        /// Gets the source property name used by simple column aggregates.
        /// </summary>
        public string? PropertyName { get; init; }

        /// <summary>
        /// Gets the aggregate expression used by computed aggregate projections.
        /// </summary>
        public LambdaExpression? Expression { get; init; }

        /// <summary>
        /// Gets the required SQL alias assigned to the aggregate result.
        /// </summary>
        public required string Alias { get; init; }

        /// <summary>
        /// Gets the query source associated with the selected column.
        /// </summary>
        public required QuerySourceDefinition Source { get; init; }
    }
}
