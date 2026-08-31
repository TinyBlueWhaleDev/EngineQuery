using TinyBlueWhale.EngineQuery.Core.QueryBuilding;
using TinyBlueWhale.EngineQuery.Metadata.Resolvers;
using TinyBlueWhale.EngineQuery.MySql.Capabilities;
using TinyBlueWhale.EngineQuery.MySql.Compilation;
using TinyBlueWhale.EngineQuery.MySql.Dialects;
using TinyBlueWhale.EngineQuery.MySql.Profiles;
using TinyBlueWhale.EngineQuery.PostgreSql.Capabilities;
using TinyBlueWhale.EngineQuery.PostgreSql.Compilation;
using TinyBlueWhale.EngineQuery.PostgreSql.Dialects;
using TinyBlueWhale.EngineQuery.PostgreSql.Profiles;
using TinyBlueWhale.EngineQuery.SqlServer.Capabilities;
using TinyBlueWhale.EngineQuery.SqlServer.Compilation;
using TinyBlueWhale.EngineQuery.SqlServer.Dialects;
using TinyBlueWhale.EngineQuery.SqlServer.Profiles;

namespace TinyBlueWhale.EngineQuery.Playground.Shared
{
    /// <summary>
    /// Provides query builders configured for the database providers
    /// used by the Playground validators.
    /// </summary>
    internal static class ProviderQueryBuilderFactory
    {
        /// <summary>
        /// Creates a SQL Server query builder using the default SQL Server profile.
        /// </summary>
        /// <param name="metadataResolver">
        /// Metadata resolver used to resolve entity and property mappings.
        /// </param>
        /// <returns>
        /// Query builder configured for the default SQL Server profile.
        /// </returns>
        public static QueryBuilder<SqlServerDefaultProfile> CreateSqlServer(
            FluentEntityMetadataResolver metadataResolver)
        {
            ArgumentNullException.ThrowIfNull(metadataResolver);

            return SqlServerQueryCompiler.Factory.Create(metadataResolver);
        }

        /// <summary>
        /// Creates a PostgreSQL query builder using the default PostgreSQL profile.
        /// </summary>
        /// <param name="metadataResolver">
        /// Metadata resolver used to resolve entity and property mappings.
        /// </param>
        /// <returns>
        /// Query builder configured for the default PostgreSQL profile.
        /// </returns>
        public static QueryBuilder<PostgreSqlDefaultProfile> CreatePostgreSql(
            FluentEntityMetadataResolver metadataResolver)
        {
            ArgumentNullException.ThrowIfNull(metadataResolver);

            return PostgreSqlQueryCompiler.Factory.Create(metadataResolver);
        }

        /// <summary>
        /// Creates a MySQL query builder using the default MySQL profile.
        /// </summary>
        /// <param name="metadataResolver">
        /// Metadata resolver used to resolve entity and property mappings.
        /// </param>
        /// <returns>
        /// Query builder configured for the default MySQL profile.
        /// </returns>
        public static QueryBuilder<MySqlDefaultProfile> CreateMySql(
            FluentEntityMetadataResolver metadataResolver)
        {
            ArgumentNullException.ThrowIfNull(metadataResolver);

            return MySqlQueryCompiler.Factory.Create(metadataResolver);
        }
    }
}
