using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
