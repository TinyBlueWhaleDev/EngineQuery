using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Core.Interfaces;
using TinyBlueWhale.EngineQuery.Metadata.Interfaces;

namespace TinyBlueWhale.EngineQuery.Core.QueryBuilding
{
    /// <summary>
    /// Default implementation of the query engine responsible for creating query builders.
    /// </summary>
    /// <remarks>
    /// The query engine acts as the main entry point for composing strongly typed SQL queries.
    /// It does not execute queries or manage database connections.
    /// </remarks>
    public sealed class QueryBuilder(IQueryCompiler queryCompiler,
        IEntityMetadataResolver? metadataResolver = null) : IQueryBuilder
    {
        private readonly IQueryCompiler _queryCompiler = queryCompiler ?? throw new ArgumentNullException(nameof(queryCompiler));

        private readonly IEntityMetadataResolver? _metadataResolver = metadataResolver;

        /// <summary>
        /// Creates a new query builder using an explicit table name.
        /// </summary>
        /// <typeparam name="T">
        /// Entity type used as the source of the query.
        /// </typeparam>
        /// <param name="tableName">
        /// Database table name associated with the query.
        /// </param>
        /// <param name="alias">
        /// Optional table alias used to qualify generated SQL column references.
        /// </param>
        /// <returns>
        /// Fluent query command builder.
        /// </returns>
        public IQueryCommandBuilder<T> From<T>(string tableName, string? alias = null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(tableName);

            if (alias is not null)
                ArgumentException.ThrowIfNullOrWhiteSpace(alias);

            return new QueryCommandBuilder<T>(_queryCompiler, tableName, alias, metadataResolver: _metadataResolver);
        }

        /// <summary>
        /// Creates a new query builder using resolved entity metadata.
        /// </summary>
        /// <typeparam name="T">
        /// Entity type used as the source of the query.
        /// </typeparam>
        /// <param name="alias">
        /// Optional table alias used to qualify generated SQL column references.
        /// </param>
        /// <returns>
        /// Fluent query command builder.
        /// </returns>
        public IQueryCommandBuilder<T> From<T>(string? alias = null)
        {
            if (alias is not null)
                ArgumentException.ThrowIfNullOrWhiteSpace(alias);

            if (_metadataResolver is null)
                throw new InvalidOperationException("No entity metadata resolver is configured.");
            

            if (!_metadataResolver.TryResolve<T>(out var metadata))
                throw new InvalidOperationException($"Metadata for entity type '{typeof(T).Name}' could not be resolved.");

            var columnMappings = metadata!.Properties
                .ToDictionary(property => property.Key, property => property.Value.ColumnName);

            return new QueryCommandBuilder<T>(_queryCompiler, metadata!.TableName, alias, columnMappings, metadataResolver: _metadataResolver);
        }
    }
}
