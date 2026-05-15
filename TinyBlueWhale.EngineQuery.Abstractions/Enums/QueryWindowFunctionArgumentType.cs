using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.Abstractions.Enums
{

    /// <summary>
    /// Represents supported SQL window function argument types.
    /// </summary>
    public enum QueryWindowFunctionArgumentType
    {
        /// <summary>
        /// Represents a column reference argument.
        /// </summary>
        Column = 1,

        /// <summary>
        /// Represents a parameterized constant value argument.
        /// </summary>
        Constant = 2
    }
}
