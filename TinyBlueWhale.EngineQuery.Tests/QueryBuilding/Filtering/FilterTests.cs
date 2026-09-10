using System.Linq.Expressions;
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

namespace TinyBlueWhale.EngineQuery.Tests.QueryBuilding.Filtering
{
    /// <summary>
    /// Validates provider-independent query filtering behavior.
    /// </summary>
    [TestFixture]
    internal sealed class FilterTests
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
        public void Build_WhenBooleanAndStringPredicatesAreConfigured_ShouldGenerateExpectedFilters()
        {
            AssertBooleanAndStringPredicates(_sqlServer);
            AssertBooleanAndStringPredicates(_postgreSql);
            AssertBooleanAndStringPredicates(_mySql);
        }

        [Test]
        public void Build_WhenPredicateContainsOr_ShouldGenerateOrOperator()
        {
            AssertOrPredicate(_sqlServer);
            AssertOrPredicate(_postgreSql);
            AssertOrPredicate(_mySql);
        }

        [Test]
        public void Build_WhenWhereIfConditionsDiffer_ShouldIncludeOnlyEnabledPredicate()
        {
            AssertWhereIf(_sqlServer);
            AssertWhereIf(_postgreSql);
            AssertWhereIf(_mySql);
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
        public void Build_WhenCollectionFiltersTargetJoinedEntity_ShouldUseJoinedSource()
        {
            AssertJoinedCollectionFilters(_sqlServer);
            AssertJoinedCollectionFilters(_postgreSql);
            AssertJoinedCollectionFilters(_mySql);
        }

        [Test]
        public void Build_WhenNullablePropertyIsComparedToNull_ShouldGenerateNullPredicates()
        {
            AssertNullPredicates(_sqlServer);
            AssertNullPredicates(_postgreSql);
            AssertNullPredicates(_mySql);
        }

        [Test]
        public void Build_WhenWhereIfIsFalseAfterWhere_ShouldPreserveExistingPredicate()
        {
            AssertDisabledWhereIfPreservesWhere(_sqlServer);
            AssertDisabledWhereIfPreservesWhere(_postgreSql);
            AssertDisabledWhereIfPreservesWhere(_mySql);
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

        [TestCase(false)]
        [TestCase(true)]
        public void WhereCollection_WhenValuesAreNull_ShouldThrow(bool isNegated)
        {
            AssertNullCollection(_sqlServer, isNegated);
            AssertNullCollection(_postgreSql, isNegated);
            AssertNullCollection(_mySql, isNegated);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void WhereCollection_WhenValuesAreEmpty_ShouldThrow(bool isNegated)
        {
            AssertEmptyCollection(_sqlServer, isNegated);
            AssertEmptyCollection(_postgreSql, isNegated);
            AssertEmptyCollection(_mySql, isNegated);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void WhereCollection_WhenValuesContainNull_ShouldThrow(bool isNegated)
        {
            AssertCollectionContainingNull(_sqlServer, isNegated);
            AssertCollectionContainingNull(_postgreSql, isNegated);
            AssertCollectionContainingNull(_mySql, isNegated);
        }

        private static void AssertBooleanAndStringPredicates<TProfile>(QueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<User>()
                .Where<User>(user =>
                    user.IsActive &&
                    user.Email.Contains("@gmail.com") &&
                    user.Age >= 18)
                .Build();

            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain("WHERE"));
                Assert.That(sql, Does.Contain("IsActive"));
                Assert.That(sql, Does.Contain("Email"));
                Assert.That(sql, Does.Contain("Age"));
                Assert.That(sql, Does.Contain("LIKE"));
                Assert.That(query.Parameters, Has.Count.EqualTo(3));
                QueryAssertionHelper.AssertParameter(query.Parameters, 1, "%@gmail.com%");
                QueryAssertionHelper.AssertParameter(query.Parameters, 2, 18);
            });
        }

        private static void AssertOrPredicate<TProfile>(QueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<User>("u")
                .Where<User>(user =>
                    user.Email.Contains("@gmail.com") ||
                    user.Email.Contains("@company.com"))
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(query.CommandText, Does.Contain("WHERE"));
                Assert.That(query.CommandText, Does.Contain(" OR "));
                Assert.That(query.CommandText, Does.Contain("LIKE"));
                Assert.That(query.Parameters, Has.Count.EqualTo(2));
                QueryAssertionHelper.AssertParameter(query.Parameters, 0, "%@gmail.com%");
                QueryAssertionHelper.AssertParameter(query.Parameters, 1, "%@company.com%");
            });
        }

        private static void AssertWhereIf<TProfile>(QueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<User>()
                .WhereIf<User>(true, user => user.IsActive)
                .WhereIf<User>(false, user => user.IsDeleted)
                .Build();

            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain("IsActive"));
                Assert.That(sql, Does.Not.Contain("IsDeleted"));
                Assert.That(query.Parameters, Has.Count.EqualTo(1));
                QueryAssertionHelper.AssertParameter(query.Parameters, 0, true);
            });
        }

        private static void AssertWhereIn<TProfile>(QueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<User>()
                .WhereIn(user => user.Id, new[] { 10, 20, 30 })
                .Build();

            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain(" IN "));
                Assert.That(sql, Does.Contain("Id"));
                Assert.That(query.Parameters, Has.Count.EqualTo(3));
                QueryAssertionHelper.AssertParameter(query.Parameters, 0, 10);
                QueryAssertionHelper.AssertParameter(query.Parameters, 1, 20);
                QueryAssertionHelper.AssertParameter(query.Parameters, 2, 30);
            });
        }

        private static void AssertWhereNotIn<TProfile>(QueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<User>()
                .WhereNotIn(user => user.Email, new[]
                {
                    "blocked@test.com",
                    "deleted@test.com"
                })
                .Build();

            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain(" NOT IN "));
                Assert.That(sql, Does.Contain("Email"));
                Assert.That(query.Parameters, Has.Count.EqualTo(2));
                QueryAssertionHelper.AssertParameter(query.Parameters, 0, "blocked@test.com");
                QueryAssertionHelper.AssertParameter(query.Parameters, 1, "deleted@test.com");
            });
        }

        private static void AssertJoinedCollectionFilters<TProfile>(QueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<JoinUser>(alias: "u")
                .InnerJoin<JoinUser, JoinOrder>(
                    alias: "o",
                    on: (user, order) => user.Id == order.UserId)
                .WhereIn<JoinOrder, int>(order => order.Id, new[] { 100, 200 })
                .WhereNotIn<JoinOrder, int>(order => order.UserId, new[] { 30, 40 })
                .Build();

            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain("o.order_id"));
                Assert.That(sql, Does.Contain("o.user_id"));
                Assert.That(sql, Does.Contain(" IN "));
                Assert.That(sql, Does.Contain(" NOT IN "));
                Assert.That(query.Parameters, Has.Count.EqualTo(4));
                QueryAssertionHelper.AssertParameter(query.Parameters, 0, 100);
                QueryAssertionHelper.AssertParameter(query.Parameters, 1, 200);
                QueryAssertionHelper.AssertParameter(query.Parameters, 2, 30);
                QueryAssertionHelper.AssertParameter(query.Parameters, 3, 40);
            });
        }

        private static void AssertNullPredicates<TProfile>(QueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<Category>(alias: "c")
                .Select<Category>(category => new
                {
                    category.Id,
                    category.ParentId
                })
                .Where<Category>(category =>
                    category.ParentId == null ||
                    category.ParentId != null)
                .Build();

            var sql = QueryAssertionHelper.NormalizeSql(query.CommandText);

            Assert.Multiple(() =>
            {
                Assert.That(sql, Does.Contain("NULL"));
                Assert.That(sql, Does.Contain(" OR "));
                Assert.That(sql, Does.Contain("parent_category_id"));
                Assert.That(query.Parameters, Is.Empty);
            });
        }

        private static void AssertDisabledWhereIfPreservesWhere<TProfile>(QueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            var query = queryBuilder
                .From<JoinUser>(alias: "u")
                .Where<JoinUser>(user => user.IsActive)
                .WhereIf<JoinUser>(false, user => user.Email.Contains("@blocked.com"))
                .Select<JoinUser>(user => new
                {
                    UserId = user.Id,
                    user.Email
                })
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(query.CommandText, Does.Contain("WHERE"));
                Assert.That(query.CommandText, Does.Contain("is_active"));
                Assert.That(query.CommandText, Does.Not.Contain("@blocked.com"));
                Assert.That(query.Parameters, Has.Count.EqualTo(1));
                QueryAssertionHelper.AssertParameter(query.Parameters, 0, true);
            });
        }

        private static void AssertNullWherePredicate<TProfile>(QueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            Expression<Func<User, bool>> predicate = null!;

            var command = queryBuilder.From<User>();

            var exception = Assert.Throws<ArgumentNullException>(() => command.Where(predicate));

            Assert.That(exception!.ParamName, Is.EqualTo("predicate"));
        }

        private static void AssertNullWhereIfPredicate<TProfile>(QueryBuilder<TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile
        {
            Expression<Func<User, bool>> predicate = null!;

            var command = queryBuilder.From<User>();

            var exception = Assert.Throws<ArgumentNullException>(() => command.WhereIf(false, predicate));

            Assert.That(exception!.ParamName, Is.EqualTo("predicate"));
        }

        private static void AssertNullCollection<TProfile>(QueryBuilder<TProfile> queryBuilder, bool isNegated)
            where TProfile : IDatabaseProviderProfile
        {
            var command = queryBuilder.From<User>();
            IEnumerable<int> values = null!;

            var exception = Assert.Throws<ArgumentNullException>(() =>
            {
                if (isNegated)
                    command.WhereNotIn(user => user.Id, values);
                else
                    command.WhereIn(user => user.Id, values);
            });

            Assert.That(exception!.ParamName, Is.EqualTo("values"));
        }

        private static void AssertEmptyCollection<TProfile>(QueryBuilder<TProfile> queryBuilder, bool isNegated)
            where TProfile : IDatabaseProviderProfile
        {
            var command = queryBuilder.From<User>();
            var values = Array.Empty<int>();

            var exception = Assert.Throws<ArgumentException>(() =>
            {
                if (isNegated)
                    command.WhereNotIn(user => user.Id, values);
                else
                    command.WhereIn(user => user.Id, values);
            });

            Assert.Multiple(() =>
            {
                Assert.That(exception!.ParamName, Is.EqualTo("values"));
                Assert.That(exception.Message, Does.StartWith("IN and NOT IN collections must contain at least one value."));
            });
        }

        private static void AssertCollectionContainingNull<TProfile>(QueryBuilder<TProfile> queryBuilder, bool isNegated)
            where TProfile : IDatabaseProviderProfile
        {
            var command = queryBuilder.From<User>();

            IEnumerable<string> values =
            [
                "valid@test.com",
                null!
            ];

            var exception = Assert.Throws<ArgumentException>(() =>
            {
                if (isNegated)
                    command.WhereNotIn(user => user.Email, values);
                else
                    command.WhereIn(user => user.Email, values);
            });

            Assert.Multiple(() =>
            {
                Assert.That(exception!.ParamName, Is.EqualTo("values"));
                Assert.That(exception.Message, Does.StartWith("IN and NOT IN collections cannot contain null values."));
            });
        }
    }
}
