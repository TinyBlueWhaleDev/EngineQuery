using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding;


namespace TinyBlueWhale.EngineQuery.Tests.Infrastructure
{

    /// <summary>
    /// Provides shared provider-specific query compiler test infrastructure.
    /// </summary>
    public abstract class QueryCompilerProviderTestBase
    {
        /// <summary>
        /// Gets the provider name used to resolve provider-specific snapshots.
        /// </summary>
        protected abstract string ProviderName { get; }

        /// <summary>
        /// Creates a provider-specific query builder.
        /// </summary>
        protected abstract QueryBuilder CreateQueryBuilder();

        /// <summary>
        /// Asserts that the generated SQL query matches a provider-specific snapshot.
        /// </summary>
        protected void AssertSnapshot(string snapshotName, GeneratedSqlQuery sql)
        {
            QuerySnapshotAssert.Matches(ProviderName, snapshotName, sql);
        }
    }
}
