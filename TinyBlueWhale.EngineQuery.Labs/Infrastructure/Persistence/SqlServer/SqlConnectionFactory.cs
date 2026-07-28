using System.Data.Common;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace TinyBlueWhale.EngineQuery.Labs.Infrastructure.Persistence.SqlServer;

public sealed class SqlConnectionFactory(IOptions<SqlServerOptions> options) : ISqlConnectionFactory
{
    private readonly string _connectionString = options.Value.ConnectionString;

    public async Task<DbConnection> OpenConnectionAsync(CancellationToken cancellationToken = default)
    {
        var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
        return connection;
    }
}
