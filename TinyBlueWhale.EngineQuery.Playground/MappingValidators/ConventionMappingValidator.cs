using TinyBlueWhale.EngineQuery.Metadata.Resolvers;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.SqlServer.Compilation;

namespace TinyBlueWhale.EngineQuery.Playground.MappingValidators
{
    /// <summary>
    /// Validates convention-based metadata resolution.
    ///
    /// Expected metadata resolution:
    /// system_logs.log_id        -> log_id
    /// system_logs.message_text  -> message_text
    /// system_logs.created_at    -> created_at
    /// system_logs.is_active     -> is_active
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
    public static class ConventionMappingValidator
    {
        public static void Run()
        {
            var queryBuilder = SqlServerQueryCompiler.Factory.Create(new ConventionEntityMetadataResolver());

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

            MappingValidatorPrinter.Print(nameof(ConventionMappingValidator), sql);
        }

    }
}
