using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Core.Interfaces;
using TinyBlueWhale.EngineQuery.DependencyInjection.Enums;
using TinyBlueWhale.EngineQuery.Metadata.Interfaces;
using TinyBlueWhale.EngineQuery.Metadata.Models;

namespace TinyBlueWhale.EngineQuery.DependencyInjection.Configuration
{
    /// <summary>
    /// Represents a configured EngineQuery provider and metadata strategy.
    /// </summary>
    internal sealed class EngineQueryRegistration
    {
        /// <summary>
        /// Gets the configured provider.
        /// </summary>
        public required QueryEngineProvider Provider { get; init; }

        /// <summary>
        /// Gets the configured metadata strategy.
        /// </summary>
        public required MetadataStrategy? MetadataStrategy { get; init; }

        /// <summary>
        /// Gets the provider profile contract associated with the registration.
        /// </summary>
        /// <remarks>
        /// The contract identifies the provider family supported by the registration
        /// and is used to resolve compatible provider profiles.
        /// </remarks>
        public required Type ProfileContract { get; init; }

        /// <summary>
        /// Gets the query compiler factory.
        /// </summary>
        /// <remarks>
        /// The selected database provider profile is supplied to the factory so
        /// version-specific capabilities and feature strategies can be resolved
        /// before the compiler is created.
        /// </remarks>
        public required Func<IServiceProvider, IDatabaseProviderProfile, IQueryCompiler> BuildCompiler { get; init; }

        /// <summary>
        /// Gets the metadata resolver factory.
        /// </summary>
        public required Func<IServiceProvider, IEntityMetadataResolver> BuildMetadataResolver { get; init; }

        /// <summary>
        /// Creates a query compiler using the specified database provider profile.
        /// </summary>
        /// <param name="serviceProvider">
        /// Service provider used to resolve compiler dependencies.
        /// </param>
        /// <param name="profile">
        /// Database provider profile used to configure version-specific compiler behavior.
        /// </param>
        /// <returns>
        /// Configured query compiler.
        /// </returns>
        public IQueryCompiler CreateCompiler(
            IServiceProvider serviceProvider,
            IDatabaseProviderProfile profile)
        {
            ArgumentNullException.ThrowIfNull(serviceProvider);
            ArgumentNullException.ThrowIfNull(profile);

            return BuildCompiler(serviceProvider, profile);
        }

        /// <summary>
        /// Creates the metadata resolver associated with the registration.
        /// </summary>
        /// <param name="serviceProvider">
        /// Service provider used to resolve metadata dependencies.
        /// </param>
        /// <returns>
        /// Configured entity metadata resolver.
        /// </returns>
        public IEntityMetadataResolver CreateMetadataResolver(IServiceProvider serviceProvider)
        {
            ArgumentNullException.ThrowIfNull(serviceProvider);

            return BuildMetadataResolver(serviceProvider);
        }
    }
}

