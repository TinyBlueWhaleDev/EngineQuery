using TinyBlueWhale.EngineQuery.Core.Enums;

namespace TinyBlueWhale.EngineQuery.Core.QueryDefinitions
{
    /// <summary>
    /// Represents an ordering definition used to generate SQL ORDER BY clauses.
    /// </summary>
    public sealed record QueryOrderingDefinition()
    {
        /// <summary>
        /// Gets the ordered columns included in this ordering group.
        /// </summary>
        public required IReadOnlyList<QueryOrderingColumnDefinition> Columns { get; init; }

        /// <summary>
        /// Gets the ordering direction applied to the property.
        /// </summary>
        public QueryOrderingDirection Direction { get; init; }

        /// <summary>
        /// Gets the CLR entity type that owns the ordered property.
        /// </summary>
        public Type? SourceType { get; init; }

        /// <summary>
        /// Gets the table alias associated with the ordered source.
        /// </summary>
        public string? SourceAlias { get; init; }

        /// <summary>
        /// Gets the property-to-column mappings associated with the ordered source.
        /// </summary>
        public IReadOnlyDictionary<string, string>? SourceColumnMappings { get; init; }
    };

    /// <summary>
    /// Represents a single column used inside an ORDER BY definition.
    /// </summary>
    public sealed record QueryOrderingColumnDefinition
    {
        /// <summary>
        /// Gets the entity property name used for ordering.
        /// </summary>
        public required string PropertyName { get; init; }
    }
}
