using TinyBlueWhale.EngineQuery.Core.QueryBuilding;
using TinyBlueWhale.EngineQuery.MySql.Compilation;
using TinyBlueWhale.EngineQuery.MySql.Dialects;
using TinyBlueWhale.EngineQuery.MySqlServer.Capabilities;
using TinyBlueWhale.EngineQuery.Tests.Infrastructure;

namespace TinyBlueWhale.EngineQuery.Tests.Providers.MySql
{    

    /// <summary>
    /// Runs MySQL edge-case snapshot tests.
    /// </summary>
    [TestFixture]
    public sealed class MySqlQueryEdgeSnapshotTests : QueryCompilerEdgeSnapshotTests
    {
        /// <inheritdoc />
        protected override string ProviderName => "MySql";

        /// <inheritdoc />
        protected override QueryBuilder CreateQueryBuilder()
        {
            return new QueryBuilder(
                new MySqlQueryCompiler(new MySqlDatabaseDialect(), new MySqlProviderCapabilities()),
                TestMetadataFactory.CreateMetadataResolver());
        }
    }
}
