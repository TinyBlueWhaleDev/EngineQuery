using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;
using TinyBlueWhale.EngineQuery.Sql.Clauses;
using TinyBlueWhale.EngineQuery.Sql.Compilation;

namespace TinyBlueWhale.EngineQuery.PostgreSql.Clauses
{
    /// <summary>
    /// Builds PostgreSQL LATERAL join clauses for APPLY definitions.
    /// </summary>
    /// <remarks>
    /// PostgreSQL represents CROSS APPLY and OUTER APPLY semantics using JOIN LATERAL
    /// and LEFT JOIN LATERAL with an ON TRUE predicate.
    /// </remarks>
    /// <remarks>
    /// Initializes a new instance of the <see cref="PostgreSqlApplyClauseBuilder"/> class.
    /// </remarks>
    /// <param name="subqueryCompiler">
    /// Subquery compiler used to compile APPLY subqueries.
    /// </param>
    public sealed class PostgreSqlApplyClauseBuilder(SubqueryCompiler subqueryCompiler) : ApplyClauseBuilder(subqueryCompiler)
    {

        /// <summary>
        /// Builds a PostgreSQL LATERAL join clause for APPLY definitions.
        /// </summary>
        /// <param name="applyDefinition">
        /// APPLY definition to build.
        /// </param>
        /// <param name="commandText">
        /// Compiled lateral subquery command text.
        /// </param>
        /// <param name="context">
        /// Current SQL compilation context.
        /// </param>
        /// <returns>
        /// PostgreSQL LATERAL join clause.
        /// </returns>
        protected override string BuildApplyClause(QueryApplyDefinition applyDefinition, string commandText,
            QueryCompilationContext context)
        {
            var applyKeyword = applyDefinition.ApplyType == QueryApplyType.Cross
                ? "JOIN LATERAL"
                : "LEFT JOIN LATERAL";

            return $"{applyKeyword} ({commandText}) AS {context.DatabaseDialect.EscapeIdentifier(applyDefinition.Alias)} ON TRUE";
        }
    }
}
