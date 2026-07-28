using System.Linq.Expressions;
using TinyBlueWhale.EngineQuery.Abstractions.Enums;

namespace TinyBlueWhale.EngineQuery.Core.QueryDefinitions
{
    /// <summary>
    /// Represents a filtering definition used to generate SQL WHERE conditions.
    /// </summary>
    public sealed record QueryWhereDefinition()
    {
        /// <summary>
        /// Gets the predicate expression associated with the filter definition.
        /// </summary>
        public LambdaExpression PredicateExpression { get; init; } = null!;

        /// <summary>
        /// Gets the query source associated with the selected column.
        /// </summary>
        public required QuerySourceDefinition Source { get; init; }

        /// <summary>
        /// Gets the logical operator used to connect this predicate
        /// with the predicate that immediately precedes it.
        /// </summary>
        public QueryLogicalOperator LogicalOperator { get; init; } = QueryLogicalOperator.And;
    }
}
