using TinyBlueWhale.EngineQuery.Core.Interfaces;
using TinyBlueWhale.EngineQuery.Core.Parameters;

namespace TinyBlueWhale.EngineQuery.Sql.Compilation
{
    /// <summary>
    /// Represents the shared state used while compiling a query definition into SQL.
    /// </summary>
    /// <remarks>
    /// This context carries the active database dialect and the SQL parameter collection
    /// used by clause builders during query compilation.
    /// </remarks>
    /// <remarks>
    /// Initializes a new instance of the <see cref="QueryCompilationContext"/> class.
    /// </remarks>
    /// <param name="databaseDialect">
    /// SQL database dialect used to escape identifiers and build provider-specific SQL fragments.
    /// </param>
    /// <param name="parameters">
    /// SQL parameter collection used during query compilation.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="databaseDialect"/> or <paramref name="parameters"/> is <see langword="null"/>.
    /// </exception>
    public sealed class QueryCompilationContext(ISqlDatabaseDialect databaseDialect, QueryParameterCollection parameters)
    {

        /// <summary>
        /// Gets the SQL database dialect used during compilation.
        /// </summary>
        public ISqlDatabaseDialect DatabaseDialect { get; } = databaseDialect ?? throw new ArgumentNullException(nameof(databaseDialect));

        /// <summary>
        /// Gets the SQL parameter collection used during compilation.
        /// </summary>
        public QueryParameterCollection Parameters { get; } = parameters ?? throw new ArgumentNullException(nameof(parameters));

        /// <summary>
        /// Adds a SQL parameter to the current compilation context.
        /// </summary>
        /// <param name="value">
        /// Parameter value to add.
        /// </param>
        /// <returns>
        /// Generated SQL parameter name.
        /// </returns>
        public string AddParameter(object? value)
        {
            return Parameters.Add(value);
        }
    }
}
