using System.ComponentModel.DataAnnotations.Schema;

namespace TinyBlueWhale.EngineQuery.Samples.Domain.AttributeMapping
{
    [Table("products")]
    public sealed class ProductAttribute
    {
        [Column("product_id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("unit_price")]
        public decimal UnitPrice { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; }
    }
}
