using TinyBlueWhale.EngineQuery.Abstractions.Models;

namespace TinyBlueWhale.EngineQuery.Samples.Results
{
    public sealed class SampleExecutionResult
    {
        public required string Provider { get; init; }

        public required string Executor { get; init; }

        public required string Metadata { get; init; }

        public required string Query { get; init; }

        public required string CommandText { get; init; }

        public IReadOnlyList<QuerySqlParameter> Parameters { get; init; } = [];

        public required string Status { get; init; }

        public int RowCount { get; init; }

        public string ResultText { get; init; } = string.Empty;

        public string? ErrorMessage { get; init; }
    }
}
