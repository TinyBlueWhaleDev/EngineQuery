using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.Core.QueryDefinitions
{
    /// <summary>
    /// Represents a column value assignment in a SQL UPDATE command.
    /// </summary>
    public sealed record QueryUpdateAssignmentDefinition
    {
        /// <summary>
        /// Gets the resolved database column name associated with the assigned value.
        /// </summary>
        public required string ColumnName { get; init; }

        /// <summary>
        /// Gets the value assigned to the target database column.
        /// </summary>
        public object? Value { get; init; }
    }
}
