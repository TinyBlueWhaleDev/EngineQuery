using TinyBlueWhale.EngineQuery.Core.Interfaces;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions.Sources;

namespace TinyBlueWhale.EngineQuery.Core.Helpers
{
    /// <summary>
    /// Provides helper methods for resolving mapped query source columns.
    /// </summary>
    public static class QueryColumnMappingHelper
    {
        /// <summary>
        /// Resolves the database column name associated with the specified entity property.
        /// </summary>
        /// <param name="source">
        /// Query source containing the property-to-column mappings.
        /// </param>
        /// <param name="propertyName">
        /// CLR property name whose database column should be resolved.
        /// </param>
        /// <returns>
        /// Mapped database column name when available; otherwise, the original property name.
        /// </returns>
        public static string ResolveColumnName(QuerySourceDefinition source, string propertyName)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentException.ThrowIfNullOrWhiteSpace(propertyName);

            return source.ColumnMappings.TryGetValue(propertyName, out var mappedColumn)
                ? mappedColumn
                : propertyName;
        }

        /// <summary>
        /// Resolves the qualified SQL column reference associated with the specified entity property.
        /// </summary>
        /// <param name="source">
        /// Query source containing the column mappings and optional table alias.
        /// </param>
        /// <param name="databaseDialect">
        /// Database dialect used to escape SQL identifiers.
        /// </param>
        /// <param name="propertyName">
        /// CLR property name whose SQL column reference should be resolved.
        /// </param>
        /// <returns>
        /// Escaped SQL column reference.
        /// </returns>
        public static string ResolveColumnReference(QuerySourceDefinition source, ISqlDatabaseDialect databaseDialect, string propertyName)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(databaseDialect);
            ArgumentException.ThrowIfNullOrWhiteSpace(propertyName);

            var columnName = ResolveColumnName(source, propertyName);
            var escapedColumnName = databaseDialect.EscapeIdentifier(columnName);

            return string.IsNullOrWhiteSpace(source.TableAlias)
                ? escapedColumnName
                : $"{databaseDialect.EscapeIdentifier(source.TableAlias)}.{escapedColumnName}";
        }
    }
}
