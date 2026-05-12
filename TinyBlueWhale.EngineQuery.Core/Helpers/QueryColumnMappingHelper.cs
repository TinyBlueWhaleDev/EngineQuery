using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;

namespace TinyBlueWhale.EngineQuery.Core.Helpers
{
    /// <summary>
    /// Provides helper methods for resolving database column mappings.
    /// </summary>
    public static class QueryColumnMappingHelper
    {
        /// <summary>
        /// Resolves the database column name associated with the specified CLR property name.
        /// </summary>
        /// <param name="queryDefinition">
        /// Query definition containing configured column mappings.
        /// </param>
        /// <param name="propertyName">
        /// CLR property name.
        /// </param>
        /// <returns>
        /// Resolved database column name when a mapping exists; otherwise, the original property name.
        /// </returns>
        public static string ResolveColumnName(CompiledQueryDefinition queryDefinition, string propertyName)
        {
            ArgumentNullException.ThrowIfNull(queryDefinition);
            ArgumentException.ThrowIfNullOrWhiteSpace(propertyName);

            return queryDefinition.ColumnMappings.TryGetValue(
                propertyName,
                out var columnName)
                    ? columnName
                    : propertyName;
        }
    }
}
