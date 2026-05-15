using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding;
using TinyBlueWhale.EngineQuery.PostgreSql.Compilation;
using TinyBlueWhale.EngineQuery.PostgreSql.Dialects;
using TinyBlueWhale.EngineQuery.PostgreSqlServer.Capabilities;
using TinyBlueWhale.EngineQuery.Tests.Infrastructure;

namespace TinyBlueWhale.EngineQuery.Tests.Providers.PostgreSql
{    

    /// <summary>
    /// Runs PostgreSQL negative validation tests.
    /// </summary>
    [TestFixture]
    public sealed class PostgreSqlQueryNegativeTests : QueryCompilerNegativeTestBase
    {
        /// <inheritdoc />
        protected override string ProviderName => "PostgreSql";

        /// <inheritdoc />
        protected override QueryBuilder CreateQueryBuilder()
        {
            return CreateQueryBuilder(
                new PostgreSqlProviderCapabilities());
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
                new PostgreSqlQueryCompiler(new PostgreSqlDatabaseDialect(), capabilities),
                TestMetadataFactory.CreateMetadataResolver());
        }
    }
}
