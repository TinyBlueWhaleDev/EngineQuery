
namespace TinyBlueWhale.EngineQuery.Core.QueryDefinitions
{

    /// <summary>
    /// Represents a query source available in the current SQL generation scope.
    /// </summary>
    public sealed record QuerySourceDefinition
    {
        /// <summary>
        /// Gets the CLR entity type associated with the query source.
        /// </summary>
        public required Type EntityType { get; init; }

        /// <summary>
        /// Gets the optional database schema name associated with the physical query source.
        /// </summary>
        public string? SchemaName { get; init; }

        /// <summary>
        /// Gets the physical database table name associated with the query source.
        /// </summary>
        public string? TableName { get; init; }

        /// <summary>
        /// Gets the derived table subquery associated with the query source.
        /// </summary>
        public CompiledQueryDefinition? Subquery { get; init; }

        /// <summary>
        /// Gets the table alias associated with the query source.
        /// </summary>
        public required string TableAlias { get; init; }

        /// <summary>
        /// Gets the property-to-column mappings associated with the query source.
        /// </summary>
        public required IReadOnlyDictionary<string, string> ColumnMappings { get; init; }

        /// <summary>
        /// Gets a value indicating whether the source represents a physical table.
        /// </summary>
        public bool IsTable => !string.IsNullOrWhiteSpace(TableName);

        /// <summary>
        /// Gets a value indicating whether the source represents a derived table.
        /// </summary>
        public bool IsDerivedTable => Subquery is not null;
    }
}
