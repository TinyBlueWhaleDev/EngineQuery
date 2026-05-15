using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Core.Interfaces;
using TinyBlueWhale.EngineQuery.Sql.Compilation;
using TinyBlueWhale.EngineQuery.SqlServer.Capabilities;

namespace TinyBlueWhale.EngineQuery.SqlServer.Compilation
{
    /// <summary>
    /// Compiles query definitions into SQL Server command text.
    /// </summary>
    public sealed class SqlServerQueryCompiler(ISqlDatabaseDialect databaseDialect, SqlServerProviderCapabilities providerCapabilities) : QueryCompilerBase(databaseDialect, providerCapabilities)
    {
        // Resolves the SQL keyword used for recursive common table expressions.
        protected override string ResolveRecursiveCteKeyword()
        {
            return "WITH";
        }
    }
}
