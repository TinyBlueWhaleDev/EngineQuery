using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.Abstractions.Paging
{
    public sealed record QueryPagedResult<T>
    {
        public required IReadOnlyList<T> Items { get; init; }
        public required QueryPaginationMetadata Pagination { get; init; }
    }
}
