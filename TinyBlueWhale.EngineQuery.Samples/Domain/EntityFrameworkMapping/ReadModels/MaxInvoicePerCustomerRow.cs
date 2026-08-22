namespace TinyBlueWhale.EngineQuery.Samples.Domain.EntityFrameworkMapping.ReadModels
{
    public sealed class MaxInvoicePerCustomerRow
    {
        public int CustomerId { get; set; }
        public decimal MaxInvoiceTotal { get; set; }
    }
}
