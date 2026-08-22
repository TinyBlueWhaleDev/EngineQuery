namespace TinyBlueWhale.EngineQuery.Samples.Domain.EntityFrameworkMapping
{
    public sealed class ProductEf
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public decimal UnitPrice { get; set; }

        public bool IsActive { get; set; }
    }
}
