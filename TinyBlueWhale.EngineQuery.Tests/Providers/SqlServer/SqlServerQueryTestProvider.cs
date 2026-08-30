using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.SqlServer.Capabilities;
using TinyBlueWhale.EngineQuery.SqlServer.Compilation;
using TinyBlueWhale.EngineQuery.SqlServer.Dialects;
using TinyBlueWhale.EngineQuery.Tests.Infrastructure;

namespace TinyBlueWhale.EngineQuery.Tests.Providers.SqlServer
{
    /// <summary>
    /// Provides SQL Server-specific query builder infrastructure
    /// for shared query feature tests.
    /// </summary>
    internal sealed class SqlServerQueryTestProvider : IQueryTestProvider
    {
        /// <summary>
        /// Gets the provider name used by shared test infrastructure.
        /// </summary>
        public string ProviderName => "SqlServer";

        /// <summary>
        /// Creates a query builder configured for SQL Server.
        /// </summary>
        /// <returns>
        /// Query builder configured with SQL Server compilation components
        /// and the shared test metadata resolver.
        /// </returns>
        public QueryBuilder CreateQueryBuilder()
        {
            return new QueryBuilder(
                new SqlServerQueryCompiler(
                    new SqlServerDatabaseDialect(),
                    new SqlServerProviderCapabilities()),
                TestMetadataFactory.CreateMetadataResolver());
        }

        /// <summary>
        /// Creates a query builder configured with the specified
        /// SQL Server provider capabilities.
        /// </summary>
        /// <param name="capabilities">
        /// Provider capabilities used by the SQL Server query compiler.
        /// </param>
        /// <returns>
        /// Query builder configured with the supplied capabilities.
        /// </returns>
        public QueryBuilder CreateQueryBuilder(IDatabaseProviderCapabilities capabilities)
        {
            ArgumentNullException.ThrowIfNull(capabilities);

            return new QueryBuilder(
                new SqlServerQueryCompiler(
                    new SqlServerDatabaseDialect(),
                    capabilities),
                TestMetadataFactory.CreateMetadataResolver());
        }

        /// <summary>
        /// Returns the provider name for readable NUnit fixture output.
        /// </summary>
        /// <returns>
        /// SQL Server provider name.
        /// </returns>
        public override string ToString()
        {
            return ProviderName;
        }
    }
}
