using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.Abstractions.Models
{
    /// <summary>
    /// Represents a generated SQL query command and its associated parameters.
    /// </summary>
    public sealed record GeneratedSqlQuery
    {
        /// <summary>
        /// Gets the generated SQL command text.
        /// </summary>
        public required string CommandText { get; init; }

        /// <summary>
        /// Gets the parameters associated with the generated SQL command.
        /// </summary>
        public required IReadOnlyList<QuerySqlParameter> Parameters { get; init; }

        /// <summary>
        /// Gets a value indicating whether the generated query contains parameters.
        /// </summary>
        public bool HasParameters => Parameters.Count > 0;
    }
    
}
