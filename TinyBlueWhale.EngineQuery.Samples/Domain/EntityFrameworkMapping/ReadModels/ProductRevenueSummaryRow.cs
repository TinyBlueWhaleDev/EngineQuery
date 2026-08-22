namespace TinyBlueWhale.EngineQuery.Samples.Domain.EntityFrameworkMapping.ReadModels
{
    public sealed class ProductRevenueSummaryRow
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public int UnitsSold { get; set; }
    }
}
