using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Core.Interfaces;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding;
using TinyBlueWhale.EngineQuery.Metadata.Interfaces;
using TinyBlueWhale.EngineQuery.PostgreSql.Capabilities;
using TinyBlueWhale.EngineQuery.PostgreSql.Composition;
using TinyBlueWhale.EngineQuery.PostgreSql.Dialects;
using TinyBlueWhale.EngineQuery.PostgreSql.Profiles;
using TinyBlueWhale.EngineQuery.PostgreSql.Profiles.Interfaces;
using TinyBlueWhale.EngineQuery.Sql.Compilation;
using TinyBlueWhale.EngineQuery.Sql.Composition;

namespace TinyBlueWhale.EngineQuery.PostgreSql.Compilation
{
    /// <summary>
    /// Compiles EngineQuery definitions using PostgreSQL-specific SQL behavior.
    /// </summary>
    public sealed class PostgreSqlQueryCompiler : QueryCompilerBase
    {
        /// <summary>
        /// Initializes a new PostgreSQL query compiler.
        /// </summary>
        /// <param name="databaseDialect">
        /// PostgreSQL database dialect.
        /// </param>
        /// <param name="providerCapabilities">
        /// PostgreSQL provider capabilities.
        /// </param>
        /// <param name="featureComposition">
        /// Query feature composition supported by the provider profile.
        /// </param>
        private PostgreSqlQueryCompiler(
            ISqlDatabaseDialect databaseDialect,
            IDatabaseProviderCapabilities providerCapabilities,
            QueryFeatureComposition featureComposition)
            : base(
                databaseDialect,
                providerCapabilities,
                PostgreSqlQueryCompilerFactory.CreateScriptBuilder(
                    databaseDialect,
                    featureComposition))
        {
        }

        /// <summary>
        /// Provides PostgreSQL query builder creation operations.
        /// </summary>
        public static class Factory
        {
            /// <summary>
            /// Creates a query builder using the default PostgreSQL profile.
            /// </summary>
            /// <param name="metadataResolver">
            /// Metadata resolver used to resolve entity and property mappings.
            /// </param>
            /// <returns>
            /// Query builder configured with the default PostgreSQL profile.
            /// </returns>
            public static QueryBuilder<PostgreSqlDefaultProfile> Create(
                IEntityMetadataResolver metadataResolver)
            {
                return Create<PostgreSqlDefaultProfile>(metadataResolver);
            }

            /// <summary>
            /// Creates a query builder using the specified PostgreSQL profile.
            /// </summary>
            /// <typeparam name="TProfile">
            /// PostgreSQL provider profile used to configure version-specific capabilities
            /// and query features.
            /// </typeparam>
            /// <param name="metadataResolver">
            /// Metadata resolver used to resolve entity and property mappings.
            /// </param>
            /// <returns>
            /// Query builder configured with the specified PostgreSQL profile.
            /// </returns>
            public static QueryBuilder<TProfile> Create<TProfile>(
                IEntityMetadataResolver metadataResolver)
                where TProfile : IPostgreSqlProfile, new()
            {
                ArgumentNullException.ThrowIfNull(metadataResolver);

                var profile = new TProfile();

                return new QueryBuilder<TProfile>(
                    CreateCompiler(profile),
                    metadataResolver,
                    profile);
            }

            /// <summary>
            /// Creates a PostgreSQL query compiler using the specified provider profile.
            /// </summary>
            /// <param name="profile">
            /// PostgreSQL provider profile used to configure the compiler.
            /// </param>
            /// <returns>
            /// Configured PostgreSQL query compiler.
            /// </returns>
            internal static PostgreSqlQueryCompiler CreateCompiler(
                IPostgreSqlProfile profile)
            {
                ArgumentNullException.ThrowIfNull(profile);

                var databaseDialect =
                    new PostgreSqlDatabaseDialect();

                var providerCapabilities =
                    new PostgreSqlProviderCapabilities(
                        profile.Version);

                var featureComposition =
                    QueryFeatureCompositionFactory.Create(
                        profile);

                return new PostgreSqlQueryCompiler(
                    databaseDialect,
                    providerCapabilities,
                    featureComposition);
            }
        }
    }
}
