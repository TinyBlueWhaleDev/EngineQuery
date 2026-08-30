using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Features;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;

namespace TinyBlueWhale.EngineQuery.DependencyInjection.Interfaces
{
    /// <summary>
    /// Represents a configured EngineQuery query builder whose provider profile supports pagination.
    /// </summary>
    /// <typeparam name="TProfile">
    /// Database provider profile associated with the query engine.
    /// </typeparam>
    public interface IQueryPaginationEngine<TProfile> :
        IQueryEngine<TProfile>,
        IQueryPaginationBuilder<TProfile>
        where TProfile : IDatabaseProviderProfile, IPaginationFeature
    {
    }
}
