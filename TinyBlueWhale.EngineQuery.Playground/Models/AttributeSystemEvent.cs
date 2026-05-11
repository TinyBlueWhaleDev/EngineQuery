using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.Playground.Models
{
    [Table("system_logs")]
    public sealed class AttributeSystemEvent
    {
        [Column("log_id")]
        public int EventKey { get; set; }

        [Column("message_text")]
        public string EventMessage { get; set; } = null!;

        [Column("created_at")]
        public DateTime EventCreatedAt { get; set; }

        [Column("is_active")]
        public bool IsEnabled { get; set; }
    }
}
