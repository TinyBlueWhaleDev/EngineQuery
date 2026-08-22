using Microsoft.Extensions.Configuration;

namespace TinyBlueWhale.EngineQuery.Samples.Settings
{
    public static class LoadSampleSettings
    {
        public static SampleSettings Create()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables()
                .Build();

            return new SampleSettings
            {
                ConnectionStrings = new SampleConnectionStrings
                {
                    SqlServer = Resolve(configuration, "SqlServer", "ENGINEQUERY_SQLSERVER_CONNECTION"),
                    PostgreSql = Resolve(configuration, "PostgreSql", "ENGINEQUERY_POSTGRESQL_CONNECTION"),
                    MySql = Resolve(configuration, "MySql", "ENGINEQUERY_MYSQL_CONNECTION")
                }
            };
        }

        private static string Resolve(IConfiguration configuration, string name, string environmentVariable)
        {
            var value = Environment.GetEnvironmentVariable(environmentVariable);

            if (!string.IsNullOrWhiteSpace(value))
                return value;

            return configuration.GetConnectionString(name) ?? string.Empty;
        }
    }
}
