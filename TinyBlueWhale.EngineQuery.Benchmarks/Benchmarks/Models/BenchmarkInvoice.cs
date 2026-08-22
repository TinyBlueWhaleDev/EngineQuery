namespace TinyBlueWhale.EngineQuery.Benchmarks.Benchmarks.Models
{
    public sealed class BenchmarkInvoice
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
