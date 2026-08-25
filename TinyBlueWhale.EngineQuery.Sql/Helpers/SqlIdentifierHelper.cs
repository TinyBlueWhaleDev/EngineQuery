using TinyBlueWhale.EngineQuery.Core.Interfaces;

namespace TinyBlueWhale.EngineQuery.Sql.Helpers
{
    /// <summary>
    /// Provides helpers for building escaped SQL identifier references.
    /// </summary>
    internal static class SqlIdentifierHelper
    {
        /// <summary>
        /// Builds an escaped table reference using an optional database schema.
        /// </summary>
        /// <param name="databaseDialect">
        /// Database dialect used to escape individual identifiers.
        /// </param>
        /// <param name="tableName">
        /// Database table name.
        /// </param>
        /// <param name="schemaName">
        /// Optional database schema name.
        /// </param>
        /// <returns>
        /// Escaped table reference.
        /// </returns>
        public static string BuildTableReference(ISqlDatabaseDialect databaseDialect, string tableName, string? schemaName)
        {
            ArgumentNullException.ThrowIfNull(databaseDialect);
            ArgumentException.ThrowIfNullOrWhiteSpace(tableName);

            var escapedTableName = databaseDialect.EscapeIdentifier(tableName);

            if (string.IsNullOrWhiteSpace(schemaName))
                return escapedTableName;

            var escapedSchemaName = databaseDialect.EscapeIdentifier(schemaName);

            return $"{escapedSchemaName}.{escapedTableName}";
        }
    }
}
