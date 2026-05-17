using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Samples.Providers;

namespace TinyBlueWhale.EngineQuery.Samples.Database.MySql
{
    public sealed class MySqlDatabaseInitializer : IDatabaseInitializer
    {
        public async Task InitializeAsync(
            SampleProviderContext provider,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(provider.ConnectionString))
                return;

            var scripts = DatabaseScriptPathResolver.Resolve(provider);

            await ExecuteSqlScriptAsync(
                provider.ConnectionString,
                scripts.SchemaScriptPath,
                cancellationToken);

            await ExecuteSqlScriptAsync(
                provider.ConnectionString,
                scripts.SeedScriptPath,
                cancellationToken);
        }

        private static async Task ExecuteSqlScriptAsync(
            string connectionString,
            string scriptPath,
            CancellationToken cancellationToken)
        {
            if (!File.Exists(scriptPath))
                throw new FileNotFoundException($"SQL script was not found: {scriptPath}");

            var script = await File.ReadAllTextAsync(scriptPath, cancellationToken);

            await using var connection = new MySqlConnection(connectionString);

            await connection.OpenAsync(cancellationToken);

            await using var command = connection.CreateCommand();

            command.CommandText = script;

            await command.ExecuteNonQueryAsync(cancellationToken);
        }
    }
}
