
namespace TinyBlueWhale.EngineQuery.Core.QueryDefinitions
{
    /// <summary>
    /// Represents a GROUP BY definition used to generate SQL grouping clauses.
    /// </summary>
    public sealed record QueryGroupByDefinition
    {
        /// <summary>
        /// Gets the grouped columns included in this grouping definition.
        /// </summary>
        public required IReadOnlyList<QueryColumnDefinition> Columns { get; init; }

        /// <summary>
        /// Gets the query source associated with the selected column.
        /// </summary>
        public required QuerySourceDefinition Source { get; init; }
    }   
}
