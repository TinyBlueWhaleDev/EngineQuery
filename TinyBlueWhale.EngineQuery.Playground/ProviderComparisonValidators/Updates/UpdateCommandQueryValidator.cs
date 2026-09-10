using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.Updates
{
    /// <summary>
    /// Validates strongly typed UPDATE command generation, metadata resolution
    /// and parameter ordering across supported database providers.
    ///
    /// SQL Server:
    /// UPDATE [users]
    /// SET [email] = @p0
    /// WHERE ([user_id] = @p1)
    ///
    /// PostgreSQL:
    /// UPDATE "users"
    /// SET "email" = @p0
    /// WHERE ("user_id" = @p1)
    ///
    /// MySQL:
    /// UPDATE `users`
    /// SET `email` = @p0
    /// WHERE (`user_id` = @p1)
    ///
    /// Expected parameters:
    /// @p0 = updated@test.com
    /// @p1 = 10
    /// </summary>
    public static class UpdateCommandQueryValidator
    {        
        public static void Run()
        {
            var metadataResolver = ProviderMetadataFactory.CreateJoinMetadataResolver();

            ProviderQueryPrinter.Print(
                "SQL Server Update Command",
                BuildQuery(ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL Update Command",
                BuildQuery(ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL Update Command",
                BuildQuery(ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));
        }

        // Builds a strongly typed UPDATE command.
        private static GeneratedSqlQuery BuildQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .Update<JoinUser>()
                .Set(user => user.Email, "updated@test.com")
                .Where(user => user.Id == 10)
                .Build();
        }
    }
}
