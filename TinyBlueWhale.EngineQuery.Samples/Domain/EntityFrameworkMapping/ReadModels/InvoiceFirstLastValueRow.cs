namespace TinyBlueWhale.EngineQuery.Samples.Domain.EntityFrameworkMapping.ReadModels
{
    public sealed class InvoiceFirstLastValueRow
    {
        public int InvoiceId { get; set; }
        public int CustomerId { get; set; }
        public decimal Total { get; set; }
        public decimal FirstInvoiceTotal { get; set; }
        public decimal LastInvoiceTotal { get; set; }
    }
}
