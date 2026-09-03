using TinyBlueWhale.EngineQuery.Abstractions.Extensions;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Features;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.SetOperations
{

    /// <summary>
    /// Validates INTERSECT and EXCEPT query generation across providers.
    /// </summary>
    public static class IntersectExceptQueryValidator
    {
        /// <summary>
        /// Runs the validator.
        /// </summary>
        public static void Run()
        {
            var metadataResolver = ProviderMetadataFactory.CreateJoinMetadataResolver();

            ProviderQueryPrinter.Print(
                "SQL Server INTERSECT",
                BuildIntersectQuery(ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL INTERSECT",
                BuildIntersectQuery(ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL INTERSECT",
                BuildIntersectQuery(ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "SQL Server EXCEPT",
                BuildExceptQuery(ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL EXCEPT",
                BuildExceptQuery(ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL EXCEPT",
                BuildExceptQuery(ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));
        }

        // Builds an INTERSECT query.
        private static GeneratedSqlQuery BuildIntersectQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
             where TProfile : IDatabaseProviderProfile, IIntersectFeature
        {
            return queryBuilder
                .From<ActiveUser>(alias: "u")
                .Select<ActiveUser>(u => new
                {
                    u.Email
                })
                .Where<ActiveUser>(u => u.Id > 10)
                .Intersect(set => set
                    .From<ArchivedUser>(alias: "a")
                    .Select<ArchivedUser>(a => new
                    {
                        a.Email
                    })
                    .Where<ArchivedUser>(a => a.Id > 100))
                .Build();
        }

        // Builds an EXCEPT query.
        private static GeneratedSqlQuery BuildExceptQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile, IExceptFeature
        {
            return queryBuilder
                .From<ActiveUser>(alias: "u")
                .Select<ActiveUser>(u => new
                {
                    u.Email
                })
                .Where<ActiveUser>(u => u.Id > 10)
                .Except(set => set
                    .From<ArchivedUser>(alias: "a")
                    .Select<ArchivedUser>(a => new
                    {
                        a.Email
                    })
                    .Where<ArchivedUser>(a => a.Id > 100))
                .Build();
        }
    }
}
