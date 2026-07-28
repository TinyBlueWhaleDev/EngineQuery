using System.Data.Common;

namespace TinyBlueWhale.EngineQuery.Labs.Infrastructure.Persistence.SqlServer;

public interface ISqlConnectionFactory
{
    Task<DbConnection> OpenConnectionAsync(CancellationToken cancellationToken = default);
}
