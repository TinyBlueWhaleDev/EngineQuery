using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding;
using TinyBlueWhale.EngineQuery.Metadata.Fluent;
using TinyBlueWhale.EngineQuery.Metadata.Resolvers;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.SqlServer.Compilation;
using TinyBlueWhale.EngineQuery.SqlServer.Dialects;

namespace TinyBlueWhale.EngineQuery.Playground.MappingValidators
{
    public static class FluentMappingValidator
    {
        public static void Run()
        {
            var registry = new EntityMetadataRegistry();

            registry.Entity<FluentAuditRecord>()
                .ToTable("system_logs")
                .Property(x => x.AuditId).HasColumnName("log_id")
                .Property(x => x.Description).HasColumnName("message_text")
                .Property(x => x.CreatedOn).HasColumnName("created_at")
                .Property(x => x.Active).HasColumnName("is_active");

            var queryBuilder = new QueryBuilder(
                new QuerySqlServerCompiler(new SqlServerDatabaseDialect()),
                new FluentEntityMetadataResolver(registry));

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

            MappingValidatorPrinter.Print(nameof(FluentMappingValidator), sql);
        }
    }
}
