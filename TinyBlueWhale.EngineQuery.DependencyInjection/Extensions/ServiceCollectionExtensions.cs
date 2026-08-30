using Microsoft.Extensions.DependencyInjection;
using TinyBlueWhale.EngineQuery.DependencyInjection.Configuration;
using TinyBlueWhale.EngineQuery.DependencyInjection.Factories;
using TinyBlueWhale.EngineQuery.DependencyInjection.Interfaces;

namespace TinyBlueWhale.EngineQuery.DependencyInjection.Extensions
{

    /// <summary>
    /// Provides EngineQuery dependency injection registration extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers EngineQuery services.
        /// </summary>
        /// <param name="services">
        /// Service collection where EngineQuery dependencies are registered.
        /// </param>
        /// <param name="configureOptions">
        /// Action used to configure EngineQuery providers and metadata strategies.
        /// </param>
        /// <returns>
        /// Current service collection.
        /// </returns>
        public static IServiceCollection AddEngineQuery(this IServiceCollection services, Action<EngineQueryOptions> configureOptions)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(configureOptions);

            var options = new EngineQueryOptions();

            configureOptions(options);

            if (options.Registrations.Count == 0)
                throw new InvalidOperationException("At least one EngineQuery provider must be configured.");

            foreach (var registration in options.Registrations)
            {
                ArgumentNullException.ThrowIfNull(registration);

                services.AddSingleton(registration);
            }

            services.AddSingleton<IQueryEngineFactory, QueryEngineFactory>();

            return services;
        }
    }
}
