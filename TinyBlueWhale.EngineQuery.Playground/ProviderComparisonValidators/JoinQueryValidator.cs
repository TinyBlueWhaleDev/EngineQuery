using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;


namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators
{
    /// <summary>
    /// Validates JOIN generation across providers.
    /// </summary>
    public static class JoinQueryValidator
    {
        /// <summary>
        /// Runs the validator.
        /// </summary>
        public static void Run()
        {
            var metadataResolver = ProviderMetadataFactory.CreateJoinMetadataResolver();

            ProviderQueryPrinter.Print("SQL Server Join", BuildQuery(ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));
            ProviderQueryPrinter.Print("PostgreSQL Join", BuildQuery(ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));
            ProviderQueryPrinter.Print("MySQL Join", BuildQuery(ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));
        }

        // Builds a query with INNER JOIN and LEFT JOIN.
        private static GeneratedSqlQuery BuildQuery(QueryBuilder queryBuilder)
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .InnerJoin<JoinUser, JoinOrder>(alias: "o", on: (u, o) => u.Id == o.UserId)
                .LeftJoin<JoinOrder, JoinOrderItem>(alias: "oi", on: (o, oi) => o.Id == oi.OrderId)
                .Build();
        }
    }
}
