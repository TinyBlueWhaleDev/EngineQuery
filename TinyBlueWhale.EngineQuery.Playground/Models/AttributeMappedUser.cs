using System.ComponentModel.DataAnnotations.Schema;

namespace TinyBlueWhale.EngineQuery.Playground.Models
{    
    [Table("attribute_users")]
    public sealed class AttributeMappedUser
    { 
        [Column("attribute_user_id")]
        public int Id { get; set; }
     
        [Column("email_address")]
        public string Email { get; set; } = string.Empty;

        [Column("active_flag")]
        public bool IsActive { get; set; }
    }
}
