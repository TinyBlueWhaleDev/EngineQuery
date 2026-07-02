
namespace TinyBlueWhale.EngineQuery.Core.Enums
{
    /// <summary>
    /// Represents the SQL join type used in a query definition.
    /// </summary>
    public enum QueryJoinType
    {
        /// <summary>
        /// Returns only rows with matching values in both joined sources.
        /// </summary>
        Inner = 1,

        /// <summary>
        /// Returns all rows from the left source and matching rows from the right source.
        /// </summary>
        Left = 2
    }
}
