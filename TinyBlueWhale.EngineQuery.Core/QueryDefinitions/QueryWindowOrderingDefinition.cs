using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Core.Enums;

namespace TinyBlueWhale.EngineQuery.Core.QueryDefinitions
{
    /// <summary>
    /// Represents an ORDER BY column used inside a SQL window function.
    /// </summary>
    public sealed record QueryWindowOrderingDefinition
    {
        /// <summary>
        /// Gets the ordered column.
        /// </summary>
        public required QueryColumnDefinition Column { get; init; }

        /// <summary>
        /// Gets the ordering direction.
        /// </summary>
        public required QueryOrderingDirection Direction { get; init; }

        /// <summary>
        /// Gets the query source associated with the ordered column.
        /// </summary>
        public required QuerySourceDefinition Source { get; init; }
    }
}
