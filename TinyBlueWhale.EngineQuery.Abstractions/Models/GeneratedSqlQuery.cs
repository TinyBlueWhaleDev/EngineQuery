using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.Abstractions.Models
{
    public sealed record QuerySqlParameter
    {
        public required string Name { get; init; }
        public object? Value { get; init; }
        public Type? ValueType => Value?.GetType();
    }
}
