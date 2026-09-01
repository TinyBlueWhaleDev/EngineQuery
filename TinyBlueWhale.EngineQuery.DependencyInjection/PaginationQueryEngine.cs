using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Features;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding;
using TinyBlueWhale.EngineQuery.DependencyInjection.Interfaces;

namespace TinyBlueWhale.EngineQuery.DependencyInjection
{
    //internal sealed class PaginationQueryEngine<TProfile>(QueryBuilder<TProfile> queryBuilder) :
    //QueryEngine<TProfile>(queryBuilder),
    //IQueryPaginationEngine<TProfile>
    //where TProfile : IDatabaseProviderProfile, IPaginationFeature
    //{
    //    IQueryPaginationCommandBuilder<T, TProfile> IQueryPaginationBuilder<TProfile>.From<T>()
    //    {
    //        return ((IQueryPaginationBuilder<TProfile>)_innerQueryBuilder).From<T>();
    //    }

    //    IQueryPaginationCommandBuilder<T, TProfile> IQueryPaginationBuilder<TProfile>.From<T>(string alias)
    //    {
    //        return ((IQueryPaginationBuilder<TProfile>)_innerQueryBuilder).From<T>(alias);
    //    }

    //    IQueryPaginationCommandBuilder<T, TProfile> IQueryPaginationBuilder<TProfile>.From<T>(string tableName, string alias)
    //    {
    //        return ((IQueryPaginationBuilder<TProfile>)_innerQueryBuilder).From<T>(tableName, alias);
    //    }
    //}
}
