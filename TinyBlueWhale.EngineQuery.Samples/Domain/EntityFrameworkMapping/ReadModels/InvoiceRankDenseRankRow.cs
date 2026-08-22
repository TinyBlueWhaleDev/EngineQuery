namespace TinyBlueWhale.EngineQuery.Samples.Domain.EntityFrameworkMapping.ReadModels
{
    public sealed class InvoiceRankDenseRankRow
    {
        public int InvoiceId { get; set; }
        public int CustomerId { get; set; }
        public decimal Total { get; set; }
        public long InvoiceRank { get; set; }
        public long DenseInvoiceRank { get; set; }
    }
}
