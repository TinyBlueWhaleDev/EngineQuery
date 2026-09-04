using TinyBlueWhale.EngineQuery.Core.Interfaces;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;
using TinyBlueWhale.EngineQuery.Sql.Clauses;
using TinyBlueWhale.EngineQuery.Sql.Clauses.Cte;
using TinyBlueWhale.EngineQuery.Sql.Clauses.LateralJoins;
using TinyBlueWhale.EngineQuery.Sql.Clauses.Pagination;
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
        /// Creates a query script builder using the specified database dialect and query feature composition.
        /// </summary>
        /// <param name="databaseDialect">
        /// Database dialect used to render provider-specific SQL syntax.
        /// </param>
        /// <param name="featureComposition">
        /// Query feature strategies associated with the active provider profile.
        /// </param>
        /// <returns>
        /// Configured query script builder.
        /// </returns>
        public static IQueryScriptBuilder CreateScriptBuilder(
            ISqlDatabaseDialect databaseDialect,
            QueryFeatureComposition featureComposition)
        {
            ArgumentNullException.ThrowIfNull(databaseDialect);
            ArgumentNullException.ThrowIfNull(featureComposition);

            var columnReferenceBuilder = new SqlColumnReferenceBuilder(databaseDialect);
            var parameterRewriter = new SqlParameterRewriter();

            IQueryScriptBuilder? scriptBuilder = null;

            var lazyScriptBuilder = new Lazy<IQueryScriptBuilder>(() => scriptBuilder!);

            var subqueryCompiler = new SubqueryCompiler(
                new DeferredQueryScriptBuilder(lazyScriptBuilder),
                parameterRewriter);

            var selectClauseBuilder = new SelectClauseBuilder(columnReferenceBuilder);

            var insertClauseBuilder = new InsertClauseBuilder(featureComposition.InsertIdentityRetrievalStrategy);

            var updateClauseBuilder = new UpdateClauseBuilder();
            var deleteClauseBuilder = new DeleteClauseBuilder();
            var fromClauseBuilder = new FromClauseBuilder(subqueryCompiler);
            var joinClauseBuilder = new JoinClauseBuilder();

            var whereClauseBuilder = new WhereClauseBuilder(
                columnReferenceBuilder,
                subqueryCompiler);

            var groupByClauseBuilder = new GroupByClauseBuilder();

            var havingClauseBuilder = new HavingClauseBuilder(
                columnReferenceBuilder);

            var orderByClauseBuilder = new OrderByClauseBuilder();
            var setOperationClauseBuilder = new SetOperationClauseBuilder(subqueryCompiler);

            var cteClauseBuilder = featureComposition.CteStrategy is not null
                ? new CteClauseBuilder(subqueryCompiler, featureComposition.CteStrategy)
                : null;

            var bodyClauseBuilders = new List<IOptionalSqlClauseBuilder>
            {
                joinClauseBuilder,
                whereClauseBuilder,
                groupByClauseBuilder,
                havingClauseBuilder,
                orderByClauseBuilder
            };

            if (featureComposition.PaginationStrategy is not null)
                bodyClauseBuilders.Add(new PaginationClauseBuilder(featureComposition.PaginationStrategy));

            if (featureComposition.LateralJoinStrategy is not null)
                bodyClauseBuilders.Add(new ApplyClauseBuilder(subqueryCompiler, featureComposition.LateralJoinStrategy));


            scriptBuilder = new QueryScriptBuilder(
                selectClauseBuilder,
                fromClauseBuilder,
                insertClauseBuilder,
                updateClauseBuilder,
                deleteClauseBuilder,
                whereClauseBuilder,
                bodyClauseBuilders,
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
