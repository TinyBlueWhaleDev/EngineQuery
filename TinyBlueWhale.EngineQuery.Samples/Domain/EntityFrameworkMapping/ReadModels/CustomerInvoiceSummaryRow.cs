namespace TinyBlueWhale.EngineQuery.Samples.Domain.EntityFrameworkMapping.ReadModels
{
    public sealed class CustomerInvoiceSummaryRow
    {
        public int CustomerId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public int InvoiceCount { get; set; }
    }
}
