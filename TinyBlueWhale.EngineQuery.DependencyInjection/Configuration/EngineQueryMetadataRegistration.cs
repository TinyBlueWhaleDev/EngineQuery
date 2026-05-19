using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.DependencyInjection.Enums;
using TinyBlueWhale.EngineQuery.Metadata.Interfaces;

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
