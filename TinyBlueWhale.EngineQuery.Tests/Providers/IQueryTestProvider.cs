using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;

namespace TinyBlueWhale.EngineQuery.Tests.Providers
{
    /// <summary>
    /// Defines provider-specific infrastructure used by shared query feature tests.
    /// </summary>
    internal interface IQueryTestProvider
    {
        /// <summary>
        /// Gets the provider name used to resolve provider-specific snapshots.
        /// </summary>
        string ProviderName { get; }

        /// <summary>
        /// Creates a query builder configured for the current database provider.
        /// </summary>
        /// <returns>
        /// Query builder configured with provider-specific SQL compilation components.
        /// </returns>
        QueryBuilder CreateQueryBuilder();

        /// <summary>
        /// Creates a query builder configured with the specified
        /// database provider capabilities.
        /// </summary>
        /// <param name="capabilities">
        /// Provider capabilities used by the query compiler.
        /// </param>
        /// <returns>
        /// Provider-specific query builder using the supplied capabilities.
        /// </returns>
        QueryBuilder CreateQueryBuilder(IDatabaseProviderCapabilities capabilities);
    }
}
