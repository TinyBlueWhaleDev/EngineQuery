using TinyBlueWhale.EngineQuery.Metadata.Fluent;
using TinyBlueWhale.EngineQuery.Metadata.Resolvers;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.SqlServer.Compilation;

namespace TinyBlueWhale.EngineQuery.Playground.MappingValidators
{
    /// <summary>
    /// Validates fluent metadata resolution.
    ///
    /// Expected metadata resolution:
    /// FluentAuditRecord -> system_logs
    /// AuditId           -> log_id
    /// Description       -> message_text
    /// CreatedOn         -> created_at
    /// Active            -> is_active
    ///
    /// Expected SQL:
    /// SELECT [log_id], [message_text], [created_at], [is_active]
    /// FROM [system_logs]
    /// WHERE ([is_active] = @p0)
    /// ORDER BY [created_at] DESC
    ///
    /// Expected parameters:
    /// @p0 = True
    /// </summary>
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

            var queryBuilder = SqlServerQueryCompiler.Factory.Create(new FluentEntityMetadataResolver(registry));

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
