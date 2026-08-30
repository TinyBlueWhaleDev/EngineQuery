using TinyBlueWhale.EngineQuery.Metadata.Resolvers;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.SqlServer.Capabilities;
using TinyBlueWhale.EngineQuery.SqlServer.Compilation;
using TinyBlueWhale.EngineQuery.SqlServer.Dialects;

namespace TinyBlueWhale.EngineQuery.Playground.MappingValidators
{
    public static class AttributeMappingValidator
    {
        public static void Run()
        {
            var queryBuilder = new QueryBuilder(
                new SqlServerQueryCompiler(new SqlServerDatabaseDialect(), new SqlServerProviderCapabilities()),
                new AttributeEntityMetadataResolver());

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
