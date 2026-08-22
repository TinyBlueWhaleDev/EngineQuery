using TinyBlueWhale.EngineQuery.Samples.Providers;

namespace TinyBlueWhale.EngineQuery.Samples.Database
{
    public interface IDatabaseInitializer
    {
        Task InitializeAsync(SampleProviderContext provider, CancellationToken cancellationToken = default);
    }
}
