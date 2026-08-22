using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators
{
    /// <summary>
    /// Validates IN and NOT IN collection conditions across providers.
    /// </summary>
    public static class WhereCollectionQueryValidator
    {
        /// <summary>
        /// Runs the validator.
        /// </summary>
        public static void Run()
        {
            var metadataResolver =
                ProviderMetadataFactory.CreateJoinMetadataResolver();

            RunProvider(
                "SQL Server",
                ProviderQueryBuilderFactory.CreateSqlServer(
                    metadataResolver));

            RunProvider(
                "PostgreSQL",
                ProviderQueryBuilderFactory.CreatePostgreSql(
                    metadataResolver));

            RunProvider(
                "MySQL",
                ProviderQueryBuilderFactory.CreateMySql(
                    metadataResolver));
        }

        // Runs all collection filtering scenarios for the specified provider.
        private static void RunProvider(
            string providerName,
            QueryBuilder queryBuilder)
        {
            ProviderQueryPrinter.Print(
                $"{providerName} Where IN and NOT IN Collections",
                BuildSelectQuery(queryBuilder));

            ProviderQueryPrinter.Print(
                $"{providerName} Insert Select Where IN Collection",
                BuildInsertSelectQuery(queryBuilder));

            ProviderQueryPrinter.Print(
                $"{providerName} Update Where IN Collection",
                BuildUpdateQuery(queryBuilder));

            ProviderQueryPrinter.Print(
                $"{providerName} Delete Where NOT IN Collection",
                BuildDeleteQuery(queryBuilder));
        }

        // Builds a SELECT command using IN and NOT IN collection conditions.
        private static GeneratedSqlQuery BuildSelectQuery(
            QueryBuilder queryBuilder)
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .WhereIn(
                    user => user.Id,
                    [10, 20, 30])
                .WhereNotIn(
                    user => user.Email,
                    [
                        "blocked@test.com",
                        "deleted@test.com"
                    ])
                .Build();
        }

        // Builds an INSERT SELECT command using an IN collection condition.
        private static GeneratedSqlQuery BuildInsertSelectQuery(
            QueryBuilder queryBuilder)
        {
            return queryBuilder
                .InsertInto<JoinOrder>()
                .Columns(order => new
                {
                    order.UserId
                })
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(user => new
                {
                    UserId = user.Id
                })
                .WhereIn<JoinUser, int>(
                    user => user.Id,
                    [10, 20, 30])
                .Build();
        }

        // Builds an UPDATE command using an IN collection condition.
        private static GeneratedSqlQuery BuildUpdateQuery(
            QueryBuilder queryBuilder)
        {
            return queryBuilder
                .Update<JoinUser>()
                .Set(user => user.IsActive, false)
                .WhereIn(
                    user => user.Id,
                    [10, 20, 30])
                .Build();
        }

        // Builds a DELETE command using a NOT IN collection condition.
        private static GeneratedSqlQuery BuildDeleteQuery(
            QueryBuilder queryBuilder)
        {
            return queryBuilder
                .DeleteFrom<JoinUser>()
                .WhereNotIn(
                    user => user.Id,
                    [10, 20, 30])
                .Build();
        }
    }
}
