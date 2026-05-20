using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.Abstractions.Enums
{
    /// <summary>
    /// Represents SQL comparison operators supported by query generation.
    /// </summary>
    public enum QueryComparisonOperator
    {
        /// <summary>
        /// Determines whether two values are equal.
        /// </summary>
        Equal = 1,

        /// <summary>
        /// Determines whether two values are not equal.
        /// </summary>
        NotEqual = 2,

        /// <summary>
        /// Determines whether the left value is greater than the right value.
        /// </summary>
        GreaterThan = 3,

        /// <summary>
        /// Determines whether the left value is greater than or equal to the right value.
        /// </summary>
        GreaterThanOrEqual = 4,

        /// <summary>
        /// Determines whether the left value is less than the right value.
        /// </summary>
        LessThan = 5,

        /// <summary>
        /// Determines whether the left value is less than or equal to the right value.
        /// </summary>
        LessThanOrEqual = 6
    }
}
