using TinyBlueWhale.EngineQuery.Metadata.Resolvers;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.SqlServer.Compilation;

namespace TinyBlueWhale.EngineQuery.Playground.MappingValidators
{
    /// <summary>
    /// Validates explicit table selection with convention-based column resolution.
    ///
    /// Expected metadata resolution:
    /// ExplicitLogEntry           -> system_logs
    /// LogIdentifier             -> LogIdentifier
    /// MessageContent            -> MessageContent
    /// RegisteredAt              -> RegisteredAt
    /// Enabled                   -> Enabled
    ///
    /// Expected projection alias:
    /// LogIdentifier             -> testId
    ///
    /// Expected SQL:
    /// SELECT [LogIdentifier] AS [testId], [MessageContent], [RegisteredAt], [Enabled]
    /// FROM [system_logs]
    /// WHERE ([Enabled] = @p0)
    /// ORDER BY [RegisteredAt] DESC
    ///
    /// Expected parameters:
    /// @p0 = True
    /// </summary>
    public static class ExplicitMappingValidator
    {
        public static void Run()
        {
            var queryBuilder = SqlServerQueryCompiler.Factory.Create(new ConventionEntityMetadataResolver());

            var sql = queryBuilder
                .From<ExplicitLogEntry>("system_logs")
                .Select(x => new
                {
                    testId = x.LogIdentifier,
                    x.MessageContent,
                    x.RegisteredAt,
                    x.Enabled
                })
                .Where(x => x.Enabled)
                .OrderByDescending(x => x.RegisteredAt)
                .Build();

            MappingValidatorPrinter.Print(nameof(ExplicitMappingValidator), sql);
        }
    }
}
