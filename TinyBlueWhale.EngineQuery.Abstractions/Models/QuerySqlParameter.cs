using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.Abstractions.Models
{
    /// <summary>
    /// Represents a parameter generated for a SQL query command.
    /// </summary>
    public sealed record QuerySqlParameter
    {
        /// <summary>
        /// Gets the SQL parameter name.
        /// </summary>
        public required string Name { get; init; }

        /// <summary>
        /// Gets the SQL parameter value.
        /// </summary>
        public object? Value { get; init; }

        /// <summary>
        /// Gets the runtime type of the parameter value when available.
        /// </summary>
        public Type? ValueType =>
            Value?.GetType();
    }
}
