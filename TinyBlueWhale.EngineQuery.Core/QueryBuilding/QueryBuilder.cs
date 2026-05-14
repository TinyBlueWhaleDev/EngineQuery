using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Core.Interfaces;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;
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

        private readonly List<QueryCteDefinition> _cteDefinitions = [];

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

        /// <summary>
        /// Creates a query command builder using a derived table as the root query source.
        /// </summary>
        /// <typeparam name="TDerived">
        /// CLR type used to represent the derived table projection.
        /// </typeparam>
        /// <typeparam name="TSubqueryRoot">
        /// Root entity type used by the derived table subquery.
        /// </typeparam>
        /// <param name="alias">
        /// Alias assigned to the derived table.
        /// </param>
        /// <param name="subqueryBuilder">
        /// Function used to build the derived table subquery.
        /// </param>
        /// <returns>
        /// Query command builder for the derived table source.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="alias"/> is null, empty or whitespace.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="subqueryBuilder"/> is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the derived table subquery builder returns an unsupported query command builder instance.
        /// </exception>
        public IQueryCommandBuilder<TDerived> FromSubquery<TDerived, TSubqueryRoot>(string alias, Func<IQueryBuilder, IQueryCommandBuilder<TSubqueryRoot>> subqueryBuilder)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(alias);
            ArgumentNullException.ThrowIfNull(subqueryBuilder);

            var nestedQueryBuilder = new QueryBuilder(_queryCompiler,_metadataResolver);

            var nestedCommandBuilder = subqueryBuilder(nestedQueryBuilder);

            if (nestedCommandBuilder is not QueryCommandBuilder<TSubqueryRoot> concreteNestedCommandBuilder)
                throw new InvalidOperationException("The derived table subquery builder returned an unsupported query command builder instance.");

            var subqueryDefinition = concreteNestedCommandBuilder.BuildDefinition();
            subqueryDefinition.ForceSelectAliases = true;

            var derivedColumnMappings = ResolveDerivedColumnMappings<TDerived>();

            var derivedSource = new QuerySourceDefinition
            {
                EntityType = typeof(TDerived),
                Subquery = subqueryDefinition,
                TableAlias = alias,
                ColumnMappings = derivedColumnMappings
            };

            return new QueryCommandBuilder<TDerived>(_queryCompiler, derivedSource, _metadataResolver);
        }

        /// <summary>
        /// Registers a common table expression that can be used as a query source.
        /// </summary>
        public IQueryBuilder With<TCte, TSubqueryRoot>(string name, Func<IQueryBuilder, IQueryCommandBuilder<TSubqueryRoot>> cteBuilder)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            ArgumentNullException.ThrowIfNull(cteBuilder);

            var nestedQueryBuilder = new QueryBuilder(
                _queryCompiler,
                _metadataResolver);

            var nestedCommandBuilder = cteBuilder(nestedQueryBuilder);            

            if (nestedCommandBuilder is not QueryCommandBuilder<TSubqueryRoot> concreteNestedCommandBuilder)
                throw new InvalidOperationException("The CTE builder returned an unsupported query command builder instance.");

            var cteQueryDefinition = concreteNestedCommandBuilder.BuildDefinition();
            cteQueryDefinition.ForceSelectAliases = true;

            _cteDefinitions.Add(
                new QueryCteDefinition
                {
                    Name = name,
                    Query = cteQueryDefinition                    
                });

            return this;
        }

        /// <summary>
        /// Creates a query command builder using a common table expression as the root source.
        /// </summary>
        public IQueryCommandBuilder<TCte> FromCte<TCte>(string name)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);

            var columnMappings = ResolveDerivedColumnMappings<TCte>();

            var cteSource = new QuerySourceDefinition
            {
                EntityType = typeof(TCte),
                TableName = name,
                TableAlias = name,
                ColumnMappings = columnMappings
            };

            var commandBuilder = new QueryCommandBuilder<TCte>(
                _queryCompiler,
                cteSource,
                _metadataResolver);

            commandBuilder.RegisterCteDefinitions(_cteDefinitions);

            return commandBuilder;
        }

        // Resolves derived table column mappings using metadata when available or property names by convention.
        private Dictionary<string, string> ResolveDerivedColumnMappings<TDerived>()
        {
            if (_metadataResolver is not null && _metadataResolver.TryResolve<TDerived>(out var metadata))
                return metadata!.Properties.ToDictionary(property => property.Key, property => property.Value.ColumnName);

            return typeof(TDerived).GetProperties().ToDictionary(property => property.Name, property => property.Name);
        }

        // Creates a query command builder with inherited outer sources.
        internal QueryCommandBuilder<TEntity> FromWithOuterSources<TEntity>(string? alias,IReadOnlyDictionary<Type, QuerySourceDefinition> outerSources)
        {
            var commandBuilder = (QueryCommandBuilder<TEntity>)From<TEntity>(alias);

            commandBuilder.RegisterOuterSources(outerSources);

            return commandBuilder;
        }

    }
}
