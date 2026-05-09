using TinyBlueWhale.EngineQuery.Sql.Dialects.SqlServer;
using TinyBlueWhale.EngineQuery.Sql.QueryBuilding;
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
                new SqlServerDatabaseDialect());
        }
    }
}
