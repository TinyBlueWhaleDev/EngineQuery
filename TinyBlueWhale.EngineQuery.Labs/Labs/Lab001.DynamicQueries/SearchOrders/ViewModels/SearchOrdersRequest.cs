using TinyBlueWhale.EngineQuery.Labs.Domain.Enums;

namespace TinyBlueWhale.EngineQuery.Labs.Labs.Lab001.DynamicQueries.SearchOrders.ViewModels
{
    public sealed class SearchOrdersRequest
    {
        public string? Search { get; init; }
        public int? CustomerId { get; init; }
        public OrderStatus? Status { get; init; }
        public DateTime? CreatedFromUtc { get; init; }
        public DateTime? CreatedToUtc { get; init; }
        public decimal? MinimumTotal { get; init; }
        public decimal? MaximumTotal { get; init; }
        public int Page { get; init; } = 1;
        public int PageSize { get; init; } = 20;
        public string SortBy { get; init; } = "OrderDateUtc";
        public string SortDirection { get; init; } = "desc";
    }
}
