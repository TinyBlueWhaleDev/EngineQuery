using NUnit.Framework.Internal;
using TinyBlueWhale.EngineQuery.Abstractions.Extensions;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding;
using TinyBlueWhale.EngineQuery.MySql.Compilation;
using TinyBlueWhale.EngineQuery.MySql.Profiles;
using TinyBlueWhale.EngineQuery.PostgreSql.Compilation;
using TinyBlueWhale.EngineQuery.PostgreSql.Profiles;
using TinyBlueWhale.EngineQuery.SqlServer.Compilation;
using TinyBlueWhale.EngineQuery.SqlServer.Profiles;
using TinyBlueWhale.EngineQuery.Tests.Infrastructure;
using TinyBlueWhale.EngineQuery.Tests.Models;

namespace TinyBlueWhale.EngineQuery.Tests.QueryBuilding.Apply
{
    /// <summary>
    /// Validates APPLY and LATERAL query behavior.
    /// </summary>
    [TestFixture]
    internal sealed class ApplyTests
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
        public void CrossApply_WhenConfigured_ShouldGenerateProviderSpecificLateralSource()
        {
            var metadataResolver = TestMetadataFactory.CreateMetadataResolver();

            var sqlServer = _sqlServer
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .CrossApply<JoinUser, JoinOrder>(
                    alias: "latest_order",
                    apply => apply
                        .Select<JoinOrder>(o => new
                        {
                            OrderId = o.Id,
                            o.UserId,
                            o.Total
                        })
                        .WhereComputed<JoinOrder, JoinUser>(
                            (o, u) => o.UserId == u.Id))
                .Build();

            var postgreSql = _postgreSql
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .CrossApply<JoinUser, JoinOrder>(
                    alias: "latest_order",
                    apply => apply
                        .Select<JoinOrder>(o => new
                        {
                            OrderId = o.Id,
                            o.UserId,
                            o.Total
                        })
                        .WhereComputed<JoinOrder, JoinUser>(
                            (o, u) => o.UserId == u.Id))
                .Build();

            var mySql = _mySql
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .CrossApply<JoinUser, JoinOrder>(
                    alias: "latest_order",
                    apply => apply
                        .Select<JoinOrder>(o => new
                        {
                            OrderId = o.Id,
                            o.UserId,
                            o.Total
                        })
                        .WhereComputed<JoinOrder, JoinUser>(
                            (o, u) => o.UserId == u.Id))
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(sqlServer.CommandText, Does.Contain("CROSS APPLY"));
                Assert.That(sqlServer.CommandText, Does.Contain("latest_order"));

                Assert.That(postgreSql.CommandText, Does.Contain("LATERAL"));
                Assert.That(postgreSql.CommandText, Does.Contain("latest_order"));

                Assert.That(mySql.CommandText, Does.Contain("LATERAL"));
                Assert.That(mySql.CommandText, Does.Contain("latest_order"));
            });
        }

        [Test]
        public void OuterApply_WhenConfigured_ShouldGenerateProviderSpecificLateralSource()
        {

            var sqlServer = _sqlServer
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => u.Id)
                .OuterApply<JoinUser, JoinOrder>(
                    alias: "latest_order",
                    apply => apply
                        .Select<JoinOrder>(o => new
                        {
                            OrderId = o.Id,
                            o.UserId,
                            o.Total
                        })
                        .WhereComputed<JoinOrder, JoinUser>(
                            (o, u) => o.UserId == u.Id))
                .Build();

            var postgreSql = _postgreSql
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => u.Id)
                .OuterApply<JoinUser, JoinOrder>(
                    alias: "latest_order",
                    apply => apply
                        .Select<JoinOrder>(o => new
                        {
                            OrderId = o.Id,
                            o.UserId,
                            o.Total
                        })
                        .WhereComputed<JoinOrder, JoinUser>(
                            (o, u) => o.UserId == u.Id))
                .Build();

            var mySql = _mySql
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => u.Id)
                .OuterApply<JoinUser, JoinOrder>(
                    alias: "latest_order",
                    apply => apply
                        .Select<JoinOrder>(o => new
                        {
                            OrderId = o.Id,
                            o.UserId,
                            o.Total
                        })
                        .WhereComputed<JoinOrder, JoinUser>(
                            (o, u) => o.UserId == u.Id))
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(sqlServer.CommandText, Does.Contain("OUTER APPLY"));
                Assert.That(sqlServer.CommandText, Does.Contain("latest_order"));

                Assert.That(postgreSql.CommandText, Does.Contain("LATERAL"));
                Assert.That(postgreSql.CommandText, Does.Contain("latest_order"));

                Assert.That(mySql.CommandText, Does.Contain("LATERAL"));
                Assert.That(mySql.CommandText, Does.Contain("latest_order"));
            });
        }

        [Test]
        public void CrossApply_WhenCorrelated_ShouldResolveOuterSource()
        {

            var sqlServer = _sqlServer
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => u.Id)
                .CrossApply<JoinUser, JoinOrder>(
                    alias: "o",
                    apply => apply
                        .Select<JoinOrder>(o => o.Id)
                        .WhereComputed<JoinOrder, JoinUser>(
                            (o, u) => o.UserId == u.Id))
                .Build();

            var postgreSql = _postgreSql
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => u.Id)
                .CrossApply<JoinUser, JoinOrder>(
                    alias: "o",
                    apply => apply
                        .Select<JoinOrder>(o => o.Id)
                        .WhereComputed<JoinOrder, JoinUser>(
                            (o, u) => o.UserId == u.Id))
                .Build();

            var mySql = _mySql
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => u.Id)
                .CrossApply<JoinUser, JoinOrder>(
                    alias: "o",
                    apply => apply
                        .Select<JoinOrder>(o => o.Id)
                        .WhereComputed<JoinOrder, JoinUser>(
                            (o, u) => o.UserId == u.Id))
                .Build();

            var sqlServerSql = NormalizeSql(sqlServer.CommandText);
            var postgreSqlSql = NormalizeSql(postgreSql.CommandText);
            var mySqlSql = NormalizeSql(mySql.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(
                    sqlServerSql,
                    Does.Contain("o.user_id = u.user_id"));

                Assert.That(
                    postgreSqlSql,
                    Does.Contain("o.user_id = u.user_id"));

                Assert.That(
                    mySqlSql,
                    Does.Contain("o.user_id = u.user_id"));
            });
        }

        [Test]
        public void OuterApply_WhenCorrelated_ShouldResolveOuterSource()
        {

            var sqlServer = _sqlServer
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => u.Id)
                .OuterApply<JoinUser, JoinOrder>(
                    alias: "o",
                    apply => apply
                        .Select<JoinOrder>(o => o.Id)
                        .WhereComputed<JoinOrder, JoinUser>(
                            (o, u) => o.UserId == u.Id))
                .Build();

            var postgreSql = _postgreSql
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => u.Id)
                .OuterApply<JoinUser, JoinOrder>(
                    alias: "o",
                    apply => apply
                        .Select<JoinOrder>(o => o.Id)
                        .WhereComputed<JoinOrder, JoinUser>(
                            (o, u) => o.UserId == u.Id))
                .Build();

            var mySql = _mySql
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => u.Id)
                .OuterApply<JoinUser, JoinOrder>(
                    alias: "o",
                    apply => apply
                        .Select<JoinOrder>(o => o.Id)
                        .WhereComputed<JoinOrder, JoinUser>(
                            (o, u) => o.UserId == u.Id))
                .Build();

            var sqlServerSql = NormalizeSql(sqlServer.CommandText);
            var postgreSqlSql = NormalizeSql(postgreSql.CommandText);
            var mySqlSql = NormalizeSql(mySql.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(
                    sqlServerSql,
                    Does.Contain("o.user_id = u.user_id"));

                Assert.That(
                    postgreSqlSql,
                    Does.Contain("o.user_id = u.user_id"));

                Assert.That(
                    mySqlSql,
                    Does.Contain("o.user_id = u.user_id"));
            });
        }

        [Test]
        public void CrossApply_WhenNestedQueryContainsOrderingAndTake_ShouldPreserveNestedClauses()
        {
            var sqlServer = _sqlServer
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => u.Id)
                .CrossApply<JoinUser, JoinOrder>(
                    alias: "latest_order",
                    apply => apply
                        .Select<JoinOrder>(o => new
                        {
                            OrderId = o.Id,
                            o.Total
                        })
                        .WhereComputed<JoinOrder, JoinUser>(
                            (o, u) => o.UserId == u.Id)
                        .OrderByDescending<JoinOrder>(o => o.Total)
                        .Take(1))
                .Build();

            var postgreSql = _postgreSql
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => u.Id)
                .CrossApply<JoinUser, JoinOrder>(
                    alias: "latest_order",
                    apply => apply
                        .Select<JoinOrder>(o => new
                        {
                            OrderId = o.Id,
                            o.Total
                        })
                        .WhereComputed<JoinOrder, JoinUser>(
                            (o, u) => o.UserId == u.Id)
                        .OrderByDescending<JoinOrder>(o => o.Total)
                        .Take(1))
                .Build();

            var mySql = _mySql
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => u.Id)
                .CrossApply<JoinUser, JoinOrder>(
                    alias: "latest_order",
                    apply => apply
                        .Select<JoinOrder>(o => new
                        {
                            OrderId = o.Id,
                            o.Total
                        })
                        .WhereComputed<JoinOrder, JoinUser>(
                            (o, u) => o.UserId == u.Id)
                        .OrderByDescending<JoinOrder>(o => o.Total)
                        .Take(1))
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(sqlServer.CommandText, Does.Contain("ORDER BY"));
                Assert.That(sqlServer.CommandText, Does.Contain("total"));

                Assert.That(postgreSql.CommandText, Does.Contain("ORDER BY"));
                Assert.That(postgreSql.CommandText, Does.Contain("total"));

                Assert.That(mySql.CommandText, Does.Contain("ORDER BY"));
                Assert.That(mySql.CommandText, Does.Contain("total"));
            });
        }

        [Test]
        public void CrossApply_WhenAliasIsConfigured_ShouldPreserveAlias()
        {

            var sqlServer = _sqlServer
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => u.Id)
                .CrossApply<JoinUser, JoinOrder>(
                    alias: "latest_order",
                    apply => apply
                        .Select<JoinOrder>(o => o.Id)
                        .WhereComputed<JoinOrder, JoinUser>(
                            (o, u) => o.UserId == u.Id))
                .Build();

            var postgreSql = _postgreSql
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => u.Id)
                .CrossApply<JoinUser, JoinOrder>(
                    alias: "latest_order",
                    apply => apply
                        .Select<JoinOrder>(o => o.Id)
                        .WhereComputed<JoinOrder, JoinUser>(
                            (o, u) => o.UserId == u.Id))
                .Build();

            var mySql = _mySql
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => u.Id)
                .CrossApply<JoinUser, JoinOrder>(
                    alias: "latest_order",
                    apply => apply
                        .Select<JoinOrder>(o => o.Id)
                        .WhereComputed<JoinOrder, JoinUser>(
                            (o, u) => o.UserId == u.Id))
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(
                    NormalizeSql(sqlServer.CommandText),
                    Does.Contain("latest_order"));

                Assert.That(
                    NormalizeSql(postgreSql.CommandText),
                    Does.Contain("latest_order"));

                Assert.That(
                    NormalizeSql(mySql.CommandText),
                    Does.Contain("latest_order"));
            });
        }

        [Test]
        public void CrossApply_WhenNestedQueryContainsParameter_ShouldPreserveParameter()
        {

            var sqlServer = _sqlServer
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => u.Id)
                .CrossApply<JoinUser, JoinOrder>(
                    alias: "latest_order",
                    apply => apply
                        .Select<JoinOrder>(o => o.Id)
                        .WhereComputed<JoinOrder, JoinUser>(
                            (o, u) =>
                                o.UserId == u.Id
                                && o.Total > 100))
                .Build();

            var postgreSql = _postgreSql
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => u.Id)
                .CrossApply<JoinUser, JoinOrder>(
                    alias: "latest_order",
                    apply => apply
                        .Select<JoinOrder>(o => o.Id)
                        .WhereComputed<JoinOrder, JoinUser>(
                            (o, u) =>
                                o.UserId == u.Id
                                && o.Total > 100))
                .Build();

            var mySql = _mySql
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => u.Id)
                .CrossApply<JoinUser, JoinOrder>(
                    alias: "latest_order",
                    apply => apply
                        .Select<JoinOrder>(o => o.Id)
                        .WhereComputed<JoinOrder, JoinUser>(
                            (o, u) =>
                                o.UserId == u.Id
                                && o.Total > 100))
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(sqlServer.Parameters, Has.Count.EqualTo(1));
                Assert.That(sqlServer.Parameters[0].Name, Is.EqualTo("@p0"));
                Assert.That(sqlServer.Parameters[0].Value, Is.EqualTo(100));

                Assert.That(postgreSql.Parameters, Has.Count.EqualTo(1));
                Assert.That(postgreSql.Parameters[0].Name, Is.EqualTo("@p0"));
                Assert.That(postgreSql.Parameters[0].Value, Is.EqualTo(100));

                Assert.That(mySql.Parameters, Has.Count.EqualTo(1));
                Assert.That(mySql.Parameters[0].Name, Is.EqualTo("@p0"));
                Assert.That(mySql.Parameters[0].Value, Is.EqualTo(100));
            });
        }

        [Test]
        public void CrossApply_WhenCombinedWithJoinAndWhere_ShouldPreserveClauseOrder()
        {

            var sqlServer = _sqlServer
                .From<JoinUser>(alias: "u")
                .InnerJoin<JoinUser, JoinOrder>(
                    alias: "o",
                    on: (u, o) => u.Id == o.UserId)
                .Select<JoinUser>(u => u.Id)
                .CrossApply<JoinUser, JoinOrder>(
                    alias: "latest_order",
                    apply => apply
                        .Select<JoinOrder>(latest => latest.Id)
                        .WhereComputed<JoinOrder, JoinUser>(
                            (latest, u) =>
                                latest.UserId == u.Id))
                .Where<JoinUser>(u => u.IsActive)
                .Build();

            var postgreSql = _postgreSql
                .From<JoinUser>(alias: "u")
                .InnerJoin<JoinUser, JoinOrder>(
                    alias: "o",
                    on: (u, o) => u.Id == o.UserId)
                .Select<JoinUser>(u => u.Id)
                .CrossApply<JoinUser, JoinOrder>(
                    alias: "latest_order",
                    apply => apply
                        .Select<JoinOrder>(latest => latest.Id)
                        .WhereComputed<JoinOrder, JoinUser>(
                            (latest, u) =>
                                latest.UserId == u.Id))
                .Where<JoinUser>(u => u.IsActive)
                .Build();

            var mySql = _mySql
                .From<JoinUser>(alias: "u")
                .InnerJoin<JoinUser, JoinOrder>(
                    alias: "o",
                    on: (u, o) => u.Id == o.UserId)
                .Select<JoinUser>(u => u.Id)
                .CrossApply<JoinUser, JoinOrder>(
                    alias: "latest_order",
                    apply => apply
                        .Select<JoinOrder>(latest => latest.Id)
                        .WhereComputed<JoinOrder, JoinUser>(
                            (latest, u) =>
                                latest.UserId == u.Id))
                .Where<JoinUser>(u => u.IsActive)
                .Build();

            AssertClauseOrder(
                sqlServer.CommandText,
                "CROSS APPLY");

            AssertClauseOrder(
                postgreSql.CommandText,
                "LATERAL");

            AssertClauseOrder(
                mySql.CommandText,
                "LATERAL");
        }

        private static void AssertClauseOrder(
            string commandText,
            string lateralToken)
        {
            var fromIndex = commandText.IndexOf(
                "FROM",
                StringComparison.Ordinal);

            var joinIndex = commandText.IndexOf(
                "INNER JOIN",
                fromIndex,
                StringComparison.Ordinal);

            var lateralIndex = commandText.IndexOf(
                lateralToken,
                joinIndex,
                StringComparison.Ordinal);

            var whereIndex = commandText.LastIndexOf(
                "WHERE",
                StringComparison.Ordinal);

            Assert.Multiple(() =>
            {
                Assert.That(fromIndex, Is.GreaterThanOrEqualTo(0));
                Assert.That(joinIndex, Is.GreaterThan(fromIndex));
                Assert.That(lateralIndex, Is.GreaterThan(joinIndex));
                Assert.That(whereIndex, Is.GreaterThan(lateralIndex));
            });
        }

        private static string NormalizeSql(string commandText)
        {
            return commandText
                .Replace("[", string.Empty, StringComparison.Ordinal)
                .Replace("]", string.Empty, StringComparison.Ordinal)
                .Replace("\"", string.Empty, StringComparison.Ordinal)
                .Replace("`", string.Empty, StringComparison.Ordinal);
        }
    }

}
