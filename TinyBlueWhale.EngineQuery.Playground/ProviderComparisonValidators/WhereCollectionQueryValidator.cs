using System.Formats.Tar;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
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
        private static void RunProvider<TProfile>(
            string providerName,
            IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
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
        private static GeneratedSqlQuery BuildSelectQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
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
        private static GeneratedSqlQuery BuildInsertSelectQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
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
        private static GeneratedSqlQuery BuildUpdateQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
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
        private static GeneratedSqlQuery BuildDeleteQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
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
