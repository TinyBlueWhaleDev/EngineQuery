namespace TinyBlueWhale.EngineQuery.Samples.Domain.FluentMapping
{
    public sealed class InvoiceLineFluent
    {
        public int Id { get; set; }

        public int InvoiceId { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal LineTotal { get; set; }
    }
}
