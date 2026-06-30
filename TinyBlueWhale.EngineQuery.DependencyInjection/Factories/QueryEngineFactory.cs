using TinyBlueWhale.EngineQuery.Core.QueryBuilding;
using TinyBlueWhale.EngineQuery.DependencyInjection.Configuration;
using TinyBlueWhale.EngineQuery.DependencyInjection.Enums;
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
    internal sealed class QueryEngineFactory(
        IServiceProvider serviceProvider,
        IEnumerable<EngineQueryRegistration> registrations) : IQueryEngineFactory
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;
        private readonly IReadOnlyList<EngineQueryRegistration> _registrations = [.. registrations];

        /// <inheritdoc />
        public IQueryEngine Create(QueryEngineProvider provider)
        {
            var matches = _registrations
                .Where(registration => registration.Provider == provider)
                .ToList();

            if (matches.Count == 0)
                throw new InvalidOperationException($"Provider '{provider}' is not registered.");

            if (matches.Count > 1)
                throw new InvalidOperationException($"Multiple metadata strategies are registered for provider '{provider}'. Specify a metadata strategy.");
           
            return Create(matches[0]);
        }

        /// <inheritdoc />
        public IQueryEngine Create(QueryEngineProvider provider, MetadataStrategy metadataStrategy)
        {
            var registration = _registrations.SingleOrDefault(
                candidate =>
                    candidate.Provider == provider &&
                    candidate.MetadataStrategy == metadataStrategy);

            return registration is null
                ? throw new InvalidOperationException($"Provider '{provider}' with metadata strategy '{metadataStrategy}' is not registered.")
                : Create(registration);
        }

        private QueryEngine Create(EngineQueryRegistration registration)
        {
            var queryBuilder = new QueryBuilder(
                registration.CreateCompiler(_serviceProvider),
                registration.CreateMetadataResolver(_serviceProvider));
            
            return new QueryEngine(queryBuilder);
        }
    }
}
