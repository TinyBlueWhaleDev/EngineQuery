using TinyBlueWhale.EngineQuery.Metadata.Models;

namespace TinyBlueWhale.EngineQuery.Metadata.EntityFramework.Models
{
    /// <summary>
    /// Provides Entity Framework metadata strategies.
    /// </summary>
    public static class EntityFrameworkMetadataStrategies
    {
        /// <summary>
        /// Entity Framework metadata strategy.
        /// </summary>
        public static readonly MetadataStrategy EntityFramework = new("EntityFramework");
    }
}
