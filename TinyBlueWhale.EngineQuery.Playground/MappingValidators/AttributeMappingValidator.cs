using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding;
using TinyBlueWhale.EngineQuery.Metadata.Resolvers;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.SqlServer.Compilation;
using TinyBlueWhale.EngineQuery.SqlServer.Dialects;

namespace TinyBlueWhale.EngineQuery.Playground.MappingValidators
{
    public static class AttributeMappingValidator
    {
        public static void Run()
        {
            var queryBuilder = new QueryBuilder(
                new SqlServerQueryCompiler(new SqlServerDatabaseDialect()),
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
