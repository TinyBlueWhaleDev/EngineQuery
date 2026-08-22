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
