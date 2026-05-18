using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Core.Interfaces;
using TinyBlueWhale.EngineQuery.PostgreSql.Clauses;
using TinyBlueWhale.EngineQuery.Sql.Composition;
using TinyBlueWhale.EngineQuery.Sql.Interfaces;

namespace TinyBlueWhale.EngineQuery.PostgreSql.Composition
{
    /// <summary>
    /// Creates PostgreSQL query compiler collaborators.
    /// </summary>
    /// <remarks>
    /// This factory configures only the PostgreSQL-specific clause builders while reusing
    /// the shared SQL query compilation pipeline from the base project.
    /// </remarks>
    public static class PostgreSqlQueryCompilerFactory
    {
        /// <summary>
        /// Creates the PostgreSQL query script builder.
        /// </summary>
        /// <param name="databaseDialect">
        /// PostgreSQL database dialect.
        /// </param>
        /// <returns>
        /// Configured PostgreSQL query script builder.
        /// </returns>
        public static IQueryScriptBuilder CreateScriptBuilder(
            ISqlDatabaseDialect databaseDialect)
        {
            return QueryCompilerFactory.CreateScriptBuilder(
                databaseDialect,
                new QueryScriptBuilderOptions
                {
                    ApplyClauseBuilderFactory = subqueryCompiler =>
                        new PostgreSqlApplyClauseBuilder(subqueryCompiler)
                });
        }
    }
}
