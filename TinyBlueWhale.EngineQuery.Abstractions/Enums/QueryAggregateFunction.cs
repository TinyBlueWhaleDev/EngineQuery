
namespace TinyBlueWhale.EngineQuery.Abstractions.Enums
{
    /// <summary>
    /// Represents SQL aggregate functions supported by query generation.
    /// </summary>
    public enum QueryAggregateFunction
    {
        /// <summary>
        /// Counts the number of rows or non-null values.
        /// </summary>
        Count = 1,

        /// <summary>
        /// Calculates the sum of numeric values.
        /// </summary>
        Sum = 2,

        /// <summary>
        /// Calculates the average of numeric values.
        /// </summary>
        Average = 3,

        /// <summary>
        /// Returns the minimum value.
        /// </summary>
        Minimum = 4,

        /// <summary>
        /// Returns the maximum value.
        /// </summary>
        Maximum = 5
    }
}
