using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;
using TinyBlueWhale.EngineQuery.Sql.Clauses;
using TinyBlueWhale.EngineQuery.Sql.Clauses.Cte;
using TinyBlueWhale.EngineQuery.Sql.Interfaces;

namespace TinyBlueWhale.EngineQuery.Sql.Compilation
{
    /// <summary>
    /// Builds complete SQL command text by orchestrating individual SQL clause builders.
    /// </summary>
    /// <remarks>
    /// This builder coordinates required clauses, optional clauses, set operations and common table
    /// expressions while keeping each clause implementation isolated in its own component.
    /// </remarks>
    /// <remarks>
    /// Initializes a new instance of the <see cref="QueryScriptBuilder"/> class.
    /// </remarks>
    /// <param name="selectClauseBuilder">
    /// SQL SELECT clause builder.
    /// </param>
    /// <param name="fromClauseBuilder">
    /// SQL FROM clause builder.
    /// </param>
    /// <param name="insertClauseBuilder">
    /// SQL INSERT clause builder.
    /// </param>
    /// /// <param name="updateClauseBuilder">
    /// SQL UPDATE clause builder.
    /// </param>
    /// /// <param name="deleteClauseBuilder">
    /// SQL DELETE clause builder.
    /// </param>
    /// <param name="whereClauseBuilder">
    /// SQL WHERE clause builder used by command-specific compilation pipelines.
    /// </param>
    /// <param name="bodyClauseBuilders">
    /// Ordered SQL clause builders used after FROM and before set operations or CTE wrapping.
    /// </param>
    /// <param name="setOperationClauseBuilder">
    /// SQL set operation clause builder.
    /// </param>
    /// <param name="cteClauseBuilder">
    /// SQL common table expression clause builder.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when any required dependency is <see langword="null"/>.
    /// </exception>
    public sealed class QueryScriptBuilder(
        IRequiredSqlClauseBuilder selectClauseBuilder,
        IRequiredSqlClauseBuilder fromClauseBuilder,
        InsertClauseBuilder insertClauseBuilder,
        IRequiredSqlClauseBuilder updateClauseBuilder,
        IRequiredSqlClauseBuilder deleteClauseBuilder,
        IOptionalSqlClauseBuilder whereClauseBuilder,
        IReadOnlyList<IOptionalSqlClauseBuilder> bodyClauseBuilders,
        SetOperationClauseBuilder setOperationClauseBuilder,
        CteClauseBuilder? cteClauseBuilder) : IQueryScriptBuilder
    {
        private readonly IRequiredSqlClauseBuilder _selectClauseBuilder = selectClauseBuilder ?? throw new ArgumentNullException(nameof(selectClauseBuilder));
        private readonly IRequiredSqlClauseBuilder _fromClauseBuilder = fromClauseBuilder ?? throw new ArgumentNullException(nameof(fromClauseBuilder));
        private readonly InsertClauseBuilder _insertClauseBuilder = insertClauseBuilder ?? throw new ArgumentNullException(nameof(insertClauseBuilder));
        private readonly IRequiredSqlClauseBuilder _updateClauseBuilder = updateClauseBuilder ?? throw new ArgumentNullException(nameof(updateClauseBuilder));
        private readonly IRequiredSqlClauseBuilder _deleteClauseBuilder = deleteClauseBuilder ?? throw new ArgumentNullException(nameof(deleteClauseBuilder));
        private readonly IOptionalSqlClauseBuilder _whereClauseBuilder = whereClauseBuilder ?? throw new ArgumentNullException(nameof(whereClauseBuilder));
        private readonly IReadOnlyList<IOptionalSqlClauseBuilder> _bodyClauseBuilders = bodyClauseBuilders ?? throw new ArgumentNullException(nameof(bodyClauseBuilders));
        private readonly SetOperationClauseBuilder _setOperationClauseBuilder = setOperationClauseBuilder ?? throw new ArgumentNullException(nameof(setOperationClauseBuilder));
        private readonly CteClauseBuilder? _cteClauseBuilder = cteClauseBuilder;

        /// <summary>
        /// Builds SQL command text for the specified query definition.
        /// </summary>
        /// <param name="queryDefinition">
        /// Query definition to compile into SQL command text.
        /// </param>
        /// <param name="context">
        /// Current SQL compilation context.
        /// </param>
        /// <returns>
        /// SQL command text.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="queryDefinition"/> or <paramref name="context"/> is <see langword="null"/>.
        /// </exception>
        public string Build(CompiledQueryDefinition queryDefinition, QueryCompilationContext context)
        {
            ArgumentNullException.ThrowIfNull(queryDefinition);
            ArgumentNullException.ThrowIfNull(context);

            return queryDefinition.CommandType switch
            {
                QueryCommandType.Select => BuildSelectQuery(queryDefinition, context),
                QueryCommandType.Insert => BuildInsertCommand(queryDefinition, context),
                QueryCommandType.Update => BuildUpdateCommand(queryDefinition, context),
                QueryCommandType.Delete => BuildDeleteCommand(queryDefinition, context),
                _ => throw new NotSupportedException($"SQL command type '{queryDefinition.CommandType}' is not supported.")
            };
        }

        // Builds the INSERT command pipeline using either VALUES or SELECT as the command source.
        private string BuildInsertCommand(CompiledQueryDefinition queryDefinition, QueryCompilationContext context)
        {
            var insertDefinition = queryDefinition.InsertDefinition
                ?? throw new InvalidOperationException("The INSERT command definition is not initialized.");

            if (insertDefinition.ValueDefinitions.Count > 0)
                return _insertClauseBuilder.Build(queryDefinition, context);

            if (insertDefinition.SourceDefinition is not null)
                return BuildInsertSelectCommand(queryDefinition, context);

            throw new InvalidOperationException("At least one value or SELECT source must be configured before building an INSERT command.");
        }

        // Builds the existing SELECT query pipeline without altering its current clause order or behavior.
        private string BuildSelectQuery(CompiledQueryDefinition queryDefinition, QueryCompilationContext context)
        {
            var sqlLines = new List<string>
            {
                _selectClauseBuilder.Build(queryDefinition, context),
                _fromClauseBuilder.Build(queryDefinition, context)
            };

            foreach (var clauseBuilder in _bodyClauseBuilders)
            {
                if (clauseBuilder.CanBuild(queryDefinition))
                    sqlLines.Add(clauseBuilder.Build(queryDefinition, context));
            }

            var commandText = string.Join(Environment.NewLine, sqlLines);

            if (SetOperationClauseBuilder.CanBuild(queryDefinition))
            {
                commandText = _setOperationClauseBuilder.Build(
                    queryDefinition,
                    context,
                    commandText);
            }

            if (CteClauseBuilder.CanBuild(queryDefinition))
            {
                if (_cteClauseBuilder is null)
                    throw new InvalidOperationException("The current provider profile does not provide a common table expression strategy.");

                commandText = _cteClauseBuilder.Build(queryDefinition, context) +
                              Environment.NewLine +
                              commandText;
            }

            return commandText;
        }

        // Builds an INSERT SELECT command using the existing SELECT query pipeline.
        private string BuildInsertSelectCommand(CompiledQueryDefinition queryDefinition, QueryCompilationContext context)
        {
            var insertDefinition = queryDefinition.InsertDefinition
                ?? throw new InvalidOperationException("The INSERT command definition is not initialized.");

            var sourceDefinition = insertDefinition.SourceDefinition
                ?? throw new InvalidOperationException("The INSERT SELECT source is not configured.");

            var insertClause = _insertClauseBuilder.BuildTarget(queryDefinition, context);
            var selectQuery = BuildSelectQuery(queryDefinition, context);

            return $"{insertClause}{Environment.NewLine}{selectQuery}";
        }

        // Builds the UPDATE command pipeline using the shared WHERE clause implementation.
        private string BuildUpdateCommand(CompiledQueryDefinition queryDefinition, QueryCompilationContext context)
        {
            var sqlLines = new List<string>
            {
                _updateClauseBuilder.Build(queryDefinition, context)
            };

            if (_whereClauseBuilder.CanBuild(queryDefinition))
                sqlLines.Add(_whereClauseBuilder.Build(queryDefinition, context));

            return string.Join(Environment.NewLine, sqlLines);
        }

        // Builds the DELETE command pipeline using the shared WHERE clause implementation.
        private string BuildDeleteCommand(CompiledQueryDefinition queryDefinition, QueryCompilationContext context)
        {
            var sqlLines = new List<string>
            {
                _deleteClauseBuilder.Build(queryDefinition, context)
            };

            if (_whereClauseBuilder.CanBuild(queryDefinition))
                sqlLines.Add(_whereClauseBuilder.Build(queryDefinition, context));

            return string.Join(Environment.NewLine, sqlLines);
        }
    }
}
