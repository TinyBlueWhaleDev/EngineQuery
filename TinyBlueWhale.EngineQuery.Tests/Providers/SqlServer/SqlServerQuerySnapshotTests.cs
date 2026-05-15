using TinyBlueWhale.EngineQuery.Core.QueryBuilding;
using TinyBlueWhale.EngineQuery.SqlServer.Capabilities;
using TinyBlueWhale.EngineQuery.SqlServer.Compilation;
using TinyBlueWhale.EngineQuery.SqlServer.Dialects;
using TinyBlueWhale.EngineQuery.Tests.Infrastructure;

namespace TinyBlueWhale.EngineQuery.Tests.Providers.SqlServer
{    

    /// <summary>
    /// Runs SQL Server query compiler snapshot tests.
    /// </summary>
    [TestFixture]
    public sealed class SqlServerQuerySnapshotTests : QueryCompilerFeatureSnapshotTests
    {
        protected override string ProviderName => "SqlServer";

        protected override QueryBuilder CreateQueryBuilder()
        {
            return new QueryBuilder(
                new SqlServerQueryCompiler(new SqlServerDatabaseDialect(), new SqlServerProviderCapabilities()),
                TestMetadataFactory.CreateMetadataResolver());
        }
    }
}
