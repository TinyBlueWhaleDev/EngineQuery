using System.ComponentModel.DataAnnotations.Schema;

namespace TinyBlueWhale.EngineQuery.Samples.Domain.AttributeMapping
{
    [Table("invoices")]
    public sealed class InvoiceAttribute
    {
        [Column("invoice_id")]
        public int Id { get; set; }

        [Column("customer_id")]
        public int CustomerId { get; set; }

        [Column("invoice_number")]
        public string InvoiceNumber { get; set; } = string.Empty;

        [Column("total")]
        public decimal Total { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
