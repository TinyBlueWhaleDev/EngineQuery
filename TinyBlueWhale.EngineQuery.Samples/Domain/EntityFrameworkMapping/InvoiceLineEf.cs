namespace TinyBlueWhale.EngineQuery.Samples.Domain.EntityFrameworkMapping
{
    public sealed class InvoiceLineEf
    {
        public int Id { get; set; }

        public int InvoiceId { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal LineTotal { get; set; }
    }
}
