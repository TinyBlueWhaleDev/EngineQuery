using TinyBlueWhale.EngineQuery.Core.Enums;

namespace TinyBlueWhale.EngineQuery.Core.QueryDefinitions
{
    /// <summary>
    /// Represents an ordering definition used to generate SQL ORDER BY clauses.
    /// </summary>
    public sealed record QueryOrderingDefinition()
    {
        /// <summary>
        /// Gets the entity property name used for ordering.
        /// </summary>
        public string PropertyName { get; init; } = null!;

        /// <summary>
        /// Gets the ordering direction applied to the property.
        /// </summary>
        public QueryOrderingDirection Direction { get; init; }
    };    
}
