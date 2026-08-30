using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Core.Interfaces;
using TinyBlueWhale.EngineQuery.MySql.Composition;
using TinyBlueWhale.EngineQuery.Sql.Compilation;
using TinyBlueWhale.EngineQuery.Sql.Composition;

namespace TinyBlueWhale.EngineQuery.MySql.Compilation
{
    /// <summary>
    /// Compiles query definitions into MySQL command text.
    /// </summary>
    /// <remarks>
    /// This compiler receives provider capabilities used by features that remain under
    /// migration and an already resolved SQL feature composition.
    /// </remarks>
    /// <param name="databaseDialect">
    /// MySQL database dialect.
    /// </param>
    /// <param name="providerCapabilities">
    /// MySQL provider capabilities.
    /// </param>
    /// <param name="featureComposition">
    /// SQL feature composition resolved from the selected provider profile.
    /// </param>
    public sealed class MySqlQueryCompiler(
        ISqlDatabaseDialect databaseDialect,
        IDatabaseProviderCapabilities providerCapabilities,
        QueryFeatureComposition featureComposition) :
        QueryCompilerBase(
            databaseDialect,
            providerCapabilities,
            MySqlQueryCompilerFactory.CreateScriptBuilder(
                databaseDialect,
                featureComposition));
}
