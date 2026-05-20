using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;
using TinyBlueWhale.EngineQuery.Sql.Compilation;
using TinyBlueWhale.EngineQuery.Sql.Interfaces;

namespace TinyBlueWhale.EngineQuery.Sql.Clauses
{
    /// <summary>
    /// Builds SQL APPLY or provider-equivalent lateral join clauses.
    /// </summary>
    /// <remarks>
    /// This builder compiles APPLY subqueries and emits the provider-specific APPLY keyword.
    /// </remarks>
    /// <remarks>
    /// Initializes a new instance of the <see cref="ApplyClauseBuilder"/> class.
    /// </remarks>
    /// <param name="subqueryCompiler">
    /// Subquery compiler used to compile APPLY subqueries.
    /// </param>
    public class ApplyClauseBuilder(SubqueryCompiler subqueryCompiler) : IOptionalSqlClauseBuilder
    {
        private readonly SubqueryCompiler _subqueryCompiler = subqueryCompiler ?? throw new ArgumentNullException(nameof(subqueryCompiler));

        /// <summary>
        /// Determines whether APPLY clauses should be built.
        /// </summary>
        /// <param name="queryDefinition">
        /// Query definition to inspect.
        /// </param>
        /// <returns>
        /// <see langword="true"/> when APPLY definitions are configured; otherwise, <see langword="false"/>.
        /// </returns>
        public bool CanBuild(CompiledQueryDefinition queryDefinition)
        {
            ArgumentNullException.ThrowIfNull(queryDefinition);

            return queryDefinition.ApplyDefinitions.Count > 0;
        }

        /// <summary>
        /// Builds all SQL APPLY clauses configured in the query definition.
        /// </summary>
        /// <param name="queryDefinition">
        /// Query definition that contains APPLY metadata.
        /// </param>
        /// <param name="context">
        /// Current SQL compilation context.
        /// </param>
        /// <returns>
        /// SQL APPLY clause text.
        /// </returns>
        public string Build(CompiledQueryDefinition queryDefinition, QueryCompilationContext context)
        {
            ArgumentNullException.ThrowIfNull(queryDefinition);
            ArgumentNullException.ThrowIfNull(context);

            var applyClauses = queryDefinition.ApplyDefinitions
                .Select(applyDefinition =>
                {
                    var commandText = _subqueryCompiler.CompileAndReindex(
                        applyDefinition.Subquery,
                        context);

                    return BuildApplyClause(applyDefinition, commandText, context);
                });

            return string.Join(Environment.NewLine, applyClauses);
        }

        /// <summary>
        /// Resolves the SQL keyword used for APPLY joins.
        /// </summary>
        /// <param name="applyType">
        /// APPLY join type to resolve.
        /// </param>
        /// <returns>
        /// SQL APPLY keyword.
        /// </returns>
        protected virtual string ResolveApplyKeyword(QueryApplyType applyType)
        {
            return applyType switch
            {
                QueryApplyType.Cross => "CROSS APPLY",
                QueryApplyType.Outer => "OUTER APPLY",
                _ => throw new NotSupportedException($"APPLY type '{applyType}' is not supported.")
            };
        }

        /// <summary>
        /// Builds an APPLY clause for the current provider dialect.
        /// </summary>
        /// <param name="applyDefinition">
        /// APPLY definition metadata.
        /// </param>
        /// <param name="commandText">
        /// SQL command text used by the APPLY source.
        /// </param>
        /// <param name="context">
        /// Current query compilation context.
        /// </param>
        /// <returns>
        /// Generated APPLY clause SQL.
        /// </returns>
        protected virtual string BuildApplyClause(QueryApplyDefinition applyDefinition, string commandText, QueryCompilationContext context)
        {
            var applyKeyword = ResolveApplyKeyword(applyDefinition.ApplyType);

            return $"{applyKeyword} ({commandText}) AS {context.DatabaseDialect.EscapeIdentifier(applyDefinition.Alias)}";
        }
    }
}
