using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Npgsql;
using TinyBlueWhale.EngineQuery.Metadata.Interfaces;
using TinyBlueWhale.EngineQuery.MySql.Capabilities;
using TinyBlueWhale.EngineQuery.MySql.Compilation;
using TinyBlueWhale.EngineQuery.MySql.Dialects;
using TinyBlueWhale.EngineQuery.PostgreSql.Capabilities;
using TinyBlueWhale.EngineQuery.PostgreSql.Compilation;
using TinyBlueWhale.EngineQuery.PostgreSql.Dialects;
using TinyBlueWhale.EngineQuery.Samples.EntityFramework;
using TinyBlueWhale.EngineQuery.Samples.Settings;
using TinyBlueWhale.EngineQuery.SqlServer.Capabilities;
using TinyBlueWhale.EngineQuery.SqlServer.Compilation;
using TinyBlueWhale.EngineQuery.SqlServer.Dialects;

namespace TinyBlueWhale.EngineQuery.Samples.Providers
{
//    public static class BuildSampleProviders
//    {
//        public static IReadOnlyList<SampleProviderContext> Create(SampleConnectionStrings connectionStrings) => [
//                new SampleProviderContext
//                {
//                    Kind = SampleProviderKind.SqlServer,
//                    Name = "SQL Server",
//                    ConnectionString = connectionStrings.SqlServer,
//                    BuildQueryBuilder = CreateSqlServerQueryBuilder,
//                    OpenConnection = () => new SqlConnection(connectionStrings.SqlServer),
//                    BuildParameter = (name,value) => new SqlParameter(name,value ?? DBNull.Value),
//                    BuildDbContextOptions =
//                        () => new DbContextOptionsBuilder<SampleDbContext>()
//                        .UseSqlServer(connectionStrings.SqlServer)
//                        .Options
//                },
//                new SampleProviderContext
//                {
//                    Kind = SampleProviderKind.PostgreSql,
//                    Name = "PostgreSQL",
//                    ConnectionString = connectionStrings.PostgreSql,
//                    BuildQueryBuilder = CreatePostgreSqlQueryBuilder,
//                    OpenConnection = () => new NpgsqlConnection(connectionStrings.PostgreSql),
//                    BuildParameter = (name,value) => new NpgsqlParameter(name,value ?? DBNull.Value),
//                    BuildDbContextOptions =
//                        () => new DbContextOptionsBuilder<SampleDbContext>()
//                        .UseNpgsql(connectionStrings.PostgreSql)
//                        .Options
//                },
//                new SampleProviderContext
//                {
//                    Kind = SampleProviderKind.MySql,
//                    Name = "MySQL",
//                    ConnectionString = connectionStrings.MySql,
//                    BuildQueryBuilder = CreateMySqlQueryBuilder,
//                    OpenConnection = () => new MySqlConnection(connectionStrings.MySql),
//                    BuildParameter = (name,value) => new MySqlParameter(name,value ?? DBNull.Value),
//                    BuildDbContextOptions = () => CreateMySqlDbContextOptions(connectionStrings.MySql)
//                }
//            ];

//        // Builds Entity Framework Core options for the MySQL provider supported by the current target framework.
//        private static DbContextOptions<SampleDbContext> CreateMySqlDbContextOptions(string connectionString)
//        {
//#if NET8_0
//            return new DbContextOptionsBuilder<SampleDbContext>()
//                .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
//                .Options;
//#else
//            return new DbContextOptionsBuilder<SampleDbContext>()
//                .UseMySQL(connectionString)
//                .Options;
//#endif
//        }

//        private static QueryBuilder CreateSqlServerQueryBuilder(IEntityMetadataResolver metadataResolver)
//        {
//            return new QueryBuilder(
//                new SqlServerQueryCompiler(new SqlServerDatabaseDialect(), new SqlServerProviderCapabilities()),
//                metadataResolver);
//        }

//        private static QueryBuilder CreatePostgreSqlQueryBuilder(IEntityMetadataResolver metadataResolver)
//        {
//            return new QueryBuilder(
//                new PostgreSqlQueryCompiler(new PostgreSqlDatabaseDialect(), new PostgreSqlProviderCapabilities()),
//                metadataResolver);
//        }

//        private static QueryBuilder CreateMySqlQueryBuilder(IEntityMetadataResolver metadataResolver)
//        {
//            return new QueryBuilder(
//                new MySqlQueryCompiler(new MySqlDatabaseDialect(), new MySqlProviderCapabilities()),
//                metadataResolver);
//        }
//    }
}
