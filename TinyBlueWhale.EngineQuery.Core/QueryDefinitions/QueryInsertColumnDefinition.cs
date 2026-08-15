using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.Core.QueryDefinitions
{
    /// <summary>
    /// Represents a target column associated with an INSERT command definition.
    /// </summary>
    public sealed record QueryInsertColumnDefinition
    {
        /// <summary>
        /// Gets the resolved target database column name.
        /// </summary>
        public required string ColumnName { get; init; }
    }
}
