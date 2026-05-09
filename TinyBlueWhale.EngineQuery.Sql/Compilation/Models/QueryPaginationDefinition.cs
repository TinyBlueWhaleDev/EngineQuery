using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.Sql.Compilation.Models
{
    public sealed record QueryPaginationDefinition()
    {
        public int? Skip { get; init; }
        public int? Take { get; init; }
        public bool HasPagination =>
            Skip.HasValue || Take.HasValue;
    }
}
