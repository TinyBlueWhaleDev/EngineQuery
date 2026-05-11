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
    public static class ConventionMappingValidator
    {
        public static void Run()
        {
            var queryBuilder = new QueryBuilder(
                new QuerySqlServerCompiler(new SqlServerDatabaseDialect()),
                new ConventionEntityMetadataResolver());

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
