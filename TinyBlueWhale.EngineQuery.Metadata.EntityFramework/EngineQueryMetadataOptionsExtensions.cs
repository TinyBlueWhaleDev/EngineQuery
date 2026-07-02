using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            return metadata.UseMetadata(
                EntityFrameworkMetadataStrategies.EntityFramework,
                serviceProvider =>
                {
                    var dbContext = serviceProvider.GetRequiredService<TDbContext>();

                    return new EntityFrameworkMetadataResolver(dbContext.Model);
                });
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

            return metadata.UseMetadata(
                EntityFrameworkMetadataStrategies.EntityFramework,
                serviceProvider =>
                {
                    var dbContext = serviceProvider.GetRequiredService<TDbContext>();

                    return new EntityFrameworkMetadataResolver(
                        dbContext.Model,
                        options);
                });
        }
    }
}
