using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Samples.Providers;

namespace TinyBlueWhale.EngineQuery.Samples.Database
{
    public static class DatabaseScriptPathResolver
    {
        public static DatabaseScriptSet Resolve(SampleProviderContext provider)
        {
            var providerFolder = provider.Kind switch
            {
                SampleProviderKind.SqlServer => "SqlServer",
                SampleProviderKind.PostgreSql => "PostgreSql",
                SampleProviderKind.MySql => "MySql",
                _ => throw new NotSupportedException($"Provider '{provider.Kind}' is not supported.")
            };

            return new DatabaseScriptSet
            {
                SchemaScriptPath = Path.Combine(AppContext.BaseDirectory, "Database", providerFolder, "scripts","schema.sql"),
                SeedScriptPath = Path.Combine(AppContext.BaseDirectory, "Database", providerFolder, "scripts", "seed.sql")
            };
        }
    }
}
