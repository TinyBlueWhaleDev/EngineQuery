using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding;
using TinyBlueWhale.EngineQuery.Metadata.Fluent;
using TinyBlueWhale.EngineQuery.Metadata.Interfaces;
using TinyBlueWhale.EngineQuery.Metadata.Resolvers;
using TinyBlueWhale.EngineQuery.SqlServer.Compilation;
using TinyBlueWhale.EngineQuery.SqlServer.Dialects;
using TinyBlueWhale.EngineQuery.Tests.TestModels.Metadata;

namespace TinyBlueWhale.EngineQuery.Tests.Metadata.Mapping
{
    [TestFixture]
    public sealed class MetadataMappingQueryGenerationTests
    {
        private const string ExpectedMappedSql =
        """
        SELECT [log_id], [message_text], [created_at], [is_active]
        FROM [system_logs]
        WHERE ([is_active] = @p0)
        ORDER BY [created_at] DESC
        """;

        [Test]
        public void Explicit_table_mapping_should_only_map_table_name()
        {
            // Arrange
            var queryBuilder = CreateQueryBuilder();

            // Act
            var sql = queryBuilder
                .From<ExplicitLogEntry>("system_logs")
                .Select(x => new
                {
                    x.LogIdentifier,
                    x.MessageContent,
                    x.RegisteredAt,
                    x.Enabled
                })
                .Where(x => x.Enabled)
                .OrderByDescending(x => x.RegisteredAt)
                .Build();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(sql.CommandText, Is.EqualTo(
                """
                SELECT [LogIdentifier], [MessageContent], [RegisteredAt], [Enabled]
                FROM [system_logs]
                WHERE ([Enabled] = @p0)
                ORDER BY [RegisteredAt] DESC
                """));

                AssertSingleTrueParameter(sql);
            });
        }

        [Test]
        public void Convention_mapping_should_generate_expected_sql_when_clr_names_match_database_names()
        {
            // Arrange
            var queryBuilder = CreateQueryBuilder(
                new ConventionEntityMetadataResolver());

            // Act
            var sql = queryBuilder
                .From<system_logs>()
                .Select(x => new
                {
                    x.log_id,
                    x.message_text,
                    x.created_at,
                    x.is_active
                })
                .Where(x => x.is_active)
                .OrderByDescending(x => x.created_at)
                .Build();

            // Assert
            AssertMappedSql(sql);
        }

        [Test]
        public void Fluent_mapping_should_generate_expected_sql_from_configured_metadata()
        {
            // Arrange
            var registry = new EntityMetadataRegistry();

            registry.Entity<FluentAuditRecord>()
                .ToTable("system_logs")
                .Property(x => x.AuditId).HasColumnName("log_id")
                .Property(x => x.Description).HasColumnName("message_text")
                .Property(x => x.CreatedOn).HasColumnName("created_at")
                .Property(x => x.Active).HasColumnName("is_active");

            var queryBuilder = CreateQueryBuilder(
                new FluentEntityMetadataResolver(registry));

            // Act
            var sql = queryBuilder
                .From<FluentAuditRecord>()
                .Select(x => new
                {
                    x.AuditId,
                    x.Description,
                    x.CreatedOn,
                    x.Active
                })
                .Where(x => x.Active)
                .OrderByDescending(x => x.CreatedOn)
                .Build();

            // Assert
            AssertMappedSql(sql);
        }

        [Test]
        public void Attribute_mapping_should_generate_expected_sql_from_table_and_column_attributes()
        {
            // Arrange
            var queryBuilder = CreateQueryBuilder(
                new AttributeEntityMetadataResolver());

            // Act
            var sql = queryBuilder
                .From<AttributeSystemEvent>()
                .Select(x => new
                {
                    x.EventKey,
                    x.EventMessage,
                    x.EventCreatedAt,
                    x.IsEnabled
                })
                .Where(x => x.IsEnabled)
                .OrderByDescending(x => x.EventCreatedAt)
                .Build();

            // Assert
            AssertMappedSql(sql);
        }

        [Test]
        public void Composite_mapping_should_use_first_matching_resolver_by_priority()
        {
            // Arrange
            var registry = new EntityMetadataRegistry();

            registry.Entity<CompositeSecurityLog>()
                .ToTable("system_logs")
                .Property(x => x.SecurityLogId).HasColumnName("log_id")
                .Property(x => x.SecurityMessage).HasColumnName("message_text")
                .Property(x => x.SecurityCreatedAt).HasColumnName("created_at")
                .Property(x => x.SecurityIsActive).HasColumnName("is_active");

            var metadataResolver = new CompositeEntityMetadataResolver(
            [
                new FluentEntityMetadataResolver(registry),
            new AttributeEntityMetadataResolver(),
            new ConventionEntityMetadataResolver()
            ]);

            var queryBuilder = CreateQueryBuilder(metadataResolver);

            // Act
            var sql = queryBuilder
                .From<CompositeSecurityLog>()
                .Select(x => new
                {
                    x.SecurityLogId,
                    x.SecurityMessage,
                    x.SecurityCreatedAt,
                    x.SecurityIsActive
                })
                .Where(x => x.SecurityIsActive)
                .OrderByDescending(x => x.SecurityCreatedAt)
                .Build();

            // Assert
            AssertMappedSql(sql);
        }

        private static QueryBuilder CreateQueryBuilder(IEntityMetadataResolver? metadataResolver = null)
        {
            return new QueryBuilder(
                new SqlServerQueryCompiler(new SqlServerDatabaseDialect(), new EngineQuery.SqlServer.Capabilities.SqlServerProviderCapabilities()),
                metadataResolver);
        }

        private static void AssertMappedSql(GeneratedSqlQuery sql)
        {
            Assert.Multiple(() =>
            {
                Assert.That(sql.CommandText, Is.EqualTo(ExpectedMappedSql));

                AssertSingleTrueParameter(sql);
            });
        }

        private static void AssertSingleTrueParameter(
            GeneratedSqlQuery sql)
        {
            Assert.Multiple(() =>
            {
                Assert.That(sql.Parameters, Has.Count.EqualTo(1));
                Assert.That(sql.Parameters[0].Name, Is.EqualTo("@p0"));
                Assert.That(sql.Parameters[0].Value, Is.EqualTo(true));
            });
        }
    }
}
