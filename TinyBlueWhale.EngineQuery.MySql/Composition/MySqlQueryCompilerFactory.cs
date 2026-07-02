using TinyBlueWhale.EngineQuery.Core.Interfaces;
using TinyBlueWhale.EngineQuery.MySql.Clauses;
using TinyBlueWhale.EngineQuery.Sql.Composition;
using TinyBlueWhale.EngineQuery.Sql.Interfaces;

namespace TinyBlueWhale.EngineQuery.MySql.Composition
{
    /// <summary>
    /// Creates MySQL query compiler collaborators.
    /// </summary>
    /// <remarks>
    /// This factory configures only the MySQL-specific clause builders while reusing
    /// the shared SQL query compilation pipeline from the base project.
    /// </remarks>
    public static class MySqlQueryCompilerFactory
    {
        /// <summary>
        /// Creates the MySQL query script builder.
        /// </summary>
        /// <param name="databaseDialect">
        /// MySQL database dialect.
        /// </param>
        /// <returns>
        /// Configured MySQL query script builder.
        /// </returns>
        public static IQueryScriptBuilder CreateScriptBuilder(
            ISqlDatabaseDialect databaseDialect)
        {
            return QueryCompilerFactory.CreateScriptBuilder(
                databaseDialect,
                new QueryScriptBuilderOptions
                {
                    ApplyClauseBuilderFactory = subqueryCompiler =>
                        new MySqlApplyClauseBuilder(subqueryCompiler)
                });
        }
    }
}
