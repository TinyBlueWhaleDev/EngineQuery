using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.Samples.Database
{
    public sealed class DatabaseScriptSet
    {
        public required string SchemaScriptPath { get; init; }

        public required string SeedScriptPath { get; init; }
    }
}
