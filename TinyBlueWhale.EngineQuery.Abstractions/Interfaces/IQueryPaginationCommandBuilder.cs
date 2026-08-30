using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;

namespace TinyBlueWhale.EngineQuery.Abstractions.Interfaces
{
    /// <summary>
    /// Defines pagination operations for strongly typed SQL query commands.
    /// </summary>
    /// <typeparam name="T">
    /// Entity type used as the source of the query.
    /// </typeparam>
    /// <typeparam name="TProfile">
    /// Database provider profile associated with the query.
    /// </typeparam>
    public interface IQueryPaginationCommandBuilder<T, TProfile> :
        IQueryCommandBuilder<T, TProfile>
        where TProfile : IDatabaseProviderProfile
    {
        /// <summary>
        /// Skips the specified number of rows when generating paginated SQL.
        /// </summary>
        IQueryPaginationCommandBuilder<T, TProfile> Skip(int count);

        /// <summary>
        /// Limits the number of rows returned by the generated SQL query.
        /// </summary>
        IQueryPaginationCommandBuilder<T, TProfile> Take(int count);
    }
}
