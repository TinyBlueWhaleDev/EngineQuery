using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Core.Interfaces;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Context;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;
using TinyBlueWhale.EngineQuery.Metadata.Interfaces;

namespace TinyBlueWhale.EngineQuery.Core.QueryBuilding
{
    /// <summary>
    /// Builds strongly typed query definitions using a fluent API.
    /// </summary>
    /// <typeparam name="T">
    /// Entity type used as the source of the query.
    /// </typeparam>
    /// <remarks>
    /// This builder does not execute database commands.
    /// It only captures query intent and delegates SQL generation to the query compiler.
    /// </remarks>
    public sealed class QueryCommandBuilder<T, TProfile> :
        QueryCompositionCommandBuilderBase<T, IQueryCommandBuilder<T, TProfile>, TProfile>,
        IQueryCommandBuilder<T, TProfile>
        where TProfile : IDatabaseProviderProfile
    {
        private readonly IQueryCompiler _queryCompiler;
        private readonly CompiledQueryDefinition _queryDefinition;
        private readonly IEntityMetadataResolver _metadataResolver;
        private readonly TProfile _profile;
        private readonly QueryCommandBuilderContext _context;
        private readonly QueryCommandBuilderComponents<TProfile> _components;
        private protected override QueryCommandBuilderComponents<TProfile> Components => _components;
        protected override IQueryCommandBuilder<T, TProfile> Current => this;

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryCommandBuilder{T}"/> class using a prebuilt query source.
        /// </summary>
        /// <param name="queryCompiler">
        /// Query compiler used to generate SQL.
        /// </param>
        /// <param name="querySource">
        /// Root query source associated with the builder.
        /// </param>
        /// <param name="metadataResolver">
        /// Optional entity metadata resolver.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="queryCompiler"/> or <paramref name="querySource"/> is null.
        /// </exception>
        internal QueryCommandBuilder(IQueryCompiler queryCompiler,
            QuerySourceDefinition querySource,
            IEntityMetadataResolver metadataResolver,
            TProfile profile)
        {
            ArgumentNullException.ThrowIfNull(queryCompiler);
            ArgumentNullException.ThrowIfNull(querySource);
            ArgumentNullException.ThrowIfNull(profile);

            var resolvedTableName = querySource.TableName ??
                querySource.TableAlias ??
                throw new InvalidOperationException("Query source must define either a table name or an alias.");

            _queryCompiler = queryCompiler;
            _metadataResolver = metadataResolver;
            _profile = profile;

            _queryDefinition = new CompiledQueryDefinition
            {
                EntityType = typeof(T),
                SchemaName = querySource.SchemaName,
                TableName = resolvedTableName,
                TableAlias = querySource.TableAlias,
                ColumnMappings = querySource.ColumnMappings
            };

            _queryDefinition.SourceDefinitions[typeof(T)] = querySource;

            _context = new QueryCommandBuilderContext
            {
                QueryCompiler = _queryCompiler,
                QueryDefinition = _queryDefinition,
                MetadataResolver = _metadataResolver,
                AliasRegistry = new QueryAliasRegistry()
            };

            _components = QueryCommandBuilderComponentFactory.Create(_context, _profile);

            if (!string.IsNullOrWhiteSpace(querySource.TableAlias))
                _context.AliasRegistry.Register(querySource.TableAlias);
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="QueryCommandBuilder{T}"/> class.
        /// </summary>
        /// <param name="queryCompiler">
        /// Query compiler used to generate provider-specific query output.
        /// </param>
        /// <param name="metadataResolver">
        /// Metadata resolver used to resolve entity table and column mappings.
        /// </param>
        /// <param name="tableName">
        /// Database table name associated with the query.
        /// </param>
        /// <param name="schemaName">
        /// Optional database schema name associated with the query source.
        /// </param>
        /// <param name="tableAlias">
        /// Optional table alias used to qualify generated SQL column references.
        /// </param>
        /// <param name="columnMappings">
        /// Optional property-to-column mappings used during SQL generation.
        /// </param>        
        internal QueryCommandBuilder(IQueryCompiler queryCompiler,
            IEntityMetadataResolver metadataResolver,
            TProfile profile,
            string tableName,
            string? schemaName = null,
            string? tableAlias = null,
            IReadOnlyDictionary<string, string>? columnMappings = null)
        {
            ArgumentNullException.ThrowIfNull(queryCompiler);
            ArgumentException.ThrowIfNullOrWhiteSpace(tableName);

            if (tableAlias is not null)
                ArgumentException.ThrowIfNullOrWhiteSpace(tableAlias);


            _queryCompiler = queryCompiler;
            _metadataResolver = metadataResolver;
            _profile = profile;

            _queryDefinition = new CompiledQueryDefinition
            {
                TableName = tableName,
                SchemaName = schemaName,
                TableAlias = tableAlias,
                ColumnMappings = columnMappings ?? new Dictionary<string, string>(),
                EntityType = typeof(T)
            };

            _context = new QueryCommandBuilderContext
            {
                QueryCompiler = _queryCompiler,
                QueryDefinition = _queryDefinition,
                MetadataResolver = _metadataResolver,
                AliasRegistry = new QueryAliasRegistry()
            };

            _components = QueryCommandBuilderComponentFactory.Create(_context, _profile);

            RegisterRootSource(tableName, tableAlias, schemaName);
        }

        #endregion

        /// <summary>
        /// Compiles the current query definition into SQL command text and parameters.
        /// </summary>
        /// <remarks>
        /// This method only compiles the captured query definition into a provider-specific SQL command.
        /// It does not execute the generated SQL against a database. Execution is the responsibility
        /// of the consuming data access technology, such as Dapper or ADO.NET.
        /// </remarks>
        /// <returns>
        /// Generated SQL query command.
        /// </returns>
        public GeneratedSqlQuery Build()
        {
            return _queryCompiler.Compile(_queryDefinition);
        }


        // Registers the root query source in the current query scope.
        private void RegisterRootSource(string tableName, string? tableAlias, string? schemaName)
        {
            _queryDefinition.SourceDefinitions[typeof(T)] =
                new QuerySourceDefinition
                {
                    EntityType = typeof(T),
                    SchemaName = schemaName,
                    TableName = tableName,
                    TableAlias = tableAlias,
                    ColumnMappings = _queryDefinition.ColumnMappings
                };

            _queryDefinition.TableAlias = tableAlias;

            if (!string.IsNullOrWhiteSpace(tableAlias))
                _context.AliasRegistry.Register(tableAlias);
        }


        // Builds the query definition without compiling SQL.
        internal CompiledQueryDefinition BuildDefinition()
        {
            return _queryDefinition;
        }

        // Registers inherited outer query sources in the current query definition.
        internal void RegisterOuterSources(IReadOnlyDictionary<Type, QuerySourceDefinition> outerSources)
        {
            ArgumentNullException.ThrowIfNull(outerSources);

            foreach (var outerSource in outerSources)
                _queryDefinition.OuterSourceDefinitions[outerSource.Key] = outerSource.Value;
        }

        // Registers common table expressions inherited from the query builder.
        internal void RegisterCteDefinitions(IReadOnlyList<QueryCteDefinition> cteDefinitions)
        {
            ArgumentNullException.ThrowIfNull(cteDefinitions);

            foreach (var cteDefinition in cteDefinitions)
                _queryDefinition.CteDefinitions.Add(cteDefinition);
        }
    }
}
