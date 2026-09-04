using TinyBlueWhale.EngineQuery.Core.QueryBuilding;
using TinyBlueWhale.EngineQuery.Metadata.Interfaces;
using TinyBlueWhale.EngineQuery.Metadata.Resolvers;
using TinyBlueWhale.EngineQuery.MySql.Compilation;
using TinyBlueWhale.EngineQuery.MySql.Profiles;
using TinyBlueWhale.EngineQuery.PostgreSql.Compilation;
using TinyBlueWhale.EngineQuery.PostgreSql.Profiles;
using TinyBlueWhale.EngineQuery.SqlServer.Compilation;
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
        public static QueryBuilder<SqlServer2012Profile> CreateSqlServer(
            IEntityMetadataResolver metadataResolver)
        {
            ArgumentNullException.ThrowIfNull(metadataResolver);

            return SqlServerQueryCompiler.Factory.Create<SqlServer2012Profile>(metadataResolver);
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
        public static QueryBuilder<PostgreSql93Profile> CreatePostgreSql(
            IEntityMetadataResolver metadataResolver)
        {
            ArgumentNullException.ThrowIfNull(metadataResolver);

            return PostgreSqlQueryCompiler.Factory.Create<PostgreSql93Profile>(metadataResolver);
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
        public static QueryBuilder<MySql8031Profile> CreateMySql(
            IEntityMetadataResolver metadataResolver)
        {
            ArgumentNullException.ThrowIfNull(metadataResolver);

            return MySqlQueryCompiler.Factory.Create<MySql8031Profile>(metadataResolver);
        }
    }
}
