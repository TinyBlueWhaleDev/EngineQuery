using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Core.Interfaces;
using TinyBlueWhale.EngineQuery.PostgreSql.Composition;
using TinyBlueWhale.EngineQuery.Sql.Compilation;

namespace TinyBlueWhale.EngineQuery.PostgreSql.Compilation
{
    /// <summary>
    /// Compiles query definitions into PostgreSQL command text.
    /// </summary>
    /// <remarks>
    /// This compiler uses PostgreSQL-specific APPLY behavior while reusing the default SQL builder pipeline.
    /// </remarks>
    /// <remarks>
    /// Initializes a new instance of the <see cref="PostgreSqlQueryCompiler"/> class.
    /// </remarks>
    /// <param name="databaseDialect">
    /// PostgreSQL database dialect.
    /// </param>
    /// <param name="providerCapabilities">
    /// PostgreSQL provider capabilities.
    /// </param>
    public sealed class PostgreSqlQueryCompiler(
        ISqlDatabaseDialect databaseDialect,
        IDatabaseProviderCapabilities providerCapabilities) : QueryCompilerBase(
            databaseDialect,
            providerCapabilities,
            PostgreSqlQueryCompilerFactory.CreateScriptBuilder(databaseDialect));    
}
