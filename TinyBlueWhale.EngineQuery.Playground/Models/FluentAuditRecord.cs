using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.Playground.Models
{
    public sealed class FluentAuditRecord
    {
        public int AuditId { get; set; }
        public string Description { get; set; } = null!;
        public DateTime CreatedOn { get; set; }
        public bool Active { get; set; }
    }
}
