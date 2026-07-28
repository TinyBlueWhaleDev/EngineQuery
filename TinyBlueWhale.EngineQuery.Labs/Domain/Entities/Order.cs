using TinyBlueWhale.EngineQuery.Labs.Domain.Enums;

namespace TinyBlueWhale.EngineQuery.Labs.Domain.Entities
{
    public sealed class Order
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public OrderStatus Status { get; set; }
        public DateTime OrderDateUtc { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
