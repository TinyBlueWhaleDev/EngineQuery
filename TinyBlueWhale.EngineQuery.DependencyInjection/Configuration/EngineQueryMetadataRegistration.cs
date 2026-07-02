using TinyBlueWhale.EngineQuery.Metadata.Interfaces;
using TinyBlueWhale.EngineQuery.Metadata.Models;

namespace TinyBlueWhale.EngineQuery.DependencyInjection.Configuration
{
    /// <summary>
    /// Represents a configured metadata resolver registration.
    /// </summary>
    internal sealed class EngineQueryMetadataRegistration
    {
        /// <summary>
        /// Gets the metadata strategy.
        /// </summary>
        public required MetadataStrategy Strategy { get; init; }

        /// <summary>
        /// Gets the metadata resolver factory.
        /// </summary>
        public required Func<IServiceProvider, IEntityMetadataResolver> BuildMetadataResolver { get; init; }
    }
}
