using System.ComponentModel.DataAnnotations.Schema;
using TinyBlueWhale.EngineQuery.Metadata.Fluent;
using TinyBlueWhale.EngineQuery.Metadata.Resolvers;

namespace TinyBlueWhale.EngineQuery.Tests.Metadata
{
    /// <summary>
    /// Validates schema resolution across supported metadata strategies.
    /// </summary>
    [TestFixture]
    internal sealed class SchemaMetadataTests
    {
        /// <summary>
        /// Validates that fluent table configuration preserves
        /// the configured schema, table and column mappings.
        /// </summary>
        [Test]
        public void FluentMetadata_WhenSchemaIsConfigured_ShouldResolveSchema()
        {
            var registry = new EntityMetadataRegistry();

            registry.Entity<FluentSchemaUser>()
                .ToTable(
                    "fluent_users",
                    schemaName: "fluent_security")
                .Property(user => user.Id)
                    .HasColumnName("fluent_user_id")
                .Property(user => user.Email)
                    .HasColumnName("email");

            var resolver = new FluentEntityMetadataResolver(registry);

            var resolved = resolver.TryResolve<FluentSchemaUser>(
                out var metadata);

            Assert.Multiple(() =>
            {
                Assert.That(resolved, Is.True);
                Assert.That(metadata, Is.Not.Null);
                Assert.That(metadata!.SchemaName, Is.EqualTo("fluent_security"));
                Assert.That(metadata.TableName, Is.EqualTo("fluent_users"));
                Assert.That(metadata.Properties["Id"].ColumnName, Is.EqualTo("fluent_user_id"));
                Assert.That(metadata.Properties["Email"].ColumnName, Is.EqualTo("email"));
            });
        }

        /// <summary>
        /// Validates that attribute metadata resolves schema,
        /// table and column mappings.
        /// </summary>
        [Test]
        public void AttributeMetadata_WhenSchemaIsConfigured_ShouldResolveSchema()
        {
            var resolver = new AttributeEntityMetadataResolver();

            var resolved = resolver.TryResolve<AttributeSchemaUser>(
                out var metadata);

            Assert.Multiple(() =>
            {
                Assert.That(resolved, Is.True);
                Assert.That(metadata, Is.Not.Null);
                Assert.That(metadata!.SchemaName, Is.EqualTo("attribute_security"));
                Assert.That(metadata.TableName, Is.EqualTo("attribute_users"));
                Assert.That(metadata.Properties["Id"].ColumnName, Is.EqualTo("attribute_user_id"));
                Assert.That(metadata.Properties["Email"].ColumnName, Is.EqualTo("email"));
            });
        }

        /// <summary>
        /// Entity used to validate schema-aware fluent metadata.
        /// </summary>
        private sealed class FluentSchemaUser
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
        /// Entity used to validate schema-aware attribute metadata.
        /// </summary>
        [Table("attribute_users", Schema = "attribute_security")]
        private sealed class AttributeSchemaUser
        {
            /// <summary>
            /// Gets or initializes the user identifier.
            /// </summary>
            [Column("attribute_user_id")]
            public int Id { get; init; }

            /// <summary>
            /// Gets or initializes the user email.
            /// </summary>
            [Column("email")]
            public string? Email { get; init; }
        }
    }
}
