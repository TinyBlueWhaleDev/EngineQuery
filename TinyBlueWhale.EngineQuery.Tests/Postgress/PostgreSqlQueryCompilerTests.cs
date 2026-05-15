using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding;
using TinyBlueWhale.EngineQuery.PostgreSql.Compilation;
using TinyBlueWhale.EngineQuery.PostgreSql.Dialects;
using TinyBlueWhale.EngineQuery.SqlServer.Compilation;
using TinyBlueWhale.EngineQuery.SqlServer.Dialects;
using TinyBlueWhale.EngineQuery.Tests.Core;

namespace TinyBlueWhale.EngineQuery.Tests.Postgress
{

    [TestFixture]
    public sealed class PostgreSqlQueryCompilerTests : QueryCompilerTestBase
    {
        protected override IQueryCompilerExpectedSyntax ExpectedSql { get; } =
            new PostgreSqlExpectedSqlSyntax();

        protected override QueryBuilder CreateQueryBuilder()
        {
            return new QueryBuilder(
                new PostgreSqlQueryCompiler(
                    new PostgreSqlDatabaseDialect(), new PostgreSqlServer.Capabilities.PostgreSqlProviderCapabilities()));
        }
    }
}
