namespace TinyBlueWhale.EngineQuery.Samples.Domain.EntityFrameworkMapping.ReadModels
{
    public sealed class InvoiceTotalWithTaxRow
    {
        public int InvoiceId { get; set; }
        public decimal Total { get; set; }
        public decimal TotalWithTax { get; set; }
    }
}
