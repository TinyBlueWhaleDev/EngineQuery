using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.Abstractions.Enums
{
    /// <summary>
    /// Represents supported SQL window functions.
    /// </summary>
    public enum QueryWindowFunction
    {
        /// <summary>
        /// Assigns a sequential number to each row within the window partition.
        /// </summary>
        RowNumber = 1,

        /// <summary>
        /// Assigns a rank to each row within the window partition with gaps for duplicate values.
        /// </summary>
        Rank = 2,

        /// <summary>
        /// Assigns a rank to each row within the window partition without gaps for duplicate values.
        /// </summary>
        DenseRank = 3,

        /// <summary>
        /// Returns the value from a preceding row within the window partition.
        /// </summary>
        Lag = 4,

        /// <summary>
        /// Returns the value from a following row within the window partition.
        /// </summary>
        Lead = 5,

        /// <summary>
        /// Returns the first value within the window partition.
        /// </summary>
        FirstValue = 6,

        /// <summary>
        /// Returns the last value within the window partition.
        /// </summary>
        LastValue = 7,

        /// <summary>
        /// Distributes rows into a specified number of groups within the window partition.
        /// </summary>
        Ntile = 8
    }
}
