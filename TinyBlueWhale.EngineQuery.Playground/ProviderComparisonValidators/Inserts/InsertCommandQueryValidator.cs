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
    /// Validates INSERT command generation across providers.
    /// </summary>
    public static class InsertCommandQueryValidator
    {
        /// <summary>
        /// Runs INSERT command validation scenarios across the supported providers.
        /// </summary>
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

        /// <summary>
        /// Builds an INSERT VALUES command.
        /// </summary>
        private static GeneratedSqlQuery BuildInsertValuesQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .InsertInto<JoinUser>()
                .Set(user => user.Email, "admin@test.com")
                .Build();
        }

        /// <summary>
        /// Builds a SQL Server INSERT command that retrieves the generated identity.
        /// </summary>
        private static GeneratedSqlQuery BuildSqlServerInsertIdentityQuery(IQueryBuilder<SqlServer2012Profile> queryBuilder)
        {
            return queryBuilder
                .InsertInto<JoinUser>()
                .Set(user => user.Email, "admin@test.com")
                .ReturnIdentity()
                .Build();
        }

        /// <summary>
        /// Builds a PostgreSQL INSERT command that retrieves the generated identity.
        /// </summary>
        private static GeneratedSqlQuery BuildPostgreSqlInsertIdentityQuery(IQueryBuilder<PostgreSql93Profile> queryBuilder)
        {
            return queryBuilder
                .InsertInto<JoinUser>()
                .Set(user => user.Email, "admin@test.com")
                .ReturnIdentity(user => user.Id)
                .Build();
        }

        /// <summary>
        /// Builds a MySQL INSERT command that retrieves the generated identity.
        /// </summary>
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
