using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;
using TinyBlueWhale.EngineQuery.Sql.Compilation;

namespace TinyBlueWhale.EngineQuery.Sql.Clauses
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
    public class CteClauseBuilder(SubqueryCompiler subqueryCompiler)
    {
        private readonly SubqueryCompiler _subqueryCompiler = subqueryCompiler ?? throw new ArgumentNullException(nameof(subqueryCompiler));

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
                ? ResolveRecursiveCteKeyword()
                : "WITH";

            var cteClauses = queryDefinition.CteDefinitions
                .Select(cteDefinition =>
                {
                    var commandText = _subqueryCompiler.CompileAndReindex(
                        cteDefinition.Query,
                        context);

                    return $"{context.DatabaseDialect.EscapeIdentifier(cteDefinition.Name)} AS ({commandText})";
                });

            return withKeyword + " " + string.Join(", ", cteClauses);
        }

        /// <summary>
        /// Resolves the SQL keyword used for recursive common table expressions.
        /// </summary>
        /// <returns>
        /// SQL recursive CTE keyword.
        /// </returns>
        protected virtual string ResolveRecursiveCteKeyword()
        {
            return "WITH RECURSIVE";
        }
    }
}
