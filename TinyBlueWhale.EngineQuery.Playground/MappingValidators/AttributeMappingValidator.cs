using TinyBlueWhale.EngineQuery.Metadata.Resolvers;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.SqlServer.Compilation;

namespace TinyBlueWhale.EngineQuery.Playground.MappingValidators
{
    /// <summary>
    /// Validates attribute-based metadata resolution.
    ///
    /// Expected metadata resolution:
    /// AttributeSystemEvent  -> system_logs
    /// EventKey              -> log_id
    /// EventMessage          -> message_text
    /// EventCreatedAt        -> created_at
    /// IsEnabled             -> is_active
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
    public static class AttributeMappingValidator
    {
        public static void Run()
        {
            var queryBuilder = SqlServerQueryCompiler.Factory.Create(new AttributeEntityMetadataResolver());

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

            MappingValidatorPrinter.Print(nameof(AttributeMappingValidator), sql);
        }
    }
}
