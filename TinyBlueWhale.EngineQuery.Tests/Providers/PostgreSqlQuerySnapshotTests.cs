using TinyBlueWhale.EngineQuery.Core.QueryBuilding;
using TinyBlueWhale.EngineQuery.PostgreSql.Compilation;
using TinyBlueWhale.EngineQuery.PostgreSql.Dialects;
using TinyBlueWhale.EngineQuery.Tests.Infrastructure;

namespace TinyBlueWhale.EngineQuery.Tests.Providers
{    

    /// <summary>
    /// Runs PostgreSQL query compiler snapshot tests.
    /// </summary>
    [TestFixture]
    public sealed class PostgreSqlQuerySnapshotTests : QueryCompilerFeatureSnapshotTests
    {
        protected override string ProviderName => "PostgreSql";

        protected override QueryBuilder CreateQueryBuilder()
        {
            return new QueryBuilder(
                new PostgreSqlQueryCompiler(new PostgreSqlDatabaseDialect(), new PostgreSqlServer.Capabilities.PostgreSqlProviderCapabilities()),
                TestMetadataFactory.CreateMetadataResolver());
        }
    }
}
