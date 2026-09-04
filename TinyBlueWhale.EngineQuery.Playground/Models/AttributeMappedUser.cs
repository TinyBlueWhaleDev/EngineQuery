using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.Playground.Models
{
    /// <summary>
    /// Represents a user entity mapped through metadata attributes.
    /// </summary>
    [Table("attribute_users")]
    public sealed class AttributeMappedUser
    {
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        [Column("attribute_user_id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the user email address.
        /// </summary>
        [Column("email_address")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets whether the user is active.
        /// </summary>
        [Column("active_flag")]
        public bool IsActive { get; set; }
    }
}
