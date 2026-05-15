using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding;
using TinyBlueWhale.EngineQuery.Playground.Models;
using TinyBlueWhale.EngineQuery.SqlServer.Compilation;
using TinyBlueWhale.EngineQuery.SqlServer.Dialects;

namespace TinyBlueWhale.EngineQuery.Playground.MappingValidators
{
    public static class ExplicitMappingValidator
    {
        public static void Run()
        {
            var queryBuilder = new QueryBuilder(
                new SqlServerQueryCompiler(
                    new SqlServerDatabaseDialect(), new SqlServer.Capabilities.SqlServerProviderCapabilities()));

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
