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
        RowNumber = 1,
        Rank = 2,
        DenseRank = 3,
        Lag = 4,
        Lead = 5,        
        FirstValue = 6,
        LastValue = 7
    }
}
