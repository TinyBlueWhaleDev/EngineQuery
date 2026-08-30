using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;

namespace TinyBlueWhale.EngineQuery.Abstractions.Interfaces
{
    /// <summary>
    /// Defines a query builder surface that supports query pagination.
    /// </summary>
    public interface IQueryPaginationBuilder<TProfile> : IQueryBuilder<TProfile>
        where TProfile : IDatabaseProviderProfile
    {
        new IQueryPaginationCommandBuilder<T, TProfile> From<T>();

        new IQueryPaginationCommandBuilder<T, TProfile> From<T>(string alias);

        new IQueryPaginationCommandBuilder<T, TProfile> From<T>(string tableName, string alias);
    }
}
