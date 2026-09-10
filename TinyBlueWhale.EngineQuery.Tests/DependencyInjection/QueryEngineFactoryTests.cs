using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TinyBlueWhale.EngineQuery.Core.Enums;
using TinyBlueWhale.EngineQuery.DependencyInjection.Extensions;
using TinyBlueWhale.EngineQuery.DependencyInjection.Interfaces;
using TinyBlueWhale.EngineQuery.Generated;
using TinyBlueWhale.EngineQuery.Metadata.EntityFramework;
using TinyBlueWhale.EngineQuery.Metadata.EntityFramework.Models;
using TinyBlueWhale.EngineQuery.Metadata.Models;
using TinyBlueWhale.EngineQuery.MySql.Profiles;
using TinyBlueWhale.EngineQuery.PostgreSql.Profiles;
using TinyBlueWhale.EngineQuery.SqlServer.Profiles;

namespace TinyBlueWhale.EngineQuery.Tests.DependencyInjection
{
    /// <summary>
    /// Validates strongly typed query engine dependency injection and factory resolution behavior.
    /// </summary>
    [TestFixture]
    internal sealed class QueryEngineFactoryTests
    {
        /// <summary>
        /// Validates strongly typed query engine factory registration for a configured provider.
        /// </summary>
        [Test]
        public void AddEngineQuery_WhenSqlServerIsConfigured_ShouldRegisterTypedFactory()
        {
            var services = new ServiceCollection();

            services.AddEngineQuery(options =>
            {
                options.Add(QueryEngineProvider.SqlServer);
            });

            using var serviceProvider = services.BuildServiceProvider();

            var factory = serviceProvider.GetService<
                IQueryEngineFactory<SqlServerDefaultProfile, ISqlServerDefaultQueryEngine>>();

            Assert.That(factory, Is.Not.Null);
        }

        /// <summary>
        /// Validates query engine creation using the SQL Server default profile.
        /// </summary>
        [Test]
        public void Create_WhenSqlServerDefaultProfileIsRequested_ShouldResolveEngine()
        {
            var services = new ServiceCollection();

            services.AddEngineQuery(options =>
            {
                options.Add(QueryEngineProvider.SqlServer);
            });

            using var serviceProvider = services.BuildServiceProvider();

            var factory = serviceProvider.GetRequiredService<
                IQueryEngineFactory<SqlServerDefaultProfile, ISqlServerDefaultQueryEngine>>();

            var queryEngine = factory.Create();

            Assert.That(queryEngine, Is.Not.Null);
        }

        /// <summary>
        /// Validates that query engine creation requires an explicit metadata strategy
        /// when multiple compatible metadata registrations are configured.
        /// </summary>
        [Test]
        public void Create_WhenMultipleMetadataStrategiesAreConfiguredWithoutSelection_ShouldThrow()
        {
            using var serviceProvider = CreateMultipleMetadataStrategyServiceProvider();

            var factory = serviceProvider.GetRequiredService<
                IQueryEngineFactory<SqlServerDefaultProfile, ISqlServerDefaultQueryEngine>>();

            var exception = Assert.Throws<InvalidOperationException>(() =>
                factory.Create());

            Assert.That(
                exception!.Message,
                Does.Contain("Multiple registrations support profile"));
        }

        /// <summary>
        /// Validates query engine resolution using the explicitly selected attribute metadata strategy.
        /// </summary>
        [Test]
        public void Create_WhenAttributeStrategyIsSelected_ShouldResolveEngine()
        {
            using var serviceProvider = CreateMultipleMetadataStrategyServiceProvider();

            var factory = serviceProvider.GetRequiredService<
                IQueryEngineFactory<SqlServerDefaultProfile, ISqlServerDefaultQueryEngine>>();

            var queryEngine = factory.Create(
                MetadataStrategy.Attribute);

            Assert.That(queryEngine, Is.Not.Null);
        }

        /// <summary>
        /// Validates query engine resolution using the explicitly selected Entity Framework
        /// metadata strategy and its convention fallback.
        /// </summary>
        [Test]
        public void Create_WhenEntityFrameworkStrategyIsSelected_ShouldUseConventionFallback()
        {
            using var serviceProvider = CreateMultipleMetadataStrategyServiceProvider();

            var factory = serviceProvider.GetRequiredService<
                IQueryEngineFactory<SqlServerDefaultProfile, ISqlServerDefaultQueryEngine>>();

            var queryEngine = factory.Create(
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
        /// Validates that independently generated query engine factories are registered
        /// for each configured database provider.
        /// </summary>
        [Test]
        public void AddEngineQuery_WhenMultipleProvidersAreConfigured_ShouldRegisterEachTypedFactory()
        {
            var services = new ServiceCollection();

            services.AddEngineQuery(options =>
            {
                options.Add(QueryEngineProvider.SqlServer);
                options.Add(QueryEngineProvider.PostgreSql);
                options.Add(QueryEngineProvider.MySql);
            });

            using var serviceProvider = services.BuildServiceProvider();

            var sqlServerFactory = serviceProvider.GetService<
                IQueryEngineFactory<SqlServerDefaultProfile, ISqlServerDefaultQueryEngine>>();

            var postgreSqlFactory = serviceProvider.GetService<
                IQueryEngineFactory<PostgreSqlDefaultProfile, IPostgreSqlDefaultQueryEngine>>();

            var mySqlFactory = serviceProvider.GetService<
                IQueryEngineFactory<MySqlDefaultProfile, IMySqlDefaultQueryEngine>>();

            Assert.Multiple(() =>
            {
                Assert.That(sqlServerFactory, Is.Not.Null);
                Assert.That(postgreSqlFactory, Is.Not.Null);
                Assert.That(mySqlFactory, Is.Not.Null);
            });
        }

        /// <summary>
        /// Validates that each configured provider creates its corresponding strongly typed query engine.
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

            var sqlServer = serviceProvider
                .GetRequiredService<IQueryEngineFactory<SqlServerDefaultProfile, ISqlServerDefaultQueryEngine>>()
                .Create();

            var postgreSql = serviceProvider
                .GetRequiredService<IQueryEngineFactory<PostgreSqlDefaultProfile, IPostgreSqlDefaultQueryEngine>>()
                .Create();

            var mySql = serviceProvider
                .GetRequiredService<IQueryEngineFactory<MySqlDefaultProfile, IMySqlDefaultQueryEngine>>()
                .Create();

            Assert.Multiple(() =>
            {
                Assert.That(sqlServer, Is.Not.Null);
                Assert.That(postgreSql, Is.Not.Null);
                Assert.That(mySql, Is.Not.Null);
            });
        }

        /// <summary>
        /// Creates a service provider configured with multiple metadata strategies for SQL Server.
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
        /// Entity used to validate convention fallback after Entity Framework strategy selection.
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
        /// Entity Framework context intentionally excluding <see cref="ConventionUser"/>
        /// from its model.
        /// </summary>
        private sealed class MetadataFallbackDbContext(
            DbContextOptions<MetadataFallbackDbContext> options)
            : DbContext(options)
        {
        }
    }
}
