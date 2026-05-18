using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Core.Interfaces;
using TinyBlueWhale.EngineQuery.Sql.Composition;
using TinyBlueWhale.EngineQuery.Sql.Interfaces;
using TinyBlueWhale.EngineQuery.SqlServer.Clauses;

namespace TinyBlueWhale.EngineQuery.SqlServer.Composition
{
    public static class SqlServerQueryCompilerFactory
    {
        public static IQueryScriptBuilder CreateScriptBuilder(
            ISqlDatabaseDialect databaseDialect)
        {
            return QueryCompilerFactory.CreateScriptBuilder(
                databaseDialect,
                new QueryScriptBuilderOptions
                {
                    CteClauseBuilderFactory = subqueryCompiler =>
                        new SqlServerCteClauseBuilder(subqueryCompiler)
                });
        }
    }
}
