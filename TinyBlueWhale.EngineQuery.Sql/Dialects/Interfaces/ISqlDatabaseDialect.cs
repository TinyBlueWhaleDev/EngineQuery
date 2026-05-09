using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.Sql.Dialects.Interfaces
{
    /// <summary>
    /// Defines provider-specific SQL syntax behavior required during query compilation.
    /// </summary>
    /// <remarks>
    /// Database dialects are responsible only for SQL syntax generation.
    /// They do not execute queries or manage database connections.
    /// </remarks>
    public interface ISqlDatabaseDialect
    {
        /// <summary>
        /// Escapes a database identifier using provider-specific syntax.
        /// </summary>
        /// <param name="identifier">
        /// Database identifier to escape.
        /// </param>
        /// <returns>
        /// Escaped database identifier.
        /// </returns>
        string EscapeIdentifier(string identifier);

        /// <summary>
        /// Builds a provider-specific SQL pagination clause.
        /// </summary>
        /// <param name="skip">
        /// Number of rows to skip.
        /// </param>
        /// <param name="take">
        /// Maximum number of rows to return.
        /// </param>
        /// <returns>
        /// Provider-specific pagination SQL fragment.
        /// </returns>
        string BuildPaginationClause(int? skip,int? take);
    }
}
