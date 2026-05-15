using TinyBlueWhale.EngineQuery.Core.QueryBuilding;
using TinyBlueWhale.EngineQuery.SqlServer.Compilation;
using TinyBlueWhale.EngineQuery.SqlServer.Dialects;
using TinyBlueWhale.EngineQuery.Tests.Core;

namespace TinyBlueWhale.EngineQuery.Tests.SqlServer
{
    [TestFixture]
    public sealed class SqlServerQueryCompilerTests : QueryCompilerTestBase
    {
        protected override IQueryCompilerExpectedSyntax ExpectedSql { get; } =
            new SqlServerExpectedSqlSyntax();

        protected override QueryBuilder CreateQueryBuilder()
        {
            return new QueryBuilder(
                new SqlServerQueryCompiler(new SqlServerDatabaseDialect(), new EngineQuery.SqlServer.Capabilities.SqlServerProviderCapabilities()));
        }
    }
}
