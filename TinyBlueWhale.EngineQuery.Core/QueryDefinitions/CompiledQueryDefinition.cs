
namespace TinyBlueWhale.EngineQuery.Core.QueryDefinitions
{
    /// <summary>
    /// Represents the internal query definition used by the SQL compiler.
    /// </summary>
    /// <remarks>
    /// This model captures query intent before SQL text is generated.
    /// It is not exposed to consumers of the public API.
    /// </remarks>
    public sealed class CompiledQueryDefinition
    {
        /// <summary>
        /// Gets or sets the source table name associated with the query.
        /// </summary>
        public required string TableName { get; set; }

        /// <summary>
        /// Gets or sets the optional table alias used to qualify generated SQL column references.
        /// </summary>
        public string? TableAlias { get; set; }

        /// <summary>
        /// Gets or sets the property-to-column mapping used during SQL generation.
        /// </summary>
        public IReadOnlyDictionary<string, string> ColumnMappings { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Gets the selected columns included in the query projection.
        /// </summary>
        public List<QuerySelectColumnDefinition> SelectDefinitions { get; } = [];

        /// <summary>
        /// Gets the filtering definitions used to generate SQL WHERE clauses.
        /// </summary>
        public List<QueryWhereDefinition> WhereDefinitions { get; } = [];

        /// <summary>
        /// Gets the ordering definitions used to generate SQL ORDER BY clauses.
        /// </summary>
        public List<QueryOrderingDefinition> OrderingDefinitions { get; } = [];

        /// <summary>
        /// Gets or sets the pagination definition used to generate SQL paging syntax.
        /// </summary>
        public QueryPaginationDefinition Pagination { get; set; } = new();

    }
}
