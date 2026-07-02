
namespace TinyBlueWhale.EngineQuery.Core.QueryDefinitions
{
    /// <summary>
    /// Represents a query column referenced by SQL clauses such as GROUP BY or ORDER BY.
    /// </summary>
    public sealed record QueryColumnDefinition
    {
        /// <summary>
        /// Gets the entity property name associated with the query column.
        /// </summary>
        public required string PropertyName { get; init; }
    }
}
