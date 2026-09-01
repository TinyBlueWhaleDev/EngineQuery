using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Features;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Sql.Interfaces.ClauseStrategies;
using TinyBlueWhale.EngineQuery.SqlServer.Clauses.Pagination.Strategies;

namespace TinyBlueWhale.EngineQuery.SqlServer.Profiles
{
    /// <summary>
    /// Represents the EngineQuery provider profile for SQL Server 2012.
    /// </summary>
    /// <remarks>
    /// SQL Server 2012 introduces OFFSET/FETCH pagination support and therefore
    /// exposes the EngineQuery pagination feature.
    /// </remarks>
    public class SqlServer2012Profile : SqlServer2008Profile,
        IOffsetFetchPaginationFeature,
        IPaginationStrategyProvider
    {
        /// <inheritdoc />
        public override DatabaseProviderVersion Version { get; } = DatabaseProviderVersion.Create(11, 0);

        public IPaginationStrategy CreatePaginationStrategy()
        {
            return new SqlServer2012PaginationStrategy();
        }
    }
}
