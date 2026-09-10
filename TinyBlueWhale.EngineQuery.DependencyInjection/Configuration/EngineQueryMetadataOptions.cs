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
        /// Registers fluent metadata resolution.
        /// </summary>
        /// <param name="metadataResolverFactory">
        /// Factory used to create the fluent entity metadata resolver.
        /// </param>
        /// <returns>
        /// Current metadata options instance.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="metadataResolverFactory"/> is null.
        /// </exception>
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
        /// Registers attribute-based metadata resolution.
        /// </summary>
        /// <returns>
        /// Current metadata options instance.
        /// </returns>
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
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="registration"/> is null.
        /// </exception>
        internal void AddRegistration(EngineQueryMetadataRegistration registration)
        {
            ArgumentNullException.ThrowIfNull(registration);

            _registrations.Add(registration);
        }
    }
}
