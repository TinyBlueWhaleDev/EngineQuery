namespace TinyBlueWhale.EngineQuery.Playground.Models
{
    public class ExplicitLogEntry
    {
        public int LogIdentifier { get; set; }
        public string MessageContent { get; set; } = null!;
        public DateTime RegisteredAt { get; set; }
        public bool Enabled { get; set; }
    }
}
