using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TinyBlueWhale.EngineQuery.DependencyInjection.Configuration;
using TinyBlueWhale.EngineQuery.Metadata.EntityFramework.Models;
using TinyBlueWhale.EngineQuery.Metadata.EntityFramework.Resolvers;

namespace TinyBlueWhale.EngineQuery.Metadata.EntityFramework
{
    /// <summary>
    /// Provides EngineQuery metadata options extensions for Entity Framework.
    /// </summary>
    public static class EngineQueryMetadataOptionsExtensions
    {
        /// <summary>
        /// Registers Entity Framework metadata using the specified database context.
        /// </summary>
        /// <typeparam name="TDbContext">
        /// Entity Framework database context type.
        /// </typeparam>
        /// <param name="metadata">
        /// EngineQuery metadata options.
        /// </param>
        /// <returns>
        /// Current metadata options.
        /// </returns>
        public static EngineQueryMetadataOptions UseEntityFrameworkMetadata<TDbContext>(
            this EngineQueryMetadataOptions metadata)
            where TDbContext : DbContext
        {
            ArgumentNullException.ThrowIfNull(metadata);

            metadata.AddRegistration(new EngineQueryMetadataRegistration
            {
                Strategy = EntityFrameworkMetadataStrategies.EntityFramework,
                BuildMetadataResolver = serviceProvider =>
                {
                    var dbContext = serviceProvider.GetRequiredService<TDbContext>();

                    return new EntityFrameworkMetadataResolver(dbContext.Model);
                }
            });

            return metadata;
        }

        /// <summary>
        /// Registers Entity Framework metadata using the specified database context and resolver options.
        /// </summary>
        /// <typeparam name="TDbContext">
        /// Entity Framework database context type.
        /// </typeparam>
        /// <param name="metadata">
        /// EngineQuery metadata options.
        /// </param>
        /// <param name="options">
        /// Entity Framework metadata resolver options.
        /// </param>
        /// <returns>
        /// Current metadata options.
        /// </returns>
        public static EngineQueryMetadataOptions UseEntityFrameworkMetadata<TDbContext>(
            this EngineQueryMetadataOptions metadata,
            EntityFrameworkMetadataResolverOptions options)
            where TDbContext : DbContext
        {
            ArgumentNullException.ThrowIfNull(metadata);
            ArgumentNullException.ThrowIfNull(options);

            metadata.AddRegistration(new EngineQueryMetadataRegistration
            {
                Strategy = EntityFrameworkMetadataStrategies.EntityFramework,
                BuildMetadataResolver = serviceProvider =>
                {
                    var dbContext = serviceProvider.GetRequiredService<TDbContext>();

                    return new EntityFrameworkMetadataResolver(dbContext.Model, options);
                }
            });

            return metadata;

        }
    }
}
