using System.Linq.Expressions;

namespace TinyBlueWhale.EngineQuery.Core.QueryDefinitions
{
    /// <summary>
    /// Represents an IN subquery condition.
    /// </summary>
    public sealed record QueryInSubqueryDefinition
    {
        /// <summary>
        /// Gets the outer property expression evaluated by the IN condition.
        /// </summary>
        public required LambdaExpression OuterSelector { get; init; }

        /// <summary>
        /// Gets the outer query source associated with the selected property.
        /// </summary>
        public required QuerySourceDefinition OuterSource { get; init; }

        /// <summary>
        /// Gets the compiled subquery definition.
        /// </summary>
        public required CompiledQueryDefinition Subquery { get; init; }
    }
}
