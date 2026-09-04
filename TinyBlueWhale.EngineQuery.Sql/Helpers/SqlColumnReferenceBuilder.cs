using TinyBlueWhale.EngineQuery.Core.Interfaces;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions.Sources;

namespace TinyBlueWhale.EngineQuery.Sql.Helpers
{
    /// <summary>
    /// Builds SQL column references using the configured database dialect.
    /// </summary>
    /// <remarks>
    /// This helper centralizes column mapping resolution, identifier escaping and table alias handling
    /// to avoid duplicated column reference logic across SQL clause builders.
    /// </remarks>
    /// <remarks>
    /// Initializes a new instance of the <see cref="SqlColumnReferenceBuilder"/> class.
    /// </remarks>
    /// <param name="databaseDialect">
    /// SQL database dialect used to escape identifiers and build qualified column references.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="databaseDialect"/> is <see langword="null"/>.
    /// </exception>
    public sealed class SqlColumnReferenceBuilder(ISqlDatabaseDialect databaseDialect)
    {
        private readonly ISqlDatabaseDialect _databaseDialect = databaseDialect ?? throw new ArgumentNullException(nameof(databaseDialect));

        /// <summary>
        /// Builds a SQL column reference for the specified query source and property name.
        /// </summary>
        /// <param name="source">
        /// Query source that provides table alias and column mappings.
        /// </param>
        /// <param name="propertyName">
        /// CLR property name to resolve into a physical database column name.
        /// </param>
        /// <returns>
        /// SQL column reference, escaped and qualified when a table alias is available.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public string Build(QuerySourceDefinition source, string propertyName)
        {
            ArgumentNullException.ThrowIfNull(source);

            var columnName = ResolveMappedColumnName(source.ColumnMappings, propertyName);

            return Build(columnName, source.TableAlias);
        }

        /// <summary>
        /// Builds a SQL column reference from explicit column mappings, table alias and property name.
        /// </summary>
        /// <param name="columnMappings">
        /// Optional property-to-column mapping dictionary.
        /// </param>
        /// <param name="tableAlias">
        /// Optional SQL table alias.
        /// </param>
        /// <param name="propertyName">
        /// CLR property name to resolve into a physical database column name.
        /// </param>
        /// <returns>
        /// SQL column reference, escaped and qualified when a table alias is available.
        /// </returns>
        public string Build(IReadOnlyDictionary<string, string>? columnMappings, string? tableAlias, string propertyName)
        {
            var columnName = ResolveMappedColumnName(columnMappings, propertyName);

            return Build(columnName, tableAlias);
        }

        /// <summary>
        /// Builds a SQL column reference from a physical column name and optional table alias.
        /// </summary>
        /// <param name="columnName">
        /// Physical database column name.
        /// </param>
        /// <param name="tableAlias">
        /// Optional SQL table alias.
        /// </param>
        /// <returns>
        /// SQL column reference, escaped and qualified when a table alias is available.
        /// </returns>
        public string Build(string columnName, string? tableAlias)
        {
            return string.IsNullOrWhiteSpace(tableAlias)
                ? _databaseDialect.EscapeIdentifier(columnName)
                : _databaseDialect.BuildQualifiedIdentifier(tableAlias, columnName);
        }

        /// <summary>
        /// Resolves the physical database column name mapped to the specified property name.
        /// </summary>
        /// <param name="columnMappings">
        /// Optional property-to-column mapping dictionary.
        /// </param>
        /// <param name="propertyName">
        /// CLR property name to resolve.
        /// </param>
        /// <returns>
        /// Mapped database column name when configured; otherwise, the original property name.
        /// </returns>
        public static string ResolveMappedColumnName(IReadOnlyDictionary<string, string>? columnMappings, string propertyName)
        {
            if (columnMappings is null)
                return propertyName;

            return columnMappings.TryGetValue(propertyName, out var columnName)
                ? columnName
                : propertyName;
        }
    }
}
