using TinyBlueWhale.EngineQuery.Core.Interfaces;

namespace TinyBlueWhale.EngineQuery.SqlServer.Dialects
{
    /// <summary>
    /// SQL Server implementation of database-specific SQL syntax rules.
    /// </summary>
    /// <remarks>
    /// Responsible for generating SQL Server compatible fragments such as
    /// escaped identifiers and pagination clauses.
    /// </remarks>
    public sealed class SqlServerDatabaseDialect : ISqlDatabaseDialect
    {
        /// <summary>
        /// Escapes a SQL identifier using SQL Server bracket syntax.
        /// </summary>
        /// <param name="identifier">
        /// Identifier to escape.
        /// </param>
        /// <returns>
        /// Escaped SQL Server identifier.
        /// </returns>
        public string EscapeIdentifier(string identifier) => $"[{identifier}]";

        /// <summary>
        /// Builds a SQL Server pagination clause using OFFSET/FETCH syntax.
        /// </summary>
        /// <param name="skip">
        /// Number of rows to skip.
        /// </param>
        /// <param name="take">
        /// Maximum number of rows to return.
        /// </param>
        /// <returns>
        /// SQL Server pagination clause.
        /// </returns>
        public string BuildPaginationClause(int? skip,int? take)
        {
            if (!skip.HasValue && !take.HasValue)
                return string.Empty;

            var offset = skip ?? 0;

            if (take.HasValue)
                return $"OFFSET {offset} ROWS FETCH NEXT {take.Value} ROWS ONLY";

            return $"OFFSET {offset} ROWS";
        }
    }
}
