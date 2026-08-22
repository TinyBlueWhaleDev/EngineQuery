using System.ComponentModel.DataAnnotations.Schema;

namespace TinyBlueWhale.EngineQuery.Samples.Domain.AttributeMapping
{
    [Table("customers")]
    public sealed class CustomerAttribute
    {
        [Column("customer_id")]
        public int Id { get; set; }

        [Column("email")]
        public string Email { get; set; } = string.Empty;

        [Column("full_name")]
        public string FullName { get; set; } = string.Empty;

        [Column("is_active")]
        public bool IsActive { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
