using TinyBlueWhale.EngineQuery.Core.Interfaces;
using TinyBlueWhale.EngineQuery.Sql.Compilation;

namespace TinyBlueWhale.EngineQuery.MySql.Compilation
{
    /// <summary>
    /// Compiles query definitions into MySQL command text.
    /// </summary>
    public sealed class MySqlQueryCompiler(ISqlDatabaseDialect databaseDialect) : QueryCompilerBase(databaseDialect);
}
