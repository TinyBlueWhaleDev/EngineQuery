using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding;
using TinyBlueWhale.EngineQuery.MySql.Compilation;
using TinyBlueWhale.EngineQuery.MySql.Dialects;
using TinyBlueWhale.EngineQuery.Tests.Core;

namespace TinyBlueWhale.EngineQuery.Tests.MySql
{
    [TestFixture]
    public sealed class MySqlQueryCompilerTests : QueryCompilerTestBase
    {
        protected override IQueryCompilerExpectedSyntax ExpectedSql { get; } =
            new MySqlExpectedSqlSyntax();

        protected override QueryBuilder CreateQueryBuilder()
        {
            return new QueryBuilder(
                new MySqlQueryCompiler(
                    new MySqlDatabaseDialect()));
        }
    }
}
