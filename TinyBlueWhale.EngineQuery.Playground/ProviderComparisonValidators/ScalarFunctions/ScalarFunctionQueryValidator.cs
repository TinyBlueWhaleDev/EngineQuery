using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.ScalarFunctions
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
        private static GeneratedSqlQuery BuildQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id
                })
                .SelectScalarFunction<JoinUser>(
                    QueryScalarFunction.Upper,
                    u => u.Email,
                    alias: "NormalizedEmail")
                .SelectScalarFunction<JoinUser>(
                    QueryScalarFunction.Length,
                    u => u.Email,
                    alias: "EmailLength")
                .SelectScalarFunction<JoinUser>(
                    QueryScalarFunction.Trim,
                    u => u.Email,
                    alias: "TrimmedEmail")
                .Build();
        }
    }
}
