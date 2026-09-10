namespace TinyBlueWhale.EngineQuery.Playground.Models
{  
    public sealed class LatestOrder
    { 
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public decimal Total { get; set; }
    }
}
