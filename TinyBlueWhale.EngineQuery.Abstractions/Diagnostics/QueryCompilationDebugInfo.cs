using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Abstractions.Models;

namespace TinyBlueWhale.EngineQuery.Abstractions.Diagnostics
{
    public sealed class QueryCompilationDebugInfo
    {
        public required DatabaseProvider Provider { get; init; }
        public required GeneratedSqlQuery GeneratedQuery { get; init; }
        public TimeSpan? CompilationTime { get; init; }
        public TimeSpan? ExecutionTime { get; init; }
        public string? QueryTag { get; init; }
    }
}
