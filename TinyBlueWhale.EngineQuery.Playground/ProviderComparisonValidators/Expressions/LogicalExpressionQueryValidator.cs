using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.Expressions
{
    /// <summary>
    /// Validates logical AND and OR predicates composed through consecutive computed WHERE clauses
    /// across supported database providers.
    ///
    /// SQL Server:
    /// SELECT [o].[order_id] AS [OrderId], [o].[total]
    /// FROM [orders] AS [o]
    /// WHERE (([o].[total] &gt; @p0) AND ([o].[total] &lt; @p1)) AND (([o].[total] &lt; @p2) OR ([o].[total] &gt; @p3))
    ///
    /// PostgreSQL:
    /// SELECT "o"."order_id" AS "OrderId", "o"."total"
    /// FROM "orders" AS "o"
    /// WHERE (("o"."total" &gt; @p0) AND ("o"."total" &lt; @p1)) AND (("o"."total" &lt; @p2) OR ("o"."total" &gt; @p3))
    ///
    /// MySQL:
    /// SELECT `o`.`order_id` AS `OrderId`, `o`.`total`
    /// FROM `orders` AS `o`
    /// WHERE ((`o`.`total` &gt; @p0) AND (`o`.`total` &lt; @p1)) AND ((`o`.`total` &lt; @p2) OR (`o`.`total` &gt; @p3))
    ///
    /// Expected parameters:
    /// @p0 = 1000
    /// @p1 = 5000
    /// @p2 = 100
    /// @p3 = 10000
    /// </summary>
    public static class LogicalExpressionQueryValidator
    {
        public static void Run()
        {
            var metadataResolver = ProviderMetadataFactory.CreateJoinMetadataResolver();

            ProviderQueryPrinter.Print(
                "SQL Server Logical Expressions",
                BuildQuery(ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL Logical Expressions",
                BuildQuery(ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL Logical Expressions",
                BuildQuery(ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));
        }

        private static GeneratedSqlQuery BuildQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .From<JoinOrder>(alias: "o")
                .Select<JoinOrder>(o => new
                {
                    OrderId = o.Id,
                    o.Total
                })
                .WhereComputed<JoinOrder>(
                    o => o.Total > 1000 && o.Total < 5000)
                .WhereComputed<JoinOrder>(
                    o => o.Total < 100 || o.Total > 10000)
                .Build();
        }
    }
}
