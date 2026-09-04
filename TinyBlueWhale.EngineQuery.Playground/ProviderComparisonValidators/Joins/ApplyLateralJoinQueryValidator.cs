using TinyBlueWhale.EngineQuery.Abstractions.Extensions;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Features;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.MySql.Profiles;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;
using TinyBlueWhale.EngineQuery.PostgreSql.Profiles;
using TinyBlueWhale.EngineQuery.SqlServer.Profiles;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.Joins
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
                BuildSqlServerCrossApplyQuery(
                    ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL LATERAL JOIN",
                BuildPostgreSqlCrossApplyQuery(
                    ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL LATERAL JOIN",
                BuildMySqlCrossApplyQuery(
                    ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "SQL Server OUTER APPLY",
                BuildSqlServerOuterApplyQuery(
                    ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL LEFT LATERAL JOIN",
                BuildPostgreSqlOuterApplyQuery(
                    ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL LEFT LATERAL JOIN",
                BuildMySqlOuterApplyQuery(
                    ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));
        }

        /// <summary>
        /// Builds a SQL Server CROSS APPLY query.
        /// </summary>
        /// <param name="queryBuilder">
        /// SQL Server query builder.
        /// </param>
        /// <returns>
        /// Generated SQL query.
        /// </returns>
        private static GeneratedSqlQuery BuildSqlServerCrossApplyQuery(
            IQueryBuilder<SqlServer2012Profile> queryBuilder)
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
                    applyBuilder: apply => apply
                        .Select<JoinOrder>(o => new
                        {
                            OrderId = o.Id,
                            o.UserId,
                            o.Total
                        })
                        .WhereComputed<JoinOrder, JoinUser>(
                            (o, u) => o.UserId == u.Id)
                        .OrderByDescending<JoinOrder>(o => o.Total))
                .Build();
        }

        /// <summary>
        /// Builds a PostgreSQL LATERAL join query.
        /// </summary>
        /// <param name="queryBuilder">
        /// PostgreSQL query builder.
        /// </param>
        /// <returns>
        /// Generated SQL query.
        /// </returns>
        private static GeneratedSqlQuery BuildPostgreSqlCrossApplyQuery(
            IQueryBuilder<PostgreSql93Profile> queryBuilder)
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
                    applyBuilder: apply => apply
                        .Select<JoinOrder>(o => new
                        {
                            OrderId = o.Id,
                            o.UserId,
                            o.Total
                        })
                        .WhereComputed<JoinOrder, JoinUser>(
                            (o, u) => o.UserId == u.Id)
                        .OrderByDescending<JoinOrder>(o => o.Total))
                .Build();
        }

        /// <summary>
        /// Builds a MySQL LATERAL join query.
        /// </summary>
        /// <param name="queryBuilder">
        /// MySQL query builder.
        /// </param>
        /// <returns>
        /// Generated SQL query.
        /// </returns>
        private static GeneratedSqlQuery BuildMySqlCrossApplyQuery(
            IQueryBuilder<MySql8031Profile> queryBuilder)
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
                    applyBuilder: apply => apply
                        .Select<JoinOrder>(o => new
                        {
                            OrderId = o.Id,
                            o.UserId,
                            o.Total
                        })
                        .WhereComputed<JoinOrder, JoinUser>(
                            (o, u) => o.UserId == u.Id)
                        .OrderByDescending<JoinOrder>(o => o.Total))
                .Build();
        }

        /// <summary>
        /// Builds a SQL Server OUTER APPLY query.
        /// </summary>
        /// <param name="queryBuilder">
        /// SQL Server query builder.
        /// </param>
        /// <returns>
        /// Generated SQL query.
        /// </returns>
        private static GeneratedSqlQuery BuildSqlServerOuterApplyQuery(
            IQueryBuilder<SqlServer2012Profile> queryBuilder)
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
                    applyBuilder: apply => apply
                        .Select<JoinOrder>(o => new
                        {
                            OrderId = o.Id,
                            o.UserId,
                            o.Total
                        })
                        .WhereComputed<JoinOrder, JoinUser>(
                            (o, u) => o.UserId == u.Id)
                        .OrderByDescending<JoinOrder>(o => o.Total))
                .Build();
        }

        /// <summary>
        /// Builds a PostgreSQL LEFT LATERAL join query.
        /// </summary>
        /// <param name="queryBuilder">
        /// PostgreSQL query builder.
        /// </param>
        /// <returns>
        /// Generated SQL query.
        /// </returns>
        private static GeneratedSqlQuery BuildPostgreSqlOuterApplyQuery(
            IQueryBuilder<PostgreSql93Profile> queryBuilder)
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
                    applyBuilder: apply => apply
                        .Select<JoinOrder>(o => new
                        {
                            OrderId = o.Id,
                            o.UserId,
                            o.Total
                        })
                        .WhereComputed<JoinOrder, JoinUser>(
                            (o, u) => o.UserId == u.Id)
                        .OrderByDescending<JoinOrder>(o => o.Total))
                .Build();
        }

        /// <summary>
        /// Builds a MySQL LEFT LATERAL join query.
        /// </summary>
        /// <param name="queryBuilder">
        /// MySQL query builder.
        /// </param>
        /// <returns>
        /// Generated SQL query.
        /// </returns>
        private static GeneratedSqlQuery BuildMySqlOuterApplyQuery(
            IQueryBuilder<MySql8031Profile> queryBuilder)
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
                    applyBuilder: apply => apply
                        .Select<JoinOrder>(o => new
                        {
                            OrderId = o.Id,
                            o.UserId,
                            o.Total
                        })
                        .WhereComputed<JoinOrder, JoinUser>(
                            (o, u) => o.UserId == u.Id)
                        .OrderByDescending<JoinOrder>(o => o.Total))
                .Build();
        }
    }
}
