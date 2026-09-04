namespace TinyBlueWhale.EngineQuery.Core.QueryDefinitions.Filtering
{
    /// <summary>
    /// Represents an EXISTS subquery condition.
    /// </summary>
    public sealed record QueryExistsDefinition
    {
        /// <summary>
        /// Gets the compiled subquery definition.
        /// </summary>
        public required CompiledQueryDefinition Subquery { get; init; }

        /// <summary>
        /// Gets whether the EXISTS condition should be negated.
        /// </summary>
        public bool IsNegated { get; init; }
    }
}
