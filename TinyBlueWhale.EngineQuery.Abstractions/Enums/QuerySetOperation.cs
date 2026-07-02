
namespace TinyBlueWhale.EngineQuery.Abstractions.Enums
{
    /// <summary>
    /// Represents supported SQL set operations.
    /// </summary>
    public enum QuerySetOperation
    {
        /// <summary>
        /// Combines query results and removes duplicates.
        /// </summary>
        Union = 1,

        /// <summary>
        /// Combines query results while preserving duplicates.
        /// </summary>
        UnionAll = 2,

        /// <summary>
        /// Returns rows common to both query results.
        /// </summary>
        Intersect = 3,

        /// <summary>
        /// Returns rows from the first query that are not present in the second query.
        /// </summary>
        Except = 4
    }
}
