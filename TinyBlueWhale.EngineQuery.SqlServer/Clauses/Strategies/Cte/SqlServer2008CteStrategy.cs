using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Sql.Clauses.Cte;
using TinyBlueWhale.EngineQuery.Sql.Interfaces.Strategies;

namespace TinyBlueWhale.EngineQuery.SqlServer.Clauses.Strategies.Cte
{
    /// <summary>
    /// Provides SQL Server 2008 common table expression behavior.
    /// </summary>
    public sealed class SqlServer2008CteStrategy : CteStrategy
    {
        /// <inheritdoc />
        public override string ResolveRecursiveCteKeyword()
        {
            return "WITH";
        }
    }
}
