using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Core.Parameters;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;
using TinyBlueWhale.EngineQuery.Sql.Helpers;
using TinyBlueWhale.EngineQuery.Sql.Interfaces;

namespace TinyBlueWhale.EngineQuery.Sql.Compilation
{
    /// <summary>
    /// Compiles nested query definitions and merges their parameters into a parent compilation context.
    /// </summary>
    /// <remarks>
    /// This service is used by SQL clauses that embed subqueries, such as CTEs, derived tables,
    /// EXISTS conditions, IN subqueries, APPLY clauses and set operations.
    /// </remarks>
    /// <remarks>
    /// Initializes a new instance of the <see cref="SubqueryCompiler"/> class.
    /// </remarks>
    /// <param name="queryScriptBuilder">
    /// SQL script builder used to compile nested query definitions.
    /// </param>
    /// <param name="parameterRewriter">
    /// SQL parameter rewriter used to merge nested parameters into the parent context.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="queryScriptBuilder"/> or <paramref name="parameterRewriter"/> is <see langword="null"/>.
    /// </exception>
    public sealed class SubqueryCompiler(IQueryScriptBuilder queryScriptBuilder, SqlParameterRewriter parameterRewriter)
    {
        private readonly IQueryScriptBuilder _queryScriptBuilder = queryScriptBuilder ?? throw new ArgumentNullException(nameof(queryScriptBuilder));
        private readonly SqlParameterRewriter _parameterRewriter = parameterRewriter ?? throw new ArgumentNullException(nameof(parameterRewriter));

        /// <summary>
        /// Compiles a nested query and rewrites its SQL parameters into the parent context.
        /// </summary>
        /// <param name="subquery">
        /// Nested query definition to compile.
        /// </param>
        /// <param name="parentContext">
        /// Parent SQL compilation context that receives rewritten parameters.
        /// </param>
        /// <returns>
        /// Nested SQL command text with parameter names rewritten for the parent context.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="subquery"/> or <paramref name="parentContext"/> is <see langword="null"/>.
        /// </exception>
        public string CompileAndReindex(CompiledQueryDefinition subquery, QueryCompilationContext parentContext)
        {
            ArgumentNullException.ThrowIfNull(subquery);
            ArgumentNullException.ThrowIfNull(parentContext);

            var subqueryContext = new QueryCompilationContext(parentContext.DatabaseDialect, new QueryParameterCollection());

            var commandText = _queryScriptBuilder.Build(subquery, subqueryContext);

            return SqlParameterRewriter.Rewrite(commandText, subqueryContext.Parameters.Parameters, parentContext.Parameters);
        }
    }
}
