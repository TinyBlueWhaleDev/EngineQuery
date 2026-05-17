using TinyBlueWhale.EngineQuery.Metadata.EntityFramework.Resolvers;
using TinyBlueWhale.EngineQuery.Metadata.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace TinyBlueWhale.EngineQuery.Metadata.EntityFramework
{

    /// <summary>
    /// Provides dependency injection extensions for Entity Framework metadata integration.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers an Entity Framework metadata resolver for EngineQuery.
        /// </summary>
        /// <typeparam name="TDbContext">
        /// Entity Framework database context type.
        /// </typeparam>
        /// <param name="services">
        /// Service collection.
        /// </param>
        /// <returns>
        /// Service collection.
        /// </returns>
        public static IServiceCollection AddEngineQueryEntityFrameworkMetadata<TDbContext>(
            this IServiceCollection services)
            where TDbContext : DbContext
        {
            ArgumentNullException.ThrowIfNull(services);

            services.AddSingleton<IEntityMetadataResolver>(serviceProvider =>
            {
                var dbContext = serviceProvider.GetRequiredService<TDbContext>();

                return new EntityFrameworkMetadataResolver(
                    dbContext.Model);
            });

            return services;
        }

        /// <summary>
        /// Registers an Entity Framework metadata resolver for EngineQuery.
        /// </summary>
        /// <typeparam name="TDbContext">
        /// Entity Framework database context type.
        /// </typeparam>
        /// <param name="services">
        /// Service collection.
        /// </param>
        /// <param name="options">
        /// Resolver options.
        /// </param>
        /// <returns>
        /// Service collection.
        /// </returns>
        public static IServiceCollection AddEngineQueryEntityFrameworkMetadata<TDbContext>(
            this IServiceCollection services,
            EntityFrameworkMetadataResolverOptions options)
            where TDbContext : DbContext
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(options);

            services.AddSingleton<IEntityMetadataResolver>(serviceProvider =>
            {
                var dbContext = serviceProvider.GetRequiredService<TDbContext>();

                return new EntityFrameworkMetadataResolver(
                    dbContext.Model,
                    options);
            });

            return services;
        }
    }
}
