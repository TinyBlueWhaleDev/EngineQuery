namespace TinyBlueWhale.EngineQuery.Labs.Domain.Entities
{
    public sealed class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}
