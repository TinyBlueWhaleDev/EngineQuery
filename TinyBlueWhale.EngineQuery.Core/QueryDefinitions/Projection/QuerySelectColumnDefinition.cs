using TinyBlueWhale.EngineQuery.Core.QueryDefinitions.Sources;

namespace TinyBlueWhale.EngineQuery.Core.QueryDefinitions.Projection
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
        /// Gets the query source associated with the selected column.
        /// </summary>
        public QuerySourceDefinition? Source { get; init; }
    }
}
