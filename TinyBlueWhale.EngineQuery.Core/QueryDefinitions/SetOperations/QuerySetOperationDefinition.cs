using TinyBlueWhale.EngineQuery.Abstractions.Enums;

namespace TinyBlueWhale.EngineQuery.Core.QueryDefinitions.SetOperations
{
    /// <summary>
    /// Represents a SQL set operation query definition.
    /// </summary>
    public sealed record QuerySetOperationDefinition
    {
        /// <summary>
        /// Gets the SQL set operation.
        /// </summary>
        public required QuerySetOperation Operation { get; init; }

        /// <summary>
        /// Gets the compiled query definition used by the set operation.
        /// </summary>
        public required CompiledQueryDefinition Query { get; init; }
    }
}
