using TinyBlueWhale.EngineQuery.Core.Interfaces;
using TinyBlueWhale.EngineQuery.Sql.Compilation;

namespace TinyBlueWhale.EngineQuery.SqlServer.Compilation
{
    /// <summary>
    /// Compiles query definitions into SQL Server command text.
    /// </summary>
    public sealed class SqlServerQueryCompiler(ISqlDatabaseDialect databaseDialect) : QueryCompilerBase(databaseDialect)
    {
        // Resolves the SQL keyword used for recursive common table expressions.
        protected override string ResolveRecursiveCteKeyword()
        {
            return "WITH";
        }
    }
}
