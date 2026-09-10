using System.Linq.Expressions;
using TinyBlueWhale.EngineQuery.Abstractions.Enums;
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

namespace TinyBlueWhale.EngineQuery.Tests.QueryBuilding.Delete
{
    /// <summary>
    /// Validates provider-independent DELETE query behavior.
    /// </summary>
    [TestFixture]
    internal sealed class DeleteTests
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
        public void Build_WhenDeleteIsValid_ShouldGenerateExpectedSql()
        {
            AssertValidDelete(_sqlServer);
            AssertValidDelete(_postgreSql);
            AssertValidDelete(_mySql);
        }

        [Test]
        public void Build_WhenWhereInIsConfigured_ShouldGenerateInPredicate()
        {
            AssertWhereIn(_sqlServer);
            AssertWhereIn(_postgreSql);
            AssertWhereIn(_mySql);
        }

        [Test]
        public void Build_WhenWhereNotInIsConfigured_ShouldGenerateNotInPredicate()
        {
            AssertWhereNotIn(_sqlServer);
            AssertWhereNotIn(_postgreSql);
            AssertWhereNotIn(_mySql);
        }

        [Test]
        public void Build_WhenMultiplePredicatesAreConfigured_ShouldPreserveParameterOrder()
        {
            AssertMultiplePredicates(_sqlServer);
            AssertMultiplePredicates(_postgreSql);
            AssertMultiplePredicates(_mySql);
        }

        [Test]
        public void Build_WhenLogicalOperatorIsAnd_ShouldGenerateAnd()
        {
            AssertAndOperator(_sqlServer);
            AssertAndOperator(_postgreSql);
            AssertAndOperator(_mySql);
        }

        [Test]
        public void Build_WhenLogicalOperatorIsOr_ShouldGenerateOr()
        {
            AssertOrOperator(_sqlServer);
            AssertOrOperator(_postgreSql);
            AssertOrOperator(_mySql);
        }

        [Test]
        public void WhereIf_WhenConditionIsTrue_ShouldIncludePredicate()
        {
            AssertWhereIfTrue(_sqlServer);
            AssertWhereIfTrue(_postgreSql);
            AssertWhereIfTrue(_mySql);
        }

        [Test]
        public void WhereIf_WhenConditionIsFalse_ShouldExcludePredicate()
        {
            AssertWhereIfFalse(_sqlServer);
            AssertWhereIfFalse(_postgreSql);
            AssertWhereIfFalse(_mySql);
        }

        [Test]
        public void WhereIf_WhenLogicalOperatorIsProvided_ShouldPreserveOperator()
        {
            AssertWhereIfOperator(_sqlServer);
            AssertWhereIfOperator(_postgreSql);
            AssertWhereIfOperator(_mySql);
        }

        [Test]
        public void DeleteFrom_WhenExplicitTableNameIsProvided_ShouldUseTableName()
        {
            AssertExplicitTableName(_sqlServer);
            AssertExplicitTableName(_postgreSql);
            AssertExplicitTableName(_mySql);
        }

        [Test]
        public void DeleteFrom_WhenExplicitTableNameIsNull_ShouldThrowArgumentNullException()
        {
            AssertNullTableName(_sqlServer);
            AssertNullTableName(_postgreSql);
            AssertNullTableName(_mySql);
        }

        [TestCase("")]
        [TestCase(" ")]
        public void DeleteFrom_WhenExplicitTableNameIsEmptyOrWhitespace_ShouldThrowArgumentException(
            string tableName)
        {
            AssertInvalidTableName(_sqlServer, tableName);
            AssertInvalidTableName(_postgreSql, tableName);
            AssertInvalidTableName(_mySql, tableName);
        }

        [Test]
        public void Where_WhenPredicateIsNull_ShouldThrowArgumentNullException()
        {
            AssertNullWherePredicate(_sqlServer);
            AssertNullWherePredicate(_postgreSql);
            AssertNullWherePredicate(_mySql);
        }

        [Test]
        public void WhereIf_WhenPredicateIsNull_ShouldThrowArgumentNullException()
        {
            AssertNullWhereIfPredicate(_sqlServer);
            AssertNullWhereIfPredicate(_postgreSql);
            AssertNullWhereIfPredicate(_mySql);
        }

        [Test]
        public void WhereIn_WhenSelectorIsNull_ShouldThrowArgumentNullException()
        {
            AssertNullWhereInSelector(_sqlServer);
            AssertNullWhereInSelector(_postgreSql);
            AssertNullWhereInSelector(_mySql);
        }

        [Test]
        public void WhereIn_WhenValuesAreNull_ShouldThrowArgumentNullException()
        {
            AssertNullWhereInValues(_sqlServer);
            AssertNullWhereInValues(_postgreSql);
            AssertNullWhereInValues(_mySql);
        }

        [Test]
        public void WhereIn_WhenValuesAreEmpty_ShouldThrowArgumentException()
        {
            AssertEmptyWhereInValues(_sqlServer);
            AssertEmptyWhereInValues(_postgreSql);
            AssertEmptyWhereInValues(_mySql);
        }

        [Test]
        public void WhereNotIn_WhenValuesContainNull_ShouldThrowArgumentException()
        {
            AssertWhereNotInNullValue(_sqlServer);
            AssertWhereNotInNullValue(_postgreSql);
            AssertWhereNotInNullValue(_mySql);
        }

        [Test]
        public void Build_WhenWherePredicateIsMissing_ShouldThrowInvalidOperationException()
        {
            AssertMissingWhere(_sqlServer);
            AssertMissingWhere(_postgreSql);
            AssertMissingWhere(_mySql);
        }

        private static void AssertValidDelete<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .DeleteFrom<User>()
                .Where(user => user.Id == 10)
                .Build();

            var sql = NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain("DELETE"));
                Assert.That(sql, Does.Contain("FROM Users"));
                Assert.That(sql, Does.Contain("WHERE (Id = @p0)"));
                Assert.That(query.Parameters, Has.Count.EqualTo(1));
                QueryAssertionHelper.AssertParameter(query.Parameters, 0, 10);
            });
        }

        private static void AssertWhereIn<TProfile>(QueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .DeleteFrom<User>()
                .WhereIn(user => user.Id, new[] { 10, 20, 30 })
                .Build();

            var sql = NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain("Id IN (@p0, @p1, @p2)"));
                AssertParameter(query.Parameters, 0, "@p0", 10);
                AssertParameter(query.Parameters, 1, "@p1", 20);
                AssertParameter(query.Parameters, 2, "@p2", 30);
            });
        }

        private static void AssertWhereNotIn<TProfile>(QueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .DeleteFrom<User>()
                .WhereNotIn(user => user.Id, new[] { 10, 20, 30 })
                .Build();

            var sql = NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain("Id NOT IN (@p0, @p1, @p2)"));
                AssertParameter(query.Parameters, 0, "@p0", 10);
                AssertParameter(query.Parameters, 1, "@p1", 20);
                AssertParameter(query.Parameters, 2, "@p2", 30);
            });
        }

        private static void AssertMultiplePredicates<TProfile>(QueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .DeleteFrom<User>()
                .Where(user => user.Id == 10)
                .Where(user => user.IsDeleted)
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(query.Parameters, Has.Count.EqualTo(2));
                AssertParameter(query.Parameters, 0, "@p0", 10);
                AssertParameter(query.Parameters, 1, "@p1", true);
            });
        }

        private static void AssertAndOperator<TProfile>(QueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .DeleteFrom<User>()
                .Where(user => user.Id == 10)
                .Where(
                    user => user.IsDeleted,
                    QueryLogicalOperator.And)
                .Build();

            Assert.That(query.CommandText, Does.Contain(" AND "));
        }

        private static void AssertOrOperator<TProfile>(QueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .DeleteFrom<User>()
                .Where(user => user.Age < 18)
                .Where(
                    user => user.IsDeleted,
                    QueryLogicalOperator.Or)
                .Build();

            Assert.That(query.CommandText, Does.Contain(" OR "));
        }

        private static void AssertWhereIfTrue<TProfile>(QueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .DeleteFrom<User>()
                .Where(user => user.Id == 10)
                .WhereIf(
                    true,
                    user => user.Age >= 18)
                .Build();

            var sql = NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain("Age"));
                Assert.That(query.Parameters, Has.Count.EqualTo(2));
                Assert.That(query.Parameters[1].Value, Is.EqualTo(18));
            });
        }

        private static void AssertWhereIfFalse<TProfile>(QueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .DeleteFrom<User>()
                .Where(user => user.Id == 10)
                .WhereIf(
                    false,
                    user => user.IsDeleted)
                .Build();

            var sql = NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(
                    sql,
                    Does.Not.Contain("IsDeleted"));

                Assert.That(query.Parameters, Has.Count.EqualTo(1));
            });
        }

        private static void AssertWhereIfOperator<TProfile>(QueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .DeleteFrom<User>()
                .Where(user => user.Id == 10)
                .WhereIf(
                    true,
                    user => user.IsDeleted,
                    QueryLogicalOperator.Or)
                .Build();

            Assert.That(query.CommandText, Does.Contain(" OR "));
        }

        private static void AssertExplicitTableName<TProfile>(QueryBuilder<TProfile> queryBuilder) where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .DeleteFrom<User>("CustomUsers")
                .Where(user => user.Id == 10)
                .Build();

            var sql = NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain("DELETE"));
                Assert.That(sql, Does.Contain("FROM CustomUsers"));
                Assert.That(sql, Does.Contain("WHERE (Id = @p0)"));
                QueryAssertionHelper.AssertParameter(query.Parameters, 0, 10);
            });
        }

        private static void AssertNullTableName<TProfile>(QueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            var exception = Assert.Throws<ArgumentNullException>(() =>
                queryBuilder.DeleteFrom<User>(null!));

            Assert.That(exception!.ParamName, Is.EqualTo("tableName"));
        }

        private static void AssertInvalidTableName<TProfile>(QueryBuilder<TProfile> queryBuilder, string tableName)
            where TProfile : IDatabaseProviderProfile
        {
            var exception = Assert.Throws<ArgumentException>(() =>
                queryBuilder.DeleteFrom<User>(tableName));

            Assert.That(exception!.ParamName, Is.EqualTo("tableName"));
        }

        private static void AssertNullWherePredicate<TProfile>(QueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            Expression<Func<User, bool>> predicate = null!;

            var command = queryBuilder.DeleteFrom<User>();

            var exception = Assert.Throws<ArgumentNullException>(() =>
                command.Where(predicate));

            Assert.That(exception!.ParamName, Is.EqualTo("predicate"));
        }

        private static void AssertNullWhereIfPredicate<TProfile>(QueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            Expression<Func<User, bool>> predicate = null!;

            var command = queryBuilder.DeleteFrom<User>();

            var exception = Assert.Throws<ArgumentNullException>(() => command.WhereIf(false, predicate));

            Assert.That(exception!.ParamName, Is.EqualTo("predicate"));
        }

        private static void AssertNullWhereInSelector<TProfile>(QueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            Expression<Func<User, int>> selector = null!;

            var command = queryBuilder.DeleteFrom<User>();

            var exception = Assert.Throws<ArgumentNullException>(() => command.WhereIn(selector, new[] { 10 }));

            Assert.That(exception!.ParamName, Is.EqualTo("selector"));
        }

        private static void AssertNullWhereInValues<TProfile>(QueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            IEnumerable<int> values = null!;

            var command = queryBuilder.DeleteFrom<User>();

            var exception = Assert.Throws<ArgumentNullException>(() => command.WhereIn(user => user.Id, values));

            Assert.That(exception!.ParamName, Is.EqualTo("values"));
        }

        private static void AssertEmptyWhereInValues<TProfile>(QueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            var command = queryBuilder.DeleteFrom<User>();

            var exception = Assert.Throws<ArgumentException>(() =>
                command.WhereIn(
                    user => user.Id,
                    Array.Empty<int>()));

            Assert.That(exception!.ParamName, Is.EqualTo("values"));
        }

        private static void AssertWhereNotInNullValue<TProfile>(QueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            string?[] values =
            [
                "one@test.com",
                null,
                "three@test.com"
            ];

            var command = queryBuilder.DeleteFrom<User>();

            var exception = Assert.Throws<ArgumentException>(() => command.WhereNotIn(user => user.Email, values));

            Assert.That(exception!.ParamName, Is.EqualTo("values"));
        }

        private static void AssertMissingWhere<TProfile>(QueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            var command = queryBuilder.DeleteFrom<User>();

            var exception = Assert.Throws<InvalidOperationException>(() =>
                command.Build());

            Assert.That(exception!.Message, Is.EqualTo("At least one WHERE predicate must be configured before building a DELETE command."));
        }

        private static void AssertParameter(IReadOnlyList<QuerySqlParameter> parameters, int index, string name, object expectedValue)
        {
            Assert.That(parameters, Has.Count.GreaterThan(index));
            Assert.Multiple(() =>
            {
                Assert.That(parameters[index].Name, Is.EqualTo(name));
                Assert.That(parameters[index].Value, Is.EqualTo(expectedValue));
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
