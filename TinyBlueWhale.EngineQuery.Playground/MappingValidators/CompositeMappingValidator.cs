using TinyBlueWhale.EngineQuery.Metadata.Fluent;
using TinyBlueWhale.EngineQuery.Metadata.Resolvers;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.SqlServer.Compilation;

namespace TinyBlueWhale.EngineQuery.Playground.MappingValidators
{
    /// <summary>
    /// Validates composite metadata resolution using fluent, attribute and convention resolvers.
    ///
    /// Expected metadata resolution:
    /// CompositeSecurityLog          -> system_logs
    /// SecurityLogId                 -> log_id
    /// SecurityMessage              -> message_text
    /// SecurityCreatedAt            -> created_at
    /// SecurityIsActive             -> is_active
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

            var queryBuilder = SqlServerQueryCompiler.Factory.Create(metadataResolver);

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

    }
}
