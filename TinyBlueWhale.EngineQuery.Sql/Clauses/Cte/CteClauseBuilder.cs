using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;
using TinyBlueWhale.EngineQuery.Sql.Compilation;
using TinyBlueWhale.EngineQuery.Sql.Interfaces.Strategies;

namespace TinyBlueWhale.EngineQuery.Sql.Clauses.Cte
{
    /// <summary>
    /// Builds SQL common table expression clauses.
    /// </summary>
    /// <remarks>
    /// This builder emits WITH or WITH RECURSIVE clauses and compiles each CTE query into the parent context.
    /// </remarks>
    /// <remarks>
    /// Initializes a new instance of the <see cref="CteClauseBuilder"/> class.
    /// </remarks>
    /// <param name="subqueryCompiler">
    /// Subquery compiler used to compile CTE query definitions.
    /// </param>
    /// <param name="cteStrategy">
    /// Provider-specific common table expression strategy.
    /// </param>
    public class CteClauseBuilder(SubqueryCompiler subqueryCompiler,
        ICTEStrategy cteStrategy)
    {
        private readonly SubqueryCompiler _subqueryCompiler = subqueryCompiler ?? throw new ArgumentNullException(nameof(subqueryCompiler));

        private readonly ICTEStrategy _cteStrategy = cteStrategy ?? throw new ArgumentNullException(nameof(cteStrategy));


        /// <summary>
        /// Determines whether a CTE clause should be built.
        /// </summary>
        /// <param name="queryDefinition">
        /// Query definition to inspect.
        /// </param>
        /// <returns>
        /// <see langword="true"/> when CTE definitions are configured; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool CanBuild(CompiledQueryDefinition queryDefinition)
        {
            ArgumentNullException.ThrowIfNull(queryDefinition);

            return queryDefinition.CteDefinitions.Count > 0;
        }

        /// <summary>
        /// Builds the SQL common table expression clause.
        /// </summary>
        /// <param name="queryDefinition">
        /// Query definition that contains CTE metadata.
        /// </param>
        /// <param name="context">
        /// Current SQL compilation context.
        /// </param>
        /// <returns>
        /// SQL CTE clause.
        /// </returns>
        public string Build(CompiledQueryDefinition queryDefinition, QueryCompilationContext context)
        {
            ArgumentNullException.ThrowIfNull(queryDefinition);
            ArgumentNullException.ThrowIfNull(context);

            var withKeyword = queryDefinition.CteDefinitions.Any(cte => cte.IsRecursive)
                ? _cteStrategy.ResolveRecursiveCteKeyword()
                : "WITH";

            var cteClauses = queryDefinition.CteDefinitions
                 .Select(cteDefinition =>
                 {
                     var commandText = _subqueryCompiler.CompileAndReindex(cteDefinition.Query, context);

                     return $"{context.DatabaseDialect.EscapeIdentifier(cteDefinition.Name)} AS ({commandText})";
                 });

            return withKeyword + " " + string.Join(", ", cteClauses);
        }
    }
}
