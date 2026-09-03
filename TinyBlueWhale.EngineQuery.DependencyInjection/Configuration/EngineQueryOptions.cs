using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Core.Enums;
using TinyBlueWhale.EngineQuery.Core.Interfaces;
using TinyBlueWhale.EngineQuery.Metadata.Resolvers;
using TinyBlueWhale.EngineQuery.MySql.Compilation;
using TinyBlueWhale.EngineQuery.MySql.Profiles.Interfaces;
using TinyBlueWhale.EngineQuery.PostgreSql.Compilation;
using TinyBlueWhale.EngineQuery.PostgreSql.Profiles.Interfaces;
using TinyBlueWhale.EngineQuery.SqlServer.Compilation;
using TinyBlueWhale.EngineQuery.SqlServer.Profiles.Interfaces;

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
            var (buildCompiler, profileContract) = BuildProviderComposition(provider);

            _registrations.Add(
                new EngineQueryRegistration
                {
                    ProfileContract = profileContract,
                    Provider = provider,
                    MetadataStrategy = null,
                    BuildCompiler = buildCompiler,
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

            var (buildCompiler, profileContract) = BuildProviderComposition(provider);

            foreach (var metadataRegistration in metadataOptions.Registrations)
            {
                _registrations.Add(
                    new EngineQueryRegistration
                    {
                        ProfileContract = profileContract,
                        Provider = provider,
                        MetadataStrategy = metadataRegistration.Strategy,
                        BuildCompiler = buildCompiler,
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

        /// <summary>
        /// Builds the compiler factory and profile contract associated with the specified provider.
        /// </summary>
        /// <param name="provider">
        /// Database provider used to resolve the provider-specific composition.
        /// </param>
        /// <returns>
        /// Compiler factory and profile contract associated with the specified provider.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// Thrown when the specified provider is not supported.
        /// </exception>
        private static (Func<IServiceProvider, IDatabaseProviderProfile, IQueryCompiler> BuildCompiler, Type ProfileContract) BuildProviderComposition(QueryEngineProvider provider)
        {
            return provider switch
            {
                QueryEngineProvider.SqlServer => (BuildSqlServerCompiler, typeof(ISqlServerProfile)),
                QueryEngineProvider.MySql => (BuildMySqlCompiler, typeof(IMySqlProfile)),
                QueryEngineProvider.PostgreSql => (BuildPostgreSqlCompiler, typeof(IPostgreSqlProfile)),
                _ => throw new NotSupportedException($"Provider '{provider}' is not supported.")
            };
        }

        private static IQueryCompiler BuildSqlServerCompiler(IServiceProvider _, IDatabaseProviderProfile profile) => SqlServerQueryCompiler.Factory.CreateCompiler((ISqlServerProfile)profile);
        private static IQueryCompiler BuildMySqlCompiler(IServiceProvider _, IDatabaseProviderProfile profile) => MySqlQueryCompiler.Factory.CreateCompiler((IMySqlProfile)profile);
        private static IQueryCompiler BuildPostgreSqlCompiler(IServiceProvider _, IDatabaseProviderProfile profile) => PostgreSqlQueryCompiler.Factory.CreateCompiler((IPostgreSqlProfile)profile);
    }
}
