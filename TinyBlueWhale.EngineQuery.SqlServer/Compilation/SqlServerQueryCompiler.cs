using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Core.Interfaces;
using TinyBlueWhale.EngineQuery.Sql.Compilation;
using TinyBlueWhale.EngineQuery.Sql.Composition;
using TinyBlueWhale.EngineQuery.Sql.Interfaces;
using TinyBlueWhale.EngineQuery.SqlServer.Capabilities;
using TinyBlueWhale.EngineQuery.SqlServer.Composition;

namespace TinyBlueWhale.EngineQuery.SqlServer.Compilation
{
    /// <summary>
    /// Compiles query definitions into SQL Server command text.
    /// </summary>
    /// <remarks>
    /// This compiler uses SQL Server-specific SQL clause builders while delegating
    /// the compilation workflow to the shared query compiler base.
    /// </remarks>
    /// <remarks>
    /// Initializes a new instance of the <see cref="SqlServerQueryCompiler"/> class.
    /// </remarks>
    /// <param name="databaseDialect">
    /// SQL Server database dialect.
    /// </param>
    /// <param name="providerCapabilities">
    /// SQL Server provider capabilities.
    /// </param>
    public sealed class SqlServerQueryCompiler(
        ISqlDatabaseDialect databaseDialect,
        IDatabaseProviderCapabilities providerCapabilities) : QueryCompilerBase(
            databaseDialect,
            providerCapabilities,
            SqlServerQueryCompilerFactory.CreateScriptBuilder(databaseDialect));      
}
