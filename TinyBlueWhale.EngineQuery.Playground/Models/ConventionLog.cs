using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.Playground.Models
{
    public sealed class system_logs
    {
        public int log_id { get; set; }
        public string message_text { get; set; } = null!;
        public DateTime created_at { get; set; }
        public bool is_active { get; set; }
    }
}
