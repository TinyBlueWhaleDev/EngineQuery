using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Core.Interfaces;
using TinyBlueWhale.EngineQuery.Sql.Compilation;
using TinyBlueWhale.EngineQuery.Sql.Composition;
using TinyBlueWhale.EngineQuery.SqlServer.Composition;

namespace TinyBlueWhale.EngineQuery.SqlServer.Compilation
{
    /// <summary>
    /// Compiles query definitions into SQL Server command text.
    /// </summary>
    /// <remarks>
    /// This compiler receives provider capabilities used by features that remain under
    /// migration and an already resolved SQL feature composition.
    /// </remarks>
    /// <param name="databaseDialect">
    /// SQL Server database dialect.
    /// </param>
    /// <param name="providerCapabilities">
    /// SQL Server provider capabilities.
    /// </param>
    /// <param name="featureComposition">
    /// SQL feature composition resolved from the selected provider profile.
    /// </param>
    public sealed class SqlServerQueryCompiler(
        ISqlDatabaseDialect databaseDialect,
        IDatabaseProviderCapabilities providerCapabilities,
        QueryFeatureComposition featureComposition) :
        QueryCompilerBase(
            databaseDialect,
            providerCapabilities,
            SqlServerQueryCompilerFactory.CreateScriptBuilder(
                databaseDialect,
                featureComposition));
}
