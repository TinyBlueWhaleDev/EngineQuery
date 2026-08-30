using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators
{

    /// <summary>
    /// Validates multi-argument scalar SQL function projections across providers.
    /// </summary>
    public static class MultiArgumentScalarFunctionQueryValidator
    {
        /// <summary>
        /// Runs the validator.
        /// </summary>
        public static void Run()
        {
            var metadataResolver = ProviderMetadataFactory.CreateJoinMetadataResolver();

            ProviderQueryPrinter.Print(
                "SQL Server Multi-Argument Scalar Functions",
                BuildQuery(ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL Multi-Argument Scalar Functions",
                BuildQuery(ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL Multi-Argument Scalar Functions",
                BuildQuery(ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));
        }

        // Builds a query with multi-argument scalar SQL function projections.
        // Builds a query with multi-argument scalar SQL function projections.
        private static GeneratedSqlQuery BuildQuery(
            QueryBuilder queryBuilder)
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id
                })
                .SelectScalarFunction<JoinUser>(
    QueryScalarFunction.Coalesce,
    u => new object[]
    {
        u.Email,
        "NO_EMAIL"
    },
    alias: "SafeEmail")
.SelectScalarFunction<JoinUser>(
    QueryScalarFunction.Concat,
    u => new object[]
    {
        u.Email,
        " - ACTIVE"
    },
    alias: "EmailLabel")
                .Build();
        }
    }
}
