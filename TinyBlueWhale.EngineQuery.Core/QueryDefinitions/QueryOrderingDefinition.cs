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
        public required IReadOnlyList<QueryColumnDefinition> Columns { get; init; }

        /// <summary>
        /// Gets the ordering direction applied to the property.
        /// </summary>
        public QueryOrderingDirection Direction { get; init; }

        /// <summary>
        /// Gets the query source associated with the selected column.
        /// </summary>
        public required QuerySourceDefinition Source { get; init; }
    };
}
