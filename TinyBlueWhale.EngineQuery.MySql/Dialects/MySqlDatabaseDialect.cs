using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Core.Interfaces;

namespace TinyBlueWhale.EngineQuery.MySql.Dialects
{
    public sealed class MySqlDatabaseDialect : ISqlDatabaseDialect
    {
        public string EscapeIdentifier(string identifier) => $"`{identifier}`";

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
        /// Builds a MySQL qualified identifier using backtick syntax.
        /// </summary>
        /// <param name="qualifier">
        /// Table name or alias used to qualify the identifier.
        /// </param>
        /// <param name="identifier">
        /// Database identifier to qualify.
        /// </param>
        /// <returns>
        /// MySQL qualified identifier.
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
