using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding;

namespace TinyBlueWhale.EngineQuery.Tests.Infrastructure
{

    /// <summary>
    /// Provides provider-shared snapshot test infrastructure for query compiler tests.
    /// </summary>
    public abstract class QueryCompilerSnapshotTestBase
    {
        /// <summary>
        /// Gets the provider name used to resolve snapshot files.
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
