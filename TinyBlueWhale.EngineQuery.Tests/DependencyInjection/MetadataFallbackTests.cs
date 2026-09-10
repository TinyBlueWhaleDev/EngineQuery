using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TinyBlueWhale.EngineQuery.Core.Enums;
using TinyBlueWhale.EngineQuery.DependencyInjection.Extensions;
using TinyBlueWhale.EngineQuery.DependencyInjection.Interfaces;
using TinyBlueWhale.EngineQuery.Generated;
using TinyBlueWhale.EngineQuery.Metadata.EntityFramework;
using TinyBlueWhale.EngineQuery.Metadata.EntityFramework.Models;
using TinyBlueWhale.EngineQuery.SqlServer.Profiles;

namespace TinyBlueWhale.EngineQuery.Tests.DependencyInjection
{
    /// <summary>
    /// Validates convention metadata fallback behavior during dependency injection resolution.
    /// </summary>
    [TestFixture]
    internal sealed class MetadataFallbackTests
    {
        [Test]
        public void From_WhenExplicitTableNameIsUsed_ShouldPreserveConventionColumnMappings()
        {
            using var serviceProvider = CreateConventionServiceProvider();
            var queryEngine = CreateSqlServerEngine(serviceProvider);

            var query = queryEngine
                .From<ConventionUser>("custom_users")
                .Select(user => new
                {
                    user.Id,
                    user.Email
                })
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(query.CommandText, Does.Contain("[custom_users]"));
                Assert.That(query.CommandText, Does.Contain("[Id]"));
                Assert.That(query.CommandText, Does.Contain("[Email]"));
            });
        }

        [Test]
        public void InsertInto_WhenExplicitTableNameIsUsed_ShouldPreserveConventionColumnMappings()
        {
            using var serviceProvider = CreateConventionServiceProvider();
            var queryEngine = CreateSqlServerEngine(serviceProvider);

            var query = queryEngine
                .InsertInto<ConventionUser>("custom_users")
                .Set(user => user.Email, "test@test.com")
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(query.CommandText, Does.Contain("[custom_users]"));
                Assert.That(query.CommandText, Does.Contain("[Email]"));
            });
        }

        [Test]
        public void Update_WhenExplicitTableNameIsUsed_ShouldPreserveConventionColumnMappings()
        {
            using var serviceProvider = CreateConventionServiceProvider();
            var queryEngine = CreateSqlServerEngine(serviceProvider);

            var query = queryEngine
                .Update<ConventionUser>("custom_users")
                .Set(user => user.Email, "updated@test.com")
                .Where(user => user.Id == 10)
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(query.CommandText, Does.Contain("[custom_users]"));
                Assert.That(query.CommandText, Does.Contain("[Email]"));
                Assert.That(query.CommandText, Does.Contain("[Id]"));
            });
        }

        [Test]
        public void DeleteFrom_WhenExplicitTableNameIsUsed_ShouldPreserveConventionColumnMappings()
        {
            using var serviceProvider = CreateConventionServiceProvider();
            var queryEngine = CreateSqlServerEngine(serviceProvider);

            var query = queryEngine
                .DeleteFrom<ConventionUser>("custom_users")
                .Where(user => user.Id == 10)
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(query.CommandText, Does.Contain("[custom_users]"));
                Assert.That(query.CommandText, Does.Contain("[Id]"));
            });
        }

        [Test]
        public void AddEngineQuery_WhenMetadataIsNotConfigured_ShouldUseConventionMetadata()
        {
            using var serviceProvider = CreateConventionServiceProvider();
            var queryEngine = CreateSqlServerEngine(serviceProvider);

            var query = queryEngine
                .From<ConventionUser>()
                .Select(user => new
                {
                    user.Id,
                    user.Email
                })
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(query.CommandText, Does.Contain("[ConventionUser]"));
                Assert.That(query.CommandText, Does.Contain("[Id]"));
                Assert.That(query.CommandText, Does.Contain("[Email]"));
            });
        }

        [Test]
        public void AddEngineQuery_WhenEntityFrameworkCannotResolveEntity_ShouldUseConventionFallback()
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
            var factory = serviceProvider.GetRequiredService<
                IQueryEngineFactory<SqlServerDefaultProfile, ISqlServerDefaultQueryEngine>>();

            var queryEngine = factory.Create(
                EntityFrameworkMetadataStrategies.EntityFramework);

            var query = queryEngine
                .From<ConventionUser>()
                .Select(user => new
                {
                    user.Id,
                    user.Email
                })
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(query.CommandText, Does.Contain("[ConventionUser]"));
                Assert.That(query.CommandText, Does.Contain("[Id]"));
                Assert.That(query.CommandText, Does.Contain("[Email]"));
            });
        }

        private static ISqlServerDefaultQueryEngine CreateSqlServerEngine(
            ServiceProvider serviceProvider)
        {
            return serviceProvider
                .GetRequiredService<
                    IQueryEngineFactory<SqlServerDefaultProfile, ISqlServerDefaultQueryEngine>>()
                .Create();
        }

        private static ServiceProvider CreateConventionServiceProvider()
        {
            var services = new ServiceCollection();

            services.AddEngineQuery(options =>
            {
                options.Add(QueryEngineProvider.SqlServer);
            });

            return services.BuildServiceProvider();
        }

        private sealed class ConventionUser
        {
            public int Id { get; init; }

            public string? Email { get; init; }
        }

        private sealed class MetadataFallbackDbContext(
            DbContextOptions<MetadataFallbackDbContext> options)
            : DbContext(options)
        {
        }
    }
}
