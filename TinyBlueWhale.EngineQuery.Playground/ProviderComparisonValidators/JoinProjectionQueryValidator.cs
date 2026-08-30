using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators
{
    /// <summary>
    /// Validates multi-source SELECT projection generation across providers.
    /// </summary>
    public static class JoinProjectionQueryValidator
    {
        /// <summary>
        /// Runs the validator.
        /// </summary>
        public static void Run()
        {
            var metadataResolver = ProviderMetadataFactory.CreateJoinMetadataResolver();

            ProviderQueryPrinter.Print("SQL Server Join Projection", BuildQuery(ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));
            ProviderQueryPrinter.Print("PostgreSQL Join Projection", BuildQuery(ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));
            ProviderQueryPrinter.Print("MySQL Join Projection", BuildQuery(ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));
        }

        // Builds a joined query with projections from multiple sources.
        private static GeneratedSqlQuery BuildQuery(QueryBuilder queryBuilder)
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .InnerJoin<JoinUser, JoinOrder>(alias: "o", on: (u, o) => u.Id == o.UserId)
                .LeftJoin<JoinOrder, JoinOrderItem>(alias: "oi", on: (o, oi) => o.Id == oi.OrderId)
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .Select<JoinOrder>(o => new
                {
                    OrderId = o.Id,
                    OrderUserId = o.UserId,
                    o.Total
                })
                .Select<JoinOrderItem>(oi => new
                {
                    OrderItemId = oi.Id,
                    oi.Quantity
                })
                .Build();
        }
    }
}
