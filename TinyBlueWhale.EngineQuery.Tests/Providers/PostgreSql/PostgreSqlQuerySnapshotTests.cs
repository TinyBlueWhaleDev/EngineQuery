using TinyBlueWhale.EngineQuery.Core.QueryBuilding;
using TinyBlueWhale.EngineQuery.PostgreSql.Capabilities;
using TinyBlueWhale.EngineQuery.PostgreSql.Compilation;
using TinyBlueWhale.EngineQuery.PostgreSql.Dialects;
using TinyBlueWhale.EngineQuery.Tests.Infrastructure;

namespace TinyBlueWhale.EngineQuery.Tests.Providers.PostgreSql
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
                new PostgreSqlQueryCompiler(new PostgreSqlDatabaseDialect(), new PostgreSqlProviderCapabilities()),
                TestMetadataFactory.CreateMetadataResolver());
        }
    }
}
