using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding;
using TinyBlueWhale.EngineQuery.Metadata.Resolvers;
using TinyBlueWhale.EngineQuery.MySql.Capabilities;
using TinyBlueWhale.EngineQuery.MySql.Compilation;
using TinyBlueWhale.EngineQuery.MySql.Dialects;
using TinyBlueWhale.EngineQuery.PostgreSql.Capabilities;
using TinyBlueWhale.EngineQuery.PostgreSql.Compilation;
using TinyBlueWhale.EngineQuery.PostgreSql.Dialects;
using TinyBlueWhale.EngineQuery.SqlServer.Capabilities;
using TinyBlueWhale.EngineQuery.SqlServer.Compilation;
using TinyBlueWhale.EngineQuery.SqlServer.Dialects;

namespace TinyBlueWhale.EngineQuery.Playground.Shared
{
    /// <summary>
    /// Creates provider-specific query builders for playground validation scenarios.
    /// </summary>
    internal static class ProviderQueryBuilderFactory
    {
        /// <summary>
        /// Creates a SQL Server query builder.
        /// </summary>
        public static QueryBuilder CreateSqlServer(FluentEntityMetadataResolver metadataResolver) =>
            new(new SqlServerQueryCompiler(new SqlServerDatabaseDialect(),new SqlServerProviderCapabilities()), metadataResolver);

        /// <summary>
        /// Creates a PostgreSQL query builder.
        /// </summary>
        public static QueryBuilder CreatePostgreSql(FluentEntityMetadataResolver metadataResolver) =>
            new(new PostgreSqlQueryCompiler(new PostgreSqlDatabaseDialect(), new PostgreSqlProviderCapabilities()), metadataResolver);

        /// <summary>
        /// Creates a MySQL query builder.
        /// </summary>
        public static QueryBuilder CreateMySql(FluentEntityMetadataResolver metadataResolver) =>
            new(new MySqlQueryCompiler(new MySqlDatabaseDialect(), new MySqlProviderCapabilities()), metadataResolver);
    }
}
