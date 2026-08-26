using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Tests.Models;
using TinyBlueWhale.EngineQuery.Tests.Providers;

namespace TinyBlueWhale.EngineQuery.Tests.QueryBuilding.Update
{
    /// <summary>
    /// Validates provider-independent UPDATE query behavior.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="UpdateTests"/> class
    /// using the specified database provider.
    /// </remarks>
    /// <param name="provider">
    /// Database provider used to compile UPDATE queries.
    /// </param>
    [TestFixtureSource(typeof(QueryTestProviderSource), nameof(QueryTestProviderSource.GetProviders))]
    internal sealed class UpdateTests(IQueryTestProvider provider)
    {
        private readonly IQueryTestProvider _provider = provider ?? throw new ArgumentNullException(nameof(provider));

        /// <summary>
        /// Validates SQL generation for a strongly typed UPDATE command.
        /// </summary>
        [Test]
        public void Build_WhenUpdateIsValid_ShouldGenerateExpectedSql()
        {
            var query = _provider
                .CreateQueryBuilder()
                .Update<User>()
                .Set(user => user.Email, "updated@test.com")
                .Set(user => user.IsActive, false)
                .Where(user => user.Id == 10)
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(query.CommandText, Does.Contain("UPDATE"));
                Assert.That(query.CommandText, Does.Contain("Users"));
                Assert.That(query.CommandText, Does.Contain("SET"));
                Assert.That(query.CommandText, Does.Contain("Email"));
                Assert.That(query.CommandText, Does.Contain("IsActive"));
                Assert.That(query.CommandText, Does.Contain("WHERE"));
                Assert.That(query.CommandText, Does.Contain("Id"));

                Assert.That(query.Parameters, Has.Count.EqualTo(3));
                Assert.That(query.Parameters[0].Value, Is.EqualTo("updated@test.com"));
                Assert.That(query.Parameters[1].Value, Is.EqualTo(false));
                Assert.That(query.Parameters[2].Value, Is.EqualTo(10));
            });
        }

        /// <summary>
        /// Validates UPDATE command generation using an IN collection condition.
        /// </summary>
        [Test]
        public void Build_WhenWhereInIsConfigured_ShouldGenerateInPredicate()
        {
            var userIds = new[] { 10, 20, 30 };

            var query = _provider
                .CreateQueryBuilder()
                .Update<User>()
                .Set(user => user.IsActive, false)
                .WhereIn(user => user.Id, userIds)
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(query.CommandText, Does.Contain("UPDATE"));
                Assert.That(query.CommandText, Does.Contain("SET"));
                Assert.That(query.CommandText, Does.Contain("IN"));
                Assert.That(query.Parameters, Has.Count.EqualTo(4));

                Assert.That(query.Parameters[0].Value, Is.EqualTo(false));
                Assert.That(query.Parameters[1].Value, Is.EqualTo(10));
                Assert.That(query.Parameters[2].Value, Is.EqualTo(20));
                Assert.That(query.Parameters[3].Value, Is.EqualTo(30));
            });
        }

        /// <summary>
        /// Validates that UPDATE parameters are generated in assignment
        /// and predicate order.
        /// </summary>
        [Test]
        public void Build_WhenMultipleValuesAreConfigured_ShouldGenerateParametersInClauseOrder()
        {
            var query = _provider
                .CreateQueryBuilder()
                .Update<User>()
                .Set(user => user.Email, "updated@test.com")
                .Set(user => user.IsActive, false)
                .Where(user => user.Id == 10)
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(query.Parameters, Has.Count.EqualTo(3));

                Assert.That(query.Parameters[0].Name, Is.EqualTo("@p0"));
                Assert.That(query.Parameters[0].Value, Is.EqualTo("updated@test.com"));

                Assert.That(query.Parameters[1].Name, Is.EqualTo("@p1"));
                Assert.That(query.Parameters[1].Value, Is.EqualTo(false));

                Assert.That(query.Parameters[2].Name, Is.EqualTo("@p2"));
                Assert.That(query.Parameters[2].Value, Is.EqualTo(10));
            });
        }

        /// <summary>
        /// Validates UPDATE parameter generation when a nullable value
        /// is explicitly assigned as null.
        /// </summary>
        [Test]
        public void Build_WhenValueIsNull_ShouldGenerateNullParameter()
        {
            var query = _provider
                .CreateQueryBuilder()
                .Update<Category>()
                .Set(category => category.ParentId, null)
                .Where(category => category.Id == 10)
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(query.Parameters, Has.Count.EqualTo(2));
                Assert.That(query.Parameters[0].Value, Is.Null);
                Assert.That(query.Parameters[1].Value, Is.EqualTo(10));
            });
        }

        /// <summary>
        /// Validates that UPDATE assignments only accept direct
        /// entity property selectors.
        /// </summary>
        [Test]
        public void Set_WhenSelectorIsNotDirectProperty_ShouldThrow()
        {
            var commandBuilder = _provider
                .CreateQueryBuilder()
                .Update<User>();

            var exception = Assert.Throws<ArgumentException>(() =>
                commandBuilder.Set(user => user.Email.ToLower(), "updated@test.com"));

            Assert.That(exception!.Message, Does.StartWith("The UPDATE selector must reference a direct entity property."));
        }

        /// <summary>
        /// Validates that the same UPDATE column cannot receive
        /// multiple value assignments.
        /// </summary>
        [Test]
        public void Set_WhenColumnIsAssignedMoreThanOnce_ShouldThrow()
        {
            var commandBuilder = _provider
                .CreateQueryBuilder()
                .Update<User>()
                .Set(user => user.Email, "first@test.com");

            var exception = Assert.Throws<InvalidOperationException>(() =>
                commandBuilder.Set(user => user.Email, "second@test.com"));

            Assert.That(exception!.Message, Is.EqualTo("Column 'Email' already has an UPDATE value assignment."));
        }

        /// <summary>
        /// Validates that UPDATE commands require at least one WHERE predicate.
        /// </summary>
        [Test]
        public void Build_WhenWherePredicateIsMissing_ShouldThrow()
        {
            var commandBuilder = _provider
                .CreateQueryBuilder()
                .Update<User>()
                .Set(user => user.Email, "updated@test.com");

            var exception = Assert.Throws<InvalidOperationException>(() => commandBuilder.Build());

            Assert.That(exception!.Message, Is.EqualTo("At least one WHERE predicate must be configured before building an UPDATE command."));
        }

        /// <summary>
        /// Validates that UPDATE commands require at least one value assignment.
        /// </summary>
        [Test]
        public void Build_WhenValueAssignmentIsMissing_ShouldThrow()
        {
            var commandBuilder = _provider
                .CreateQueryBuilder()
                .Update<User>()
                .Where(user => user.Id == 10);

            var exception = Assert.Throws<InvalidOperationException>(() => commandBuilder.Build());

            Assert.That(exception!.Message, Is.EqualTo("At least one value must be configured before building an UPDATE command."));
        }

        /// <summary>
        /// Validates UPDATE generation using an explicitly configured table name.
        /// </summary>
        [Test]
        public void Build_WhenExplicitTableNameIsProvided_ShouldUseTableName()
        {
            var query = _provider
                .CreateQueryBuilder()
                .Update<User>("CustomUsers")
                .Set(user => user.Email, "updated@test.com")
                .Where(user => user.Id == 10)
                .Build();

            Assert.That(query.CommandText, Does.Contain("CustomUsers"));
        }

        /// <summary>
        /// Validates logical operator composition between UPDATE WHERE predicates.
        /// </summary>
        [Test]
        public void Build_WhenMultiplePredicatesUseLogicalOperator_ShouldGenerateOperator()
        {
            var query = _provider
                .CreateQueryBuilder()
                .Update<User>()
                .Set(user => user.IsActive, false)
                .Where(user => user.Age < 18)
                .Where(user => user.IsDeleted, QueryLogicalOperator.Or)
                .Build();

            Assert.That(query.CommandText, Does.Contain(" OR "));
        }

        /// <summary>
        /// Validates conditional UPDATE WHERE predicate generation.
        /// </summary>
        [Test]
        public void Build_WhenWhereIfConditionsDiffer_ShouldIncludeOnlyEnabledPredicate()
        {
            var query = _provider
                .CreateQueryBuilder()
                .Update<User>()
                .Set(user => user.IsActive, false)
                .WhereIf(true, user => user.Age >= 18)
                .WhereIf(false, user => user.IsDeleted)
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(query.CommandText, Does.Contain("Age"));
                Assert.That(query.CommandText, Does.Not.Contain("IsDeleted"));
                Assert.That(query.Parameters, Has.Count.EqualTo(2));
            });
        }
    }
}
