namespace TinyBlueWhale.EngineQuery.Labs.Infrastructure.Persistence.FluentMappings;

public sealed class OrderSearchText
{
    public int OrderId { get; set; }
    public string SearchText { get; set; } = string.Empty;
}
