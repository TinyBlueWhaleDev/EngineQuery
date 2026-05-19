using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.DependencyInjection.Enums;
using TinyBlueWhale.EngineQuery.Metadata.Interfaces;
using TinyBlueWhale.EngineQuery.Metadata.Resolvers;

namespace TinyBlueWhale.EngineQuery.DependencyInjection.Configuration
{        

    /// <summary>
    /// Configures EngineQuery metadata strategies.
    /// </summary>
    public sealed class EngineQueryMetadataOptions
    {
        private readonly List<EngineQueryMetadataRegistration> _registrations = [];

        /// <summary>
        /// Gets the configured metadata registrations.
        /// </summary>
        internal IReadOnlyList<EngineQueryMetadataRegistration> Registrations => _registrations;

        /// <summary>
        /// Registers fluent metadata.
        /// </summary>
        public EngineQueryMetadataOptions UseFluentMetadata(Func<IEntityMetadataResolver> metadataResolverFactory)
        {
            ArgumentNullException.ThrowIfNull(metadataResolverFactory);
            _registrations.Add(
                new EngineQueryMetadataRegistration
                {
                    Strategy = MetadataStrategy.Fluent,
                    BuildMetadataResolver = _ => metadataResolverFactory()
                });
            return this;
        }

        /// <summary>
        /// Registers attribute metadata.
        /// </summary>
        public EngineQueryMetadataOptions UseAttributeMetadata()
        {
            _registrations.Add(
                new EngineQueryMetadataRegistration
                {
                    Strategy = MetadataStrategy.Attribute,
                    BuildMetadataResolver = _ => new AttributeEntityMetadataResolver()
                });
            return this;
        }

        /// <summary>
        /// Registers metadata using a service provider based factory.
        /// </summary>
        public EngineQueryMetadataOptions UseMetadata(
            MetadataStrategy strategy,
            Func<IServiceProvider, IEntityMetadataResolver> metadataResolverFactory)
        {
            ArgumentNullException.ThrowIfNull(metadataResolverFactory);
            _registrations.Add(
                new EngineQueryMetadataRegistration
                {
                    Strategy = strategy,
                    BuildMetadataResolver = metadataResolverFactory
                });
            return this;
        }
    }
}
