namespace TinyBlueWhale.EngineQuery.Abstractions.Interfaces
{
    /// <summary>
    /// Represents the main entry point for creating strongly typed query builders.
    /// </summary>
    public interface IQueryBuilder
    {
        /// <summary>
        /// Creates a new query command builder for the specified entity type and table name.
        /// </summary>
        /// <typeparam name="T">
        /// Entity type used as the source of the query.
        /// </typeparam>
        /// <param name="tableName">
        /// Database table name associated with the query.
        /// </param>
        /// <returns>
        /// A fluent query command builder for composing and generating SQL queries.
        /// </returns>
        IQueryCommandBuilder<T> From<T>(string tableName);

        /// <summary>
        /// Creates a new query builder using resolved entity metadata.
        /// </summary>
        /// <typeparam name="T">
        /// Entity type used as the source of the query.
        /// </typeparam>
        /// <returns>
        /// Fluent query command builder.
        /// </returns>
        IQueryCommandBuilder<T> From<T>();
    }
}
