using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Sql.Enums;

namespace TinyBlueWhale.EngineQuery.Sql.Compilation.Models
{
    public sealed record QueryOrderingDefinition()
    {
        public string PropertyName { get; init; } = null!;
        public QueryOrderingDirection Direction { get; init; }
    };    
}
