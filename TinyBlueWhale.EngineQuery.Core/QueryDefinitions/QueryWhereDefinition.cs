using System.Linq.Expressions;

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
    }
}
