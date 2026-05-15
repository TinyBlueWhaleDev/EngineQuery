using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding;
using TinyBlueWhale.EngineQuery.MySql.Capabilities;
using TinyBlueWhale.EngineQuery.MySql.Compilation;
using TinyBlueWhale.EngineQuery.MySql.Dialects;
using TinyBlueWhale.EngineQuery.Tests.Infrastructure;

namespace TinyBlueWhale.EngineQuery.Tests.Providers.MySql
{    

    /// <summary>
    /// Runs MySQL negative validation tests.
    /// </summary>
    [TestFixture]
    public sealed class MySqlQueryNegativeTests : QueryCompilerNegativeTestBase
    {
        /// <inheritdoc />
        protected override string ProviderName => "MySql";

        /// <inheritdoc />
        protected override QueryBuilder CreateQueryBuilder()
        {
            return CreateQueryBuilder(
                new MySqlProviderCapabilities());
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
                new MySqlQueryCompiler(new MySqlDatabaseDialect(), capabilities),
                TestMetadataFactory.CreateMetadataResolver());
        }
    }
}
