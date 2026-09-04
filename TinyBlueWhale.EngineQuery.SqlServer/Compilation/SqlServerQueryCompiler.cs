using TinyBlueWhale.EngineQuery.Core.Interfaces;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding;
using TinyBlueWhale.EngineQuery.Metadata.Interfaces;
using TinyBlueWhale.EngineQuery.Sql.Compilation;
using TinyBlueWhale.EngineQuery.Sql.Composition;
using TinyBlueWhale.EngineQuery.SqlServer.Composition;
using TinyBlueWhale.EngineQuery.SqlServer.Dialects;
using TinyBlueWhale.EngineQuery.SqlServer.Profiles;
using TinyBlueWhale.EngineQuery.SqlServer.Profiles.Interfaces;

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
    public sealed class SqlServerQueryCompiler : QueryCompilerBase
    {
        /// <summary>
        /// Initializes a new SQL Server query compiler.
        /// </summary>
        /// <param name="databaseDialect">
        /// SQL Server database dialect.
        /// </param>
        /// <param name="providerCapabilities">
        /// SQL Server provider capabilities.
        /// </param>
        /// <param name="featureComposition">
        /// Query feature composition supported by the provider profile.
        /// </param>
        private SqlServerQueryCompiler(
            ISqlDatabaseDialect databaseDialect,
            QueryFeatureComposition featureComposition)
            : base(
                databaseDialect,
                SqlServerQueryCompilerFactory.CreateScriptBuilder(
                    databaseDialect,
                    featureComposition))
        {
        }

        /// <summary>
        /// Provides SQL Server query builder creation operations.
        /// </summary>
        public static class Factory
        {
            /// <summary>
            /// Creates a query builder using the default SQL Server profile.
            /// </summary>
            /// <param name="metadataResolver">
            /// Metadata resolver used to resolve entity and property mappings.
            /// </param>
            /// <returns>
            /// Query builder configured with the default SQL Server profile.
            /// </returns>
            public static QueryBuilder<SqlServerDefaultProfile> Create(IEntityMetadataResolver metadataResolver)
            {
                return Create<SqlServerDefaultProfile>(metadataResolver);
            }

            /// <summary>
            /// Creates a query builder using the specified SQL Server profile.
            /// </summary>
            /// <typeparam name="TProfile">
            /// SQL Server provider profile used to determine version-specific
            /// capabilities and query features.
            /// </typeparam>
            /// <param name="metadataResolver">
            /// Metadata resolver used to resolve entity and property mappings.
            /// </param>
            /// <returns>
            /// Query builder configured with the specified SQL Server profile.
            /// </returns>
            public static QueryBuilder<TProfile> Create<TProfile>(IEntityMetadataResolver metadataResolver)
                where TProfile : ISqlServerProfile, new()
            {
                ArgumentNullException.ThrowIfNull(metadataResolver);

                var profile = new TProfile();

                var queryCompiler = new SqlServerQueryCompiler(
                    new SqlServerDatabaseDialect(),
                    QueryFeatureCompositionFactory.Create(profile));

                return new QueryBuilder<TProfile>(
                    queryCompiler,
                    metadataResolver,
                    profile);
            }

            /// <summary>
            /// Creates a SQL Server query compiler using the specified provider profile.
            /// </summary>
            /// <param name="profile">
            /// SQL Server provider profile used to configure the compiler.
            /// </param>
            /// <returns>
            /// Configured SQL Server query compiler.
            /// </returns>
            internal static SqlServerQueryCompiler CreateCompiler(ISqlServerProfile profile)
            {
                ArgumentNullException.ThrowIfNull(profile);

                var databaseDialect = new SqlServerDatabaseDialect();

                var featureComposition = QueryFeatureCompositionFactory.Create(profile);

                return new SqlServerQueryCompiler(
                    databaseDialect,
                    featureComposition);
            }
        }
    }
}
