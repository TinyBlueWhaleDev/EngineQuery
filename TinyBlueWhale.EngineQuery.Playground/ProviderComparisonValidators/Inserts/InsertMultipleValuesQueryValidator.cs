using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.Inserts
{
    /// <summary>
    /// Validates multi-column INSERT VALUES generation and parameter ordering
    /// across supported database providers.
    ///
    /// SQL Server:
    /// INSERT INTO [users] ([email], [is_active])
    /// VALUES (@p0, @p1)
    ///
    /// PostgreSQL:
    /// INSERT INTO "users" ("email", "is_active")
    /// VALUES (@p0, @p1)
    ///
    /// MySQL:
    /// INSERT INTO `users` (`email`, `is_active`)
    /// VALUES (@p0, @p1)
    ///
    /// Expected parameters:
    /// @p0 = admin@test.com
    /// @p1 = True
    /// </summary>
    public static class InsertMultipleValuesQueryValidator
    {
        public static void Run()
        {
            var metadataResolver = ProviderMetadataFactory.CreateJoinMetadataResolver();

            ProviderQueryPrinter.Print(
                "SQL Server Insert Multiple Values",
                BuildQuery(ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL Insert Multiple Values",
                BuildQuery(ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL Insert Multiple Values",
                BuildQuery(ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));
        }

        // Builds a multi-column INSERT VALUES command.
        private static GeneratedSqlQuery BuildQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .InsertInto<JoinUser>()
                .Set(user => user.Email, "admin@test.com")
                .Set(user => user.IsActive, true)
                .Build();
        }
    }
}
