using TinyBlueWhale.EngineQuery.Abstractions.Enums;

namespace TinyBlueWhale.EngineQuery.Core.QueryDefinitions.Sources
{
    /// <summary>
    /// Represents an APPLY or LATERAL subquery join definition.
    /// </summary>
    public sealed record QueryApplyDefinition
    {
        /// <summary>
        /// Gets the APPLY join type.
        /// </summary>
        public required QueryApplyType ApplyType { get; init; }

        /// <summary>
        /// Gets the alias assigned to the APPLY subquery.
        /// </summary>
        public required string Alias { get; init; }

        /// <summary>
        /// Gets the compiled subquery definition used by the APPLY join.
        /// </summary>
        public required CompiledQueryDefinition Subquery { get; init; }
    }
}
