using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.Metadata.Models
{
    /// <summary>
    /// Represents metadata associated with an entity and its mapped database table.
    /// </summary>
    public sealed record EntityMetadata
    {
        /// <summary>
        /// Gets the CLR entity type.
        /// </summary>
        public required Type EntityType { get; init; }

        /// <summary>
        /// Gets the mapped database table name.
        /// </summary>
        public required string TableName { get; init; }

        /// <summary>
        /// Gets the mapped entity properties.
        /// </summary>
        public required IReadOnlyDictionary<string, EntityPropertyMetadata> Properties { get; init; }
    }
}
