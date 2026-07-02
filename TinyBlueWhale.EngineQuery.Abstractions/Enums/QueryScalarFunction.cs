
namespace TinyBlueWhale.EngineQuery.Abstractions.Enums
{
    /// <summary>
    /// Represents scalar SQL functions supported by query generation.
    /// </summary>
    public enum QueryScalarFunction
    {
        /// <summary>
        /// Converts a string value to lowercase.
        /// </summary>
        Lower = 1,

        /// <summary>
        /// Converts a string value to uppercase.
        /// </summary>
        Upper = 2,

        /// <summary>
        /// Returns the length of a string value.
        /// </summary>
        Length = 3,

        /// <summary>
        /// Removes leading and trailing whitespace from a string value.
        /// </summary>
        Trim = 4,

        /// <summary>
        /// Returns the first non-null value from the provided expressions.
        /// </summary>
        Coalesce = 5,

        /// <summary>
        /// Concatenates multiple string values into a single string.
        /// </summary>
        Concat = 6
    }
}
