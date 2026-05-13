
namespace TinyBlueWhale.EngineQuery.Core.QueryDefinitions
{
    /// <summary>
    /// Represents a selected column in a SQL query projection.
    /// </summary>
    public sealed record QuerySelectColumnDefinition
    {
        /// <summary>
        /// Gets the entity property name used to generate the selected SQL column.
        /// </summary>
        public required string PropertyName { get; init; }

        /// <summary>
        /// Gets the optional SQL column alias.
        /// </summary>
        public string? Alias { get; init; }

        /// <summary>
        /// Gets the CLR entity type that owns the selected property.
        /// </summary>
        public Type? SourceType { get; init; }

        /// <summary>
        /// Gets the table alias associated with the selected property source.
        /// </summary>
        public string? SourceAlias { get; init; }

        /// <summary>
        /// Gets the property-to-column mappings associated with the selected property source.
        /// </summary>
        public IReadOnlyDictionary<string, string>? SourceColumnMappings { get; init; }
    }
}
