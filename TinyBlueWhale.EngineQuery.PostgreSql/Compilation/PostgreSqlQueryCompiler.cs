using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Core.Interfaces;
using TinyBlueWhale.EngineQuery.PostgreSql.Composition;
using TinyBlueWhale.EngineQuery.Sql.Compilation;
using TinyBlueWhale.EngineQuery.Sql.Composition;

namespace TinyBlueWhale.EngineQuery.PostgreSql.Compilation
{
    /// <summary>
    /// Compiles query definitions into PostgreSQL command text.
    /// </summary>
    /// <remarks>
    /// This compiler receives provider capabilities used by features that remain under
    /// migration and an already resolved SQL feature composition.
    /// </remarks>
    /// <param name="databaseDialect">
    /// PostgreSQL database dialect.
    /// </param>
    /// <param name="providerCapabilities">
    /// PostgreSQL provider capabilities.
    /// </param>
    /// <param name="featureComposition">
    /// SQL feature composition resolved from the selected provider profile.
    /// </param>
    public sealed class PostgreSqlQueryCompiler(
        ISqlDatabaseDialect databaseDialect,
        IDatabaseProviderCapabilities providerCapabilities,
        QueryFeatureComposition featureComposition) :
        QueryCompilerBase(
            databaseDialect,
            providerCapabilities,
            PostgreSqlQueryCompilerFactory.CreateScriptBuilder(
                databaseDialect,
                featureComposition));
}
