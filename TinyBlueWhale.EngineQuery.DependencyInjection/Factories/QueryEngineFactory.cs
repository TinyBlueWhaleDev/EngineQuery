using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding;
using TinyBlueWhale.EngineQuery.DependencyInjection.Configuration;
using TinyBlueWhale.EngineQuery.DependencyInjection.Interfaces;
using TinyBlueWhale.EngineQuery.Metadata.Models;

namespace TinyBlueWhale.EngineQuery.DependencyInjection.Factories
{

    /// <summary>
    /// Default EngineQuery factory implementation.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="QueryEngineFactory"/> class.
    /// </remarks>
    /// <summary>
    /// Creates configured EngineQuery instances.
    /// </summary>
    internal sealed partial class QueryEngineFactory(
        IServiceProvider serviceProvider,
        IEnumerable<EngineQueryRegistration> registrations) : IQueryEngineFactory
    {
        private readonly IServiceProvider _serviceProvider =
            serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

        private readonly IReadOnlyList<EngineQueryRegistration> _registrations =
            registrations?.ToList() ?? throw new ArgumentNullException(nameof(registrations));

        /// <inheritdoc />
        public IQueryEngine<TProfile> Create<TProfile>()
            where TProfile : IDatabaseProviderProfile, new()
        {
            var registration = ResolveRegistration<TProfile>();
            var profile = new TProfile();

            return CreateEngine(registration, profile);
        }

        /// <inheritdoc />
        public IQueryEngine<TProfile> Create<TProfile>(MetadataStrategy metadataStrategy)
            where TProfile : IDatabaseProviderProfile, new()
        {
            var registration = ResolveRegistration<TProfile>(metadataStrategy);
            var profile = new TProfile();

            return CreateEngine(registration, profile);
        }

        /// <summary>
        /// Creates a query engine using the specified provider registration and database provider profile.
        /// </summary>
        /// <typeparam name="TProfile">
        /// Database provider profile associated with the query engine.
        /// </typeparam>
        /// <param name="registration">
        /// Provider registration used to create the query engine dependencies.
        /// </param>
        /// <param name="profile">
        /// Database provider profile used to configure version-specific behavior.
        /// </param>
        /// <returns>
        /// Configured query engine.
        /// </returns>
        private IQueryEngine<TProfile> CreateEngine<TProfile>(
            EngineQueryRegistration registration,
            TProfile profile)
            where TProfile : IDatabaseProviderProfile
        {
            ArgumentNullException.ThrowIfNull(registration);
            ArgumentNullException.ThrowIfNull(profile);

            var queryCompiler = registration.CreateCompiler(
                _serviceProvider,
                profile);

            var metadataResolver = registration.CreateMetadataResolver(
                _serviceProvider);

            var queryBuilder = new QueryBuilder<TProfile>(
                queryCompiler,
                metadataResolver,
                profile);

            return new QueryEngine<TProfile>(queryBuilder);
        }

        /// <summary>
        /// Resolves the provider registration compatible with the specified database provider profile
        /// and optional metadata strategy.
        /// </summary>
        /// <typeparam name="TProfile">
        /// Database provider profile used to identify the compatible provider registration.
        /// </typeparam>
        /// <param name="metadataStrategy">
        /// Optional metadata strategy used to narrow the provider registration.
        /// When <see langword="null"/>, all registrations compatible with the profile are considered.
        /// </param>
        /// <returns>
        /// Provider registration compatible with the specified database provider profile
        /// and metadata strategy.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when no compatible provider registration is configured or when multiple
        /// compatible registrations are found.
        /// </exception>
        private EngineQueryRegistration ResolveRegistration<TProfile>(
            MetadataStrategy? metadataStrategy = null)
            where TProfile : IDatabaseProviderProfile
        {
            var matches = _registrations
                .Where(registration =>
                    registration.ProfileContract.IsAssignableFrom(typeof(TProfile)) &&
                    (metadataStrategy is null || registration.MetadataStrategy == metadataStrategy))
                .ToList();

            if (matches.Count == 0)
            {
                var metadataDescription = metadataStrategy is null
                    ? string.Empty
                    : $" with metadata strategy '{metadataStrategy}'";

                throw new InvalidOperationException($"No provider registration supports profile '{typeof(TProfile).Name}'{metadataDescription}.");
            }

            if (matches.Count > 1)
                throw new InvalidOperationException($"Multiple registrations support profile '{typeof(TProfile).Name}'. Specify a metadata strategy.");

            return matches[0];
        }
    }
}
