using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.Sql.Compilation.Models
{
    public sealed record QuerySelectColumnDefinition
    {
        public required string PropertyName { get; init; }
        public string? Alias { get; init; }
    }
}
