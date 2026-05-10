using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Abstractions.Models;

namespace TinyBlueWhale.EngineQuery.Abstractions.Diagnostics
{
    /// <summary>
    /// Represents diagnostic information generated during SQL query compilation.
    /// </summary>
    public sealed class QueryCompilationDebugInfo
    {
        /// <summary>
        /// Gets the database provider used for query generation.
        /// </summary>
        public required DatabaseProvider Provider { get; init; }

        /// <summary>
        /// Gets the generated SQL query and its associated parameters.
        /// </summary>
        public required GeneratedSqlQuery GeneratedQuery { get; init; }

        /// <summary>
        /// Gets the time spent compiling the query into SQL.
        /// </summary>
        public TimeSpan? CompilationTime { get; init; }

        /// <summary>
        /// Gets the total execution time of the generated query.
        /// </summary>
        public TimeSpan? ExecutionTime { get; init; }

        /// <summary>
        /// Gets an optional tag used for query diagnostics or tracing.
        /// </summary>
        public string? QueryTag { get; init; }
    }
}
