using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.Core.QueryDefinitions
{
    /// <summary>
    /// Represents identity retrieval metadata associated with a direct INSERT VALUES command.
    /// </summary>
    public sealed class QueryInsertIdentityDefinition
    {
        /// <summary>
        /// Gets or initializes the optional target identity column.
        /// </summary>
        /// <remarks>
        /// SQL Server and MySQL use connection-scoped identity functions and do not require this value.
        /// PostgreSQL requires the mapped target column for its RETURNING clause.
        /// </remarks>
        public string? ColumnName { get; init; }
    }
}
