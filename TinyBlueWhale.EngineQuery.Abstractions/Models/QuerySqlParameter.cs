using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.Abstractions.Models
{
    public sealed record GeneratedSqlQuery
    {
        public required string CommandText { get; init; }
        public required IReadOnlyList<QuerySqlParameter> Parameters { get; init; }
        public bool HasParameters => Parameters.Count > 0;
    }
}
