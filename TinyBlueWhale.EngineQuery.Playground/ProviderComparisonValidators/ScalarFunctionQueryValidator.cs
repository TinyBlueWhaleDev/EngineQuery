using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators
{    
    /// <summary>
    /// Validates scalar SQL function projections across providers.
    /// </summary>
    public static class ScalarFunctionQueryValidator
    {
        /// <summary>
        /// Runs the validator.
        /// </summary>
        public static void Run()
        {
            var metadataResolver = ProviderMetadataFactory.CreateJoinMetadataResolver();

            ProviderQueryPrinter.Print(
                "SQL Server Scalar Functions",
                BuildQuery(
                    ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL Scalar Functions",
                BuildQuery(
                    ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL Scalar Functions",
                BuildQuery(
                    ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));
        }

        // Builds a query with scalar SQL function projections.
        private static GeneratedSqlQuery BuildQuery(
            QueryBuilder queryBuilder)
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id
                })
                .SelectFunction<JoinUser>(
                    QueryScalarFunction.Upper,
                    u => u.Email,
                    alias: "NormalizedEmail")
                .SelectFunction<JoinUser>(
                    QueryScalarFunction.Length,
                    u => u.Email,
                    alias: "EmailLength")
                .SelectFunction<JoinUser>(
                    QueryScalarFunction.Trim,
                    u => u.Email,
                    alias: "TrimmedEmail")
                .Build();
        }
    }
}
