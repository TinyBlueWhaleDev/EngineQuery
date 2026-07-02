using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;
using TinyBlueWhale.EngineQuery.Sql.Clauses;
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
        IReadOnlyList<IOptionalSqlClauseBuilder> bodyClauseBuilders,
        SetOperationClauseBuilder setOperationClauseBuilder,
        CteClauseBuilder cteClauseBuilder) : IQueryScriptBuilder
    {
        private readonly IRequiredSqlClauseBuilder _selectClauseBuilder = selectClauseBuilder ?? throw new ArgumentNullException(nameof(selectClauseBuilder));
        private readonly IRequiredSqlClauseBuilder _fromClauseBuilder = fromClauseBuilder ?? throw new ArgumentNullException(nameof(fromClauseBuilder));
        private readonly IReadOnlyList<IOptionalSqlClauseBuilder> _bodyClauseBuilders = bodyClauseBuilders ?? throw new ArgumentNullException(nameof(bodyClauseBuilders));
        private readonly SetOperationClauseBuilder _setOperationClauseBuilder = setOperationClauseBuilder ?? throw new ArgumentNullException(nameof(setOperationClauseBuilder));
        private readonly CteClauseBuilder _cteClauseBuilder = cteClauseBuilder ?? throw new ArgumentNullException(nameof(cteClauseBuilder));

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
                commandText = _cteClauseBuilder.Build(queryDefinition, context) +
                              Environment.NewLine +
                              commandText;
            }

            return commandText;
        }
    }
}
