using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Sql.Enums;

namespace TinyBlueWhale.EngineQuery.Sql.Compilation.Models
{
    /// <summary>
    /// Represents an ordering definition used to generate SQL ORDER BY clauses.
    /// </summary>
    public sealed record QueryOrderingDefinition()
    {
        /// <summary>
        /// Gets the entity property name used for ordering.
        /// </summary>
        public string PropertyName { get; init; } = null!;

        /// <summary>
        /// Gets the ordering direction applied to the property.
        /// </summary>
        public QueryOrderingDirection Direction { get; init; }
    };    
}
