using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators
{

    /// <summary>
    /// Validates APPLY and provider-equivalent LATERAL join generation across providers.
    /// </summary>
    public static class ApplyLateralJoinQueryValidator
    {
        /// <summary>
        /// Runs the validator.
        /// </summary>
        public static void Run()
        {
            var metadataResolver = ProviderMetadataFactory.CreateJoinMetadataResolver();

            ProviderQueryPrinter.Print(
                "SQL Server CROSS APPLY",
                BuildCrossApplyQuery(ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL LATERAL JOIN",
                BuildCrossApplyQuery(ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL LATERAL JOIN",
                BuildCrossApplyQuery(ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "SQL Server OUTER APPLY",
                BuildOuterApplyQuery(ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL LEFT LATERAL JOIN",
                BuildOuterApplyQuery(ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL LEFT LATERAL JOIN",
                BuildOuterApplyQuery(ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));
        }

        // Builds a CROSS APPLY or provider-equivalent LATERAL join query.
        private static GeneratedSqlQuery BuildCrossApplyQuery(
            QueryBuilder queryBuilder)
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .CrossApply<JoinUser, JoinOrder>(
                    alias: "latest_order",
                    apply => apply
                        .Select<JoinOrder>(o => new
                        {
                            OrderId = o.Id,
                            o.UserId,
                            o.Total
                        })
                        .WhereComputed<JoinOrder, JoinUser>(
                            (o, u) => o.UserId == u.Id)
                        .OrderByDescending<JoinOrder>(o => o.Total)
                        .Take(1))
                .Build();
        }

        // Builds an OUTER APPLY or provider-equivalent LEFT LATERAL join query.
        private static GeneratedSqlQuery BuildOuterApplyQuery(
            QueryBuilder queryBuilder)
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .OuterApply<JoinUser, JoinOrder>(
                    alias: "latest_order",
                    apply => apply
                        .Select<JoinOrder>(o => new
                        {
                            OrderId = o.Id,
                            o.UserId,
                            o.Total
                        })
                        .WhereComputed<JoinOrder, JoinUser>(
                            (o, u) => o.UserId == u.Id)
                        .OrderByDescending<JoinOrder>(o => o.Total)
                        .Take(1))
                .Build();
        }
    }
}
