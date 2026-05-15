using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding;
using TinyBlueWhale.EngineQuery.Metadata.Fluent;
using TinyBlueWhale.EngineQuery.Metadata.Resolvers;
using TinyBlueWhale.EngineQuery.MySql.Compilation;
using TinyBlueWhale.EngineQuery.MySql.Dialects;
using TinyBlueWhale.EngineQuery.PostgreSql.Compilation;
using TinyBlueWhale.EngineQuery.PostgreSql.Dialects;
using TinyBlueWhale.EngineQuery.SqlServer.Compilation;
using TinyBlueWhale.EngineQuery.SqlServer.Dialects;

namespace TinyBlueWhale.EngineQuery.Tests.Providers
{
   
    [TestFixture]
    public sealed class MultiProviderQueryGenerationTests
    {
        [Test]
        public void SqlServerProvider_Should_Generate_Expected_Sql()
        {
            var queryBuilder = new QueryBuilder(
                new SqlServerQueryCompiler(new SqlServerDatabaseDialect(), new EngineQuery.SqlServer.Capabilities.SqlServerProviderCapabilities()),
                CreateMetadataResolver());

            var sql = BuildQuery(queryBuilder);

            Assert.Multiple(() =>
            {
                Assert.That(sql.CommandText, Is.EqualTo(
    """
SELECT [log_id] AS [LogId], [message_text] AS [Message], [created_at] AS [CreatedAt], [is_active] AS [IsActive]
FROM [system_logs]
WHERE (([is_active] = @p0) AND ([message_text] LIKE @p1))
ORDER BY [created_at] DESC
OFFSET 20 ROWS FETCH NEXT 10 ROWS ONLY
"""));
                AssertParameters(sql);
            });
        }

        [Test]
        public void PostgreSqlProvider_Should_Generate_Expected_Sql()
        {
            var queryBuilder = new QueryBuilder(
                new PostgreSqlQueryCompiler(new PostgreSqlDatabaseDialect(), new PostgreSqlServer.Capabilities.PostgreSqlProviderCapabilities()),
                CreateMetadataResolver());

            var sql = BuildQuery(queryBuilder);

            Assert.Multiple(() =>
            {
                Assert.That(sql.CommandText, Is.EqualTo(
    """
SELECT "log_id" AS "LogId", "message_text" AS "Message", "created_at" AS "CreatedAt", "is_active" AS "IsActive"
FROM "system_logs"
WHERE (("is_active" = @p0) AND ("message_text" LIKE @p1))
ORDER BY "created_at" DESC
LIMIT 10 OFFSET 20
"""));
                AssertParameters(sql);
            });
        }

        [Test]
        public void MySqlProvider_Should_Generate_Expected_Sql()
        {
            var queryBuilder = new QueryBuilder(
                new MySqlQueryCompiler(new MySqlDatabaseDialect(), new EngineQuery.MySqlServer.Capabilities.MySqlProviderCapabilities()),
                CreateMetadataResolver());

            var sql = BuildQuery(queryBuilder);

            Assert.Multiple(() =>
            {
                Assert.That(sql.CommandText, Is.EqualTo(
    """
SELECT `log_id` AS `LogId`, `message_text` AS `Message`, `created_at` AS `CreatedAt`, `is_active` AS `IsActive`
FROM `system_logs`
WHERE ((`is_active` = @p0) AND (`message_text` LIKE @p1))
ORDER BY `created_at` DESC
LIMIT 10 OFFSET 20
"""));
                AssertParameters(sql);
            });
        }

        private static GeneratedSqlQuery BuildQuery(QueryBuilder queryBuilder)
        {
            return queryBuilder
                .From<ProviderComparisonLog>()
                .Select(x => new
                {
                    LogId = x.AuditId,
                    Message = x.Description,
                    CreatedAt = x.CreatedOn,
                    IsActive = x.Active
                })
                .Where(x => x.Active && x.Description.Contains("error"))
                .OrderByDescending(x => x.CreatedOn)
                .Skip(20)
                .Take(10)
                .Build();
        }

        private static FluentEntityMetadataResolver CreateMetadataResolver()
        {
            var registry = new EntityMetadataRegistry();

            registry.Entity<ProviderComparisonLog>()
                .ToTable("system_logs")
                .Property(x => x.AuditId).HasColumnName("log_id")
                .Property(x => x.Description).HasColumnName("message_text")
                .Property(x => x.CreatedOn).HasColumnName("created_at")
                .Property(x => x.Active).HasColumnName("is_active");

            return new FluentEntityMetadataResolver(registry);
        }

        private static void AssertParameters(GeneratedSqlQuery sql)
        {
            Assert.Multiple(() =>
            {
                Assert.That(sql.Parameters, Has.Count.EqualTo(2));
                Assert.That(sql.Parameters[0].Name, Is.EqualTo("@p0"));
                Assert.That(sql.Parameters[0].Value, Is.EqualTo(true));
                Assert.That(sql.Parameters[1].Name, Is.EqualTo("@p1"));
                Assert.That(sql.Parameters[1].Value, Is.EqualTo("%error%"));
            });
        }

        private sealed class ProviderComparisonLog
        {
            public int AuditId { get; set; }

            public string Description { get; set; } = null!;

            public DateTime CreatedOn { get; set; }

            public bool Active { get; set; }
        }
    }
}
