using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Samples.Providers;

namespace TinyBlueWhale.EngineQuery.Samples.Database
{
    public interface IDatabaseInitializer
    {
        Task InitializeAsync(SampleProviderContext provider, CancellationToken cancellationToken = default);
    }
}
