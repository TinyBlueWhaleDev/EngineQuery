using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.Metadata.Models
{
    /// <summary>
    /// Represents metadata associated with an entity property and its mapped database column.
    /// </summary>
    public sealed record EntityPropertyMetadata
    {
        /// <summary>
        /// Gets the CLR property name.
        /// </summary>
        public required string PropertyName { get; init; }

        /// <summary>
        /// Gets the mapped database column name.
        /// </summary>
        public required string ColumnName { get; init; }
    }
}
