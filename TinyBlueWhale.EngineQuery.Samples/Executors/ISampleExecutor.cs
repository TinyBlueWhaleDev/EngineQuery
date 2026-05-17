using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Samples.Providers;
using TinyBlueWhale.EngineQuery.Samples.Queries;
using TinyBlueWhale.EngineQuery.Samples.Results;

namespace TinyBlueWhale.EngineQuery.Samples.Executors
{
    public interface ISampleExecutor
    {
        string Name { get; }

        Task<SampleExecutionResult> ExecuteAsync(
            SampleProviderContext provider,
            SalesQueryScenario scenario,
            CancellationToken cancellationToken = default);
    }
}
