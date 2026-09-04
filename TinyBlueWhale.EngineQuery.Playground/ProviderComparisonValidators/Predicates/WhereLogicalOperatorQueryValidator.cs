using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Metadata.Resolvers;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.Predicates
{
    /// <summary>
    /// Validates logical WHERE predicate operators across providers.
    /// </summary>
    public static class WhereLogicalOperatorQueryValidator
    {
        /// <summary>
        /// Runs the validator.
        /// </summary>
        public static void Run()
        {
            var metadataResolver =
                ProviderMetadataFactory.CreateJoinMetadataResolver();

            PrintDefaultAndQueries(metadataResolver);
            PrintExplicitOrQueries(metadataResolver);
            PrintConsecutiveOrQueries(metadataResolver);
            PrintOrBlockFollowedByAndQueries(metadataResolver);
            PrintConditionalOrEnabledQueries(metadataResolver);
            PrintConditionalOrDisabledQueries(metadataResolver);
            PrintSingleOrQueries(metadataResolver);
            PrintOrThenAndQueries(metadataResolver);
            PrintTrailingOrQueries(metadataResolver);
            PrintMultipleOrGroupsQueries(metadataResolver);
        }

        // Prints queries that validate a single OR group.
        private static void PrintSingleOrQueries(FluentEntityMetadataResolver metadataResolver)
        {
            ProviderQueryPrinter.Print(
                "SQL Server Where Logical Operators - Single OR",
                BuildSingleOrQuery(ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL Where Logical Operators - Single OR",
                BuildSingleOrQuery(ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL Where Logical Operators - Single OR",
                BuildSingleOrQuery(ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));
        }

        // Prints queries that validate an OR group followed by AND.
        private static void PrintOrThenAndQueries(FluentEntityMetadataResolver metadataResolver)
        {
            ProviderQueryPrinter.Print("SQL Server Where Logical Operators - OR Then AND",
                BuildOrThenAndQuery(ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print("PostgreSQL Where Logical Operators - OR Then AND",
                BuildOrThenAndQuery(ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print("MySQL Where Logical Operators - OR Then AND",
                BuildOrThenAndQuery(ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));
        }

        // Prints queries that validate an OR group starting after an AND.
        private static void PrintTrailingOrQueries(FluentEntityMetadataResolver metadataResolver)
        {
            ProviderQueryPrinter.Print(
                "SQL Server Where Logical Operators - Trailing OR",
                BuildTrailingOrQuery(ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL Where Logical Operators - Trailing OR",
                BuildTrailingOrQuery(ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL Where Logical Operators - Trailing OR",
                BuildTrailingOrQuery(ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));
        }

        // Prints queries that validate multiple OR groups.
        private static void PrintMultipleOrGroupsQueries(FluentEntityMetadataResolver metadataResolver)
        {
            ProviderQueryPrinter.Print(
                "SQL Server Where Logical Operators - Multiple OR Groups",
                BuildMultipleOrGroupsQuery(ProviderQueryBuilderFactory.CreateSqlServer(metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL Where Logical Operators - Multiple OR Groups",
                BuildMultipleOrGroupsQuery(ProviderQueryBuilderFactory.CreatePostgreSql(metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL Where Logical Operators - Multiple OR Groups",
                BuildMultipleOrGroupsQuery(ProviderQueryBuilderFactory.CreateMySql(metadataResolver)));
        }

        // Prints queries that validate the default logical AND behavior.
        private static void PrintDefaultAndQueries(FluentEntityMetadataResolver metadataResolver)
        {
            ProviderQueryPrinter.Print(
                "SQL Server Where Logical Operators - Default AND",
                BuildDefaultAndQuery(
                    ProviderQueryBuilderFactory.CreateSqlServer(
                        metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL Where Logical Operators - Default AND",
                BuildDefaultAndQuery(
                    ProviderQueryBuilderFactory.CreatePostgreSql(
                        metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL Where Logical Operators - Default AND",
                BuildDefaultAndQuery(
                    ProviderQueryBuilderFactory.CreateMySql(
                        metadataResolver)));
        }

        // Prints queries that validate an explicit logical OR operator.
        private static void PrintExplicitOrQueries(FluentEntityMetadataResolver metadataResolver)
        {
            ProviderQueryPrinter.Print(
                "SQL Server Where Logical Operators - Explicit OR",
                BuildExplicitOrQuery(
                    ProviderQueryBuilderFactory.CreateSqlServer(
                        metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL Where Logical Operators - Explicit OR",
                BuildExplicitOrQuery(
                    ProviderQueryBuilderFactory.CreatePostgreSql(
                        metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL Where Logical Operators - Explicit OR",
                BuildExplicitOrQuery(
                    ProviderQueryBuilderFactory.CreateMySql(
                        metadataResolver)));
        }

        // Prints queries that validate consecutive logical OR operators.
        private static void PrintConsecutiveOrQueries(FluentEntityMetadataResolver metadataResolver)
        {
            ProviderQueryPrinter.Print(
                "SQL Server Where Logical Operators - Consecutive OR",
                BuildConsecutiveOrQuery(
                    ProviderQueryBuilderFactory.CreateSqlServer(
                        metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL Where Logical Operators - Consecutive OR",
                BuildConsecutiveOrQuery(
                    ProviderQueryBuilderFactory.CreatePostgreSql(
                        metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL Where Logical Operators - Consecutive OR",
                BuildConsecutiveOrQuery(
                    ProviderQueryBuilderFactory.CreateMySql(
                        metadataResolver)));
        }

        // Prints queries that validate an OR block followed by an AND predicate.
        private static void PrintOrBlockFollowedByAndQueries(FluentEntityMetadataResolver metadataResolver)
        {
            ProviderQueryPrinter.Print(
                "SQL Server Where Logical Operators - OR Followed By AND",
                BuildOrBlockFollowedByAndQuery(
                    ProviderQueryBuilderFactory.CreateSqlServer(
                        metadataResolver)));

            ProviderQueryPrinter.Print(
                "PostgreSQL Where Logical Operators - OR Followed By AND",
                BuildOrBlockFollowedByAndQuery(
                    ProviderQueryBuilderFactory.CreatePostgreSql(
                        metadataResolver)));

            ProviderQueryPrinter.Print(
                "MySQL Where Logical Operators - OR Followed By AND",
                BuildOrBlockFollowedByAndQuery(
                    ProviderQueryBuilderFactory.CreateMySql(
                        metadataResolver)));
        }

        // Prints queries that validate an enabled conditional OR predicate.
        private static void PrintConditionalOrEnabledQueries(FluentEntityMetadataResolver metadataResolver)
        {
            ProviderQueryPrinter.Print(
                "SQL Server Where Logical Operators - Conditional OR Enabled",
                BuildConditionalOrQuery(
                    ProviderQueryBuilderFactory.CreateSqlServer(
                        metadataResolver),
                    includeEmailPredicate: true));

            ProviderQueryPrinter.Print(
                "PostgreSQL Where Logical Operators - Conditional OR Enabled",
                BuildConditionalOrQuery(
                    ProviderQueryBuilderFactory.CreatePostgreSql(
                        metadataResolver),
                    includeEmailPredicate: true));

            ProviderQueryPrinter.Print(
                "MySQL Where Logical Operators - Conditional OR Enabled",
                BuildConditionalOrQuery(
                    ProviderQueryBuilderFactory.CreateMySql(
                        metadataResolver),
                    includeEmailPredicate: true));
        }

        // Prints queries that validate a disabled conditional OR predicate.
        private static void PrintConditionalOrDisabledQueries(FluentEntityMetadataResolver metadataResolver)
        {
            ProviderQueryPrinter.Print(
                "SQL Server Where Logical Operators - Conditional OR Disabled",
                BuildConditionalOrQuery(
                    ProviderQueryBuilderFactory.CreateSqlServer(
                        metadataResolver),
                    includeEmailPredicate: false));

            ProviderQueryPrinter.Print(
                "PostgreSQL Where Logical Operators - Conditional OR Disabled",
                BuildConditionalOrQuery(
                    ProviderQueryBuilderFactory.CreatePostgreSql(
                        metadataResolver),
                    includeEmailPredicate: false));

            ProviderQueryPrinter.Print(
                "MySQL Where Logical Operators - Conditional OR Disabled",
                BuildConditionalOrQuery(
                    ProviderQueryBuilderFactory.CreateMySql(
                        metadataResolver),
                    includeEmailPredicate: false));
        }

        // Builds a query containing multiple OR groups.
        private static GeneratedSqlQuery BuildMultipleOrGroupsQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .Where(u => u.Id == 1)
                .Where(
                    u => u.Email == "admin@test.com",
                    QueryLogicalOperator.Or)
                .Where(
                    u => u.Email == "support@test.com",
                    QueryLogicalOperator.Or)
                .Where(u => u.Id > 0)
                .Where(
                    u => u.Id < 100,
                    QueryLogicalOperator.Or)
                .Build();
        }


        // Builds a query where an OR group starts after an AND predicate.
        private static GeneratedSqlQuery BuildTrailingOrQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .Where(u => u.Id == 1)
                .Where(u => u.Id > 0)
                .Where(
                    u => u.Email == "admin@test.com",
                    QueryLogicalOperator.Or)
                .Build();
        }


        // Builds a query containing a single OR group.
        private static GeneratedSqlQuery BuildSingleOrQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .Where(u => u.Id == 1)
                .Where(
                    u => u.Email == "admin@test.com",
                    QueryLogicalOperator.Or)
                .Build();
        }

        // Builds a query containing an OR group followed by AND.
        private static GeneratedSqlQuery BuildOrThenAndQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .Where(u => u.Id == 1)
                .Where(
                    u => u.Email == "admin@test.com",
                    QueryLogicalOperator.Or)
                .Where(u => u.Id > 0)
                .Build();
        }


        // Builds a query that preserves the default logical AND behavior.
        private static GeneratedSqlQuery BuildDefaultAndQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .Where(u => u.Id > 0)
                .Where(u => u.Email != null)
                .Build();
        }

        // Builds a query that connects two predicates with logical OR.
        private static GeneratedSqlQuery BuildExplicitOrQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .Where(u => u.Id == 10)
                .Where(
                    u => u.Email == "admin@test.com",
                    QueryLogicalOperator.Or)
                .Build();
        }

        // Builds a query containing a consecutive logical OR sequence.
        private static GeneratedSqlQuery BuildConsecutiveOrQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .Where(u => u.Id == 10)
                .Where(
                    u => u.Email == "admin@test.com",
                    QueryLogicalOperator.Or)
                .Where(
                    u => u.Email == "support@test.com",
                    QueryLogicalOperator.Or)
                .Build();
        }

        // Builds a query where an OR block is followed by an AND predicate.
        private static GeneratedSqlQuery BuildOrBlockFollowedByAndQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .Where(u => u.Id == 10)
                .Where(
                    u => u.Email == "admin@test.com",
                    QueryLogicalOperator.Or)
                .Where(u => u.Id > 0)
                .Build();
        }

        // Builds a query with an optional logical OR predicate.
        private static GeneratedSqlQuery BuildConditionalOrQuery<TProfile>(IQueryBuilder<TProfile> queryBuilder, bool includeEmailPredicate)
            where TProfile : IDatabaseProviderProfile
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .Where(u => u.Id == 10)
                .WhereIf(
                    includeEmailPredicate,
                    u => u.Email == "admin@test.com",
                    QueryLogicalOperator.Or)
                .Build();
        }
    }
}
