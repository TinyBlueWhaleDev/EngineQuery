using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.Abstractions.Enums
{
    /// <summary>
    /// Represents scalar SQL functions supported by query generation.
    /// </summary>
    public enum QueryScalarFunction
    {
        Lower = 1,
        Upper = 2,
        Length = 3,
        Trim = 4,
        Coalesce = 5,
        Concat = 6
    }
}
