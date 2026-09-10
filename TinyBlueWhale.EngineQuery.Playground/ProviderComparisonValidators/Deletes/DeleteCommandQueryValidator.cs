using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.Deletes
{
    /// <summary>
    /// Validates strongly typed DELETE command generation using resolved entity metadata
    /// and parameterized predicates across supported database providers.
    ///
    /// SQL Server:
    /// DELETE
    /// FROM [users]
    /// WHERE ([user_id] = @p0)
    ///
    /// PostgreSQL:
    /// DELETE
    /// FROM "users"
    /// WHERE ("user_id" = @p0)
    ///
    /// MySQL:
    /// DELETE
    /// FROM `users`
    /// WHERE (`user_id` = @p0)
    ///
    /// Expected parameters:
    /// @p0 = 10
    /// </summary>
    public static class DeleteCommandQueryValidator
    {
        public static void Run()
        {
            var metadataResolver = ProviderMetadataFactory.CreateJoinMetadataResolver();

            ProviderQueryPrinter.Print(
                "SQL Server Delete Command",
                BuildQuery(ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL Delete Command",
                BuildQuery(ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL Delete Command",
                BuildQuery(ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));
        }

        private static GeneratedSqlQuery BuildQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .DeleteFrom<JoinUser>()
                .Where(user => user.Id == 10)
                .Build();
        }
    }
}
