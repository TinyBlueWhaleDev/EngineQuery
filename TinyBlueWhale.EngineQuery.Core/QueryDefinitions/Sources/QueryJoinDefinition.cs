using System.Linq.Expressions;
using TinyBlueWhale.EngineQuery.Core.Enums;

namespace TinyBlueWhale.EngineQuery.Core.QueryDefinitions.Sources
{
    /// <summary>
    /// Represents a SQL JOIN definition associated with a query.
    /// </summary>
    public sealed class QueryJoinDefinition
    {
        /// <summary>
        /// Gets the SQL join type.
        /// </summary>
        public required QueryJoinType JoinType { get; init; }

        /// <summary>
        /// Gets the query source located on the left side of the join predicate.
        /// </summary>
        public required QuerySourceDefinition Source { get; init; }

        /// <summary>
        /// Gets the query source introduced by the JOIN clause.
        /// </summary>
        public required QuerySourceDefinition JoinSource { get; init; }

        /// <summary>
        /// Gets the join predicate expression.
        /// </summary>
        public required LambdaExpression JoinExpression { get; init; }
    }
}
