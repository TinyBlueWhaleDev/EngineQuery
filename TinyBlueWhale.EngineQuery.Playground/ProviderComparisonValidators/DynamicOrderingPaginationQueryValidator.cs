using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators
{
    /// <summary>
    /// Validates dynamic ordering followed by secondary ordering and pagination
    /// across providers.
    /// </summary>
    public static class DynamicOrderingPaginationQueryValidator
    {
        /// <summary>
        /// Runs the validator.
        /// </summary>
        public static void Run()
        {
            var metadataResolver = ProviderMetadataFactory.CreateJoinMetadataResolver();

            ProviderQueryPrinter.Print(
                "SQL Server Dynamic Ordering Pagination",
                BuildQuery(
                    ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL Dynamic Ordering Pagination",
                BuildQuery(
                    ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL Dynamic Ordering Pagination",
                BuildQuery(
                    ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));
        }

        // Builds a query using dynamic ordering followed by ThenBy and pagination.
        private static GeneratedSqlQuery BuildQuery(
            QueryBuilder queryBuilder)
        {
            const string sortBy = "EMAIL";
            const bool descending = true;
            const int skip = 20;
            const int take = 10;

            var query = queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                });

            IOrderedQueryCommandBuilder<JoinUser> orderedQuery = sortBy switch
            {
                "EMAIL" => descending
                                        ? query.OrderByDescending<JoinUser>(u => u.Email)
                                        : query.OrderBy<JoinUser>(u => u.Email),
                _ => descending
                                        ? query.OrderByDescending<JoinUser>(u => u.Id)
                                        : query.OrderBy<JoinUser>(u => u.Id),
            };

            return orderedQuery
                .ThenBy<JoinUser>(u => u.Id)
                .Skip(skip)
                .Take(take)
                .Build();
        }
    }
}
