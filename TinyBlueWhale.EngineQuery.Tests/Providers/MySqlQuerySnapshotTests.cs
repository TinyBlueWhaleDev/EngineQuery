using TinyBlueWhale.EngineQuery.Core.QueryBuilding;
using TinyBlueWhale.EngineQuery.MySql.Compilation;
using TinyBlueWhale.EngineQuery.MySql.Dialects;
using TinyBlueWhale.EngineQuery.Tests.Infrastructure;

namespace TinyBlueWhale.EngineQuery.Tests.Providers
{    

    /// <summary>
    /// Runs MySQL query compiler snapshot tests.
    /// </summary>
    [TestFixture]
    public sealed class MySqlQuerySnapshotTests : QueryCompilerFeatureSnapshotTests
    {
        protected override string ProviderName => "MySql";

        protected override QueryBuilder CreateQueryBuilder()
        {
            return new QueryBuilder(
                new MySqlQueryCompiler(new MySqlDatabaseDialect(), new MySqlServer.Capabilities.MySqlProviderCapabilities()),
                TestMetadataFactory.CreateMetadataResolver());
        }
    }
}
