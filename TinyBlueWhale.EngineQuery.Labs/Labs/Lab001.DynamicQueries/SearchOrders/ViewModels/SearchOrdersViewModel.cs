using TinyBlueWhale.EngineQuery.Labs.Domain.Enums;

namespace TinyBlueWhale.EngineQuery.Labs.Labs.Lab001.DynamicQueries.SearchOrders.ViewModels
{
    public sealed class SearchOrdersViewModel
    {
        public IReadOnlyCollection<SearchOrderItemViewModel> Items { get; init; } = [];
        public int Page { get; init; }
        public int PageSize { get; init; }
        public int TotalCount { get; init; }
        public int TotalPages =>
            PageSize <= 0
                ? 0
                : (int)Math.Ceiling(TotalCount / (double)PageSize);
    }

    public sealed class SearchOrderItemViewModel
    {
        public int OrderId { get; init; }
        public string OrderNumber { get; init; } = string.Empty;
        public DateTime OrderDateUtc { get; init; }
        public OrderStatus Status { get; init; }
        public decimal TotalAmount { get; init; }
        public int CustomerId { get; init; }
        public string CustomerFirstName { get; init; } = string.Empty;
        public string CustomerLastName { get; init; } = string.Empty;
        public string CustomerEmail { get; init; } = string.Empty;
        public string CustomerName =>$"{CustomerFirstName} {CustomerLastName}".Trim();
    }
}
