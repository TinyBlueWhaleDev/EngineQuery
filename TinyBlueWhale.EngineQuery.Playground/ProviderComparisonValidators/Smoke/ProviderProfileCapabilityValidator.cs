using TinyBlueWhale.EngineQuery.Abstractions.Extensions;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Features;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding;
using TinyBlueWhale.EngineQuery.MySql.Compilation;
using TinyBlueWhale.EngineQuery.MySql.Profiles;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.Playground.Shared;
using TinyBlueWhale.EngineQuery.PostgreSql.Compilation;
using TinyBlueWhale.EngineQuery.PostgreSql.Profiles;
using TinyBlueWhale.EngineQuery.SqlServer.Compilation;
using TinyBlueWhale.EngineQuery.SqlServer.Profiles;

namespace TinyBlueWhale.EngineQuery.Playground.ProviderComparisonValidators.Smoke
{
    /// <summary>
    /// Validates the positive compile-time feature contract exposed by concrete
    /// database provider profiles.
    ///
    /// Expected capabilities:
    ///
    /// SQL Server 2008:
    /// CTE             -> PASS
    /// Recursive CTE   -> PASS
    /// Window Function -> PASS
    /// APPLY           -> PASS
    /// INTERSECT       -> PASS
    /// EXCEPT          -> PASS
    ///
    /// SQL Server 2012:
    /// CTE             -> PASS
    /// Recursive CTE   -> PASS
    /// Window Function -> PASS
    /// APPLY           -> PASS
    /// Pagination      -> PASS
    /// INTERSECT       -> PASS
    /// EXCEPT          -> PASS
    ///
    /// PostgreSQL 8.4:
    /// CTE             -> PASS
    /// Recursive CTE   -> PASS
    /// Window Function -> PASS
    /// Pagination      -> PASS
    /// INTERSECT       -> PASS
    /// EXCEPT          -> PASS
    ///
    /// PostgreSQL 9.3:
    /// CTE             -> PASS
    /// Recursive CTE   -> PASS
    /// Window Function -> PASS
    /// LATERAL         -> PASS
    /// Pagination      -> PASS
    /// INTERSECT       -> PASS
    /// EXCEPT          -> PASS
    ///
    /// MySQL 5.7:
    /// Pagination      -> PASS
    ///
    /// MySQL 8.0:
    /// Pagination      -> PASS
    /// CTE             -> PASS
    /// Recursive CTE   -> PASS
    /// Window Function -> PASS
    ///
    /// MySQL 8.0.14:
    /// Pagination      -> PASS
    /// CTE             -> PASS
    /// Recursive CTE   -> PASS
    /// Window Function -> PASS
    /// LATERAL         -> PASS
    ///
    /// MySQL 8.0.31:
    /// Pagination      -> PASS
    /// CTE             -> PASS
    /// Recursive CTE   -> PASS
    /// Window Function -> PASS
    /// LATERAL         -> PASS
    /// INTERSECT       -> PASS
    /// EXCEPT          -> PASS
    /// </summary>
    public static class ProviderProfileCapabilityValidator
    {        
        public static void Run()
        {
            var metadataResolver = ProviderMetadataFactory.CreateJoinMetadataResolver();

            var sqlServer2008 =
                SqlServerQueryCompiler.Factory.Create<SqlServer2008Profile>(metadataResolver);

            var sqlServer2012 =
                SqlServerQueryCompiler.Factory.Create<SqlServer2012Profile>(metadataResolver);

            var postgreSql84 =
                PostgreSqlQueryCompiler.Factory.Create<PostgreSql84Profile>(metadataResolver);

            var postgreSql93 =
                PostgreSqlQueryCompiler.Factory.Create<PostgreSql93Profile>(metadataResolver);

            var mySql57 =
                MySqlQueryCompiler.Factory.Create<MySql57Profile>(metadataResolver);

            var mySql80 =
                MySqlQueryCompiler.Factory.Create<MySql80Profile>(metadataResolver);

            var mySql8014 =
                MySqlQueryCompiler.Factory.Create<MySql8014Profile>(metadataResolver);

            var mySql8031 =
                MySqlQueryCompiler.Factory.Create<MySql8031Profile>(metadataResolver);

            ValidateSqlServer2008(sqlServer2008);
            ValidateSqlServer2012(sqlServer2012);

            ValidatePostgreSql84(postgreSql84);
            ValidatePostgreSql93(postgreSql93);

            ValidateMySql57(mySql57);
            ValidateMySql80(mySql80);
            ValidateMySql8014(mySql8014);
            ValidateMySql8031(mySql8031);
        }

        private static void ValidateSqlServer2008(
            QueryBuilder<SqlServer2008Profile> queryBuilder)
        {
            PrintProfile("SQL Server 2008");

            Validate(
                "CTE",
                () => BuildSqlServer2008Cte(queryBuilder));

            Validate(
                "Recursive CTE",
                () => BuildSqlServer2008RecursiveCte(queryBuilder));

            Validate(
                "Window Function",
                () => BuildWindow(queryBuilder));

            Validate(
                "APPLY",
                () => BuildSqlServer2008Apply(queryBuilder));

            Validate(
                "INTERSECT",
                () => BuildIntersect(queryBuilder));

            Validate(
                "EXCEPT",
                () => BuildExcept(queryBuilder));

            Console.WriteLine();
        }

        private static void ValidateSqlServer2012(
            QueryBuilder<SqlServer2012Profile> queryBuilder)
        {
            PrintProfile("SQL Server 2012");

            Validate(
                "CTE",
                () => BuildSqlServer2012Cte(queryBuilder));

            Validate(
                "Recursive CTE",
                () => BuildSqlServer2012RecursiveCte(queryBuilder));

            Validate(
                "Window Function",
                () => BuildWindow(queryBuilder));

            Validate(
                "APPLY",
                () => BuildSqlServer2012Apply(queryBuilder));

            Validate(
                "Pagination",
                () => BuildPagination(queryBuilder));

            Validate(
                "INTERSECT",
                () => BuildIntersect(queryBuilder));

            Validate(
                "EXCEPT",
                () => BuildExcept(queryBuilder));

            Console.WriteLine();
        }

        private static void ValidatePostgreSql84(
            QueryBuilder<PostgreSql84Profile> queryBuilder)
        {
            PrintProfile("PostgreSQL 8.4");

            Validate(
                "CTE",
                () => BuildPostgreSql84Cte(queryBuilder));

            Validate(
                "Recursive CTE",
                () => BuildPostgreSql84RecursiveCte(queryBuilder));

            Validate(
                "Window Function",
                () => BuildWindow(queryBuilder));

            Validate(
                "Pagination",
                () => BuildPagination(queryBuilder));

            Validate(
                "INTERSECT",
                () => BuildIntersect(queryBuilder));

            Validate(
                "EXCEPT",
                () => BuildExcept(queryBuilder));

            Console.WriteLine();
        }

        private static void ValidatePostgreSql93(
            QueryBuilder<PostgreSql93Profile> queryBuilder)
        {
            PrintProfile("PostgreSQL 9.3");

            Validate(
                "CTE",
                () => BuildPostgreSql93Cte(queryBuilder));

            Validate(
                "Recursive CTE",
                () => BuildPostgreSql93RecursiveCte(queryBuilder));

            Validate(
                "Window Function",
                () => BuildWindow(queryBuilder));

            Validate(
                "LATERAL",
                () => BuildPostgreSql93Lateral(queryBuilder));

            Validate(
                "Pagination",
                () => BuildPagination(queryBuilder));

            Validate(
                "INTERSECT",
                () => BuildIntersect(queryBuilder));

            Validate(
                "EXCEPT",
                () => BuildExcept(queryBuilder));

            Console.WriteLine();
        }

        private static void ValidateMySql57(
            QueryBuilder<MySql57Profile> queryBuilder)
        {
            PrintProfile("MySQL 5.7");

            Validate(
                "Pagination",
                () => BuildPagination(queryBuilder));

            Console.WriteLine();
        }

        private static void ValidateMySql80(
            QueryBuilder<MySql80Profile> queryBuilder)
        {
            PrintProfile("MySQL 8.0");

            Validate(
                "Pagination",
                () => BuildPagination(queryBuilder));

            Validate(
                "CTE",
                () => BuildMySql80Cte(queryBuilder));

            Validate(
                "Recursive CTE",
                () => BuildMySql80RecursiveCte(queryBuilder));

            Validate(
                "Window Function",
                () => BuildWindow(queryBuilder));

            Console.WriteLine();
        }

        private static void ValidateMySql8014(
            QueryBuilder<MySql8014Profile> queryBuilder)
        {
            PrintProfile("MySQL 8.0.14");

            Validate(
                "Pagination",
                () => BuildPagination(queryBuilder));

            Validate(
                "CTE",
                () => BuildMySql8014Cte(queryBuilder));

            Validate(
                "Recursive CTE",
                () => BuildMySql8014RecursiveCte(queryBuilder));

            Validate(
                "Window Function",
                () => BuildWindow(queryBuilder));

            Validate(
                "LATERAL",
                () => BuildMySql8014Lateral(queryBuilder));

            Console.WriteLine();
        }

        private static void ValidateMySql8031(
            QueryBuilder<MySql8031Profile> queryBuilder)
        {
            PrintProfile("MySQL 8.0.31");

            Validate(
                "Pagination",
                () => BuildPagination(queryBuilder));

            Validate(
                "CTE",
                () => BuildMySql8031Cte(queryBuilder));

            Validate(
                "Recursive CTE",
                () => BuildMySql8031RecursiveCte(queryBuilder));

            Validate(
                "Window Function",
                () => BuildWindow(queryBuilder));

            Validate(
                "LATERAL",
                () => BuildMySql8031Lateral(queryBuilder));

            Validate(
                "INTERSECT",
                () => BuildIntersect(queryBuilder));

            Validate(
                "EXCEPT",
                () => BuildExcept(queryBuilder));

            Console.WriteLine();
        }

        private static GeneratedSqlQuery BuildPagination<TProfile>(
            IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile, IPaginationFeature
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .OrderBy<JoinUser>(u => u.Id)
                .Skip(20)
                .Take(10)
                .Build();
        }

        private static GeneratedSqlQuery BuildWindow<TProfile>(
            IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile, IWindowFunctionFeature
        {
            return queryBuilder
                .From<JoinOrder>(alias: "o")
                .Select<JoinOrder>(o => new
                {
                    OrderId = o.Id,
                    o.UserId,
                    o.Total
                })
                .SelectRowNumber(
                    alias: "UserOrderRank",
                    window => window
                        .PartitionBy<JoinOrder>(o => o.UserId)
                        .OrderByDescending<JoinOrder>(o => o.Total))
                .Build();
        }

        private static GeneratedSqlQuery BuildIntersect<TProfile>(
            IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile, IIntersectFeature
        {
            return queryBuilder
                .From<ActiveUser>(alias: "u")
                .Select<ActiveUser>(u => new
                {
                    u.Email
                })
                .Intersect(set => set
                    .From<ArchivedUser>(alias: "a")
                    .Select<ArchivedUser>(a => new
                    {
                        a.Email
                    }))
                .Build();
        }

        private static GeneratedSqlQuery BuildExcept<TProfile>(
            IQueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile, IExceptFeature
        {
            return queryBuilder
                .From<ActiveUser>(alias: "u")
                .Select<ActiveUser>(u => new
                {
                    u.Email
                })
                .Except(set => set
                    .From<ArchivedUser>(alias: "a")
                    .Select<ArchivedUser>(a => new
                    {
                        a.Email
                    }))
                .Build();
        }

        private static GeneratedSqlQuery BuildSqlServer2008Cte(
            QueryBuilder<SqlServer2008Profile> queryBuilder)
        {
            return queryBuilder
                .With<OrderSummary, JoinOrder>(
                    "order_summary",
                    cte => cte
                        .From<JoinOrder>(alias: "o")
                        .Select<JoinOrder>(o => new
                        {
                            UserId = o.UserId
                        }))
                .FromCte<OrderSummary>("order_summary")
                .Select<OrderSummary>(summary => new
                {
                    summary.UserId
                })
                .Build();
        }

        private static GeneratedSqlQuery BuildSqlServer2012Cte(
            QueryBuilder<SqlServer2012Profile> queryBuilder)
        {
            return queryBuilder
                .With<OrderSummary, JoinOrder>(
                    "order_summary",
                    cte => cte
                        .From<JoinOrder>(alias: "o")
                        .Select<JoinOrder>(o => new
                        {
                            UserId = o.UserId
                        }))
                .FromCte<OrderSummary>("order_summary")
                .Select<OrderSummary>(summary => new
                {
                    summary.UserId
                })
                .Build();
        }

        private static GeneratedSqlQuery BuildPostgreSql84Cte(
            QueryBuilder<PostgreSql84Profile> queryBuilder)
        {
            return queryBuilder
                .With<OrderSummary, JoinOrder>(
                    "order_summary",
                    cte => cte
                        .From<JoinOrder>(alias: "o")
                        .Select<JoinOrder>(o => new
                        {
                            UserId = o.UserId
                        }))
                .FromCte<OrderSummary>("order_summary")
                .Select<OrderSummary>(summary => new
                {
                    summary.UserId
                })
                .Build();
        }

        private static GeneratedSqlQuery BuildPostgreSql93Cte(
            QueryBuilder<PostgreSql93Profile> queryBuilder)
        {
            return queryBuilder
                .With<OrderSummary, JoinOrder>(
                    "order_summary",
                    cte => cte
                        .From<JoinOrder>(alias: "o")
                        .Select<JoinOrder>(o => new
                        {
                            UserId = o.UserId
                        }))
                .FromCte<OrderSummary>("order_summary")
                .Select<OrderSummary>(summary => new
                {
                    summary.UserId
                })
                .Build();
        }

        private static GeneratedSqlQuery BuildMySql80Cte(
            QueryBuilder<MySql80Profile> queryBuilder)
        {
            return queryBuilder
                .With<OrderSummary, JoinOrder>(
                    "order_summary",
                    cte => cte
                        .From<JoinOrder>(alias: "o")
                        .Select<JoinOrder>(o => new
                        {
                            UserId = o.UserId
                        }))
                .FromCte<OrderSummary>("order_summary")
                .Select<OrderSummary>(summary => new
                {
                    summary.UserId
                })
                .Build();
        }

        private static GeneratedSqlQuery BuildMySql8014Cte(
            QueryBuilder<MySql8014Profile> queryBuilder)
        {
            return queryBuilder
                .With<OrderSummary, JoinOrder>(
                    "order_summary",
                    cte => cte
                        .From<JoinOrder>(alias: "o")
                        .Select<JoinOrder>(o => new
                        {
                            UserId = o.UserId
                        }))
                .FromCte<OrderSummary>("order_summary")
                .Select<OrderSummary>(summary => new
                {
                    summary.UserId
                })
                .Build();
        }

        private static GeneratedSqlQuery BuildMySql8031Cte(
            QueryBuilder<MySql8031Profile> queryBuilder)
        {
            return queryBuilder
                .With<OrderSummary, JoinOrder>(
                    "order_summary",
                    cte => cte
                        .From<JoinOrder>(alias: "o")
                        .Select<JoinOrder>(o => new
                        {
                            UserId = o.UserId
                        }))
                .FromCte<OrderSummary>("order_summary")
                .Select<OrderSummary>(summary => new
                {
                    summary.UserId
                })
                .Build();
        }

        private static GeneratedSqlQuery BuildSqlServer2008RecursiveCte(
            QueryBuilder<SqlServer2008Profile> queryBuilder)
        {
            return queryBuilder
                .WithRecursive<CategoryTree, Category, Category>(
                    name: "category_tree",
                    baseQueryBuilder: baseQuery => baseQuery
                        .From<Category>(alias: "c")
                        .Select<Category>(c => new
                        {
                            c.Id,
                            c.ParentId,
                            c.Name
                        })
                        .Where<Category>(c => c.ParentId == null),
                    recursiveQueryBuilder: recursiveQuery => recursiveQuery
                        .From<Category>(alias: "c")
                        .InnerJoin<Category, CategoryTree>(
                            alias: "ct",
                            on: (c, ct) => c.ParentId == ct.Id)
                        .Select<Category>(c => new
                        {
                            c.Id,
                            c.ParentId,
                            c.Name
                        }))
                .FromCte<CategoryTree>("category_tree")
                .Select<CategoryTree>(tree => new
                {
                    tree.Id,
                    tree.ParentId,
                    tree.Name
                })
                .Build();
        }

        private static GeneratedSqlQuery BuildSqlServer2012RecursiveCte(
            QueryBuilder<SqlServer2012Profile> queryBuilder)
        {
            return queryBuilder
                .WithRecursive<CategoryTree, Category, Category>(
                    name: "category_tree",
                    baseQueryBuilder: baseQuery => baseQuery
                        .From<Category>(alias: "c")
                        .Select<Category>(c => new
                        {
                            c.Id,
                            c.ParentId,
                            c.Name
                        })
                        .Where<Category>(c => c.ParentId == null),
                    recursiveQueryBuilder: recursiveQuery => recursiveQuery
                        .From<Category>(alias: "c")
                        .InnerJoin<Category, CategoryTree>(
                            alias: "ct",
                            on: (c, ct) => c.ParentId == ct.Id)
                        .Select<Category>(c => new
                        {
                            c.Id,
                            c.ParentId,
                            c.Name
                        }))
                .FromCte<CategoryTree>("category_tree")
                .Select<CategoryTree>(tree => new
                {
                    tree.Id,
                    tree.ParentId,
                    tree.Name
                })
                .Build();
        }

        private static GeneratedSqlQuery BuildPostgreSql84RecursiveCte(
            QueryBuilder<PostgreSql84Profile> queryBuilder)
        {
            return queryBuilder
                .WithRecursive<CategoryTree, Category, Category>(
                    name: "category_tree",
                    baseQueryBuilder: baseQuery => baseQuery
                        .From<Category>(alias: "c")
                        .Select<Category>(c => new
                        {
                            c.Id,
                            c.ParentId,
                            c.Name
                        })
                        .Where<Category>(c => c.ParentId == null),
                    recursiveQueryBuilder: recursiveQuery => recursiveQuery
                        .From<Category>(alias: "c")
                        .InnerJoin<Category, CategoryTree>(
                            alias: "ct",
                            on: (c, ct) => c.ParentId == ct.Id)
                        .Select<Category>(c => new
                        {
                            c.Id,
                            c.ParentId,
                            c.Name
                        }))
                .FromCte<CategoryTree>("category_tree")
                .Select<CategoryTree>(tree => new
                {
                    tree.Id,
                    tree.ParentId,
                    tree.Name
                })
                .Build();
        }

        private static GeneratedSqlQuery BuildPostgreSql93RecursiveCte(
            QueryBuilder<PostgreSql93Profile> queryBuilder)
        {
            return queryBuilder
                .WithRecursive<CategoryTree, Category, Category>(
                    name: "category_tree",
                    baseQueryBuilder: baseQuery => baseQuery
                        .From<Category>(alias: "c")
                        .Select<Category>(c => new
                        {
                            c.Id,
                            c.ParentId,
                            c.Name
                        })
                        .Where<Category>(c => c.ParentId == null),
                    recursiveQueryBuilder: recursiveQuery => recursiveQuery
                        .From<Category>(alias: "c")
                        .InnerJoin<Category, CategoryTree>(
                            alias: "ct",
                            on: (c, ct) => c.ParentId == ct.Id)
                        .Select<Category>(c => new
                        {
                            c.Id,
                            c.ParentId,
                            c.Name
                        }))
                .FromCte<CategoryTree>("category_tree")
                .Select<CategoryTree>(tree => new
                {
                    tree.Id,
                    tree.ParentId,
                    tree.Name
                })
                .Build();
        }

        private static GeneratedSqlQuery BuildMySql80RecursiveCte(
            QueryBuilder<MySql80Profile> queryBuilder)
        {
            return queryBuilder
                .WithRecursive<CategoryTree, Category, Category>(
                    name: "category_tree",
                    baseQueryBuilder: baseQuery => baseQuery
                        .From<Category>(alias: "c")
                        .Select<Category>(c => new
                        {
                            c.Id,
                            c.ParentId,
                            c.Name
                        })
                        .Where<Category>(c => c.ParentId == null),
                    recursiveQueryBuilder: recursiveQuery => recursiveQuery
                        .From<Category>(alias: "c")
                        .InnerJoin<Category, CategoryTree>(
                            alias: "ct",
                            on: (c, ct) => c.ParentId == ct.Id)
                        .Select<Category>(c => new
                        {
                            c.Id,
                            c.ParentId,
                            c.Name
                        }))
                .FromCte<CategoryTree>("category_tree")
                .Select<CategoryTree>(tree => new
                {
                    tree.Id,
                    tree.ParentId,
                    tree.Name
                })
                .Build();
        }

        private static GeneratedSqlQuery BuildMySql8014RecursiveCte(
            QueryBuilder<MySql8014Profile> queryBuilder)
        {
            return queryBuilder
                .WithRecursive<CategoryTree, Category, Category>(
                    name: "category_tree",
                    baseQueryBuilder: baseQuery => baseQuery
                        .From<Category>(alias: "c")
                        .Select<Category>(c => new
                        {
                            c.Id,
                            c.ParentId,
                            c.Name
                        })
                        .Where<Category>(c => c.ParentId == null),
                    recursiveQueryBuilder: recursiveQuery => recursiveQuery
                        .From<Category>(alias: "c")
                        .InnerJoin<Category, CategoryTree>(
                            alias: "ct",
                            on: (c, ct) => c.ParentId == ct.Id)
                        .Select<Category>(c => new
                        {
                            c.Id,
                            c.ParentId,
                            c.Name
                        }))
                .FromCte<CategoryTree>("category_tree")
                .Select<CategoryTree>(tree => new
                {
                    tree.Id,
                    tree.ParentId,
                    tree.Name
                })
                .Build();
        }

        private static GeneratedSqlQuery BuildMySql8031RecursiveCte(
            QueryBuilder<MySql8031Profile> queryBuilder)
        {
            return queryBuilder
                .WithRecursive<CategoryTree, Category, Category>(
                    name: "category_tree",
                    baseQueryBuilder: baseQuery => baseQuery
                        .From<Category>(alias: "c")
                        .Select<Category>(c => new
                        {
                            c.Id,
                            c.ParentId,
                            c.Name
                        })
                        .Where<Category>(c => c.ParentId == null),
                    recursiveQueryBuilder: recursiveQuery => recursiveQuery
                        .From<Category>(alias: "c")
                        .InnerJoin<Category, CategoryTree>(
                            alias: "ct",
                            on: (c, ct) => c.ParentId == ct.Id)
                        .Select<Category>(c => new
                        {
                            c.Id,
                            c.ParentId,
                            c.Name
                        }))
                .FromCte<CategoryTree>("category_tree")
                .Select<CategoryTree>(tree => new
                {
                    tree.Id,
                    tree.ParentId,
                    tree.Name
                })
                .Build();
        }

        private static GeneratedSqlQuery BuildSqlServer2008Apply(
            IQueryBuilder<SqlServer2008Profile> queryBuilder)
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .CrossApply<JoinUser, JoinOrder>(
                    alias: "latest_order",
                    applyBuilder: apply => apply
                        .Select<JoinOrder>(o => new
                        {
                            OrderId = o.Id,
                            o.UserId,
                            o.Total
                        })
                        .WhereComputed<JoinOrder, JoinUser>(
                            (o, u) => o.UserId == u.Id)
                        .OrderByDescending<JoinOrder>(o => o.Total))
                .Build();
        }

        private static GeneratedSqlQuery BuildSqlServer2012Apply(
            IQueryBuilder<SqlServer2012Profile> queryBuilder)
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .CrossApply<JoinUser, JoinOrder>(
                    alias: "latest_order",
                    applyBuilder: apply => apply
                        .Select<JoinOrder>(o => new
                        {
                            OrderId = o.Id,
                            o.UserId,
                            o.Total
                        })
                        .WhereComputed<JoinOrder, JoinUser>(
                            (o, u) => o.UserId == u.Id)
                        .OrderByDescending<JoinOrder>(o => o.Total))
                .Build();
        }

        private static GeneratedSqlQuery BuildPostgreSql93Lateral(
            IQueryBuilder<PostgreSql93Profile> queryBuilder)
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .CrossApply<JoinUser, JoinOrder>(
                    alias: "latest_order",
                    applyBuilder: apply => apply
                        .Select<JoinOrder>(o => new
                        {
                            OrderId = o.Id,
                            o.UserId,
                            o.Total
                        })
                        .WhereComputed<JoinOrder, JoinUser>(
                            (o, u) => o.UserId == u.Id)
                        .OrderByDescending<JoinOrder>(o => o.Total))
                .Build();
        }

        private static GeneratedSqlQuery BuildMySql8014Lateral(
            IQueryBuilder<MySql8014Profile> queryBuilder)
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .CrossApply<JoinUser, JoinOrder>(
                    alias: "latest_order",
                    applyBuilder: apply => apply
                        .Select<JoinOrder>(o => new
                        {
                            OrderId = o.Id,
                            o.UserId,
                            o.Total
                        })
                        .WhereComputed<JoinOrder, JoinUser>(
                            (o, u) => o.UserId == u.Id)
                        .OrderByDescending<JoinOrder>(o => o.Total))
                .Build();
        }

        private static GeneratedSqlQuery BuildMySql8031Lateral(
            IQueryBuilder<MySql8031Profile> queryBuilder)
        {
            return queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .CrossApply<JoinUser, JoinOrder>(
                    alias: "latest_order",
                    applyBuilder: apply => apply
                        .Select<JoinOrder>(o => new
                        {
                            OrderId = o.Id,
                            o.UserId,
                            o.Total
                        })
                        .WhereComputed<JoinOrder, JoinUser>(
                            (o, u) => o.UserId == u.Id)
                        .OrderByDescending<JoinOrder>(o => o.Total))
                .Build();
        }

        private static void Validate(
            string feature,
            Func<GeneratedSqlQuery> queryFactory)
        {
            queryFactory();

            Console.WriteLine($"{feature}: PASS");
        }

        private static void PrintProfile(string profile)
        {
            Console.WriteLine($"--- {profile} Profile Capabilities ---");
        }
    }
}
