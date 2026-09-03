using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Sql.Interfaces.Strategies;

namespace TinyBlueWhale.EngineQuery.Sql.Clauses.Cte
{
    /// <summary>
    /// Provides the standard SQL behavior required to build common table expression clauses.
    /// </summary>
    public class CteStrategy : ICTEStrategy
    {
        /// <inheritdoc />
        public virtual string ResolveRecursiveCteKeyword()
        {
            return "WITH RECURSIVE";
        }
    }
}
