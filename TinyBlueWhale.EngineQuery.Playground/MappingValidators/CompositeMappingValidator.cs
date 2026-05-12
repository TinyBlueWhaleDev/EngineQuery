using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding;
using TinyBlueWhale.EngineQuery.Metadata.Fluent;
using TinyBlueWhale.EngineQuery.Metadata.Interfaces;
using TinyBlueWhale.EngineQuery.Metadata.Resolvers;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.SqlServer.Compilation;
using TinyBlueWhale.EngineQuery.SqlServer.Dialects;

namespace TinyBlueWhale.EngineQuery.Playground.MappingValidators
{
    public static class CompositeMappingValidator
    {
        public static void Run()
        {
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

            MappingValidatorPrinter.Print(nameof(CompositeMappingValidator), sql);
        }

        private static QueryBuilder CreateQueryBuilder(IEntityMetadataResolver metadataResolver)
        {
            return new QueryBuilder(
                new SqlServerQueryCompiler(
                    new SqlServerDatabaseDialect()),
                metadataResolver);
        }
    }
}
