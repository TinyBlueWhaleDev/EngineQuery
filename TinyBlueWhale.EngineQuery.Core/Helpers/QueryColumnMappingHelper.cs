using TinyBlueWhale.EngineQuery.Core.Interfaces;
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

        /// <summary>
        /// Resolves the database column reference associated with the specified CLR property name.
        /// </summary>
        /// <param name="queryDefinition">
        /// Query definition containing configured table alias and column mappings.
        /// </param>
        /// <param name="databaseDialect">
        /// Database dialect used to generate provider-specific identifiers.
        /// </param>
        /// <param name="propertyName">
        /// CLR property name.
        /// </param>
        /// <returns>
        /// Resolved SQL column reference.
        /// </returns>
        public static string ResolveColumnReference(CompiledQueryDefinition queryDefinition,ISqlDatabaseDialect databaseDialect,string propertyName)
        {
            ArgumentNullException.ThrowIfNull(queryDefinition);
            ArgumentNullException.ThrowIfNull(databaseDialect);
            ArgumentException.ThrowIfNullOrWhiteSpace(propertyName);

            var columnName = ResolveColumnName(queryDefinition, propertyName);

            return string.IsNullOrWhiteSpace(queryDefinition.TableAlias)
                ? databaseDialect.EscapeIdentifier(columnName)
                : databaseDialect.BuildQualifiedIdentifier(queryDefinition.TableAlias, columnName);
        }
    }
}
