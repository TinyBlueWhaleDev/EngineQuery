using TinyBlueWhale.EngineQuery.Core.QueryBuilding;
using TinyBlueWhale.EngineQuery.PostgreSql.Capabilities;
using TinyBlueWhale.EngineQuery.PostgreSql.Compilation;
using TinyBlueWhale.EngineQuery.PostgreSql.Dialects;
using TinyBlueWhale.EngineQuery.Tests.Infrastructure;

namespace TinyBlueWhale.EngineQuery.Tests.Providers.PostgreSql
{

    /// <summary>
    /// Runs PostgreSQL edge-case snapshot tests.
    /// </summary>
    [TestFixture]
    public sealed class PostgreSqlQueryEdgeSnapshotTests : QueryCompilerEdgeSnapshotTests
    {
        /// <inheritdoc />
        protected override string ProviderName => "PostgreSql";

        /// <inheritdoc />
        protected override QueryBuilder CreateQueryBuilder()
        {
            return new QueryBuilder(
                new PostgreSqlQueryCompiler(new PostgreSqlDatabaseDialect(), new PostgreSqlProviderCapabilities()),
                TestMetadataFactory.CreateMetadataResolver());
        }
    }
}
