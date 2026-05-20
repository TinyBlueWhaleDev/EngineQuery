using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Core.Interfaces;

namespace TinyBlueWhale.EngineQuery.PostgreSql.Dialects
{
    /// <summary>
    /// PostgreSql implementation of database-specific SQL syntax rules.
    /// </summary>
    /// <remarks>
    /// Responsible for generating PostgreSql compatible fragments such as
    /// escaped identifiers and pagination clauses.
    /// </remarks>
    public sealed class PostgreSqlDatabaseDialect : ISqlDatabaseDialect
    {
        /// <summary>
        /// Escapes a SQL identifier using PostgreSql bracket syntax.
        /// </summary>
        /// <param name="identifier">
        /// Identifier to escape.
        /// </param>
        /// <returns>
        /// Escaped PostgreSql identifier.
        /// </returns>
        public string EscapeIdentifier(string identifier) => $"\"{identifier}\"";

        /// <summary>
        /// Builds a PostgreSql pagination clause using LIMIT/OFFSET syntax.
        /// </summary>
        /// <param name="skip">
        /// Number of rows to skip.
        /// </param>
        /// <param name="take">
        /// Maximum number of rows to return.
        /// </param>
        /// <returns>
        /// PostgreSql pagination clause.
        /// </returns>
        public string BuildPaginationClause(int? skip, int? take)
        {
            if (!skip.HasValue && !take.HasValue)
                return string.Empty;

            if (take.HasValue && skip.HasValue)
                return $"LIMIT {take.Value} OFFSET {skip.Value}";

            if (take.HasValue)
                return $"LIMIT {take.Value}";

            return $"OFFSET {skip!.Value}";
        }

        /// <summary>
        /// Builds a PostgreSQL qualified identifier using double quote syntax.
        /// </summary>
        /// <param name="qualifier">
        /// Table name or alias used to qualify the identifier.
        /// </param>
        /// <param name="identifier">
        /// Database identifier to qualify.
        /// </param>
        /// <returns>
        /// PostgreSQL qualified identifier.
        /// </returns>
        public string BuildQualifiedIdentifier(string qualifier, string identifier)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(qualifier);
            ArgumentException.ThrowIfNullOrWhiteSpace(identifier);

            return $"{EscapeIdentifier(qualifier)}.{EscapeIdentifier(identifier)}";
        }

        /// <summary>
        /// Resolves the provider-specific scalar function name.
        /// </summary>
        /// <param name="functionName">
        /// Canonical scalar function name.
        /// </param>
        /// <returns>
        /// Provider-specific scalar function name.
        /// </returns>
        public string ResolveScalarFunctionName(string functionName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(functionName);

            return functionName;
        }

    }
}
