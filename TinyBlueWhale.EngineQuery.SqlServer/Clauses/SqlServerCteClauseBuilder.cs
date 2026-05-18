using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Sql.Clauses;
using TinyBlueWhale.EngineQuery.Sql.Compilation;

namespace TinyBlueWhale.EngineQuery.SqlServer.Clauses
{
    /// <summary>
    /// Builds SQL Server common table expression clauses.
    /// </summary>
    /// <remarks>
    /// SQL Server uses the WITH keyword for both recursive and non-recursive common table expressions.
    /// </remarks>
    /// <remarks>
    /// Initializes a new instance of the <see cref="SqlServerCteClauseBuilder"/> class.
    /// </remarks>
    /// <param name="subqueryCompiler">
    /// Subquery compiler used to compile CTE query definitions.
    /// </param>
    public sealed class SqlServerCteClauseBuilder(SubqueryCompiler subqueryCompiler) : CteClauseBuilder(subqueryCompiler)
    {

        /// <summary>
        /// Resolves the SQL keyword used for SQL Server common table expressions.
        /// </summary>
        /// <returns>
        /// SQL Server CTE keyword.
        /// </returns>
        protected override string ResolveRecursiveCteKeyword()
        {
            return "WITH";
        }
    }
}
