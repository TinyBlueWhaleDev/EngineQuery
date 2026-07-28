using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.Abstractions.Enums
{
    /// <summary>
    /// Defines the logical operator used to connect a WHERE predicate
    /// with the predicate that immediately precedes it.
    /// </summary>
    public enum QueryLogicalOperator
    {
        /// <summary>
        /// Connects the predicate using a logical AND operation.
        /// </summary>
        And = 1,

        /// <summary>
        /// Connects the predicate using a logical OR operation.
        /// </summary>
        Or = 2
    }
}
