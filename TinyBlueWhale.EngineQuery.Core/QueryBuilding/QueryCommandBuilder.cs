using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Core.Interfaces;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Context;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions.Cte;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions.Sources;
using TinyBlueWhale.EngineQuery.Metadata.Interfaces;

namespace TinyBlueWhale.EngineQuery.Core.QueryBuilding
{
    /// <summary>
    /// Builds strongly typed query definitions using a fluent API.
    /// </summary>
    /// <typeparam name="T">
    /// Entity type used as the source of the query.
    /// </typeparam>
    /// <typeparam name="TProfile">
    /// Database provider profile associated with the query builder.
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

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryCommandBuilder{T, TProfile}"/> class using a prebuilt query source.
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
        /// <param name="profile">
        /// Database provider profile associated with the query builder.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="queryCompiler"/>, <paramref name="querySource"/> or <paramref name="profile"/> is null.
        /// </exception>
        internal QueryCommandBuilder(IQueryCompiler queryCompiler, QuerySourceDefinition querySource, IEntityMetadataResolver metadataResolver, TProfile profile)
        {
            ArgumentNullException.ThrowIfNull(queryCompiler);
            ArgumentNullException.ThrowIfNull(querySource);
            ArgumentNullException.ThrowIfNull(profile);

            _queryCompiler = queryCompiler;
            _metadataResolver = metadataResolver;
            _profile = profile;

            _queryDefinition = new CompiledQueryDefinition
            {
                RootSource = querySource
            };

            _queryDefinition.Sources.Add(querySource);

            _context = new QueryCommandBuilderContext
            {
                QueryCompiler = _queryCompiler,
                QueryDefinition = _queryDefinition,
                MetadataResolver = _metadataResolver,
                AliasRegistry = new QueryAliasRegistry()
            };

            _components = QueryCommandBuilderComponentFactory.Create(_context, _profile);

            RegisterAlias(querySource.TableAlias);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryCommandBuilder{T, TProfile}"/> class.
        /// </summary>
        /// <param name="queryCompiler">
        /// Query compiler used to generate provider-specific query output.
        /// </param>
        /// <param name="metadataResolver">
        /// Metadata resolver used to resolve entity table and column mappings.
        /// </param>
        /// <param name="profile">
        /// Database provider profile associated with the query builder.
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
        internal QueryCommandBuilder(IQueryCompiler queryCompiler, IEntityMetadataResolver metadataResolver, TProfile profile, string tableName, string? schemaName = null, string? tableAlias = null, IReadOnlyDictionary<string, string>? columnMappings = null)
        {
            ArgumentNullException.ThrowIfNull(queryCompiler);
            ArgumentNullException.ThrowIfNull(profile);
            ArgumentException.ThrowIfNullOrWhiteSpace(tableName);

            if (schemaName is not null)
                ArgumentException.ThrowIfNullOrWhiteSpace(schemaName);

            if (tableAlias is not null)
                ArgumentException.ThrowIfNullOrWhiteSpace(tableAlias);

            var rootSource = new QuerySourceDefinition
            {
                EntityType = typeof(T),
                SchemaName = schemaName,
                TableName = tableName,
                TableAlias = tableAlias,
                ColumnMappings = columnMappings ?? new Dictionary<string, string>()
            };

            _queryCompiler = queryCompiler;
            _metadataResolver = metadataResolver;
            _profile = profile;

            _queryDefinition = new CompiledQueryDefinition
            {
                RootSource = rootSource
            };

            _queryDefinition.Sources.Add(rootSource);

            _context = new QueryCommandBuilderContext
            {
                QueryCompiler = _queryCompiler,
                QueryDefinition = _queryDefinition,
                MetadataResolver = _metadataResolver,
                AliasRegistry = new QueryAliasRegistry()
            };

            _components = QueryCommandBuilderComponentFactory.Create(_context, _profile);

            RegisterAlias(rootSource.TableAlias);
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

        // Registers the specified source alias in the current query scope.
        private void RegisterAlias(string? alias)
        {
            if (!string.IsNullOrWhiteSpace(alias))
                _context.AliasRegistry.Register(alias);
        }

        // Builds the query definition without compiling SQL.
        internal CompiledQueryDefinition BuildDefinition()
        {
            return _queryDefinition;
        }

        // Registers inherited outer query sources in the current query definition.
        internal void RegisterOuterSources(IReadOnlyList<QuerySourceDefinition> outerSources)
        {
            ArgumentNullException.ThrowIfNull(outerSources);

            foreach (var outerSource in outerSources)
            {
                if (!_queryDefinition.OuterSources.Contains(outerSource))
                    _queryDefinition.OuterSources.Add(outerSource);
            }
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
