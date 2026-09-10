using TinyBlueWhale.EngineQuery.Abstractions.Extensions;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding;
using TinyBlueWhale.EngineQuery.MySql.Compilation;
using TinyBlueWhale.EngineQuery.MySql.Profiles;
using TinyBlueWhale.EngineQuery.PostgreSql.Compilation;
using TinyBlueWhale.EngineQuery.PostgreSql.Profiles;
using TinyBlueWhale.EngineQuery.SqlServer.Compilation;
using TinyBlueWhale.EngineQuery.SqlServer.Profiles;
using TinyBlueWhale.EngineQuery.Tests.Helpers;
using TinyBlueWhale.EngineQuery.Tests.Infrastructure;
using TinyBlueWhale.EngineQuery.Tests.Models;

namespace TinyBlueWhale.EngineQuery.Tests.QueryBuilding.SetOperations
{
    /// <summary>
    /// Validates set operation behavior and projection arity requirements.
    /// </summary>
    [TestFixture]
    internal sealed class SetOperationTests
    {
        private QueryBuilder<SqlServer2012Profile> _sqlServer = null!;
        private QueryBuilder<PostgreSql93Profile> _postgreSql = null!;
        private QueryBuilder<MySql8031Profile> _mySql = null!;

        [SetUp]
        public void SetUp()
        {
            var metadataResolver = TestMetadataFactory.CreateMetadataResolver();

            _sqlServer = SqlServerQueryCompiler.Factory
                .Create<SqlServer2012Profile>(metadataResolver);

            _postgreSql = PostgreSqlQueryCompiler.Factory
                .Create<PostgreSql93Profile>(metadataResolver);

            _mySql = MySqlQueryCompiler.Factory
                .Create<MySql8031Profile>(metadataResolver);
        }

        [Test]
        public void Build_WhenUnionAllIsConfigured_ShouldGenerateUnionAll()
        {
            AssertUnionAll(_sqlServer);
            AssertUnionAll(_postgreSql);
            AssertUnionAll(_mySql);
        }

        [Test]
        public void Build_WhenUnionAllProjectionArityDiffers_ShouldThrowInvalidOperationException()
        {
            AssertUnionAllArityMismatch(_sqlServer);
            AssertUnionAllArityMismatch(_postgreSql);
            AssertUnionAllArityMismatch(_mySql);
        }

        [Test]
        public void Build_WhenUnionAllUsesDifferentEntitiesWithCompatibleProjection_ShouldGenerateUnionAll()
        {
            AssertDifferentEntityUnionAll(_sqlServer);
            AssertDifferentEntityUnionAll(_postgreSql);
            AssertDifferentEntityUnionAll(_mySql);
        }

        [Test]
        public void Build_WhenIntersectUsesDifferentEntitiesWithCompatibleProjection_ShouldGenerateIntersect()
        {
            var sqlServer = _sqlServer
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(user => new
                {
                    Value = user.Id
                })
                .Intersect(set => set
                    .From<JoinOrder>(alias: "o")
                    .Select<JoinOrder>(order => new
                    {
                        Value = order.UserId
                    }))
                .Build();

            var postgreSql = _postgreSql
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(user => new
                {
                    Value = user.Id
                })
                .Intersect(set => set
                    .From<JoinOrder>(alias: "o")
                    .Select<JoinOrder>(order => new
                    {
                        Value = order.UserId
                    }))
                .Build();

            var mySql = _mySql
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(user => new
                {
                    Value = user.Id
                })
                .Intersect(set => set
                    .From<JoinOrder>(alias: "o")
                    .Select<JoinOrder>(order => new
                    {
                        Value = order.UserId
                    }))
                .Build();

            AssertDifferentEntitySetOperation(sqlServer, "INTERSECT");
            AssertDifferentEntitySetOperation(postgreSql, "INTERSECT");
            AssertDifferentEntitySetOperation(mySql, "INTERSECT");
        }

        [Test]
        public void Build_WhenExceptUsesDifferentEntitiesWithCompatibleProjection_ShouldGenerateExcept()
        {
            var sqlServer = _sqlServer
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(user => new
                {
                    Value = user.Id
                })
                .Except(set => set
                    .From<JoinOrder>(alias: "o")
                    .Select<JoinOrder>(order => new
                    {
                        Value = order.UserId
                    }))
                .Build();

            var postgreSql = _postgreSql
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(user => new
                {
                    Value = user.Id
                })
                .Except(set => set
                    .From<JoinOrder>(alias: "o")
                    .Select<JoinOrder>(order => new
                    {
                        Value = order.UserId
                    }))
                .Build();

            var mySql = _mySql
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(user => new
                {
                    Value = user.Id
                })
                .Except(set => set
                    .From<JoinOrder>(alias: "o")
                    .Select<JoinOrder>(order => new
                    {
                        Value = order.UserId
                    }))
                .Build();

            AssertDifferentEntitySetOperation(sqlServer, "EXCEPT");
            AssertDifferentEntitySetOperation(postgreSql, "EXCEPT");
            AssertDifferentEntitySetOperation(mySql, "EXCEPT");
        }


        [Test]
        public void Build_WhenIntersectIsConfigured_ShouldGenerateIntersect()
        {
            var sqlServer = _sqlServer
                .From<ActiveUser>(alias: "u")
                .Select<ActiveUser>(user => new
                {
                    user.Email
                })
                .Intersect(set => set
                    .From<ArchivedUser>(alias: "a")
                    .Select<ArchivedUser>(user => new
                    {
                        user.Email
                    }))
                .Build();

            var postgreSql = _postgreSql
                .From<ActiveUser>(alias: "u")
                .Select<ActiveUser>(user => new
                {
                    user.Email
                })
                .Intersect(set => set
                    .From<ArchivedUser>(alias: "a")
                    .Select<ArchivedUser>(user => new
                    {
                        user.Email
                    }))
                .Build();

            var mySql = _mySql
                .From<ActiveUser>(alias: "u")
                .Select<ActiveUser>(user => new
                {
                    user.Email
                })
                .Intersect(set => set
                    .From<ArchivedUser>(alias: "a")
                    .Select<ArchivedUser>(user => new
                    {
                        user.Email
                    }))
                .Build();

            AssertSetOperation(sqlServer, "INTERSECT");
            AssertSetOperation(postgreSql, "INTERSECT");
            AssertSetOperation(mySql, "INTERSECT");
        }

        [Test]
        public void Build_WhenExceptIsConfigured_ShouldGenerateExcept()
        {
            var sqlServer = _sqlServer
                .From<ActiveUser>(alias: "u")
                .Select<ActiveUser>(user => new
                {
                    user.Email
                })
                .Except(set => set
                    .From<ArchivedUser>(alias: "a")
                    .Select<ArchivedUser>(user => new
                    {
                        user.Email
                    }))
                .Build();

            var postgreSql = _postgreSql
                .From<ActiveUser>(alias: "u")
                .Select<ActiveUser>(user => new
                {
                    user.Email
                })
                .Except(set => set
                    .From<ArchivedUser>(alias: "a")
                    .Select<ArchivedUser>(user => new
                    {
                        user.Email
                    }))
                .Build();

            var mySql = _mySql
                .From<ActiveUser>(alias: "u")
                .Select<ActiveUser>(user => new
                {
                    user.Email
                })
                .Except(set => set
                    .From<ArchivedUser>(alias: "a")
                    .Select<ArchivedUser>(user => new
                    {
                        user.Email
                    }))
                .Build();

            AssertSetOperation(sqlServer, "EXCEPT");
            AssertSetOperation(postgreSql, "EXCEPT");
            AssertSetOperation(mySql, "EXCEPT");
        }

        [Test]
        public void Build_WhenMultipleSetOperationsAreConfigured_ShouldPreserveOperationOrder()
        {
            var sqlServer = _sqlServer
                .From<ActiveUser>(alias: "u")
                .Select<ActiveUser>(user => new
                {
                    user.Email
                })
                .UnionAll(set => set
                    .From<ArchivedUser>(alias: "a")
                    .Select<ArchivedUser>(user => new
                    {
                        user.Email
                    }))
                .Intersect(set => set
                    .From<ArchivedUser>(alias: "a2")
                    .Select<ArchivedUser>(user => new
                    {
                        user.Email
                    }))
                .Except(set => set
                    .From<ArchivedUser>(alias: "a3")
                    .Select<ArchivedUser>(user => new
                    {
                        user.Email
                    }))
                .Build();

            var postgreSql = _postgreSql
                .From<ActiveUser>(alias: "u")
                .Select<ActiveUser>(user => new
                {
                    user.Email
                })
                .UnionAll(set => set
                    .From<ArchivedUser>(alias: "a")
                    .Select<ArchivedUser>(user => new
                    {
                        user.Email
                    }))
                .Intersect(set => set
                    .From<ArchivedUser>(alias: "a2")
                    .Select<ArchivedUser>(user => new
                    {
                        user.Email
                    }))
                .Except(set => set
                    .From<ArchivedUser>(alias: "a3")
                    .Select<ArchivedUser>(user => new
                    {
                        user.Email
                    }))
                .Build();

            var mySql = _mySql
                .From<ActiveUser>(alias: "u")
                .Select<ActiveUser>(user => new
                {
                    user.Email
                })
                .UnionAll(set => set
                    .From<ArchivedUser>(alias: "a")
                    .Select<ArchivedUser>(user => new
                    {
                        user.Email
                    }))
                .Intersect(set => set
                    .From<ArchivedUser>(alias: "a2")
                    .Select<ArchivedUser>(user => new
                    {
                        user.Email
                    }))
                .Except(set => set
                    .From<ArchivedUser>(alias: "a3")
                    .Select<ArchivedUser>(user => new
                    {
                        user.Email
                    }))
                .Build();

            AssertOperationOrder(sqlServer);
            AssertOperationOrder(postgreSql);
            AssertOperationOrder(mySql);
        }

        [Test]
        public void Build_WhenIntersectProjectionArityDiffers_ShouldThrowInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => _sqlServer
                .From<ActiveUser>()
                .Select<ActiveUser>(user => new
                {
                    user.Id,
                    user.Email
                })
                .Intersect(set => set
                    .From<ArchivedUser>()
                    .Select<ArchivedUser>(user => user.Email)));

            Assert.Throws<InvalidOperationException>(() => _postgreSql
                .From<ActiveUser>()
                .Select<ActiveUser>(user => new
                {
                    user.Id,
                    user.Email
                })
                .Intersect(set => set
                    .From<ArchivedUser>()
                    .Select<ArchivedUser>(user => user.Email)));

            Assert.Throws<InvalidOperationException>(() => _mySql
                .From<ActiveUser>()
                .Select<ActiveUser>(user => new
                {
                    user.Id,
                    user.Email
                })
                .Intersect(set => set
                    .From<ArchivedUser>()
                    .Select<ArchivedUser>(user => user.Email)));
        }

        [Test]
        public void Build_WhenExceptProjectionArityDiffers_ShouldThrowInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => _sqlServer
                .From<ActiveUser>()
                .Select<ActiveUser>(user => new
                {
                    user.Id,
                    user.Email
                })
                .Except(set => set
                    .From<ArchivedUser>()
                    .Select<ArchivedUser>(user => user.Email)));

            Assert.Throws<InvalidOperationException>(() => _postgreSql
                .From<ActiveUser>()
                .Select<ActiveUser>(user => new
                {
                    user.Id,
                    user.Email
                })
                .Except(set => set
                    .From<ArchivedUser>()
                    .Select<ArchivedUser>(user => user.Email)));

            Assert.Throws<InvalidOperationException>(() => _mySql
                .From<ActiveUser>()
                .Select<ActiveUser>(user => new
                {
                    user.Id,
                    user.Email
                })
                .Except(set => set
                    .From<ArchivedUser>()
                    .Select<ArchivedUser>(user => user.Email)));
        }

        private static void AssertUnionAll<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<ActiveUser>(alias: "u")
                .Select<ActiveUser>(user => new
                {
                    user.Email
                })
                .UnionAll(set => set
                    .From<ArchivedUser>(alias: "a")
                    .Select<ArchivedUser>(user => new
                    {
                        user.Email
                    }))
                .Build();

            AssertSetOperation(query, "UNION ALL");
        }

        private static void AssertUnionAllArityMismatch<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            Assert.Throws<InvalidOperationException>(() => queryBuilder
                .From<ActiveUser>()
                .Select<ActiveUser>(user => new
                {
                    user.Id,
                    user.Email
                })
                .UnionAll<ArchivedUser>(set => set
                    .From<ArchivedUser>()
                    .Select<ArchivedUser>(user => user.Email)));
        }

        private static void AssertDifferentEntityUnionAll<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(user => new
                {
                    Value = user.Id
                })
                .UnionAll(set => set
                    .From<JoinOrder>(alias: "o")
                    .Select<JoinOrder>(order => new
                    {
                        Value = order.UserId
                    }))
                .Build();

            AssertDifferentEntitySetOperation(query, "UNION ALL");
        }

        private static void AssertDifferentEntitySetOperation(GeneratedSqlQuery query, string operation)
        {
            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain(operation));
                Assert.That(sql, Does.Contain("users"));
                Assert.That(sql, Does.Contain("orders"));
                Assert.That(sql, Does.Contain("user_id"));
                Assert.That(query.Parameters, Is.Empty);
            });
        }

        private static void AssertSetOperation(GeneratedSqlQuery query, string operation)
        {
            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain(operation));
                Assert.That(sql, Does.Contain("users"));
                Assert.That(sql, Does.Contain("archived_users"));
                Assert.That(sql, Does.Contain("email"));
                Assert.That(query.Parameters, Is.Empty);
            });
        }

        private static void AssertOperationOrder(GeneratedSqlQuery query)
        {
            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);
            var unionAllIndex = sql.IndexOf("UNION ALL", StringComparison.Ordinal);
            var intersectIndex = sql.IndexOf("INTERSECT", StringComparison.Ordinal);
            var exceptIndex = sql.IndexOf("EXCEPT", StringComparison.Ordinal);

            Assert.Multiple(() =>
            {
                Assert.That(unionAllIndex, Is.GreaterThanOrEqualTo(0));
                Assert.That(intersectIndex, Is.GreaterThan(unionAllIndex));
                Assert.That(exceptIndex, Is.GreaterThan(intersectIndex));
                Assert.That(query.Parameters, Is.Empty);
            });
        }
    }
}
