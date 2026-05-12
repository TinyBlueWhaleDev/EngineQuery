using TinyBlueWhale.EngineQuery.Core.Interfaces;
using TinyBlueWhale.EngineQuery.Sql.Compilation;

namespace TinyBlueWhale.EngineQuery.PostgreSql.Compilation
{
    /// <summary>
    /// Compiles query definitions into PostgreSQL command text.
    /// </summary>
    public sealed class PostgreSqlQueryCompiler(ISqlDatabaseDialect databaseDialect) : QueryCompilerBase(databaseDialect);
}
