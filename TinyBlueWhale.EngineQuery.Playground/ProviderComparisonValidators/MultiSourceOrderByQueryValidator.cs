using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators
{
    /// <summary>
    /// Validates multi-source ORDER BY generation across providers.
    /// </summary>
    public static class MultiSourceOrderByQueryValidator
    {
        /// <summary>
        /// Runs the validator.
        /// </summary>
        public static void Run()
        {
            var metadataResolver = ProviderMetadataFactory.CreateJoinMetadataResolver();

            ProviderQueryPrinter.Print("SQL Server Multi-Source OrderBy", BuildQuery(ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));
            ProviderQueryPrinter.Print("PostgreSQL Multi-Source OrderBy", BuildQuery(ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));
            ProviderQueryPrinter.Print("MySQL Multi-Source OrderBy", BuildQuery(ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));
        }

        // Builds a joined query with grouped multi-source ordering.
        private static GeneratedSqlQuery BuildQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .InnerJoin<JoinUser, JoinOrder>(alias: "o", on: (u, o) => u.Id == o.UserId)
                .LeftJoin<JoinOrder, JoinOrderItem>(alias: "oi", on: (o, oi) => o.Id == oi.OrderId)
                .OrderByDescending<JoinOrder>(o => new
                {
                    o.Total,
                    o.UserId
                })
                .ThenBy<JoinUser>(u => u.Email)
                .ThenByDescending<JoinOrderItem>(oi => oi.Quantity)
                .Build();
        }
    }
}
