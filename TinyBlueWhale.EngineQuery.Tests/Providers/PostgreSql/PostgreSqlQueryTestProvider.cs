using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.PostgreSql.Capabilities;
using TinyBlueWhale.EngineQuery.PostgreSql.Compilation;
using TinyBlueWhale.EngineQuery.PostgreSql.Dialects;
using TinyBlueWhale.EngineQuery.Tests.Infrastructure;

namespace TinyBlueWhale.EngineQuery.Tests.Providers.PostgreSql
{
    /// <summary>
    /// Provides PostgreSQL-specific query builder infrastructure
    /// for shared query feature tests.
    /// </summary>
    internal sealed class PostgreSqlQueryTestProvider : IQueryTestProvider
    {
        /// <summary>
        /// Gets the provider name used by shared test infrastructure.
        /// </summary>
        public string ProviderName => "PostgreSql";

        /// <summary>
        /// Creates a query builder configured for PostgreSQL.
        /// </summary>
        /// <returns>
        /// Query builder configured with PostgreSQL compilation components
        /// and the shared test metadata resolver.
        /// </returns>
        public QueryBuilder CreateQueryBuilder()
        {
            return new QueryBuilder(
                new PostgreSqlQueryCompiler(
                    new PostgreSqlDatabaseDialect(),
                    new PostgreSqlProviderCapabilities()),
                TestMetadataFactory.CreateMetadataResolver());
        }

        /// <summary>
        /// Creates a query builder configured with the specified
        /// PostgreSQL provider capabilities.
        /// </summary>
        /// <param name="capabilities">
        /// Provider capabilities used by the PostgreSQL query compiler.
        /// </param>
        /// <returns>
        /// Query builder configured with the supplied capabilities.
        /// </returns>
        public QueryBuilder CreateQueryBuilder(IDatabaseProviderCapabilities capabilities)
        {
            ArgumentNullException.ThrowIfNull(capabilities);

            return new QueryBuilder(
                new PostgreSqlQueryCompiler(
                    new PostgreSqlDatabaseDialect(),
                    capabilities),
                TestMetadataFactory.CreateMetadataResolver());
        }


        /// <summary>
        /// Returns the provider name for readable NUnit fixture output.
        /// </summary>
        /// <returns>
        /// PostgreSQL provider name.
        /// </returns>
        public override string ToString()
        {
            return ProviderName;
        }
    }
}
