namespace TinyBlueWhale.EngineQuery.Tests.Providers
{
    /// <summary>
    /// Provides compilation sources used to validate generated provider surfaces.
    /// </summary>
    internal static class QueryTestProviderSource
    {
        public const string SqlServer2012Pagination = """
            using TinyBlueWhale.EngineQuery.Abstractions.Extensions;
            using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
            using TinyBlueWhale.EngineQuery.SqlServer.Profiles;

            namespace GeneratorContractTests
            {
                public sealed class TestProfile : SqlServer2012Profile
                {
                }

                public sealed class TestEntity
                {
                    public int Id { get; set; }
                }

                public static class Scenario
                {
                    public static void Build(
                        IQueryBuilder<TestProfile> queryBuilder)
                    {
                        queryBuilder
                            .From<TestEntity>(alias: "e")
                            .OrderBy<TestEntity>(e => e.Id)
                            .Skip(10)
                            .Take(10);
                    }
                }
            }
            """;

        public const string SqlServer2008Pagination = """
            using TinyBlueWhale.EngineQuery.Abstractions.Extensions;
            using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
            using TinyBlueWhale.EngineQuery.SqlServer.Profiles;

            namespace GeneratorContractTests
            {
                public sealed class TestProfile : SqlServer2008Profile
                {
                }

                public sealed class TestEntity
                {
                    public int Id { get; set; }
                }

                public static class Scenario
                {
                    public static void Build(
                        IQueryBuilder<TestProfile> queryBuilder)
                    {
                        queryBuilder
                            .From<TestEntity>(alias: "e")
                            .OrderBy<TestEntity>(e => e.Id)
                            .Skip(10)
                            .Take(10);
                    }
                }
            }
            """;

        public const string PostgreSql93Lateral = """
            using TinyBlueWhale.EngineQuery.Abstractions.Extensions;
            using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
            using TinyBlueWhale.EngineQuery.PostgreSql.Profiles;

            namespace GeneratorContractTests
            {
                public sealed class User
                {
                    public int Id { get; set; }
                }

                public sealed class Order
                {
                    public int Id { get; set; }

                    public int UserId { get; set; }
                }

                public static class Scenario
                {
                    public static void Build(
                        IQueryBuilder<PostgreSql93Profile> queryBuilder)
                    {
                        queryBuilder
                            .From<User>(alias: "u")
                            .CrossApply<User, Order>(
                                alias: "o",
                                applyBuilder: apply => apply);
                    }
                }
            }
            """;

        public const string PostgreSql84Lateral = """
            using TinyBlueWhale.EngineQuery.Abstractions.Extensions;
            using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
            using TinyBlueWhale.EngineQuery.PostgreSql.Profiles;

            namespace GeneratorContractTests
            {
                public sealed class TestProfile : PostgreSql84Profile
                {
                }

                public sealed class User
                {
                    public int Id { get; set; }
                }

                public sealed class Order
                {
                    public int Id { get; set; }

                    public int UserId { get; set; }
                }

                public static class Scenario
                {
                    public static void Build(
                        IQueryBuilder<TestProfile> queryBuilder)
                    {
                        queryBuilder
                            .From<User>(alias: "u")
                            .CrossApply<User, Order>(
                                alias: "o",
                                applyBuilder: apply => apply);
                    }
                }
            }
            """;

        public const string SqlServerScalarIdentity = """
            using TinyBlueWhale.EngineQuery.Abstractions.Extensions;
            using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
            using TinyBlueWhale.EngineQuery.SqlServer.Profiles;

            namespace GeneratorContractTests
            {
                public sealed class TestEntity
                {
                    public int Id { get; set; }

                    public string Name { get; set; } = string.Empty;
                }

                public static class Scenario
                {
                    public static void Build(
                        IQueryBuilder<SqlServer2012Profile> queryBuilder)
                    {
                        queryBuilder
                            .InsertInto<TestEntity>()
                            .Set(entity => entity.Name, "Test")
                            .ReturnIdentity()
                            .Build();
                    }
                }
            }
            """;

        public const string SqlServerReturningIdentity = """
            using TinyBlueWhale.EngineQuery.Abstractions.Extensions;
            using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
            using TinyBlueWhale.EngineQuery.SqlServer.Profiles;

            namespace GeneratorContractTests
            {
                public sealed class TestEntity
                {
                    public int Id { get; set; }

                    public string Name { get; set; } = string.Empty;
                }

                public static class Scenario
                {
                    public static void Build(
                        IQueryBuilder<SqlServer2012Profile> queryBuilder)
                    {
                        queryBuilder
                            .InsertInto<TestEntity>()
                            .Set(entity => entity.Name, "Test")
                            .ReturnIdentity(entity => entity.Id)
                            .Build();
                    }
                }
            }
            """;

        public const string PostgreSqlScalarIdentity = """
            using TinyBlueWhale.EngineQuery.Abstractions.Extensions;
            using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
            using TinyBlueWhale.EngineQuery.PostgreSql.Profiles;

            namespace GeneratorContractTests
            {
                public sealed class TestEntity
                {
                    public int Id { get; set; }

                    public string Name { get; set; } = string.Empty;
                }

                public static class Scenario
                {
                    public static void Build(
                        IQueryBuilder<PostgreSql93Profile> queryBuilder)
                    {
                        queryBuilder
                            .InsertInto<TestEntity>()
                            .Set(entity => entity.Name, "Test")
                            .ReturnIdentity()
                            .Build();
                    }
                }
            }
            """;

        public const string PostgreSqlReturningIdentity = """
            using TinyBlueWhale.EngineQuery.Abstractions.Extensions;
            using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
            using TinyBlueWhale.EngineQuery.PostgreSql.Profiles;

            namespace GeneratorContractTests
            {
                public sealed class TestEntity
                {
                    public int Id { get; set; }

                    public string Name { get; set; } = string.Empty;
                }

                public static class Scenario
                {
                    public static void Build(
                        IQueryBuilder<PostgreSql93Profile> queryBuilder)
                    {
                        queryBuilder
                            .InsertInto<TestEntity>()
                            .Set(entity => entity.Name, "Test")
                            .ReturnIdentity(entity => entity.Id)
                            .Build();
                    }
                }
            }
            """;

        public const string MySqlScalarIdentity = """
            using TinyBlueWhale.EngineQuery.Abstractions.Extensions;
            using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
            using TinyBlueWhale.EngineQuery.MySql.Profiles;

            namespace GeneratorContractTests
            {
                public sealed class TestEntity
                {
                    public int Id { get; set; }

                    public string Name { get; set; } = string.Empty;
                }

                public static class Scenario
                {
                    public static void Build(
                        IQueryBuilder<MySql8031Profile> queryBuilder)
                    {
                        queryBuilder
                            .InsertInto<TestEntity>()
                            .Set(entity => entity.Name, "Test")
                            .ReturnIdentity()
                            .Build();
                    }
                }
            }
            """;

        public const string MySqlReturningIdentity = """
            using TinyBlueWhale.EngineQuery.Abstractions.Extensions;
            using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
            using TinyBlueWhale.EngineQuery.MySql.Profiles;

            namespace GeneratorContractTests
            {
                public sealed class TestEntity
                {
                    public int Id { get; set; }

                    public string Name { get; set; } = string.Empty;
                }

                public static class Scenario
                {
                    public static void Build(
                        IQueryBuilder<MySql8031Profile> queryBuilder)
                    {
                        queryBuilder
                            .InsertInto<TestEntity>()
                            .Set(entity => entity.Name, "Test")
                            .ReturnIdentity(entity => entity.Id)
                            .Build();
                    }
                }
            }
            """;
    }
}
