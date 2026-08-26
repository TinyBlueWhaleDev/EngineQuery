using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TinyBlueWhale.EngineQuery.DependencyInjection.Enums;
using TinyBlueWhale.EngineQuery.DependencyInjection.Extensions;
using TinyBlueWhale.EngineQuery.DependencyInjection.Interfaces;
using TinyBlueWhale.EngineQuery.Metadata.EntityFramework;
using TinyBlueWhale.EngineQuery.Metadata.EntityFramework.Models;
using TinyBlueWhale.EngineQuery.Metadata.Models;

namespace TinyBlueWhale.EngineQuery.Tests.DependencyInjection
{
    /// <summary>
    /// Validates query engine dependency injection and factory resolution behavior.
    /// </summary>
    [TestFixture]
    internal sealed class QueryEngineFactoryTests
    {
        /// <summary>
        /// Validates direct query engine resolution when a single
        /// metadata strategy is configured.
        /// </summary>
        [Test]
        public void AddEngineQuery_WhenSingleMetadataStrategyIsConfigured_ShouldAllowDirectEngineResolution()
        {
            var services = new ServiceCollection();

            services.AddDbContext<MetadataFallbackDbContext>(options =>
            {
                options.UseInMemoryDatabase(nameof(MetadataFallbackDbContext));
            });

            services.AddEngineQuery(options =>
            {
                options.Add(QueryEngineProvider.SqlServer, metadata =>
                {
                    metadata.UseEntityFrameworkMetadata<MetadataFallbackDbContext>();
                });
            });

            using var serviceProvider = services.BuildServiceProvider();

            var queryEngine = serviceProvider.GetService<IQueryEngine>();

            Assert.That(queryEngine, Is.Not.Null);
        }

        /// <summary>
        /// Validates that direct query engine resolution is not registered
        /// when multiple metadata strategies are configured.
        /// </summary>
        [Test]
        public void AddEngineQuery_WhenMultipleMetadataStrategiesAreConfigured_ShouldNotRegisterDirectEngine()
        {
            var services = new ServiceCollection();

            services.AddDbContext<MetadataFallbackDbContext>(options =>
            {
                options.UseInMemoryDatabase(nameof(MetadataFallbackDbContext));
            });

            services.AddEngineQuery(options =>
            {
                options.Add(QueryEngineProvider.SqlServer, metadata =>
                {
                    metadata.UseAttributeMetadata();
                    metadata.UseEntityFrameworkMetadata<MetadataFallbackDbContext>();
                });
            });

            using var serviceProvider = services.BuildServiceProvider();

            var queryEngine = serviceProvider.GetService<IQueryEngine>();

            Assert.That(queryEngine, Is.Null);
        }

        /// <summary>
        /// Validates that the query engine factory requires an explicit
        /// metadata strategy when multiple strategies are configured.
        /// </summary>
        [Test]
        public void Create_WhenMultipleMetadataStrategiesAreConfiguredWithoutSelection_ShouldThrow()
        {
            using var serviceProvider = CreateMultipleMetadataStrategyServiceProvider();

            var factory = serviceProvider.GetRequiredService<IQueryEngineFactory>();

            var exception = Assert.Throws<InvalidOperationException>(() =>
                factory.Create(QueryEngineProvider.SqlServer));

            Assert.That(
                exception!.Message,
                Does.Contain("Multiple metadata strategies"));
        }

        /// <summary>
        /// Validates query engine resolution using the explicitly
        /// selected attribute metadata strategy.
        /// </summary>
        [Test]
        public void Create_WhenAttributeStrategyIsSelected_ShouldResolveEngine()
        {
            using var serviceProvider = CreateMultipleMetadataStrategyServiceProvider();

            var factory = serviceProvider.GetRequiredService<IQueryEngineFactory>();

            var queryEngine = factory.Create(
                QueryEngineProvider.SqlServer,
                MetadataStrategy.Attribute);

            Assert.That(queryEngine, Is.Not.Null);
        }

        /// <summary>
        /// Validates query engine resolution using the explicitly selected
        /// Entity Framework metadata strategy and its convention fallback.
        /// </summary>
        [Test]
        public void Create_WhenEntityFrameworkStrategyIsSelected_ShouldUseConventionFallback()
        {
            using var serviceProvider = CreateMultipleMetadataStrategyServiceProvider();

            var factory = serviceProvider.GetRequiredService<IQueryEngineFactory>();

            var queryEngine = factory.Create(
                QueryEngineProvider.SqlServer,
                EntityFrameworkMetadataStrategies.EntityFramework);

            var query = queryEngine
                .From<ConventionUser>()
                .Select(user => user.Id)
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(query, Is.Not.Null);
                Assert.That(query.CommandText, Does.Contain("[ConventionUser]"));
                Assert.That(query.CommandText, Does.Contain("[Id]"));
            });
        }

        /// <summary>
        /// Validates that the query engine factory resolves each configured
        /// provider when convention metadata is used.
        /// </summary>
        [Test]
        public void Create_WhenMultipleProvidersUseConvention_ShouldResolveEachProvider()
        {
            var services = new ServiceCollection();

            services.AddEngineQuery(options =>
            {
                options.Add(QueryEngineProvider.SqlServer);
                options.Add(QueryEngineProvider.PostgreSql);
                options.Add(QueryEngineProvider.MySql);
            });

            using var serviceProvider = services.BuildServiceProvider();

            var factory = serviceProvider.GetRequiredService<IQueryEngineFactory>();

            var sqlServer = factory.Create(QueryEngineProvider.SqlServer);
            var postgreSql = factory.Create(QueryEngineProvider.PostgreSql);
            var mySql = factory.Create(QueryEngineProvider.MySql);

            Assert.Multiple(() =>
            {
                Assert.That(sqlServer, Is.Not.Null);
                Assert.That(postgreSql, Is.Not.Null);
                Assert.That(mySql, Is.Not.Null);
            });
        }

        /// <summary>
        /// Creates a service provider configured with multiple
        /// metadata strategies for SQL Server.
        /// </summary>
        /// <returns>
        /// Configured service provider.
        /// </returns>
        private static ServiceProvider CreateMultipleMetadataStrategyServiceProvider()
        {
            var services = new ServiceCollection();

            services.AddDbContext<MetadataFallbackDbContext>(options =>
            {
                options.UseInMemoryDatabase(nameof(MetadataFallbackDbContext));
            });

            services.AddEngineQuery(options =>
            {
                options.Add(QueryEngineProvider.SqlServer, metadata =>
                {
                    metadata.UseAttributeMetadata();
                    metadata.UseEntityFrameworkMetadata<MetadataFallbackDbContext>();
                });
            });

            return services.BuildServiceProvider();
        }

        /// <summary>
        /// Entity used to validate convention fallback after
        /// Entity Framework strategy selection.
        /// </summary>
        private sealed class ConventionUser
        {
            /// <summary>
            /// Gets or initializes the user identifier.
            /// </summary>
            public int Id { get; init; }

            /// <summary>
            /// Gets or initializes the user email.
            /// </summary>
            public string? Email { get; init; }
        }

        /// <summary>
        /// Entity Framework context intentionally excluding
        /// <see cref="ConventionUser"/> from its model.
        /// </summary>
        private sealed class MetadataFallbackDbContext(
            DbContextOptions<MetadataFallbackDbContext> options)
            : DbContext(options)
        {
        }
    }
}
