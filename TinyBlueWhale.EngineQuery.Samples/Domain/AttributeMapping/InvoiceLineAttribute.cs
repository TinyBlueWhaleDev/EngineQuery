using System.ComponentModel.DataAnnotations.Schema;

namespace TinyBlueWhale.EngineQuery.Samples.Domain.AttributeMapping
{
    [Table("invoice_lines")]
    public sealed class InvoiceLineAttribute
    {
        [Column("invoice_line_id")]
        public int Id { get; set; }

        [Column("invoice_id")]
        public int InvoiceId { get; set; }

        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        [Column("line_total")]
        public decimal LineTotal { get; set; }
    }
}
