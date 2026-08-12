using TinyBlueWhale.EngineQuery.Core.Interfaces;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;
using TinyBlueWhale.EngineQuery.Sql.Clauses;
using TinyBlueWhale.EngineQuery.Sql.Compilation;
using TinyBlueWhale.EngineQuery.Sql.Helpers;
using TinyBlueWhale.EngineQuery.Sql.Interfaces;

namespace TinyBlueWhale.EngineQuery.Sql.Composition
{
    /// <summary>
    /// Creates query compiler collaborators for manual composition scenarios.
    /// </summary>
    /// <remarks>
    /// This factory centralizes query script builder wiring and allows providers to override only
    /// the SQL clause builders that require provider-specific behavior.
    /// </remarks>
    public static class QueryCompilerFactory
    {
        /// <summary>
        /// Creates a SQL script builder using the specified provider options.
        /// </summary>
        /// <param name="databaseDialect">
        /// SQL database dialect used by helper services and compilation context.
        /// </param>
        /// <param name="options">
        /// Provider-specific query script builder options.
        /// </param>
        /// <returns>
        /// Configured SQL script builder.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="databaseDialect"/> or <paramref name="options"/> is <see langword="null"/>.
        /// </exception>
        public static IQueryScriptBuilder CreateScriptBuilder(
            ISqlDatabaseDialect databaseDialect,
            QueryScriptBuilderOptions options)
        {
            ArgumentNullException.ThrowIfNull(databaseDialect);
            ArgumentNullException.ThrowIfNull(options);

            var columnReferenceBuilder = new SqlColumnReferenceBuilder(databaseDialect);
            var parameterRewriter = new SqlParameterRewriter();

            IQueryScriptBuilder? scriptBuilder = null;

            var lazyScriptBuilder = new Lazy<IQueryScriptBuilder>(() => scriptBuilder!);

            var subqueryCompiler = new SubqueryCompiler(
                new DeferredQueryScriptBuilder(lazyScriptBuilder),
                parameterRewriter);

            var selectClauseBuilder = new SelectClauseBuilder(columnReferenceBuilder);
            var insertClauseBuilder = new InsertClauseBuilder();
            var updateClauseBuilder = new UpdateClauseBuilder();
            var fromClauseBuilder = new FromClauseBuilder(subqueryCompiler);
            var joinClauseBuilder = new JoinClauseBuilder();

            var applyClauseBuilder = options.ApplyClauseBuilderFactory?.Invoke(subqueryCompiler)
                ?? new ApplyClauseBuilder(subqueryCompiler);

            var whereClauseBuilder = new WhereClauseBuilder(
                columnReferenceBuilder,
                subqueryCompiler);

            var groupByClauseBuilder = new GroupByClauseBuilder();

            var havingClauseBuilder = new HavingClauseBuilder(
                columnReferenceBuilder);

            var orderByClauseBuilder = new OrderByClauseBuilder();
            var paginationClauseBuilder = new PaginationClauseBuilder();
            var setOperationClauseBuilder = new SetOperationClauseBuilder(subqueryCompiler);

            var cteClauseBuilder = options.CteClauseBuilderFactory?.Invoke(subqueryCompiler)
                ?? new CteClauseBuilder(subqueryCompiler);

            scriptBuilder = new QueryScriptBuilder(
                selectClauseBuilder,
                fromClauseBuilder,
                insertClauseBuilder,
                updateClauseBuilder,
                whereClauseBuilder,
                [
                    joinClauseBuilder,
                    applyClauseBuilder,
                    whereClauseBuilder,
                    groupByClauseBuilder,
                    havingClauseBuilder,
                    orderByClauseBuilder,
                    paginationClauseBuilder
                ],
                setOperationClauseBuilder,
                cteClauseBuilder);

            return scriptBuilder;
        }

        private sealed class DeferredQueryScriptBuilder(Lazy<IQueryScriptBuilder> queryScriptBuilder) : IQueryScriptBuilder
        {
            private readonly Lazy<IQueryScriptBuilder> _queryScriptBuilder = queryScriptBuilder ?? throw new ArgumentNullException(nameof(queryScriptBuilder));

            public string Build(CompiledQueryDefinition queryDefinition, QueryCompilationContext context)
            {
                return _queryScriptBuilder.Value.Build(queryDefinition, context);
            }
        }
    }

}
