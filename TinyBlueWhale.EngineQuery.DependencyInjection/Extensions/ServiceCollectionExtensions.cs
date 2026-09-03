using Microsoft.Extensions.DependencyInjection;
using TinyBlueWhale.EngineQuery.DependencyInjection.Configuration;

namespace TinyBlueWhale.EngineQuery.DependencyInjection.Extensions
{

    /// <summary>
    /// Provides EngineQuery dependency injection registration extensions.
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers EngineQuery services, database providers and generated query engine factories.
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
        public static IServiceCollection AddEngineQuery(
            this IServiceCollection services,
            Action<EngineQueryOptions> configureOptions)
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

            RegisterGeneratedQueryEngineFactories(services);

            return services;
        }

        /// <summary>
        /// Registers strongly typed query engine factories and generated engine surfaces
        /// discovered from database provider profiles.
        /// </summary>
        /// <param name="services">
        /// Service collection where generated EngineQuery services are registered.
        /// </param>
        static partial void RegisterGeneratedQueryEngineFactories(
            IServiceCollection services);
    }
}
