using TinyBlueWhale.EngineQuery.Core.Interfaces;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding;
using TinyBlueWhale.EngineQuery.Metadata.Interfaces;
using TinyBlueWhale.EngineQuery.MySql.Dialects;
using TinyBlueWhale.EngineQuery.MySql.Profiles;
using TinyBlueWhale.EngineQuery.MySql.Profiles.Interfaces;
using TinyBlueWhale.EngineQuery.Sql.Compilation;
using TinyBlueWhale.EngineQuery.Sql.Composition;

namespace TinyBlueWhale.EngineQuery.MySql.Compilation
{
    /// <summary>
    /// Compiles EngineQuery definitions using MySQL-specific SQL behavior.
    /// </summary>
    public sealed class MySqlQueryCompiler : QueryCompilerBase
    {
        /// <summary>
        /// Initializes a new MySQL query compiler.
        /// </summary>
        /// <param name="databaseDialect">
        /// MySQL database dialect.
        /// </param>
        /// <param name="featureComposition">
        /// SQL feature composition resolved from the selected provider profile.
        /// </param>
        private MySqlQueryCompiler(
            ISqlDatabaseDialect databaseDialect,
            QueryFeatureComposition featureComposition)
            : base(
                databaseDialect,
                QueryCompilerFactory.CreateScriptBuilder(
                    databaseDialect,
                    featureComposition))
        {
        }

        /// <summary>
        /// Provides MySQL query builder creation operations.
        /// </summary>
        public static class Factory
        {
            /// <summary>
            /// Creates a query builder using the default MySQL profile.
            /// </summary>
            /// <param name="metadataResolver">
            /// Metadata resolver used to resolve entity and property mappings.
            /// </param>
            /// <returns>
            /// Query builder configured with the default MySQL profile.
            /// </returns>
            public static QueryBuilder<MySqlDefaultProfile> Create(
                IEntityMetadataResolver metadataResolver)
            {
                return Create<MySqlDefaultProfile>(metadataResolver);
            }

            /// <summary>
            /// Creates a query builder using the specified MySQL profile.
            /// </summary>
            /// <typeparam name="TProfile">
            /// MySQL provider profile used to determine version-specific query features.
            /// </typeparam>
            /// <param name="metadataResolver">
            /// Metadata resolver used to resolve entity and property mappings.
            /// </param>
            /// <returns>
            /// Query builder configured with the specified MySQL profile.
            /// </returns>
            public static QueryBuilder<TProfile> Create<TProfile>(
                IEntityMetadataResolver metadataResolver)
                where TProfile : IMySqlProfile, new()
            {
                ArgumentNullException.ThrowIfNull(metadataResolver);

                var profile = new TProfile();

                return new QueryBuilder<TProfile>(
                    CreateCompiler(profile),
                    metadataResolver,
                    profile);
            }

            /// <summary>
            /// Creates a MySQL query compiler using the specified provider profile.
            /// </summary>
            /// <param name="profile">
            /// MySQL provider profile used to configure the compiler.
            /// </param>
            /// <returns>
            /// Configured MySQL query compiler.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when <paramref name="profile"/> is null.
            /// </exception>
            internal static MySqlQueryCompiler CreateCompiler(IMySqlProfile profile)
            {
                ArgumentNullException.ThrowIfNull(profile);

                var databaseDialect =
                    new MySqlDatabaseDialect();


                var featureComposition =
                    QueryFeatureCompositionFactory.Create(
                        profile);

                return new MySqlQueryCompiler(
                    databaseDialect,
                    featureComposition);
            }
        }
    }
}
