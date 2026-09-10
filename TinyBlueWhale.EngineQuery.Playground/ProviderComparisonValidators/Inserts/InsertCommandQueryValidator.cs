using TinyBlueWhale.EngineQuery.Abstractions.Extensions;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.MySql.Profiles;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;
using TinyBlueWhale.EngineQuery.PostgreSql.Profiles;
using TinyBlueWhale.EngineQuery.SqlServer.Profiles;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.Inserts
{
    /// <summary>
    /// Validates INSERT VALUES generation and provider-specific identity retrieval
    /// across supported database providers.
    ///
    /// SQL Server INSERT VALUES:
    /// INSERT INTO [users] ([email])
    /// VALUES (@p0)
    /// @p0 = admin@test.com
    ///
    /// PostgreSQL INSERT VALUES:
    /// INSERT INTO "users" ("email")
    /// VALUES (@p0)
    /// @p0 = admin@test.com
    ///
    /// MySQL INSERT VALUES:
    /// INSERT INTO `users` (`email`)
    /// VALUES (@p0)
    /// @p0 = admin@test.com
    ///
    /// SQL Server identity retrieval:
    /// INSERT INTO [users] ([email])
    /// VALUES (@p0);
    /// SELECT CAST(SCOPE_IDENTITY() AS BIGINT);
    /// @p0 = admin@test.com
    ///
    /// PostgreSQL identity retrieval:
    /// INSERT INTO "users" ("email")
    /// VALUES (@p0)
    /// RETURNING "user_id";
    /// @p0 = admin@test.com
    ///
    /// MySQL identity retrieval:
    /// INSERT INTO `users` (`email`)
    /// VALUES (@p0);
    /// SELECT LAST_INSERT_ID();
    /// @p0 = admin@test.com
    /// </summary>
    public static class InsertCommandQueryValidator
    {
        public static void Run()
        {
            var metadataResolver = ProviderMetadataFactory.CreateJoinMetadataResolver();

            var sqlServer = ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver);
            var postgreSql = ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver);
            var mySql = ProviderQueryBuilderFactory.CreateMySql(metadataResolver);

            ProviderQueryPrinter.Print(
                "SQL Server Insert Values",
                BuildInsertValuesQuery(sqlServer));

            ProviderQueryPrinter.Print(
                "PostgreSQL Insert Values",
                BuildInsertValuesQuery(postgreSql));

            ProviderQueryPrinter.Print(
                "MySQL Insert Values",
                BuildInsertValuesQuery(mySql));

            ProviderQueryPrinter.Print(
                "SQL Server Insert Return Identity",
                BuildSqlServerInsertIdentityQuery(sqlServer));

            ProviderQueryPrinter.Print(
                "PostgreSQL Insert Return Identity",
                BuildPostgreSqlInsertIdentityQuery(postgreSql));

            ProviderQueryPrinter.Print(
                "MySQL Insert Return Identity",
                BuildMySqlInsertIdentityQuery(mySql));
        }

        // Builds an INSERT VALUES command.
        private static GeneratedSqlQuery BuildInsertValuesQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .InsertInto<JoinUser>()
                .Set(user => user.Email, "admin@test.com")
                .Build();
        }

        // Builds a SQL Server INSERT command that returns the generated identity.
        private static GeneratedSqlQuery BuildSqlServerInsertIdentityQuery(IQueryBuilder<SqlServer2012Profile> queryBuilder)
        {
            return queryBuilder
                .InsertInto<JoinUser>()
                .Set(user => user.Email, "admin@test.com")
                .ReturnIdentity()
                .Build();
        }

        // Builds a PostgreSQL INSERT command that returns the generated identity.
        private static GeneratedSqlQuery BuildPostgreSqlInsertIdentityQuery(IQueryBuilder<PostgreSql93Profile> queryBuilder)
        {
            return queryBuilder
                .InsertInto<JoinUser>()
                .Set(user => user.Email, "admin@test.com")
                .ReturnIdentity(user => user.Id)
                .Build();
        }

        // Builds a MySQL INSERT command that returns the generated identity.
        private static GeneratedSqlQuery BuildMySqlInsertIdentityQuery(IQueryBuilder<MySql8031Profile> queryBuilder)
        {
            return queryBuilder
                .InsertInto<JoinUser>()
                .Set(user => user.Email, "admin@test.com")
                .ReturnIdentity()
                .Build();
        }

    }
}
