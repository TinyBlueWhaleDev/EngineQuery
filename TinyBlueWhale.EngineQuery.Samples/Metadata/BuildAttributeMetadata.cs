using TinyBlueWhale.EngineQuery.Metadata.Interfaces;
using TinyBlueWhale.EngineQuery.Metadata.Resolvers;

namespace TinyBlueWhale.EngineQuery.Samples.Metadata
{
    public static class BuildAttributeMetadata
    {
        public static IEntityMetadataResolver Create()
        {
            return new AttributeEntityMetadataResolver();
        }
    }
}
