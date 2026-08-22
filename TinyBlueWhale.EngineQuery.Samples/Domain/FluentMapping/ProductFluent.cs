namespace TinyBlueWhale.EngineQuery.Samples.Domain.FluentMapping
{
    public sealed class ProductFluent
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public decimal UnitPrice { get; set; }

        public bool IsActive { get; set; }
    }
}
