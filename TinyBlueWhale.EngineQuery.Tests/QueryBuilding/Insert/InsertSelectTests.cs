using System.Linq.Expressions;
using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
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

namespace TinyBlueWhale.EngineQuery.Tests.QueryBuilding.Insert
{
    /// <summary>
    /// Validates provider-independent INSERT SELECT behavior.
    /// </summary>
    [TestFixture]
    internal sealed class InsertSelectTests
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
        public void Build_WhenInsertSelectIsValid_ShouldGenerateExpectedSql()
        {
            AssertValidInsertSelect(_sqlServer);
            AssertValidInsertSelect(_postgreSql);
            AssertValidInsertSelect(_mySql);
        }

        [Test]
        public void Build_WhenJoinIsConfigured_ShouldGenerateJoin()
        {
            AssertInsertSelectWithJoin(_sqlServer);
            AssertInsertSelectWithJoin(_postgreSql);
            AssertInsertSelectWithJoin(_mySql);
        }

        [Test]
        public void Build_WhenSourceUsesDifferentEntity_ShouldPreserveInsertTarget()
        {
            AssertDifferentSourceEntity(_sqlServer);
            AssertDifferentSourceEntity(_postgreSql);
            AssertDifferentSourceEntity(_mySql);
        }

        [Test]
        public void Build_WhenColumnsAreNotConfigured_ShouldInferProjectionColumns()
        {
            AssertInferredProjectionColumns(_sqlServer);
            AssertInferredProjectionColumns(_postgreSql);
            AssertInferredProjectionColumns(_mySql);
        }

        [Test]
        public void Build_WhenProjectionHasNoAliases_ShouldInferPropertyNames()
        {
            AssertInferredPropertyNames(_sqlServer);
            AssertInferredPropertyNames(_postgreSql);
            AssertInferredPropertyNames(_mySql);
        }

        [Test]
        public void Build_WhenAggregateProjectionIsUsed_ShouldInferAlias()
        {
            AssertAggregateProjectionAlias(_sqlServer);
            AssertAggregateProjectionAlias(_postgreSql);
            AssertAggregateProjectionAlias(_mySql);
        }

        [Test]
        public void Build_WhenScalarFunctionProjectionIsUsed_ShouldInferAlias()
        {
            AssertScalarProjectionAlias(_sqlServer);
            AssertScalarProjectionAlias(_postgreSql);
            AssertScalarProjectionAlias(_mySql);
        }

        [Test]
        public void Build_WhenComputedProjectionIsUsed_ShouldInferAlias()
        {
            AssertComputedProjectionAlias(_sqlServer);
            AssertComputedProjectionAlias(_postgreSql);
            AssertComputedProjectionAlias(_mySql);
        }

        [Test]
        public void Build_WhenCaseProjectionIsUsed_ShouldInferAlias()
        {
            AssertCaseProjectionAlias(_sqlServer);
            AssertCaseProjectionAlias(_postgreSql);
            AssertCaseProjectionAlias(_mySql);
        }

        [Test]
        public void Build_WhenInferredColumnsHaveNoProjection_ShouldThrowInvalidOperationException()
        {
            AssertMissingProjection(_sqlServer);
            AssertMissingProjection(_postgreSql);
            AssertMissingProjection(_mySql);
        }

        [Test]
        public void Build_WhenInferredTargetColumnIsDuplicated_ShouldThrowInvalidOperationException()
        {
            AssertDuplicateInferredColumn(_sqlServer);
            AssertDuplicateInferredColumn(_postgreSql);
            AssertDuplicateInferredColumn(_mySql);
        }

        [Test]
        public void Build_WhenProjectionTypesResolveSameTargetColumn_ShouldThrowInvalidOperationException()
        {
            AssertDuplicateProjectionAlias(_sqlServer);
            AssertDuplicateProjectionAlias(_postgreSql);
            AssertDuplicateProjectionAlias(_mySql);
        }

        [Test]
        public void Columns_WhenTargetColumnIsConfiguredMoreThanOnce_ShouldThrowInvalidOperationException()
        {
            AssertDuplicateExplicitColumn(_sqlServer);
            AssertDuplicateExplicitColumn(_postgreSql);
            AssertDuplicateExplicitColumn(_mySql);
        }

        [Test]
        public void Columns_WhenSelectorIsNull_ShouldThrowArgumentNullException()
        {
            AssertNullColumnsSelector(_sqlServer);
            AssertNullColumnsSelector(_postgreSql);
            AssertNullColumnsSelector(_mySql);
        }

        [Test]
        public void Columns_WhenSelectorIsNotDirectProperty_ShouldThrowArgumentException()
        {
            AssertInvalidColumnsSelector(_sqlServer);
            AssertInvalidColumnsSelector(_postgreSql);
            AssertInvalidColumnsSelector(_mySql);
        }

        [Test]
        public void From_WhenAliasIsWhitespace_ShouldThrowArgumentException()
        {
            AssertWhitespaceSourceAlias(_sqlServer);
            AssertWhitespaceSourceAlias(_postgreSql);
            AssertWhitespaceSourceAlias(_mySql);
        }

        [Test]
        public void From_WhenExplicitTableNameIsEmpty_ShouldThrowArgumentException()
        {
            AssertEmptySourceTableName(_sqlServer);
            AssertEmptySourceTableName(_postgreSql);
            AssertEmptySourceTableName(_mySql);
        }

        private static void AssertValidInsertSelect<TProfile>(QueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .InsertInto<JoinUser>()
                .Columns(user => new
                {
                    user.Id,
                    user.Email
                })
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(user => new
                {
                    user.Id,
                    user.Email
                })
                .Where<JoinUser>(user => user.IsActive)
                .Build();

            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain("INSERT"));
                Assert.That(sql, Does.Contain("users"));
                Assert.That(sql, Does.Contain("SELECT"));
                Assert.That(sql, Does.Contain("user_id"));
                Assert.That(sql, Does.Contain("email"));
                Assert.That(sql, Does.Contain("WHERE"));
                QueryAssertionHelper.AssertParameter(query.Parameters, 0, true);
            });
        }

        private static void AssertInsertSelectWithJoin<TProfile>(QueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .InsertInto<JoinOrder>()
                .Columns(order => new
                {
                    order.UserId,
                    order.Total
                })
                .From<JoinUser>(alias: "u")
                .InnerJoin<JoinUser, JoinOrder>(
                    alias: "o",
                    on: (user, order) => user.Id == order.UserId)
                .Select<JoinUser>(user => new
                {
                    UserId = user.Id
                })
                .Select<JoinOrder>(order => new
                {
                    order.Total
                })
                .Where<JoinUser>(user => user.IsActive)
                .Build();

            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain("INSERT"));
                Assert.That(sql, Does.Contain("orders"));
                Assert.That(sql, Does.Contain("INNER JOIN"));
                Assert.That(sql, Does.Contain("users"));
                Assert.That(sql, Does.Contain("total"));
                QueryAssertionHelper.AssertParameter(query.Parameters, 0, true);
            });
        }

        private static void AssertDifferentSourceEntity<TProfile>(QueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .InsertInto<JoinOrder>()
                .Columns(order => new
                {
                    order.UserId,
                    order.Total
                })
                .From<JoinUser>(alias: "u")
                .InnerJoin<JoinUser, JoinOrder>(
                    alias: "o",
                    on: (user, order) => user.Id == order.UserId)
                .Select<JoinUser>(user => new
                {
                    UserId = user.Id
                })
                .Select<JoinOrder>(order => new
                {
                    order.Total
                })
                .Build();

            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain("INSERT INTO orders"));
                Assert.That(sql, Does.Contain("FROM users"));
                Assert.That(sql, Does.Contain("INNER JOIN orders"));
            });
        }

        private static void AssertInferredProjectionColumns<TProfile>(QueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .InsertInto<JoinOrder>()
                .From<JoinUser>(alias: "u")
                .InnerJoin<JoinUser, JoinOrder>(
                    alias: "o",
                    on: (user, order) => user.Id == order.UserId)
                .Select<JoinUser>(user => new
                {
                    UserId = user.Id
                })
                .Select<JoinOrder>(order => new
                {
                    order.Total
                })
                .Build();

            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain("UserId"));
                Assert.That(sql, Does.Contain("Total"));
                Assert.That(sql, Does.Contain("SELECT"));
            });
        }

        private static void AssertInferredPropertyNames<TProfile>(QueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .InsertInto<JoinUser>()
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(user => new
                {
                    user.Id,
                    user.Email
                })
                .Build();

            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain("Id"));
                Assert.That(sql, Does.Contain("Email"));
            });
        }

        private static void AssertAggregateProjectionAlias<TProfile>(QueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .InsertInto<JoinOrder>("projection_results")
                .From<JoinOrder>(alias: "o")
                .SelectAggregate<JoinOrder>(
                    QueryAggregateFunction.Sum,
                    order => order.Total,
                    "TotalAmount")
                .Build();

            Assert.That(QueryAssertionHelper.NormalizeSql(query.CommandText), Does.Contain("TotalAmount"));
        }

        private static void AssertScalarProjectionAlias<TProfile>(QueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .InsertInto<JoinUser>("projection_results")
                .From<JoinUser>(alias: "u")
                .SelectScalarFunction<JoinUser>(
                    QueryScalarFunction.Upper,
                    user => user.Email,
                    "NormalizedEmail")
                .Build();

            Assert.That(QueryAssertionHelper.NormalizeSql(query.CommandText), Does.Contain("NormalizedEmail"));
        }

        private static void AssertComputedProjectionAlias<TProfile>(QueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .InsertInto<JoinOrder>("projection_results")
                .From<JoinOrder>(alias: "o")
                .SelectComputed<JoinOrder>(
                    order => order.Total * 1.16m,
                    "TotalWithTax")
                .Build();

            Assert.That(QueryAssertionHelper.NormalizeSql(query.CommandText), Does.Contain("TotalWithTax"));
        }

        private static void AssertCaseProjectionAlias<TProfile>(QueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .InsertInto<JoinOrder>("projection_results")
                .From<JoinOrder>(alias: "o")
                .SelectCaseWhen<JoinOrder>(
                    order => order.Total > 1000m,
                    "VIP",
                    "STANDARD",
                    "CustomerType")
                .Build();

            Assert.That(QueryAssertionHelper.NormalizeSql(query.CommandText), Does.Contain("CustomerType"));
        }

        private static void AssertMissingProjection<TProfile>(QueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            var command = queryBuilder
                .InsertInto<JoinUser>()
                .From<JoinUser>(alias: "u");

            var exception = Assert.Throws<InvalidOperationException>(() => command.Build());

            Assert.That(
                exception!.Message,
                Is.EqualTo("At least one SELECT projection must be configured when INSERT target columns are not explicitly configured."));
        }

        private static void AssertDuplicateInferredColumn<TProfile>(QueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            var command = queryBuilder
                .InsertInto<JoinOrder>()
                .From<JoinUser>(alias: "u")
                .InnerJoin<JoinUser, JoinOrder>(
                    alias: "o",
                    on: (user, order) => user.Id == order.UserId)
                .Select<JoinUser>(user => new
                {
                    UserId = user.Id
                })
                .Select<JoinOrder>(order => new
                {
                    UserId = order.UserId
                });

            var exception = Assert.Throws<InvalidOperationException>(() => command.Build());

            Assert.That(
                exception!.Message,
                Is.EqualTo("Target INSERT column 'UserId' was resolved more than once from the SELECT projection."));
        }

        private static void AssertDuplicateProjectionAlias<TProfile>(QueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            var command = queryBuilder
                .InsertInto<JoinOrder>("projection_results")
                .From<JoinOrder>(alias: "o")
                .Select<JoinOrder>(order => new
                {
                    TotalAmount = order.Total
                })
                .SelectAggregate<JoinOrder>(
                    QueryAggregateFunction.Sum,
                    order => order.Total,
                    "TotalAmount");

            var exception = Assert.Throws<InvalidOperationException>(() => command.Build());

            Assert.That(
                exception!.Message,
                Is.EqualTo("Target INSERT column 'TotalAmount' was resolved more than once from the SELECT projection."));
        }

        private static void AssertDuplicateExplicitColumn<TProfile>(QueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            var command = queryBuilder
                .InsertInto<JoinUser>()
                .Columns(user => new
                {
                    user.Id,
                    user.Email
                });

            var exception = Assert.Throws<InvalidOperationException>(() => command.Columns(user => user.Email));

            Assert.That(
                exception!.Message,
                Is.EqualTo("Property 'Email' is already configured as an INSERT target column."));
        }

        private static void AssertNullColumnsSelector<TProfile>(QueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            Expression<Func<JoinUser, object>> selector = null!;

            var command = queryBuilder.InsertInto<JoinUser>();

            var exception = Assert.Throws<ArgumentNullException>(() => command.Columns(selector));

            Assert.That(exception!.ParamName, Is.EqualTo("selector"));
        }

        private static void AssertInvalidColumnsSelector<TProfile>(QueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            var command = queryBuilder.InsertInto<JoinUser>();

            var exception = Assert.Throws<ArgumentException>(() =>
                command.Columns(user => new
                {
                    Value = user.Email!.Length
                }));

            Assert.Multiple(() =>
            {
                Assert.That(exception!.ParamName, Is.EqualTo("selector"));
                Assert.That(exception.Message, Does.StartWith("The INSERT columns selector must reference direct entity properties."));
            });
        }

        private static void AssertWhitespaceSourceAlias<TProfile>(QueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            var command = queryBuilder.InsertInto<JoinUser>();

            var exception = Assert.Throws<ArgumentException>(() => command.From<JoinUser>(alias: " "));

            Assert.That(exception!.ParamName, Is.EqualTo("alias"));
        }

        private static void AssertEmptySourceTableName<TProfile>(QueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            var command = queryBuilder.InsertInto<JoinUser>();

            var exception = Assert.Throws<ArgumentException>(() => command.From<JoinUser>("", alias: "u"));

            Assert.That(exception!.ParamName, Is.EqualTo("tableName"));
        }
    }
}
