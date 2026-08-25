using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;
using TinyBlueWhale.EngineQuery.Sql.Compilation;
using TinyBlueWhale.EngineQuery.Sql.Helpers;
using TinyBlueWhale.EngineQuery.Sql.Interfaces;

namespace TinyBlueWhale.EngineQuery.Sql.Clauses
{
    /// <summary>
    /// Builds SQL UPDATE clauses from compiled UPDATE command definitions.
    /// </summary>
    /// <remarks>
    /// This builder generates parameterized UPDATE SET statements while delegating
    /// identifier escaping and parameter allocation to the active compilation context.
    /// </remarks>
    public sealed class UpdateClauseBuilder : IRequiredSqlClauseBuilder
    {
        /// <summary>
        /// Builds the SQL UPDATE statement.
        /// </summary>
        /// <param name="queryDefinition">
        /// Compiled query definition that contains UPDATE command metadata.
        /// </param>
        /// <param name="context">
        /// Current SQL compilation context.
        /// </param>
        /// <returns>
        /// SQL UPDATE statement.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="queryDefinition"/> or <paramref name="context"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the compiled query definition does not contain UPDATE assignments.
        /// </exception>
        public string Build(CompiledQueryDefinition queryDefinition, QueryCompilationContext context)
        {
            ArgumentNullException.ThrowIfNull(queryDefinition);
            ArgumentNullException.ThrowIfNull(context);

            var updateDefinition = queryDefinition.UpdateDefinition;

            if (updateDefinition is null || updateDefinition.AssignmentDefinitions.Count == 0)
                throw new InvalidOperationException("The UPDATE command requires at least one value assignment.");

            var tableName = SqlIdentifierHelper.BuildTableReference(context.DatabaseDialect, queryDefinition.TableName, queryDefinition.SchemaName);

            var assignments = updateDefinition.AssignmentDefinitions.Select(definition =>
            {
                var columnName = context.DatabaseDialect.EscapeIdentifier(definition.ColumnName);
                var parameterName = context.AddParameter(definition.Value);

                return $"{columnName} = {parameterName}";
            });

            return $"UPDATE {tableName}{Environment.NewLine}" + $"SET {string.Join(", ", assignments)}";
        }
    }
}
