using TinyBlueWhale.EngineQuery.Samples.Database.MySql;
using TinyBlueWhale.EngineQuery.Samples.Database.PostgreSql;
using TinyBlueWhale.EngineQuery.Samples.Database.SqlServer;
using TinyBlueWhale.EngineQuery.Samples.Providers;

namespace TinyBlueWhale.EngineQuery.Samples.Database
{
    public static class BuildDatabaseInitializer
    {
        public static IDatabaseInitializer Create(SampleProviderContext provider)
        {
            return provider.Kind switch
            {
                SampleProviderKind.SqlServer => new SqlServerDatabaseInitializer(),
                SampleProviderKind.PostgreSql => new PostgreSqlDatabaseInitializer(),
                SampleProviderKind.MySql => new MySqlDatabaseInitializer(),
                _ => new NoOpDatabaseInitializer()
            };
        }
    }
}
