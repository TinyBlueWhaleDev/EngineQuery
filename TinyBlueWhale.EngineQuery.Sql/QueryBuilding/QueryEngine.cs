

using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Sql.Dialects.Interfaces;
using TinyBlueWhale.EngineQuery.Sql.Dialects.SqlServer;

namespace TinyBlueWhale.EngineQuery.Sql.QueryBuilding
{
    public sealed class QueryEngine(ISqlDatabaseDialect databaseDialect) : IQueryEngine
    {
        private readonly ISqlDatabaseDialect _databaseDialect = databaseDialect;

        public QueryEngine()
            : this(new SqlServerDatabaseDialect())
        {
        }

        public IQueryCommandBuilder<T> Query<T>()
        {
            return new QueryCommandBuilder<T>(_databaseDialect);
        }

    }
}
