using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions.Sources;
using TinyBlueWhale.EngineQuery.Sql.Compilation;
using TinyBlueWhale.EngineQuery.Sql.Interfaces;
using TinyBlueWhale.EngineQuery.Sql.Interfaces.Strategies;

namespace TinyBlueWhale.EngineQuery.Sql.Clauses.LateralJoins
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
    /// <param name="lateralJoinStrategy">
    /// Lateral join strategy used to build APPLY clauses.
    /// </param>
    public class ApplyClauseBuilder(SubqueryCompiler subqueryCompiler,
        ILateralJoinStrategy lateralJoinStrategy) : IOptionalSqlClauseBuilder
    {
        private readonly SubqueryCompiler _subqueryCompiler = subqueryCompiler ?? throw new ArgumentNullException(nameof(subqueryCompiler));
        private readonly ILateralJoinStrategy _lateralJoinStrategy = lateralJoinStrategy ?? throw new ArgumentNullException(nameof(lateralJoinStrategy));

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
        /// Builds an APPLY or provider-equivalent lateral join clause.
        /// </summary>
        /// <param name="applyDefinition">
        /// APPLY definition metadata.
        /// </param>
        /// <param name="commandText">
        /// Compiled lateral subquery command text.
        /// </param>
        /// <param name="context">
        /// Current query compilation context.
        /// </param>
        /// <returns>
        /// Generated lateral join clause SQL.
        /// </returns>
        private string BuildApplyClause(QueryApplyDefinition applyDefinition, string commandText, QueryCompilationContext context)
        {
            var joinKeyword = _lateralJoinStrategy.GetJoinKeyword(applyDefinition.ApplyType);
            var joinSuffix = _lateralJoinStrategy.GetJoinSuffix();

            return $"{joinKeyword} ({commandText}) AS {context.DatabaseDialect.EscapeIdentifier(applyDefinition.Alias)}{joinSuffix}";
        }
    }
}
