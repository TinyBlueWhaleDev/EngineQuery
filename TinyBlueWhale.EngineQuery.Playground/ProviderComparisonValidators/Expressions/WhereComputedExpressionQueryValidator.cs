using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.Expressions
{

    /// <summary>
    /// Validates arithmetic expressions inside computed WHERE predicates
    /// across supported database providers.
    ///
    /// SQL Server:
    /// SELECT [o].[order_id] AS [OrderId], [o].[total]
    /// FROM [orders] AS [o]
    /// WHERE (([o].[total] * @p0) &gt; @p1) AND (([o].[total] - @p2) &lt;= @p3)
    ///
    /// PostgreSQL:
    /// SELECT "o"."order_id" AS "OrderId", "o"."total"
    /// FROM "orders" AS "o"
    /// WHERE (("o"."total" * @p0) &gt; @p1) AND (("o"."total" - @p2) &lt;= @p3)
    ///
    /// MySQL:
    /// SELECT `o`.`order_id` AS `OrderId`, `o`.`total`
    /// FROM `orders` AS `o`
    /// WHERE ((`o`.`total` * @p0) &gt; @p1) AND ((`o`.`total` - @p2) &lt;= @p3)
    ///
    /// Expected parameters:
    /// @p0 = 1.16
    /// @p1 = 1000
    /// @p2 = 50
    /// @p3 = 500
    /// </summary>
    public static class WhereComputedExpressionQueryValidator
    {
        public static void Run()
        {
            var metadataResolver = ProviderMetadataFactory.CreateJoinMetadataResolver();

            ProviderQueryPrinter.Print(
                "SQL Server Where Computed Expressions",
                BuildQuery(ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL Where Computed Expressions",
                BuildQuery(ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL Where Computed Expressions",
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
                    o => (o.Total * 1.16m) > 1000)
                .WhereComputed<JoinOrder>(
                    o => (o.Total - 50) <= 500)
                .Build();
        }
    }
}
