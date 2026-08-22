using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators
{

    /// <summary>
    /// Validates multi-source computed WHERE expressions across providers.
    /// </summary>
    public static class MultiSourceComputedWhereQueryValidator
    {
        /// <summary>
        /// Runs the validator.
        /// </summary>
        public static void Run()
        {
            var metadataResolver = ProviderMetadataFactory.CreateJoinMetadataResolver();

            ProviderQueryPrinter.Print(
                "SQL Server Multi-Source Computed Where",
                BuildQuery(ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL Multi-Source Computed Where",
                BuildQuery(ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL Multi-Source Computed Where",
                BuildQuery(ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));
        }

        // Builds a joined query with a multi-source computed WHERE predicate.
        private static GeneratedSqlQuery BuildQuery(
     QueryBuilder queryBuilder)
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .InnerJoin<JoinUser, JoinOrder>(
                    alias: "o",
                    on: (u, o) => u.Id == o.UserId)
                .LeftJoin<JoinOrder, JoinOrderItem>(
                    alias: "oi",
                    on: (o, oi) => o.Id == oi.OrderId)
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .WhereComputed<JoinOrder, JoinUser>(
                    (o, u) => o.UserId == u.Id && o.Total > 10)
                .WhereComputed<JoinOrderItem, JoinOrder>(
                    (oi, o) => oi.OrderId == o.Id && oi.Quantity < 100)
                .Build();
        }
    }
}
