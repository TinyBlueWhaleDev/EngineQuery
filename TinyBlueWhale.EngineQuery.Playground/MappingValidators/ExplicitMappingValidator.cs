using TinyBlueWhale.EngineQuery.Metadata.Resolvers;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.SqlServer.Compilation;

namespace TinyBlueWhale.EngineQuery.Playground.MappingValidators
{
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
