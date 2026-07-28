namespace TinyBlueWhale.EngineQuery.Labs.Domain.Entities
{
    public sealed class Product
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Sku { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}
