using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding;
using TinyBlueWhale.EngineQuery.SqlServer.Capabilities;
using TinyBlueWhale.EngineQuery.SqlServer.Compilation;
using TinyBlueWhale.EngineQuery.SqlServer.Dialects;
using TinyBlueWhale.EngineQuery.Tests.Infrastructure;

namespace TinyBlueWhale.EngineQuery.Tests.Providers.SqlServer
{    

    /// <summary>
    /// Runs SQL Server negative validation tests.
    /// </summary>
    [TestFixture]
    public sealed class SqlServerQueryNegativeTests : QueryCompilerNegativeTestBase
    {
        /// <inheritdoc />
        protected override string ProviderName => "SqlServer";

        /// <inheritdoc />
        protected override QueryBuilder CreateQueryBuilder()
        {
            return CreateQueryBuilder(
                new SqlServerProviderCapabilities());
        }

        /// <inheritdoc />
        protected override QueryBuilder CreateQueryBuilderWithoutWindowFunctions()
        {
            return CreateQueryBuilder(
                new UnsupportedWindowFunctionCapabilities());
        }

        /// <inheritdoc />
        protected override QueryBuilder CreateQueryBuilderWithoutLateralJoins()
        {
            return CreateQueryBuilder(
                new UnsupportedLateralJoinCapabilities());
        }

        /// <inheritdoc />
        protected override QueryBuilder CreateQueryBuilderWithoutSetOperations()
        {
            return CreateQueryBuilder(
                new UnsupportedSetOperationCapabilities());
        }

        /// <inheritdoc />
        protected override QueryBuilder CreateQueryBuilderWithoutRecursiveCte()
        {
            return CreateQueryBuilder(
                new UnsupportedRecursiveCteCapabilities());
        }

        private static QueryBuilder CreateQueryBuilder(IDatabaseProviderCapabilities capabilities)
        {
            return new QueryBuilder(
                new SqlServerQueryCompiler(
                    new SqlServerDatabaseDialect(),
                    capabilities),
                TestMetadataFactory.CreateMetadataResolver());
        }
    }
}
