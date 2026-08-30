using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Metadata.Models;

namespace TinyBlueWhale.EngineQuery.DependencyInjection.Interfaces
{
    /// <summary>
    /// Creates configured EngineQuery instances based on database provider profiles.
    /// </summary>
    public interface IQueryEngineFactory<TProfile, out TEngine>
            where TProfile : IDatabaseProviderProfile, new()
    {
        /// <summary>
        /// Creates a query engine using the configured database provider profile.
        /// </summary>
        /// <returns>
        /// Query engine exposing the feature surface associated with the provider profile.
        /// </returns>
        TEngine Create();

        /// <summary>
        /// Creates a query engine using the configured database provider profile
        /// and metadata strategy.
        /// </summary>
        /// <param name="metadataStrategy">
        /// Metadata strategy used to resolve entity metadata.
        /// </param>
        /// <returns>
        /// Query engine exposing the feature surface associated with the provider profile.
        /// </returns>
        TEngine Create(MetadataStrategy metadataStrategy);
    }
}
