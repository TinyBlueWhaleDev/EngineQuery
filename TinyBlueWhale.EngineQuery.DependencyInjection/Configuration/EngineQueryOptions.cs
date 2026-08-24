using TinyBlueWhale.EngineQuery.Core.Interfaces;
using TinyBlueWhale.EngineQuery.DependencyInjection.Enums;
using TinyBlueWhale.EngineQuery.Metadata.Resolvers;
using TinyBlueWhale.EngineQuery.MySql.Capabilities;
using TinyBlueWhale.EngineQuery.MySql.Compilation;
using TinyBlueWhale.EngineQuery.MySql.Dialects;
using TinyBlueWhale.EngineQuery.PostgreSql.Capabilities;
using TinyBlueWhale.EngineQuery.PostgreSql.Compilation;
using TinyBlueWhale.EngineQuery.PostgreSql.Dialects;
using TinyBlueWhale.EngineQuery.SqlServer.Capabilities;
using TinyBlueWhale.EngineQuery.SqlServer.Compilation;
using TinyBlueWhale.EngineQuery.SqlServer.Dialects;

namespace TinyBlueWhale.EngineQuery.DependencyInjection.Configuration
{

    /// <summary>
    /// Configures EngineQuery providers.
    /// </summary>
    public sealed class EngineQueryOptions
    {
        private readonly List<EngineQueryRegistration> _registrations = [];

        /// <summary>
        /// Gets the configured registrations.
        /// </summary>
        internal IReadOnlyList<EngineQueryRegistration> Registrations => _registrations;

        /// <summary>
        /// Registers a provider using convention-based metadata resolution.
        /// </summary>
        /// <param name="provider">
        /// Query engine provider to register.
        /// </param>
        /// <returns>
        /// Current EngineQuery options instance.
        /// </returns>
        public EngineQueryOptions Add(QueryEngineProvider provider)
        {
            _registrations.Add(
                new EngineQueryRegistration
                {
                    Provider = provider,
                    MetadataStrategy = null,
                    BuildCompiler = BuildCompilerFactory(provider),
                    BuildMetadataResolver = _ => new ConventionEntityMetadataResolver()
                });

            return this;
        }

        /// <summary>
        /// Registers a provider using metadata options.
        /// </summary>
        public EngineQueryOptions Add(QueryEngineProvider provider, Action<EngineQueryMetadataOptions> configureMetadata)
        {
            ArgumentNullException.ThrowIfNull(configureMetadata);

            var metadataOptions = new EngineQueryMetadataOptions();
            configureMetadata(metadataOptions);

            if (metadataOptions.Registrations.Count == 0)
                throw new InvalidOperationException("At least one metadata strategy must be configured.");

            foreach (var metadataRegistration in metadataOptions.Registrations)
            {
                _registrations.Add(
                    new EngineQueryRegistration
                    {
                        Provider = provider,
                        MetadataStrategy = metadataRegistration.Strategy,
                        BuildCompiler = BuildCompilerFactory(provider),
                        BuildMetadataResolver = serviceProvider =>
                        {
                            var configuredResolver = metadataRegistration.BuildMetadataResolver(serviceProvider);

                            return new CompositeEntityMetadataResolver(
                            [
                                configuredResolver,
                                new ConventionEntityMetadataResolver()
                            ]);
                        }
                    });
            }
            return this;
        }

        private static Func<IServiceProvider, IQueryCompiler> BuildCompilerFactory(QueryEngineProvider provider)
        {
            return provider switch
            {
                QueryEngineProvider.SqlServer => _ => new SqlServerQueryCompiler(
                    new SqlServerDatabaseDialect(),
                    new SqlServerProviderCapabilities()),
                QueryEngineProvider.MySql => _ => new MySqlQueryCompiler(
                    new MySqlDatabaseDialect(),
                    new MySqlProviderCapabilities()),
                QueryEngineProvider.PostgreSql => _ => new PostgreSqlQueryCompiler(
                    new PostgreSqlDatabaseDialect(),
                    new PostgreSqlProviderCapabilities()),
                _ => throw new NotSupportedException($"Provider '{provider}' is not supported.")
            };
        }
    }
}
