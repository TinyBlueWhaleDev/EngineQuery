namespace TinyBlueWhale.EngineQuery.Benchmarks.Benchmarks.Models
{
    public sealed class BenchmarkCustomer
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
