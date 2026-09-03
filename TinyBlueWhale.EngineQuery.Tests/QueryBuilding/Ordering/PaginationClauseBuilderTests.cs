using TinyBlueWhale.EngineQuery.Core.Parameters;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;
using TinyBlueWhale.EngineQuery.MySql.Dialects;
using TinyBlueWhale.EngineQuery.Sql.Clauses.Pagination;
using TinyBlueWhale.EngineQuery.Sql.Compilation;
using TinyBlueWhale.EngineQuery.Sql.Interfaces.Strategies;

namespace TinyBlueWhale.EngineQuery.Tests.QueryBuilding.Ordering
{
    /// <summary>
    /// Validates pagination clause builder behavior.
    /// </summary>
    [TestFixture]
    public sealed class PaginationClauseBuilderTests
    {
        [Test]
        public void Build_ShouldDelegateToConfiguredPaginationStrategy()
        {
            var strategy = new TestPaginationStrategy("PAGINATION");

            var builder = new PaginationClauseBuilder(strategy);

            var queryDefinition = CreateQueryDefinition();

            var context = CreateCompilationContext();

            var result = builder.Build(queryDefinition, context);

            Assert.That(result, Is.EqualTo("PAGINATION"));
            Assert.That(strategy.BuildCallCount, Is.EqualTo(1));
            Assert.That(strategy.LastQueryDefinition, Is.SameAs(queryDefinition));
            Assert.That(strategy.LastContext, Is.SameAs(context));
        }

        [Test]
        public void Constructor_WhenPaginationStrategyIsNull_ShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() => new PaginationClauseBuilder(null!));
        }

        [Test]
        public void Build_WhenQueryDefinitionIsNull_ShouldThrow()
        {
            var strategy = new TestPaginationStrategy("PAGINATION");

            var builder = new PaginationClauseBuilder(strategy);

            var context = CreateCompilationContext();

            Assert.Throws<ArgumentNullException>(() => builder.Build(null!, context));
        }

        [Test]
        public void Build_WhenCompilationContextIsNull_ShouldThrow()
        {
            var strategy = new TestPaginationStrategy("PAGINATION");

            var builder = new PaginationClauseBuilder(strategy);

            var queryDefinition = CreateQueryDefinition();

            Assert.Throws<ArgumentNullException>(() => builder.Build(queryDefinition, null!));
        }

        private static CompiledQueryDefinition CreateQueryDefinition()
        {
            return new CompiledQueryDefinition
            {
                TableName = "Users",
                EntityType = typeof(object)
            };
        }

        private static QueryCompilationContext CreateCompilationContext()
        {
            return new QueryCompilationContext(
                new MySqlDatabaseDialect(),
                new QueryParameterCollection());
        }

        private sealed class TestPaginationStrategy(string result) : IPaginationStrategy
        {
            private readonly string _result = result;

            public int BuildCallCount { get; private set; }

            public CompiledQueryDefinition? LastQueryDefinition { get; private set; }

            public QueryCompilationContext? LastContext { get; private set; }

            public string Build(CompiledQueryDefinition queryDefinition, QueryCompilationContext context)
            {
                BuildCallCount++;
                LastQueryDefinition = queryDefinition;
                LastContext = context;

                return _result;
            }
        }
    }
}
