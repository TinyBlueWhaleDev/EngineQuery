using TinyBlueWhale.EngineQuery.Metadata.EntityFramework.Resolvers;
using TinyBlueWhale.EngineQuery.Samples.EntityFramework;
using TinyBlueWhale.EngineQuery.Samples.Providers;

namespace TinyBlueWhale.EngineQuery.Samples.Metadata
{
    public sealed class BuildEntityFrameworkMetadata
    {
        public static SampleDbContext CreateDbContext(SampleProviderContext provider)
        {
            return new SampleDbContext(provider.BuildDbContextOptions());
        }

        public static EntityFrameworkMetadataResolver CreateResolver(SampleProviderContext provider)
        {
            using var dbContext = CreateDbContext(provider);

            return new EntityFrameworkMetadataResolver(dbContext.Model);
        }
    }
}
