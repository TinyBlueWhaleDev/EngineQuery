using TinyBlueWhale.EngineQuery.Core.Interfaces;
using TinyBlueWhale.EngineQuery.Sql.Composition;
using TinyBlueWhale.EngineQuery.Sql.Interfaces;
using TinyBlueWhale.EngineQuery.SqlServer.Clauses;

namespace TinyBlueWhale.EngineQuery.SqlServer.Composition
{
    /// <summary>
    /// Creates SQL Server query compiler components.
    /// </summary>
    public static class SqlServerQueryCompilerFactory
    {
        /// <summary>
        /// Creates the SQL script builder used by the SQL Server query compiler.
        /// </summary>
        /// <param name="databaseDialect">
        /// Database dialect used during SQL generation.
        /// </param>
        /// <param name="featureComposition">
        /// SQL feature composition resolved from the selected provider profile.
        /// </param>
        /// <returns>
        /// Configured SQL script builder instance.
        /// </returns>
        public static IQueryScriptBuilder CreateScriptBuilder(ISqlDatabaseDialect databaseDialect,
           QueryFeatureComposition featureComposition)
        {
            return QueryCompilerFactory.CreateScriptBuilder(
                databaseDialect,
                new QueryScriptBuilderOptions
                {
                    CteClauseBuilderFactory = subqueryCompiler =>
                        new SqlServerCteClauseBuilder(subqueryCompiler),
                    InsertClauseBuilderFactory = () => new SqlServerInsertClauseBuilder(),
                    PaginationStrategy = featureComposition.PaginationStrategy
                });
        }
    }
}
