using Microsoft.Extensions.DependencyInjection;
using TinyBlueWhale.EngineQuery.DependencyInjection.Enums;
using TinyBlueWhale.EngineQuery.DependencyInjection.Extensions;
using TinyBlueWhale.EngineQuery.DependencyInjection.Interfaces;

namespace TinyBlueWhale.EngineQuery.Tests.Infrastructure
{
    /// <summary>
    /// Validates SQL alias generation and column qualification behavior.
    /// </summary>
    [TestFixture]
    public sealed class AliasSqlGenerationTests
    {
        private IQueryEngine _queryEngine = null!;

        /// <summary>
        /// Initializes the query engine used by alias generation tests.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            var services = new ServiceCollection();

            services.AddEngineQuery(options =>
            {
                options.Add(QueryEngineProvider.SqlServer);
            });

            var serviceProvider = services.BuildServiceProvider();

            _queryEngine = serviceProvider.GetRequiredService<IQueryEngine>();
        }

        /// <summary>
        /// Validates that a common table expression source does not generate
        /// an unnecessary alias when one is not explicitly configured.
        /// </summary>
        [Test]
        public void FromCte_WhenAliasIsNotProvided_ShouldNotGenerateAlias()
        {
            // Act
            var query = _queryEngine
                .With<CteOrderSummary, AliasOrder>(
                    "order_summary",
                    cte => cte
                        .From<AliasOrder>()
                        .Select(order => new
                        {
                            order.UserId
                        }))
                .FromCte<CteOrderSummary>("order_summary")
                .Select(summary => summary.UserId)
                .Build();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(
                    query.CommandText,
                    Does.Contain("FROM [order_summary]"));

                Assert.That(
                    query.CommandText,
                    Does.Not.Contain("FROM [order_summary] AS"));

                Assert.That(
                    query.CommandText,
                    Does.Contain("[UserId]"));

                Assert.That(
                    query.CommandText,
                    Does.Not.Contain("[order_summary].[UserId]"));
            });
        }

        /// <summary>
        /// Validates that an explicitly configured common table expression alias
        /// is preserved and used to qualify generated column references.
        /// </summary>
        [Test]
        public void FromCte_WhenAliasIsProvided_ShouldUseExplicitAlias()
        {

            // Act
            var query = _queryEngine
                .With<CteOrderSummary, AliasOrder>(
                    "order_summary",
                    cte => cte
                        .From<AliasOrder>()
                        .Select(order => new
                        {
                            order.UserId
                        }))
                .FromCte<CteOrderSummary>(
                    "order_summary",
                    alias: "os")
                .Select(summary => summary.UserId)
                .Build();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(
                    query.CommandText,
                    Does.Contain("FROM [order_summary] AS [os]"));

                Assert.That(
                    query.CommandText,
                    Does.Contain("[os].[UserId]"));
            });
        }

        /// <summary>
        /// Validates that a single query source does not generate an unnecessary alias.
        /// </summary>
        [Test]
        public void From_WhenAliasIsNotProvided_ShouldNotGenerateAlias()
        {
            // Act
            var query = _queryEngine
                .From<AliasUser>()
                .Select(user => new
                {
                    user.Id,
                    user.Email
                })
                .Where(user => user.Id == 1)
                .Build();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(
                    query.CommandText,
                    Does.Contain("FROM [AliasUser]"));

                Assert.That(
                    query.CommandText,
                    Does.Not.Contain("FROM [AliasUser] AS"));

                Assert.That(
                    query.CommandText,
                    Does.Contain("[Id]"));

                Assert.That(
                    query.CommandText,
                    Does.Contain("[Email]"));

                Assert.That(
                    query.CommandText,
                    Does.Not.Contain("[t0].[Id]"));
            });
        }

        /// <summary>
        /// Validates that an explicitly configured alias is preserved and used
        /// to qualify generated column references.
        /// </summary>
        [Test]
        public void From_WhenAliasIsProvided_ShouldUseExplicitAlias()
        {
            // Act
            var query = _queryEngine
                .From<AliasUser>(alias: "u")
                .Select(user => new
                {
                    user.Id,
                    user.Email
                })
                .Where(user => user.Id == 1)
                .Build();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(
                    query.CommandText,
                    Does.Contain("FROM [AliasUser] AS [u]"));

                Assert.That(
                    query.CommandText,
                    Does.Contain("[u].[Id]"));

                Assert.That(
                    query.CommandText,
                    Does.Contain("[u].[Email]"));
            });
        }

        /// <summary>
        /// Validates that deterministic aliases are generated when multiple
        /// query sources require column qualification.
        /// </summary>
        [Test]
        public void InnerJoin_WhenAliasesAreNotProvided_ShouldGenerateDeterministicAliases()
        {
            // Act
            var query = _queryEngine
                .From<AliasUser>()
                .InnerJoin<AliasUser, AliasOrder>(
                    alias: null,
                    on: (user, order) => user.Id == order.UserId)
                .Select<AliasUser>(user => user.Id)
                .Select<AliasOrder>(order => order.Id)
                .Build();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(
                    query.CommandText,
                    Does.Contain("FROM [AliasUser] AS [t0]"));

                Assert.That(
                    query.CommandText,
                    Does.Contain("INNER JOIN [AliasOrder] AS [t1]"));

                Assert.That(
                    query.CommandText,
                    Does.Contain("[t0].[Id]"));

                Assert.That(
                    query.CommandText,
                    Does.Contain("[t1].[Id]"));

                Assert.That(
                    query.CommandText,
                    Does.Contain("[t0].[Id] = [t1].[UserId]"));
            });
        }

        /// <summary>
        /// Validates that explicitly configured aliases are preserved when
        /// multiple query sources participate in the query.
        /// </summary>
        [Test]
        public void InnerJoin_WhenAliasesAreProvided_ShouldPreserveExplicitAliases()
        {
            // Act
            var query = _queryEngine
                .From<AliasUser>(alias: "u")
                .InnerJoin<AliasUser, AliasOrder>(
                    alias: "o",
                    on: (user, order) => user.Id == order.UserId)
                .Select<AliasUser>(user => user.Id)
                .Select<AliasOrder>(order => order.Id)
                .Build();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(
                    query.CommandText,
                    Does.Contain("FROM [AliasUser] AS [u]"));

                Assert.That(
                    query.CommandText,
                    Does.Contain("INNER JOIN [AliasOrder] AS [o]"));

                Assert.That(
                    query.CommandText,
                    Does.Contain("[u].[Id]"));

                Assert.That(
                    query.CommandText,
                    Does.Contain("[o].[Id]"));

                Assert.That(
                    query.CommandText,
                    Does.Contain("[u].[Id] = [o].[UserId]"));
            });
        }

        /// <summary>
        /// Entity used as the root source in alias generation tests.
        /// </summary>
        private sealed class AliasUser
        {
            /// <summary>
            /// Gets or initializes the user identifier.
            /// </summary>
            public int Id { get; init; }

            /// <summary>
            /// Gets or initializes the user email address.
            /// </summary>
            public string? Email { get; init; }
        }

        /// <summary>
        /// Entity used as the joined source in alias generation tests.
        /// </summary>
        private sealed class AliasOrder
        {
            /// <summary>
            /// Gets or initializes the order identifier.
            /// </summary>
            public int Id { get; init; }

            /// <summary>
            /// Gets or initializes the related user identifier.
            /// </summary>
            public int UserId { get; init; }
        }

        /// <summary>
        /// Projection type used to validate common table expression alias behavior.
        /// </summary>
        private sealed class CteOrderSummary
        {
            /// <summary>
            /// Gets or initializes the related user identifier.
            /// </summary>
            public int UserId { get; init; }
        }
    }
}
