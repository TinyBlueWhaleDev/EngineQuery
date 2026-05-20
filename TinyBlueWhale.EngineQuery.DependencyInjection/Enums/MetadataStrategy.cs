using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.DependencyInjection.Enums
{
    /// <summary>
    /// Defines the supported metadata resolution strategies.
    /// </summary>
    public enum MetadataStrategy
    {
        /// <summary>
        /// Resolves metadata using fluent mapping configuration.
        /// </summary>
        Fluent = 1,

        /// <summary>
        /// Resolves metadata using attributes applied to entities and properties.
        /// </summary>
        Attribute = 2
    }
}
