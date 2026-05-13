using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.Abstractions.Enums
{
    /// <summary>
    /// Represents SQL aggregate functions supported by query generation.
    /// </summary>
    public enum QueryAggregateFunction
    {
        Count = 1,
        Sum = 2,
        Average = 3,
        Minimum = 4,
        Maximum = 5
    }
}
