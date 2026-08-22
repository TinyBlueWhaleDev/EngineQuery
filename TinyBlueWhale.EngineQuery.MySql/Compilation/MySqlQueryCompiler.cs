using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Core.Interfaces;
using TinyBlueWhale.EngineQuery.MySql.Composition;
using TinyBlueWhale.EngineQuery.Sql.Compilation;

namespace TinyBlueWhale.EngineQuery.MySql.Compilation
{
    /// <summary>
    /// Compiles query definitions into MySQL command text.
    /// </summary>
    /// <remarks>
    /// This compiler uses MySQL-specific APPLY behavior while reusing the default SQL builder pipeline.
    /// </remarks>
    public sealed class MySqlQueryCompiler(
        ISqlDatabaseDialect databaseDialect,
        IDatabaseProviderCapabilities providerCapabilities) : QueryCompilerBase(
            databaseDialect,
            providerCapabilities,
            MySqlQueryCompilerFactory.CreateScriptBuilder(databaseDialect));
}
