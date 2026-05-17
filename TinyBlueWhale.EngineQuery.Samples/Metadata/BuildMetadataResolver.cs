using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Metadata.Interfaces;
using TinyBlueWhale.EngineQuery.Samples.Providers;

namespace TinyBlueWhale.EngineQuery.Samples.Metadata
{
    public static class BuildMetadataResolver
    {
        public static IEntityMetadataResolver Create(SampleProviderContext provider, MetadataStrategy strategy)
        {
            return strategy switch
            {
                MetadataStrategy.Fluent => BuildFluentMetadata.Create(),
                MetadataStrategy.Attribute => BuildAttributeMetadata.Create(),
                MetadataStrategy.EntityFramework => BuildEntityFrameworkMetadata.CreateResolver(provider),
                _ => throw new NotSupportedException($"Metadata strategy '{strategy}' is not supported.")
            };
        }

        public static string GetDisplayName(MetadataStrategy strategy)
        {
            return strategy switch
            {
                MetadataStrategy.Fluent => "Fluent",
                MetadataStrategy.Attribute => "Attribute",
                MetadataStrategy.EntityFramework => "Entity Framework",
                _ => strategy.ToString()
            };
        }
    }
}
