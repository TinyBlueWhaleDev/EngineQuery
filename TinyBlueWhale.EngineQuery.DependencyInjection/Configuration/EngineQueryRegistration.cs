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
        /// Gets the query compiler factory.
        /// </summary>
        public required Func<IServiceProvider, IQueryCompiler> BuildCompiler { get; init; }

        /// <summary>
        /// Gets the metadata resolver factory.
        /// </summary>
        public required Func<IServiceProvider, IEntityMetadataResolver> BuildMetadataResolver { get; init; }

        /// <summary>
        /// Creates a query compiler.
        /// </summary>
        public IQueryCompiler CreateCompiler(IServiceProvider serviceProvider)
        {
            return BuildCompiler(serviceProvider);
        }

        /// <summary>
        /// Creates a metadata resolver.
        /// </summary>
        public IEntityMetadataResolver CreateMetadataResolver(IServiceProvider serviceProvider)
        {
            return BuildMetadataResolver(serviceProvider);
        }
    }
}
