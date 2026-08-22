using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;
using TinyBlueWhale.EngineQuery.Sql.Compilation;
using TinyBlueWhale.EngineQuery.Sql.Interfaces;

namespace TinyBlueWhale.EngineQuery.Sql.Clauses
{
    /// <summary>
    /// Builds SQL DELETE clauses from compiled DELETE command definitions.
    /// </summary>
    /// <remarks>
    /// This builder generates DELETE statements while delegating identifier escaping
    /// to the active compilation context.
    /// </remarks>
    public sealed class DeleteClauseBuilder : IRequiredSqlClauseBuilder
    {
        /// <summary>
        /// Builds the SQL DELETE statement.
        /// </summary>
        /// <param name="queryDefinition">
        /// Compiled query definition that contains DELETE command metadata.
        /// </param>
        /// <param name="context">
        /// Current SQL compilation context.
        /// </param>
        /// <returns>
        /// SQL DELETE statement.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="queryDefinition"/> or <paramref name="context"/> is <see langword="null"/>.
        /// </exception>
        public string Build(CompiledQueryDefinition queryDefinition, QueryCompilationContext context)
        {
            ArgumentNullException.ThrowIfNull(queryDefinition);
            ArgumentNullException.ThrowIfNull(context);

            var tableName = context.DatabaseDialect.EscapeIdentifier(queryDefinition.TableName);

            return $"DELETE FROM {tableName}";
        }
    }
}
