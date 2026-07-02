
namespace TinyBlueWhale.EngineQuery.Core.QueryDefinitions
{
    /// <summary>
    /// Represents a common table expression used during SQL generation.
    /// </summary>
    public sealed record QueryCteDefinition
    {
        /// <summary>
        /// Gets the common table expression name.
        /// </summary>
        public required string Name { get; init; }

        /// <summary>
        /// Gets whether the common table expression is recursive.
        /// </summary>
        public bool IsRecursive { get; init; }

        /// <summary>
        /// Gets the compiled query definition used by the common table expression.
        /// </summary>
        public required CompiledQueryDefinition Query { get; init; }
    }
}
