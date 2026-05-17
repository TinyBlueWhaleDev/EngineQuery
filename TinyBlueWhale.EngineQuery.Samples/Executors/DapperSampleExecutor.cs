using Dapper;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Samples.Metadata;
using TinyBlueWhale.EngineQuery.Samples.Providers;
using TinyBlueWhale.EngineQuery.Samples.Queries;
using TinyBlueWhale.EngineQuery.Samples.Results;

namespace TinyBlueWhale.EngineQuery.Samples.Executors
{
    public sealed class DapperSampleExecutor : ISampleExecutor
    {
        public string Name => "Dapper";

        public async Task<SampleExecutionResult> ExecuteAsync(
            SampleProviderContext provider,
            SalesQueryScenario scenario,
            CancellationToken cancellationToken = default)
        {
            var metadataName = BuildMetadataResolver.GetDisplayName(scenario.MetadataStrategy);
            GeneratedSqlQuery? query = null;

            try
            {
                var metadataResolver = BuildMetadataResolver.Create(provider, scenario.MetadataStrategy);
                var queryBuilder = provider.BuildQueryBuilder(metadataResolver);

                query = scenario.Build(queryBuilder);

                if (string.IsNullOrWhiteSpace(provider.ConnectionString))
                    return BuildSkipped(provider, scenario, metadataName, query);

                await using var connection = provider.OpenConnection();

                await connection.OpenAsync(cancellationToken);

                var parameters = new DynamicParameters();

                foreach (var parameter in query.Parameters)
                    parameters.Add(parameter.Name, parameter.Value);

                var rows = (await connection.QueryAsync(query.CommandText, parameters)).ToList();
                var resultText = SerializeRowsAsCsv.FromDapperRows(rows);

                return new SampleExecutionResult
                {
                    Provider = provider.Name,
                    Executor = Name,
                    Metadata = metadataName,
                    Query = scenario.Name,
                    CommandText = query.CommandText,
                    Parameters = query.Parameters,
                    Status = "Success",
                    RowCount = rows.Count,
                    ResultText = resultText
                };
            }
            catch (Exception exception)
            {
                return BuildError(provider, scenario, metadataName, query, exception);
            }
        }

        private SampleExecutionResult BuildSkipped(
            SampleProviderContext provider,
            SalesQueryScenario scenario,
            string metadataName,
            GeneratedSqlQuery query)
        {
            return new SampleExecutionResult
            {
                Provider = provider.Name,
                Executor = Name,
                Metadata = metadataName,
                Query = scenario.Name,
                CommandText = query.CommandText,
                Parameters = query.Parameters,
                Status = "Skipped",
                RowCount = 0,
                ErrorMessage = "Connection string is not configured."
            };
        }

        private SampleExecutionResult BuildError(
            SampleProviderContext provider,
            SalesQueryScenario scenario,
            string metadataName,
            GeneratedSqlQuery? query,
            Exception exception)
        {
            return new SampleExecutionResult
            {
                Provider = provider.Name,
                Executor = Name,
                Metadata = metadataName,
                Query = scenario.Name,
                CommandText = query?.CommandText ?? "(SQL generation failed)",
                Parameters = query?.Parameters ?? [],
                Status = "Error",
                RowCount = 0,
                ErrorMessage = exception.ToString()
            };
        }
    }
}
