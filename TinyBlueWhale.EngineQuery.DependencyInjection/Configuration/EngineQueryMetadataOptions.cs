using TinyBlueWhale.EngineQuery.Metadata.Models;
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
        public EngineQueryMetadataOptions UseFluentMetadata(Func<FluentEntityMetadataResolver> metadataResolverFactory)
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
        /// Adds a supported metadata registration to the current metadata configuration.
        /// </summary>
        /// <param name="registration">
        /// Metadata registration to add.
        /// </param>
        internal void AddRegistration(EngineQueryMetadataRegistration registration)
        {
            ArgumentNullException.ThrowIfNull(registration);

            _registrations.Add(registration);
        }
    }
}
