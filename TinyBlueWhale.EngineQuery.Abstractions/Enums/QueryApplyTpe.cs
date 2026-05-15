using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.Abstractions.Enums
{
    /// <summary>
    /// Represents supported SQL APPLY join types.
    /// </summary>
    public enum QueryApplyType
    {
        /// <summary>
        /// Represents CROSS APPLY semantics.
        /// </summary>
        Cross = 1,

        /// <summary>
        /// Represents OUTER APPLY semantics.
        /// </summary>
        Outer = 2
    }
}
