namespace TinyBlueWhale.EngineQuery.Samples.Domain.EntityFrameworkMapping.ReadModels
{
    public sealed class LatestInvoicePerCustomerRow
    {
        public int CustomerId { get; set; }
        public string FullName { get; set; } = string.Empty;
    }
}
