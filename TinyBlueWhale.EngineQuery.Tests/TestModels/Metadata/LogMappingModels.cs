using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.Tests.TestModels.Metadata
{
    public sealed class ExplicitLogEntry
    {
        public int LogIdentifier { get; set; }

        public string MessageContent { get; set; } = null!;

        public DateTime RegisteredAt { get; set; }

        public bool Enabled { get; set; }
    }

    public sealed class system_logs
    {
        public int log_id { get; set; }

        public string message_text { get; set; } = null!;

        public DateTime created_at { get; set; }

        public bool is_active { get; set; }
    }

    public sealed class FluentAuditRecord
    {
        public int AuditId { get; set; }

        public string Description { get; set; } = null!;

        public DateTime CreatedOn { get; set; }

        public bool Active { get; set; }
    }

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

    public sealed class CompositeSecurityLog
    {
        public int SecurityLogId { get; set; }

        public string SecurityMessage { get; set; } = null!;

        public DateTime SecurityCreatedAt { get; set; }

        public bool SecurityIsActive { get; set; }
    }
}
