using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;
using TinyBlueWhale.EngineQuery.Sql.Compilation;
using TinyBlueWhale.EngineQuery.Sql.Helpers;
using TinyBlueWhale.EngineQuery.Sql.Interfaces;
using TinyBlueWhale.EngineQuery.Sql.Interfaces.Strategies;

namespace TinyBlueWhale.EngineQuery.Sql.Clauses
{
    /// <summary>
    /// Builds SQL INSERT clauses from compiled INSERT command definitions.
    /// </summary>
    /// <remarks>
    /// This builder generates parameterized INSERT VALUES statements while delegating
    /// identifier escaping, parameter allocation, and identity retrieval rendering
    /// to the active compilation context and provider strategy.
    /// </remarks>
    public sealed class InsertClauseBuilder(IInsertIdentityRetrievalStrategy? identityRetrievalStrategy) : IRequiredSqlClauseBuilder
    {
        private readonly IInsertIdentityRetrievalStrategy? _identityRetrievalStrategy = identityRetrievalStrategy;

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
        /// Thrown when the compiled query definition does not contain INSERT values,
        /// a valid INSERT target source, or identity retrieval is requested without
        /// an available provider strategy.
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

            if (_identityRetrievalStrategy is null)
                throw new InvalidOperationException("INSERT identity retrieval is not supported by the current database provider profile.");

            return _identityRetrievalStrategy.AppendIdentityRetrieval(insertDefinition.IdentityDefinition, commandText, context);
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
        /// Thrown when the compiled query definition does not contain INSERT target columns
        /// or the root query source does not define a table name.
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

            var targetSource = queryDefinition.RootSource;

            if (string.IsNullOrWhiteSpace(targetSource.TableName))
                throw new InvalidOperationException("The INSERT target source does not define a table name.");

            var tableName = SqlIdentifierHelper.BuildTableReference(
                context.DatabaseDialect,
                targetSource.TableName,
                targetSource.SchemaName);

            var escapedColumns = columns.Select(context.DatabaseDialect.EscapeIdentifier);

            return $"INSERT INTO {tableName} ({string.Join(", ", escapedColumns)})";
        }
    }
}
