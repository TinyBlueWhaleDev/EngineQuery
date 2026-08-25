using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TinyBlueWhale.EngineQuery.DependencyInjection.Enums;
using TinyBlueWhale.EngineQuery.DependencyInjection.Extensions;
using TinyBlueWhale.EngineQuery.DependencyInjection.Interfaces;
using TinyBlueWhale.EngineQuery.Metadata.EntityFramework;

namespace TinyBlueWhale.EngineQuery.Tests.Infrastructure
{
    /// <summary>
    /// Validates schema-aware SQL generation using Entity Framework metadata.
    /// </summary>
    [TestFixture]
    public sealed class SchemaSqlGenerationTests
    {
        private ServiceProvider _serviceProvider = null!;
        private IQueryEngine _queryEngine = null!;

        /// <summary>
        /// Configures EngineQuery with SQL Server and Entity Framework metadata.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            var services = new ServiceCollection();

            services.AddDbContext<SchemaDbContext>(options =>
            {
                options.UseInMemoryDatabase(nameof(SchemaDbContext));
            });

            services.AddEngineQuery(options =>
            {
                options.Add(QueryEngineProvider.SqlServer, metadata =>
                {
                    metadata.UseEntityFrameworkMetadata<SchemaDbContext>();
                });
            });

            _serviceProvider = services.BuildServiceProvider();
            _queryEngine = _serviceProvider.GetRequiredService<IQueryEngine>();
        }

        /// <summary>
        /// Releases services created for the current test.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            _serviceProvider.Dispose();
        }

        /// <summary>
        /// Validates schema-qualified SELECT generation.
        /// </summary>
        [Test]
        public void From_WhenEntityHasSchema_GeneratesQualifiedTableName()
        {
            var query = _queryEngine
                .From<SchemaUser>()
                .Select(user => new
                {
                    user.Id,
                    user.Email
                })
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(
                    query.CommandText,
                    Does.Contain("FROM [security].[schema_users]"));

                Assert.That(
                    query.CommandText,
                    Does.Contain("[schema_user_id]"));

                Assert.That(
                    query.CommandText,
                    Does.Contain("[email]"));
            });
        }

        /// <summary>
        /// Validates schema-qualified INSERT generation.
        /// </summary>
        [Test]
        public void InsertInto_WhenEntityHasSchema_GeneratesQualifiedTableName()
        {
            var query = _queryEngine
                .InsertInto<SchemaUser>()
                .Set(user => user.Email, "test@test.com")
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(
                    query.CommandText,
                    Does.Contain("INSERT INTO [security].[schema_users]"));

                Assert.That(
                    query.CommandText,
                    Does.Contain("[email]"));
            });
        }

        /// <summary>
        /// Validates schema-qualified UPDATE generation.
        /// </summary>
        [Test]
        public void Update_WhenEntityHasSchema_GeneratesQualifiedTableName()
        {
            var query = _queryEngine
                .Update<SchemaUser>()
                .Set(user => user.Email, "updated@test.com")
                .Where(user => user.Id == 10)
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(
                    query.CommandText,
                    Does.Contain("UPDATE [security].[schema_users]"));

                Assert.That(
                    query.CommandText,
                    Does.Contain("[email]"));

                Assert.That(
                    query.CommandText,
                    Does.Contain("[schema_user_id]"));
            });
        }

        /// <summary>
        /// Validates schema-qualified DELETE generation.
        /// </summary>
        [Test]
        public void DeleteFrom_WhenEntityHasSchema_GeneratesQualifiedTableName()
        {
            var query = _queryEngine
                .DeleteFrom<SchemaUser>()
                .Where(user => user.Id == 10)
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(
                    query.CommandText,
                    Does.Contain($"FROM [security].[schema_users]"));

                Assert.That(
                    query.CommandText,
                    Does.Contain("[schema_user_id]"));
            });
        }

        /// <summary>
        /// Validates independent target and source schemas for INSERT SELECT.
        /// </summary>
        [Test]
        public void InsertSelect_WhenTargetAndSourceHaveDifferentSchemas_GeneratesQualifiedTableNames()
        {
            var query = _queryEngine
                .InsertInto<ArchiveUser>()
                .Columns(user => new
                {
                    user.Id,
                    user.Email
                })
                .From<SchemaUser>()
                .Select<SchemaUser>(user => new
                {
                    user.Id,
                    user.Email
                })
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(
                    query.CommandText,
                    Does.Contain("INSERT INTO [archive].[archived_users]"));

                Assert.That(
                    query.CommandText,
                    Does.Contain("FROM [security].[schema_users]"));
            });
        }

        /// <summary>
        /// Validates that an explicit SELECT table name overrides only the table
        /// while preserving schema and column mappings from metadata.
        /// </summary>
        [Test]
        public void From_WhenExplicitTableNameIsProvided_PreservesMetadataSchema()
        {
            var query = _queryEngine
                .From<SchemaUser>("schema_users_archive")
                .Select(user => user.Id)
                .Build();

            Assert.That(
                query.CommandText,
                Does.Contain("FROM [security].[schema_users] AS [schema_users_archive]"));
        }

        /// <summary>
        /// Validates that an explicit INSERT table name overrides only the table
        /// while preserving schema and column mappings from metadata.
        /// </summary>
        [Test]
        public void InsertInto_WhenExplicitTableNameIsProvided_PreservesMetadataSchema()
        {
            var query = _queryEngine
                .InsertInto<SchemaUser>("schema_users_archive")
                .Set(user => user.Email, "test@test.com")
                .Build();

            Assert.That(
                query.CommandText,
                Does.Contain("INSERT INTO [security].[schema_users_archive]"));
        }

        /// <summary>
        /// Entity Framework context used to provide schema-aware metadata.
        /// </summary>
        private sealed class SchemaDbContext(
            DbContextOptions<SchemaDbContext> options)
            : DbContext(options)
        {
            /// <summary>
            /// Configures physical table, schema and column mappings used by the tests.
            /// </summary>
            /// <param name="modelBuilder">
            /// Entity Framework model builder.
            /// </param>
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<SchemaUser>(entity =>
                {
                    entity.ToTable("schema_users", "security");

                    entity.HasKey(user => user.Id);

                    entity.Property(user => user.Id)
                        .HasColumnName("schema_user_id");

                    entity.Property(user => user.Email)
                        .HasColumnName("email");
                });

                modelBuilder.Entity<ArchiveUser>(entity =>
                {
                    entity.ToTable("archived_users", "archive");

                    entity.HasKey(user => user.Id);

                    entity.Property(user => user.Id)
                        .HasColumnName("archive_user_id");

                    entity.Property(user => user.Email)
                        .HasColumnName("email");
                });
            }
        }

        /// <summary>
        /// Source entity mapped to the security schema.
        /// </summary>
        private sealed class SchemaUser
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
        /// Target entity mapped to the archive schema.
        /// </summary>
        private sealed class ArchiveUser
        {
            /// <summary>
            /// Gets or initializes the archived user identifier.
            /// </summary>
            public int Id { get; init; }

            /// <summary>
            /// Gets or initializes the archived user email address.
            /// </summary>
            public string? Email { get; init; }
        }
    }
}
