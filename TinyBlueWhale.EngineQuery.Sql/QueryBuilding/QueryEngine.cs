

using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Sql.Dialects.Interfaces;
using TinyBlueWhale.EngineQuery.Sql.Dialects.SqlServer;

namespace TinyBlueWhale.EngineQuery.Sql.QueryBuilding
{
    public sealed class QueryEngine : IQueryEngine
    {
        private readonly ISqlDatabaseDialect _databaseDialect;

        public QueryEngine()
            : this(new SqlServerDialect())
        {
        }

        public QueryEngine(ISqlDatabaseDialect databaseDialect)
        {
            _databaseDialect = databaseDialect;
        }

        public IQueryCommandBuilder<T> Query<T>()
        {
            return new QueryCommandBuilder<T>(_databaseDialect);
        }

    }
}
