using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;

namespace TinyBlueWhale.EngineQuery.DependencyInjection.Interfaces
{
    /// <summary>
    /// Represents a configured EngineQuery query builder associated with a database provider profile.
    /// </summary>
    /// <typeparam name="TProfile">
    /// Database provider profile associated with the query engine.
    /// </typeparam>
    public interface IQueryEngine<TProfile> : IQueryBuilder<TProfile>
        where TProfile : IDatabaseProviderProfile
    {
    }
}
