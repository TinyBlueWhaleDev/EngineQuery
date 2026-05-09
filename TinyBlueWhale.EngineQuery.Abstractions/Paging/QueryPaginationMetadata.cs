using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.Abstractions.Paging
{
    public sealed record QueryPaginationMetadata
    {
        public required int Page { get; init; }
        public required int PageSize { get; init; }
        public required long TotalItems { get; init; }
        public int TotalPages =>PageSize <= 0 ? 0 : (int)Math.Ceiling((double)TotalItems / PageSize);
        public bool HasPreviousPage => Page > 1;
        public bool HasNextPage => Page < TotalPages;
    }
}
