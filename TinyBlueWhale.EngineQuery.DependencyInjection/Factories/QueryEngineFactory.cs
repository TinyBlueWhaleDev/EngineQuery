using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding;
using TinyBlueWhale.EngineQuery.DependencyInjection.Configuration;
using TinyBlueWhale.EngineQuery.DependencyInjection.Interfaces;
using TinyBlueWhale.EngineQuery.Metadata.Models;

namespace TinyBlueWhale.EngineQuery.DependencyInjection.Factories
{

    /// <summary>
    /// Creates strongly typed EngineQuery instances associated with a database provider profile
    /// and its generated query engine surface.
    /// </summary>
    /// <typeparam name="TProfile">
    /// Database provider profile that determines provider version and available query features.
    /// </typeparam>
    /// <typeparam name="TEngine">
    /// Query engine surface generated for the specified database provider profile.
    /// </typeparam>
    internal sealed class QueryEngineFactory<TProfile, TEngine>(
        IServiceProvider serviceProvider,
        IEnumerable<EngineQueryRegistration> registrations,
        Func<QueryBuilder<TProfile>, TEngine> engineFactory) :
        IQueryEngineFactory<TProfile, TEngine>
        where TProfile : IDatabaseProviderProfile, new()
    {
        private readonly IServiceProvider _serviceProvider =
            serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

        private readonly IReadOnlyList<EngineQueryRegistration> _registrations =
            registrations?.ToList() ?? throw new ArgumentNullException(nameof(registrations));

        private readonly Func<QueryBuilder<TProfile>, TEngine> _engineFactory =
            engineFactory ?? throw new ArgumentNullException(nameof(engineFactory));

        /// <inheritdoc />
        public TEngine Create()
        {
            var registration = ResolveRegistration();
            var profile = new TProfile();

            return CreateEngine(registration, profile);
        }

        /// <inheritdoc />
        public TEngine Create(MetadataStrategy metadataStrategy)
        {
            var registration = ResolveRegistration(metadataStrategy);
            var profile = new TProfile();

            return CreateEngine(registration, profile);
        }

        /// <summary>
        /// Resolves the provider registration compatible with the current database provider profile
        /// and optional metadata strategy.
        /// </summary>
        /// <param name="metadataStrategy">
        /// Optional metadata strategy used to narrow the provider registration.
        /// When <see langword="null"/>, all registrations compatible with the profile are considered.
        /// </param>
        /// <returns>
        /// Provider registration compatible with the current database provider profile
        /// and metadata strategy.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when no compatible provider registration is configured or when multiple
        /// compatible registrations are found.
        /// </exception>
        private EngineQueryRegistration ResolveRegistration(
            MetadataStrategy? metadataStrategy = null)
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

        /// <summary>
        /// Creates the strongly typed query engine associated with the specified registration
        /// and database provider profile.
        /// </summary>
        /// <param name="registration">
        /// Provider registration used to create compiler and metadata dependencies.
        /// </param>
        /// <param name="profile">
        /// Database provider profile used to configure version-specific behavior.
        /// </param>
        /// <returns>
        /// Query engine exposing the generated feature surface associated with the provider profile.
        /// </returns>
        private TEngine CreateEngine(
            EngineQueryRegistration registration,
            TProfile profile)
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

            return _engineFactory(queryBuilder);
        }
    }
}
