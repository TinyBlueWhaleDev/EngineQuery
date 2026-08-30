using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators
{
    /// <summary>
    /// Validates aggregate SELECT generation across providers.
    /// </summary>
    public static class AggregateQueryValidator
    {
        /// <summary>
        /// Runs the validator.
        /// </summary>
        public static void Run()
        {
            var metadataResolver = ProviderMetadataFactory.CreateJoinMetadataResolver();

            ProviderQueryPrinter.Print("SQL Server Aggregate", BuildQuery(ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));
            ProviderQueryPrinter.Print("PostgreSQL Aggregate", BuildQuery(ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));
            ProviderQueryPrinter.Print("MySQL Aggregate", BuildQuery(ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));
        }

        // Builds a grouped query with aggregate projections.
        private static GeneratedSqlQuery BuildQuery(QueryBuilder queryBuilder)
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .InnerJoin<JoinUser, JoinOrder>(alias: "o", on: (u, o) => u.Id == o.UserId)
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .SelectAggregate<JoinOrder>(
                    QueryAggregateFunction.Sum,
                    o => o.Total,
                    alias: "TotalAmount")
                .SelectAggregate<JoinOrder>(
                    QueryAggregateFunction.Count,
                    o => o.Id,
                    alias: "OrderCount")
                .GroupBy<JoinUser>(u => new
                {
                    u.Id,
                    u.Email
                })
                .Build();
        }
    }
}
