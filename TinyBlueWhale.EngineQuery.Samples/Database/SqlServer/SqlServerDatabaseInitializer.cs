using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using TinyBlueWhale.EngineQuery.Samples.Providers;

namespace TinyBlueWhale.EngineQuery.Samples.Database.SqlServer
{
    public sealed partial class SqlServerDatabaseInitializer : IDatabaseInitializer
    {
        public async Task InitializeAsync(SampleProviderContext provider, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(provider.ConnectionString))
                return;

            var scripts = DatabaseScriptPathResolver.Resolve(provider);

            await ExecuteSqlScriptAsync(provider.ConnectionString, scripts.SchemaScriptPath, cancellationToken);
            await ExecuteSqlScriptAsync(provider.ConnectionString, scripts.SeedScriptPath, cancellationToken);
        }

        private static async Task ExecuteSqlScriptAsync(string connectionString, string scriptPath, CancellationToken cancellationToken)
        {
            if (!File.Exists(scriptPath))
                throw new FileNotFoundException($"SQL script was not found: {scriptPath}");

            var script = await File.ReadAllTextAsync(scriptPath, cancellationToken);
            var batches = SplitSqlServerBatches(script);

            await using var connection = new SqlConnection(connectionString);

            await connection.OpenAsync(cancellationToken);

            foreach (var batch in batches)
            {
                if (string.IsNullOrWhiteSpace(batch))
                    continue;

                await using var command = connection.CreateCommand();

                command.CommandText = batch;

                await command.ExecuteNonQueryAsync(cancellationToken);
            }
        }

        private static List<string> SplitSqlServerBatches(string script)
        {
            return [.. MyRegex().Split(script)
                .Where(batch => !string.IsNullOrWhiteSpace(batch))
                .Select(batch => batch.Trim())];
        }

        [GeneratedRegex(@"^\s*GO\s*;?\s*$", RegexOptions.IgnoreCase | RegexOptions.Multiline, "en-US")]
        private static partial Regex MyRegex();
    }
}
