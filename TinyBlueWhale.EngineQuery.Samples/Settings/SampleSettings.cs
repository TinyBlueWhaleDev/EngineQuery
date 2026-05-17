using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.Samples.Settings
{
    public sealed class SampleSettings
    {
        public SampleConnectionStrings ConnectionStrings { get; set; } = new();
    }
}
