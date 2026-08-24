using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TinyBlueWhale.EngineQuery.DependencyInjection.Enums;
using TinyBlueWhale.EngineQuery.DependencyInjection.Extensions;
using TinyBlueWhale.EngineQuery.DependencyInjection.Interfaces;
using TinyBlueWhale.EngineQuery.Metadata.EntityFramework;
using TinyBlueWhale.EngineQuery.Metadata.EntityFramework.Models;
using TinyBlueWhale.EngineQuery.Metadata.Models;

namespace TinyBlueWhale.EngineQuery.Tests.Infrastructure
{
    [TestFixture]
    public sealed class DependencyInjectionMetadataFallbackTests
    {
        [Test]
        public void From_WhenExplicitTableNameIsUsed_PreservesConventionColumnMappings()
        {
            // Arrange
            var services = new ServiceCollection();

            services.AddEngineQuery(options =>
            {
                options.Add(QueryEngineProvider.SqlServer);
            });

            using var serviceProvider = services.BuildServiceProvider();

            var queryEngine = serviceProvider.GetRequiredService<IQueryEngine>();

            // Act
            var query = queryEngine
                .From<ConventionUser>("custom_users")
                .Select(user => new
                {
                    user.Id,
                    user.Email
                })
                .Build();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(query.CommandText, Does.Contain("[custom_users]"));
                Assert.That(query.CommandText, Does.Contain("[Id]"));
                Assert.That(query.CommandText, Does.Contain("[Email]"));
            });
        }

        [Test]
        public void InsertInto_WhenExplicitTableNameIsUsed_PreservesConventionColumnMappings()
        {
            // Arrange
            var services = new ServiceCollection();

            services.AddEngineQuery(options =>
            {
                options.Add(QueryEngineProvider.SqlServer);
            });

            using var serviceProvider = services.BuildServiceProvider();

            var queryEngine = serviceProvider.GetRequiredService<IQueryEngine>();

            // Act
            var query = queryEngine
                .InsertInto<ConventionUser>("custom_users")
                .Set(user => user.Email, "test@test.com")
                .Build();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(query.CommandText, Does.Contain("[custom_users]"));
                Assert.That(query.CommandText, Does.Contain("[Email]"));
            });
        }

        [Test]
        public void Update_WhenExplicitTableNameIsUsed_PreservesConventionColumnMappings()
        {
            // Arrange
            var services = new ServiceCollection();

            services.AddEngineQuery(options =>
            {
                options.Add(QueryEngineProvider.SqlServer);
            });

            using var serviceProvider = services.BuildServiceProvider();

            var queryEngine = serviceProvider.GetRequiredService<IQueryEngine>();

            // Act
            var query = queryEngine
                .Update<ConventionUser>("custom_users")
                .Set(user => user.Email, "updated@test.com")
                .Where(user => user.Id == 10)
                .Build();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(query.CommandText, Does.Contain("[custom_users]"));
                Assert.That(query.CommandText, Does.Contain("[Email]"));
                Assert.That(query.CommandText, Does.Contain("[Id]"));
            });
        }

        [Test]
        public void DeleteFrom_WhenExplicitTableNameIsUsed_PreservesConventionColumnMappings()
        {
            // Arrange
            var services = new ServiceCollection();

            services.AddEngineQuery(options =>
            {
                options.Add(QueryEngineProvider.SqlServer);
            });

            using var serviceProvider = services.BuildServiceProvider();

            var queryEngine = serviceProvider.GetRequiredService<IQueryEngine>();

            // Act
            var query = queryEngine
                .DeleteFrom<ConventionUser>("custom_users")
                .Where(user => user.Id == 10)
                .Build();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(query.CommandText, Does.Contain("[custom_users]"));
                Assert.That(query.CommandText, Does.Contain("[Id]"));
            });
        }


        [Test]
        public void AddEngineQuery_WhenMetadataIsNotConfigured_UsesConventionMetadata()
        {
            // Arrange
            var services = new ServiceCollection();

            services.AddEngineQuery(options =>
            {
                options.Add(QueryEngineProvider.SqlServer);
            });

            using var serviceProvider = services.BuildServiceProvider();

            var queryEngine = serviceProvider.GetRequiredService<IQueryEngine>();

            // Act
            var query = queryEngine
                .From<ConventionUser>()
                .Select(user => new
                {
                    user.Id,
                    user.Email
                })
                .Build();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(query, Is.Not.Null);
                Assert.That(query.CommandText, Does.Contain("[ConventionUser]"));
                Assert.That(query.CommandText, Does.Contain("[Id]"));
                Assert.That(query.CommandText, Does.Contain("[Email]"));
            });
        }

        [Test]
        public void AddEngineQuery_WhenEntityFrameworkCannotResolveEntity_UsesConventionFallback()
        {
            // Arrange
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

            var queryEngine = serviceProvider.GetRequiredService<IQueryEngine>();

            // Act
            var query = queryEngine
                .From<ConventionUser>()
                .Select(user => new
                {
                    user.Id,
                    user.Email
                })
                .Build();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(query, Is.Not.Null);
                Assert.That(query.CommandText, Does.Contain("[ConventionUser]"));
                Assert.That(query.CommandText, Does.Contain("[Id]"));
                Assert.That(query.CommandText, Does.Contain("[Email]"));
            });
        }

        [Test]
        public void AddEngineQuery_WhenSingleMetadataStrategyIsConfigured_AllowsDirectEngineResolution()
        {
            // Arrange
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

            // Act
            var queryEngine = serviceProvider.GetService<IQueryEngine>();

            // Assert
            Assert.That(queryEngine, Is.Not.Null);
        }

        [Test]
        public void AddEngineQuery_WhenMultipleMetadataStrategiesAreConfigured_DoesNotRegisterDirectEngine()
        {
            // Arrange
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

            // Act
            var queryEngine = serviceProvider.GetService<IQueryEngine>();

            // Assert
            Assert.That(queryEngine, Is.Null);
        }

        [Test]
        public void QueryEngineFactory_WhenMultipleMetadataStrategiesAreConfigured_RequiresExplicitStrategy()
        {
            // Arrange
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

            var factory = serviceProvider.GetRequiredService<IQueryEngineFactory>();

            // Act
            var exception = Assert.Throws<InvalidOperationException>(
                () => factory.Create(QueryEngineProvider.SqlServer));

            // Assert
            Assert.That(
                exception!.Message,
                Does.Contain("Multiple metadata strategies"));
        }

        [Test]
        public void QueryEngineFactory_WhenAttributeStrategyIsSelected_ResolvesEngine()
        {
            // Arrange
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

            var factory = serviceProvider.GetRequiredService<IQueryEngineFactory>();

            // Act
            var queryEngine = factory.Create(
                QueryEngineProvider.SqlServer,
                MetadataStrategy.Attribute);

            // Assert
            Assert.That(queryEngine, Is.Not.Null);
        }

        [Test]
        public void QueryEngineFactory_WhenEntityFrameworkStrategyIsSelected_UsesConventionFallback()
        {
            // Arrange
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

            var factory = serviceProvider.GetRequiredService<IQueryEngineFactory>();

            var queryEngine = factory.Create(
                QueryEngineProvider.SqlServer,
                EntityFrameworkMetadataStrategies.EntityFramework);

            // Act
            var query = queryEngine
                .From<ConventionUser>()
                .Select(user => user.Id)
                .Build();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(query, Is.Not.Null);
                Assert.That(query.CommandText, Does.Contain("[ConventionUser]"));
                Assert.That(query.CommandText, Does.Contain("[Id]"));
            });
        }

        [Test]
        public void QueryEngineFactory_WhenMultipleProvidersUseConvention_ResolvesEachProvider()
        {
            // Arrange
            var services = new ServiceCollection();

            services.AddEngineQuery(options =>
            {
                options.Add(QueryEngineProvider.SqlServer);
                options.Add(QueryEngineProvider.PostgreSql);
                options.Add(QueryEngineProvider.MySql);
            });

            using var serviceProvider = services.BuildServiceProvider();

            var factory = serviceProvider.GetRequiredService<IQueryEngineFactory>();

            // Act
            var sqlServer = factory.Create(QueryEngineProvider.SqlServer);
            var postgreSql = factory.Create(QueryEngineProvider.PostgreSql);
            var mySql = factory.Create(QueryEngineProvider.MySql);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(sqlServer, Is.Not.Null);
                Assert.That(postgreSql, Is.Not.Null);
                Assert.That(mySql, Is.Not.Null);
            });
        }

        private sealed class ConventionUser
        {
            public int Id { get; init; }

            public string? Email { get; init; }
        }

        /// <summary>
        /// Entity Framework Core context used to validate metadata fallback behavior.
        /// </summary>
        private sealed class MetadataFallbackDbContext(
            DbContextOptions<MetadataFallbackDbContext> options)
            : DbContext(options)
        {
        }
    }

}
