using TinyBlueWhale.EngineQuery.Core.QueryBuilding;
using TinyBlueWhale.EngineQuery.SqlServer.Compilation;
using TinyBlueWhale.EngineQuery.SqlServer.Dialects;
using TinyBlueWhale.EngineQuery.Tests.Core;

namespace TinyBlueWhale.EngineQuery.Tests.Sql
{
    [TestFixture]
    public sealed class SqlServerQueryCompilerTests : QueryCompilerTestBase
    {
        protected override IQueryCompilerExpectedSyntax ExpectedSql { get; } =
            new SqlServerExpectedSqlSyntax();

        protected override QueryEngine CreateQueryEngine()
        {
            return new QueryEngine(
                new QuerySqlServerCompiler(new SqlServerDatabaseDialect()));
        }
    }
}
