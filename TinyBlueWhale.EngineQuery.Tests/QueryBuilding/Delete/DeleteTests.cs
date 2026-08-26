using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Tests.Models;
using TinyBlueWhale.EngineQuery.Tests.Providers;

namespace TinyBlueWhale.EngineQuery.Tests.QueryBuilding.Delete
{
    /// <summary>
    /// Validates provider-independent DELETE query behavior.
    /// </summary>
    [TestFixtureSource(typeof(QueryTestProviderSource), nameof(QueryTestProviderSource.GetProviders))]
    internal sealed class DeleteTests(IQueryTestProvider provider)
    {
        private readonly IQueryTestProvider _provider = provider ?? throw new ArgumentNullException(nameof(provider));

        /// <summary>
        /// Validates SQL generation for a strongly typed DELETE command.
        /// </summary>
        [Test]
        public void Build_WhenDeleteIsValid_ShouldGenerateExpectedSql()
        {
            var query = _provider
                .CreateQueryBuilder()
                .DeleteFrom<User>()
                .Where(user => user.Id == 10)
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(query.CommandText, Does.Contain("DELETE"));
                Assert.That(query.CommandText, Does.Contain("FROM"));
                Assert.That(query.CommandText, Does.Contain("Users"));
                Assert.That(query.CommandText, Does.Contain("WHERE"));
                Assert.That(query.CommandText, Does.Contain("Id"));

                Assert.That(query.Parameters, Has.Count.EqualTo(1));
                Assert.That(query.Parameters[0].Name, Is.EqualTo("@p0"));
                Assert.That(query.Parameters[0].Value, Is.EqualTo(10));
            });
        }

        /// <summary>
        /// Validates DELETE command generation using a NOT IN collection condition.
        /// </summary>
        [Test]
        public void Build_WhenWhereNotInIsConfigured_ShouldGenerateNotInPredicate()
        {
            var excludedUserIds = new[] { 10, 20, 30 };

            var query = _provider
                .CreateQueryBuilder()
                .DeleteFrom<User>()
                .WhereNotIn(user => user.Id, excludedUserIds)
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(query.CommandText, Does.Contain("DELETE"));
                Assert.That(query.CommandText, Does.Contain("NOT IN"));
                Assert.That(query.CommandText, Does.Contain("Id"));

                Assert.That(query.Parameters, Has.Count.EqualTo(3));
                Assert.That(query.Parameters[0].Value, Is.EqualTo(10));
                Assert.That(query.Parameters[1].Value, Is.EqualTo(20));
                Assert.That(query.Parameters[2].Value, Is.EqualTo(30));
            });
        }

        /// <summary>
        /// Validates DELETE parameter generation in predicate order.
        /// </summary>
        [Test]
        public void Build_WhenMultiplePredicatesAreConfigured_ShouldGenerateParametersInClauseOrder()
        {
            var query = _provider
                .CreateQueryBuilder()
                .DeleteFrom<User>()
                .Where(user => user.Id == 10)
                .Where(user => user.IsDeleted, QueryLogicalOperator.Or)
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(query.Parameters, Has.Count.EqualTo(2));

                Assert.That(query.Parameters[0].Name, Is.EqualTo("@p0"));
                Assert.That(query.Parameters[0].Value, Is.EqualTo(10));

                Assert.That(query.Parameters[1].Name, Is.EqualTo("@p1"));
                Assert.That(query.Parameters[1].Value, Is.EqualTo(true));
            });
        }

        /// <summary>
        /// Validates conditional DELETE WHERE predicate generation.
        /// </summary>
        [Test]
        public void Build_WhenWhereIfConditionsDiffer_ShouldIncludeOnlyEnabledPredicate()
        {
            var query = _provider
                .CreateQueryBuilder()
                .DeleteFrom<User>()
                .WhereIf(true, user => user.Age >= 18)
                .WhereIf(false, user => user.IsDeleted)
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(query.CommandText, Does.Contain("Age"));
                Assert.That(query.CommandText, Does.Not.Contain("IsDeleted"));
                Assert.That(query.Parameters, Has.Count.EqualTo(1));
            });
        }

        /// <summary>
        /// Validates logical operator composition between DELETE WHERE predicates.
        /// </summary>
        [Test]
        public void Build_WhenMultiplePredicatesUseLogicalOperator_ShouldGenerateOperator()
        {
            var query = _provider
                .CreateQueryBuilder()
                .DeleteFrom<User>()
                .Where(user => user.Age < 18)
                .Where(user => user.IsDeleted, QueryLogicalOperator.Or)
                .Build();

            Assert.That(query.CommandText, Does.Contain(" OR "));
        }

        /// <summary>
        /// Validates DELETE generation using an explicitly configured table name.
        /// </summary>
        [Test]
        public void Build_WhenExplicitTableNameIsProvided_ShouldUseTableName()
        {
            var query = _provider
                .CreateQueryBuilder()
                .DeleteFrom<User>("CustomUsers")
                .Where(user => user.Id == 10)
                .Build();

            Assert.That(query.CommandText, Does.Contain("CustomUsers"));
        }

        /// <summary>
        /// Validates that DELETE commands require at least one WHERE predicate.
        /// </summary>
        [Test]
        public void Build_WhenWherePredicateIsMissing_ShouldThrow()
        {
            var commandBuilder = _provider
                .CreateQueryBuilder()
                .DeleteFrom<User>();

            var exception = Assert.Throws<InvalidOperationException>(() => commandBuilder.Build());

            Assert.That(
                exception!.Message,
                Is.EqualTo("At least one WHERE predicate must be configured before building a DELETE command."));
        }
    }
}
