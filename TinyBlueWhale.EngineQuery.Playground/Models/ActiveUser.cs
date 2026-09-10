namespace TinyBlueWhale.EngineQuery.Playground.Models
{
    public sealed class ActiveUser
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
    }

    public sealed class ArchivedUser
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
    }
}
