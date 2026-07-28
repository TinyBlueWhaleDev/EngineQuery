namespace TinyBlueWhale.EngineQuery.Labs.Infrastructure.Persistence.SqlServer;

public sealed class SqlServerOptions
{
    public const string SectionName = "SqlServer";
    public string ConnectionString { get; set; } = string.Empty;
}
