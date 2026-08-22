namespace TinyBlueWhale.EngineQuery.Playground.Models
{
    public sealed class CompositeSecurityLog
    {
        public int SecurityLogId { get; set; }
        public string SecurityMessage { get; set; } = null!;
        public DateTime SecurityCreatedAt { get; set; }
        public bool SecurityIsActive { get; set; }
    }
}
