using Microsoft.EntityFrameworkCore;
using TinyBlueWhale.EngineQuery.Metadata.EntityFramework.Resolvers;

namespace TinyBlueWhale.EngineQuery.Tests.EntityFrameworkMetadata
{
    /// <summary>
    /// Validates Entity Framework metadata resolver behavior.
    /// </summary>
    [TestFixture]
    public sealed class EntityFrameworkMetadataResolverTests
    {
        [Test]
        public void TryResolve_Should_Read_Table_And_Column_Mappings_From_Ef_Model()
        {
            using var dbContext = new SampleDbContext();

            var resolver = new EntityFrameworkMetadataResolver(dbContext.Model);

            var resolved = resolver.TryResolve<EfUser>(out var metadata);

            Assert.Multiple(() =>
            {
                Assert.That(resolved, Is.True);
                Assert.That(metadata, Is.Not.Null);
                Assert.That(metadata!.EntityType, Is.EqualTo(typeof(EfUser)));
                Assert.That(metadata.TableName, Is.EqualTo("users"));
                Assert.That(metadata.Properties["Id"].ColumnName, Is.EqualTo("user_id"));
                Assert.That(metadata.Properties["Email"].ColumnName, Is.EqualTo("email"));
                Assert.That(metadata.Properties["IsActive"].ColumnName, Is.EqualTo("is_active"));
            });
        }

        [Test]
        public void TryResolve_Should_Return_False_When_Entity_Is_Not_Registered_In_Ef_Model()
        {
            using var dbContext = new SampleDbContext();

            var resolver = new EntityFrameworkMetadataResolver(
                dbContext.Model);

            var resolved = resolver.TryResolve<UnmappedEntity>(out var metadata);

            Assert.Multiple(() =>
            {
                Assert.That(resolved, Is.False);
                Assert.That(metadata, Is.Null);
            });
        }

        [Test]
        public void TryResolve_Should_Include_Schema_When_Entity_Has_Schema()
        {
            using var dbContext = new SampleDbContext();

            var resolver = new EntityFrameworkMetadataResolver(
                dbContext.Model);

            var resolved = resolver.TryResolve<EfSchemaUser>(out var metadata);

            Assert.Multiple(() =>
            {
                Assert.That(resolved, Is.True);
                Assert.That(metadata, Is.Not.Null);
                Assert.That(metadata!.TableName, Is.EqualTo("security.schema_users"));
                Assert.That(metadata.Properties["Id"].ColumnName, Is.EqualTo("schema_user_id"));
                Assert.That(metadata.Properties["Email"].ColumnName, Is.EqualTo("email"));
            });
        }

        [Test]
        public void TryResolve_Should_Ignore_Shadow_Properties_By_Default()
        {
            using var dbContext = new SampleDbContext();

            var resolver = new EntityFrameworkMetadataResolver(
                dbContext.Model);

            var resolved = resolver.TryResolve<EfUserWithShadowProperty>(out var metadata);

            Assert.Multiple(() =>
            {
                Assert.That(resolved, Is.True);
                Assert.That(metadata, Is.Not.Null);
                Assert.That(metadata!.Properties.ContainsKey("ShadowCode"), Is.False);
            });
        }

        [Test]
        public void TryResolve_Should_Include_Shadow_Properties_When_Configured()
        {
            using var dbContext = new SampleDbContext();

            var resolver = new EntityFrameworkMetadataResolver(
                dbContext.Model,
                new EntityFrameworkMetadataResolverOptions
                {
                    IncludeShadowProperties = true
                });

            var resolved = resolver.TryResolve<EfUserWithShadowProperty>(out var metadata);

            Assert.Multiple(() =>
            {
                Assert.That(resolved, Is.True);
                Assert.That(metadata, Is.Not.Null);
                Assert.That(metadata!.Properties.ContainsKey("ShadowCode"), Is.True);
                Assert.That(metadata.Properties["ShadowCode"].ColumnName, Is.EqualTo("shadow_code"));
            });
        }

        [Test]
        public void TryResolve_Should_Ignore_NotMapped_Property()
        {
            using var dbContext = new SampleDbContext();

            var resolver = new EntityFrameworkMetadataResolver(
                dbContext.Model);

            var resolved = resolver.TryResolve<EfUserWithIgnoredProperty>(out var metadata);

            Assert.Multiple(() =>
            {
                Assert.That(resolved, Is.True);
                Assert.That(metadata, Is.Not.Null);
                Assert.That(metadata!.Properties.ContainsKey(nameof(EfUserWithIgnoredProperty.IgnoredValue)), Is.False);
            });
        }

        private sealed class SampleDbContext : DbContext
        {
            public DbSet<EfUser> Users => Set<EfUser>();

            public DbSet<EfSchemaUser> SchemaUsers => Set<EfSchemaUser>();

            public DbSet<EfUserWithShadowProperty> UsersWithShadowProperties => Set<EfUserWithShadowProperty>();

            public DbSet<EfUserWithIgnoredProperty> UsersWithIgnoredProperties => Set<EfUserWithIgnoredProperty>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<EfUser>(entity =>
                {
                    entity.ToTable("users");

                    entity.Property(x => x.Id)
                        .HasColumnName("user_id");

                    entity.Property(x => x.Email)
                        .HasColumnName("email");

                    entity.Property(x => x.IsActive)
                        .HasColumnName("is_active");
                });

                modelBuilder.Entity<EfSchemaUser>(entity =>
                {
                    entity.ToTable(
                        "schema_users",
                        "security");

                    entity.Property(x => x.Id)
                        .HasColumnName("schema_user_id");

                    entity.Property(x => x.Email)
                        .HasColumnName("email");
                });

                modelBuilder.Entity<EfUserWithShadowProperty>(entity =>
                {
                    entity.ToTable("users_with_shadow_properties");

                    entity.Property(x => x.Id)
                        .HasColumnName("user_id");

                    entity.Property<string>("ShadowCode")
                        .HasColumnName("shadow_code");
                });

                modelBuilder.Entity<EfUserWithIgnoredProperty>(entity =>
                {
                    entity.ToTable("users_with_ignored_properties");

                    entity.Property(x => x.Id)
                        .HasColumnName("user_id");

                    entity.Ignore(x => x.IgnoredValue);
                });
            }
        }

        private sealed class EfUser
        {
            public int Id { get; set; }

            public string Email { get; set; } = string.Empty;

            public bool IsActive { get; set; }
        }

        private sealed class EfSchemaUser
        {
            public int Id { get; set; }

            public string Email { get; set; } = string.Empty;
        }

        private sealed class EfUserWithShadowProperty
        {
            public int Id { get; set; }
        }

        private sealed class EfUserWithIgnoredProperty
        {
            public int Id { get; set; }

            public string IgnoredValue { get; set; } = string.Empty;
        }

        private sealed class UnmappedEntity
        {
            public int Id { get; set; }
        }
    }
}
