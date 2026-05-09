using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.Sql.Enums
{
    /// <summary>
    /// Represents the SQL ordering direction used in ORDER BY clauses.
    /// </summary>
    public enum QueryOrderingDirection
    {
        /// <summary>
        /// Ascending ordering.
        /// </summary>
        Ascending = 1,

        /// <summary>
        /// Descending ordering.
        /// </summary>
        Descending = 2
    }
}
