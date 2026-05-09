namespace TinyBlueWhale.EngineQuery.Abstractions.Interfaces
{
    /// <summary>
    /// Represents the main entry point for creating strongly typed query builders.
    /// </summary>
    public interface IQueryEngine
    {
        /// <summary>
        /// Creates a new query command builder for the specified entity type.
        /// </summary>
        /// <typeparam name="T">
        /// Entity type used as the source of the query.
        /// </typeparam>
        /// <returns>
        /// A fluent query command builder for composing SQL queries.
        /// </returns>
        IQueryCommandBuilder<T> Query<T>();
    }
}
