using System.Linq.Expressions;

namespace TinyBlueWhale.EngineQuery.Core.QueryDefinitions
{
    /// <summary>
    /// Represents an IN or NOT IN collection condition.
    /// </summary>
    public sealed record QueryWhereCollectionDefinition
    {
        /// <summary>
        /// Gets the property expression evaluated by the collection condition.
        /// </summary>
        public required LambdaExpression Selector { get; init; }

        /// <summary>
        /// Gets the query source associated with the selected property.
        /// </summary>
        public required QuerySourceDefinition Source { get; init; }

        /// <summary>
        /// Gets the values evaluated by the collection condition.
        /// </summary>
        public required IReadOnlyList<object> Values { get; init; }

        /// <summary>
        /// Gets a value indicating whether the collection condition is negated.
        /// </summary>
        public bool IsNegated { get; init; }
    }
}
