using TinyBlueWhale.EngineQuery.Samples.Providers;

namespace TinyBlueWhale.EngineQuery.Samples.Database
{
    public sealed class NoOpDatabaseInitializer : IDatabaseInitializer
    {
        public Task InitializeAsync(SampleProviderContext provider, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
