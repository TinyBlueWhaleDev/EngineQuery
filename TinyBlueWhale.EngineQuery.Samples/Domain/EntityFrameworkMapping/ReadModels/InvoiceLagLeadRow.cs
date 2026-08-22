namespace TinyBlueWhale.EngineQuery.Samples.Domain.EntityFrameworkMapping.ReadModels
{
    public sealed class InvoiceLagLeadRow
    {
        public int InvoiceId { get; set; }
        public int CustomerId { get; set; }
        public decimal Total { get; set; }
        public decimal? PreviousInvoiceTotal { get; set; }
        public decimal? NextInvoiceTotal { get; set; }
    }
}
