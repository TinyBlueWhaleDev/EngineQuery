

using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Sql.Dialects.Interfaces;
using TinyBlueWhale.EngineQuery.Sql.Dialects.SqlServer;

namespace TinyBlueWhale.EngineQuery.Sql.QueryBuilding
{
    /// <summary>
    /// Default implementation of the query engine responsible for creating query builders.
    /// </summary>
    /// <remarks>
    /// The query engine acts as the main entry point for composing strongly typed SQL queries.
    /// It does not execute queries or manage database connections.
    /// </remarks>
    public sealed class QueryEngine(ISqlDatabaseDialect databaseDialect) : IQueryEngine
    {
        private readonly ISqlDatabaseDialect _databaseDialect = databaseDialect;

        public QueryEngine()
            : this(new SqlServerDatabaseDialect())
        {
        }

        /// <summary>
        /// Creates a new fluent query command builder for the specified entity type.
        /// </summary>
        /// <typeparam name="T">
        /// Entity type used as the source of the query.
        /// </typeparam>
        /// <returns>
        /// Query command builder instance.
        /// </returns>
        public IQueryCommandBuilder<T> Query<T>()
        {
            return new QueryCommandBuilder<T>(_databaseDialect);
        }

    }
}
