using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding;
using TinyBlueWhale.EngineQuery.MySql.Capabilities;
using TinyBlueWhale.EngineQuery.MySql.Compilation;
using TinyBlueWhale.EngineQuery.MySql.Dialects;
using TinyBlueWhale.EngineQuery.Tests.Infrastructure;
using TinyBlueWhale.EngineQuery.Tests.Models;

namespace TinyBlueWhale.EngineQuery.Tests.Providers.MySql
{

    /// <summary>
    /// Runs MySQL query compiler snapshot tests.
    /// </summary>
    [TestFixture]
    public sealed class MySqlQuerySnapshotTests : QueryCompilerFeatureSnapshotTests
    {
        protected override string ProviderName => "MySql";

        protected override IInsertValuesCommandBuilder<JoinUser> ConfigureReturnIdentity(IInsertValuesCommandBuilder<JoinUser> commandBuilder)
        {
            return commandBuilder.ReturnIdentity();
        }

        protected override QueryBuilder CreateQueryBuilder()
        {
            return new QueryBuilder(
                new MySqlQueryCompiler(new MySqlDatabaseDialect(), new MySqlProviderCapabilities()),
                TestMetadataFactory.CreateMetadataResolver());
        }
    }
}
