namespace TinyBlueWhale.EngineQuery.Samples.Domain.FluentMapping
{
    public sealed class InvoiceFluent
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public string InvoiceNumber { get; set; } = string.Empty;

        public decimal Total { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
