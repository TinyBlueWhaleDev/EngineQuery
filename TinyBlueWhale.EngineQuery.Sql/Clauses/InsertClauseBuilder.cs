using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;
using TinyBlueWhale.EngineQuery.Sql.Compilation;
using TinyBlueWhale.EngineQuery.Sql.Interfaces;

namespace TinyBlueWhale.EngineQuery.Sql.Clauses
{
    /// <summary>
    /// Builds SQL INSERT clauses from compiled INSERT command definitions.
    /// </summary>
    /// <remarks>
    /// This builder generates parameterized INSERT VALUES statements while delegating
    /// identifier escaping and parameter allocation to the active compilation context.
    /// </remarks>
    public class InsertClauseBuilder : IRequiredSqlClauseBuilder
    {
        /// <summary>
        /// Builds the SQL INSERT statement.
        /// </summary>
        /// <param name="queryDefinition">
        /// Compiled query definition that contains INSERT command metadata.
        /// </param>
        /// <param name="context">
        /// Current SQL compilation context.
        /// </param>
        /// <returns>
        /// SQL INSERT statement.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="queryDefinition"/> or <paramref name="context"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the compiled query definition does not contain INSERT values.
        /// </exception>
        public string Build(CompiledQueryDefinition queryDefinition, QueryCompilationContext context)
        {
            ArgumentNullException.ThrowIfNull(queryDefinition);
            ArgumentNullException.ThrowIfNull(context);

            var insertDefinition = queryDefinition.InsertDefinition;

            if (insertDefinition is null || insertDefinition.ValueDefinitions.Count == 0)
                throw new InvalidOperationException("The INSERT command requires at least one value assignment.");

            var insertTarget = BuildTarget(queryDefinition, context);
            var parameters = insertDefinition.ValueDefinitions.Select(definition => context.AddParameter(definition.Value));

            var commandText =
                $"{insertTarget}{Environment.NewLine}" +
                $"VALUES ({string.Join(", ", parameters)})";

            if (insertDefinition.IdentityDefinition is null)
                return commandText;

            return AppendIdentityRetrieval(insertDefinition.IdentityDefinition, commandText, context);
        }

        /// <summary>
        /// Builds the SQL INSERT target clause.
        /// </summary>
        /// <param name="queryDefinition">
        /// Compiled query definition that contains INSERT command metadata.
        /// </param>
        /// <param name="context">
        /// Current SQL compilation context.
        /// </param>
        /// <returns>
        /// SQL INSERT target clause.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="queryDefinition"/> or <paramref name="context"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the compiled query definition does not contain INSERT target columns.
        /// </exception>
        public string BuildTarget(CompiledQueryDefinition queryDefinition, QueryCompilationContext context)
        {
            ArgumentNullException.ThrowIfNull(queryDefinition);
            ArgumentNullException.ThrowIfNull(context);

            var insertDefinition = queryDefinition.InsertDefinition
                ?? throw new InvalidOperationException("The INSERT command definition is not initialized.");

            var columns = insertDefinition.ColumnDefinitions.Count > 0
                ? insertDefinition.ColumnDefinitions.Select(definition => definition.ColumnName)
                : insertDefinition.ValueDefinitions.Select(definition => definition.ColumnName);

            if (!columns.Any())
                throw new InvalidOperationException("The INSERT command requires at least one target column.");

            var tableName = context.DatabaseDialect.EscapeIdentifier(queryDefinition.TableName);
            var escapedColumns = columns.Select(context.DatabaseDialect.EscapeIdentifier);

            return $"INSERT INTO {tableName} ({string.Join(", ", escapedColumns)})";
        }

        /// <summary>
        /// Appends provider-specific identity retrieval SQL to the generated INSERT command.
        /// </summary>
        /// <param name="identityDefinition">
        /// Identity retrieval metadata configured by the INSERT VALUES builder.
        /// </param>
        /// <param name="commandText">
        /// Generated INSERT VALUES command text.
        /// </param>
        /// <param name="context">
        /// Current SQL compilation context.
        /// </param>
        /// <returns>
        /// INSERT command text with provider-specific identity retrieval SQL.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// Thrown when the current provider does not implement identity retrieval.
        /// </exception>
        protected virtual string AppendIdentityRetrieval(QueryInsertIdentityDefinition identityDefinition, string commandText, QueryCompilationContext context)
        {
            throw new NotSupportedException(
                "INSERT identity retrieval is not supported by the current database provider.");
        }
    }
}
