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

namespace TinyBlueWhale.EngineQuery.Tests.QueryBuilding.Update
{
    /// <summary>
    /// Validates provider-independent UPDATE query behavior.
    /// </summary>
    [TestFixture]
    internal sealed class UpdateTests
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
        public void Build_WhenUpdateIsValid_ShouldGenerateExpectedSql()
        {
            AssertValidUpdate(_sqlServer);
            AssertValidUpdate(_postgreSql);
            AssertValidUpdate(_mySql);
        }

        [Test]
        public void Build_WhenWhereInIsConfigured_ShouldGenerateInPredicate()
        {
            AssertWhereIn(_sqlServer);
            AssertWhereIn(_postgreSql);
            AssertWhereIn(_mySql);
        }

        [Test]
        public void Build_WhenMultipleValuesAreConfigured_ShouldGenerateParametersInClauseOrder()
        {
            AssertParameterOrder(_sqlServer);
            AssertParameterOrder(_postgreSql);
            AssertParameterOrder(_mySql);
        }

        [Test]
        public void Build_WhenValueIsNull_ShouldGenerateNullParameter()
        {
            AssertNullValue(_sqlServer);
            AssertNullValue(_postgreSql);
            AssertNullValue(_mySql);
        }

        [Test]
        public void Set_WhenSelectorIsNotDirectProperty_ShouldThrowArgumentException()
        {
            AssertInvalidSelector(_sqlServer);
            AssertInvalidSelector(_postgreSql);
            AssertInvalidSelector(_mySql);
        }

        [Test]
        public void Set_WhenColumnIsAssignedMoreThanOnce_ShouldThrowInvalidOperationException()
        {
            AssertDuplicateAssignment(_sqlServer);
            AssertDuplicateAssignment(_postgreSql);
            AssertDuplicateAssignment(_mySql);
        }

        [Test]
        public void Build_WhenWherePredicateIsMissing_ShouldThrowInvalidOperationException()
        {
            AssertMissingWhere(_sqlServer);
            AssertMissingWhere(_postgreSql);
            AssertMissingWhere(_mySql);
        }

        [Test]
        public void Build_WhenValueAssignmentIsMissing_ShouldThrowInvalidOperationException()
        {
            AssertMissingAssignment(_sqlServer);
            AssertMissingAssignment(_postgreSql);
            AssertMissingAssignment(_mySql);
        }

        [Test]
        public void Build_WhenExplicitTableNameIsProvided_ShouldUseTableName()
        {
            AssertExplicitTableName(_sqlServer);
            AssertExplicitTableName(_postgreSql);
            AssertExplicitTableName(_mySql);
        }

        [Test]
        public void Build_WhenMultiplePredicatesUseLogicalOperator_ShouldGenerateOperator()
        {
            AssertLogicalOperator(_sqlServer);
            AssertLogicalOperator(_postgreSql);
            AssertLogicalOperator(_mySql);
        }

        [Test]
        public void Build_WhenWhereIfConditionIsTrue_ShouldIncludePredicate()
        {
            AssertWhereIfTrue(_sqlServer);
            AssertWhereIfTrue(_postgreSql);
            AssertWhereIfTrue(_mySql);
        }

        [Test]
        public void Build_WhenWhereIfConditionIsFalse_ShouldExcludePredicate()
        {
            AssertWhereIfFalse(_sqlServer);
            AssertWhereIfFalse(_postgreSql);
            AssertWhereIfFalse(_mySql);
        }

        private static void AssertValidUpdate<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .Update<User>()
                .Set(user => user.Email, "updated@test.com")
                .Set(user => user.IsActive, false)
                .Where(user => user.Id == 10)
                .Build();

            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain("UPDATE"));
                Assert.That(sql, Does.Contain("Users").IgnoreCase);
                Assert.That(sql, Does.Contain("SET"));
                Assert.That(sql, Does.Contain("Email").IgnoreCase);
                Assert.That(sql, Does.Contain("IsActive").IgnoreCase);
                Assert.That(sql, Does.Contain("WHERE"));
                Assert.That(sql, Does.Contain("Id").IgnoreCase);
                Assert.That(query.Parameters, Has.Count.EqualTo(3));
                QueryAssertionHelper.AssertParameter(query.Parameters, 0, "updated@test.com");
                QueryAssertionHelper.AssertParameter(query.Parameters, 1, false);
                QueryAssertionHelper.AssertParameter(query.Parameters, 2, 10);
            });
        }

        private static void AssertWhereIn<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var userIds = new[] { 10, 20, 30 };

            var query = queryBuilder
                .Update<User>()
                .Set(user => user.IsActive, false)
                .WhereIn(user => user.Id, userIds)
                .Build();

            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain("UPDATE"));
                Assert.That(sql, Does.Contain("SET"));
                Assert.That(sql, Does.Contain(" IN "));
                Assert.That(query.Parameters, Has.Count.EqualTo(4));
                QueryAssertionHelper.AssertParameter(query.Parameters, 0, false);
                QueryAssertionHelper.AssertParameter(query.Parameters, 1, 10);
                QueryAssertionHelper.AssertParameter(query.Parameters, 2, 20);
                QueryAssertionHelper.AssertParameter(query.Parameters, 3, 30);
            });
        }

        private static void AssertParameterOrder<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .Update<User>()
                .Set(user => user.Email, "updated@test.com")
                .Set(user => user.IsActive, false)
                .Where(user => user.Id == 10)
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(query.Parameters, Has.Count.EqualTo(3));
                QueryAssertionHelper.AssertParameter(query.Parameters, 0, "updated@test.com");
                QueryAssertionHelper.AssertParameter(query.Parameters, 1, false);
                QueryAssertionHelper.AssertParameter(query.Parameters, 2, 10);
            });
        }

        private static void AssertNullValue<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .Update<Category>()
                .Set(category => category.ParentId, null)
                .Where(category => category.Id == 10)
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(query.Parameters, Has.Count.EqualTo(2));
                QueryAssertionHelper.AssertParameter(query.Parameters, 0, null!);
                QueryAssertionHelper.AssertParameter(query.Parameters, 1, 10);
            });
        }

        private static void AssertInvalidSelector<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var command = queryBuilder.Update<User>();

            var exception = Assert.Throws<ArgumentException>(() =>
                command.Set(user => user.Email.ToLower(), "updated@test.com"));

            Assert.That(exception!.Message, Does.StartWith("The UPDATE selector must reference a direct entity property."));
        }

        private static void AssertDuplicateAssignment<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var command = queryBuilder
                .Update<User>()
                .Set(user => user.Email, "first@test.com");

            var exception = Assert.Throws<InvalidOperationException>(() =>
                command.Set(user => user.Email, "second@test.com"));

            Assert.That(exception!.Message, Is.EqualTo("Column 'Email' already has an UPDATE value assignment."));
        }

        private static void AssertMissingWhere<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var command = queryBuilder
                .Update<User>()
                .Set(user => user.Email, "updated@test.com");

            var exception = Assert.Throws<InvalidOperationException>(() => command.Build());

            Assert.That(exception!.Message, Is.EqualTo("At least one WHERE predicate must be configured before building an UPDATE command."));
        }

        private static void AssertMissingAssignment<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var command = queryBuilder
                .Update<User>()
                .Where(user => user.Id == 10);

            var exception = Assert.Throws<InvalidOperationException>(() => command.Build());

            Assert.That(exception!.Message, Is.EqualTo("At least one value must be configured before building an UPDATE command."));
        }

        private static void AssertExplicitTableName<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .Update<User>("CustomUsers")
                .Set(user => user.Email, "updated@test.com")
                .Where(user => user.Id == 10)
                .Build();

            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain("CustomUsers"));
                Assert.That(query.Parameters, Has.Count.EqualTo(2));
                QueryAssertionHelper.AssertParameter(query.Parameters, 0, "updated@test.com");
                QueryAssertionHelper.AssertParameter(query.Parameters, 1, 10);
            });
        }

        private static void AssertLogicalOperator<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .Update<User>()
                .Set(user => user.IsActive, false)
                .Where(user => user.Age < 18)
                .Where(user => user.IsDeleted, QueryLogicalOperator.Or)
                .Build();

            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain(" OR "));
                Assert.That(query.Parameters, Has.Count.EqualTo(3));
                QueryAssertionHelper.AssertParameter(query.Parameters, 0, false);
                QueryAssertionHelper.AssertParameter(query.Parameters, 1, 18);
                QueryAssertionHelper.AssertParameter(query.Parameters, 2, true);
            });
        }

        private static void AssertWhereIfTrue<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .Update<User>()
                .Set(user => user.IsActive, false)
                .WhereIf(true, user => user.Age >= 18)
                .Build();

            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain("Age").IgnoreCase);
                Assert.That(query.Parameters, Has.Count.EqualTo(2));
                QueryAssertionHelper.AssertParameter(query.Parameters, 0, false);
                QueryAssertionHelper.AssertParameter(query.Parameters, 1, 18);
            });
        }

        private static void AssertWhereIfFalse<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .Update<User>()
                .Set(user => user.IsActive, false)
                .WhereIf(false, user => user.IsDeleted)
                .Where(user => user.Id == 10)
                .Build();

            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Not.Contain("IsDeleted").IgnoreCase);
                Assert.That(query.Parameters, Has.Count.EqualTo(2));
                QueryAssertionHelper.AssertParameter(query.Parameters, 0, false);
                QueryAssertionHelper.AssertParameter(query.Parameters, 1, 10);
            });
        }
    }
}
