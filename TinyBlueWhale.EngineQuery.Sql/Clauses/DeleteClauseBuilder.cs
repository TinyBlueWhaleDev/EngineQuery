using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;
using TinyBlueWhale.EngineQuery.Sql.Compilation;
using TinyBlueWhale.EngineQuery.Sql.Helpers;
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
        /// <exception cref="InvalidOperationException">
        /// Thrown when the DELETE root source does not define a physical table name.
        /// </exception>
        public string Build(CompiledQueryDefinition queryDefinition, QueryCompilationContext context)
        {
            ArgumentNullException.ThrowIfNull(queryDefinition);
            ArgumentNullException.ThrowIfNull(context);

            var targetSource = queryDefinition.RootSource;

            if (string.IsNullOrWhiteSpace(targetSource.TableName))
                throw new InvalidOperationException("The DELETE target source does not define a table name.");

            var tableName = SqlIdentifierHelper.BuildTableReference(
                context.DatabaseDialect,
                targetSource.TableName,
                targetSource.SchemaName);

            return $"DELETE FROM {tableName}";
        }
    }
}
