using System.Linq.Expressions;
using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Core.Enums;
using TinyBlueWhale.EngineQuery.Core.Helpers;
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
    public sealed class QueryCommandBuilder<T> : QueryCompositionCommandBuilderBase<T, IQueryCommandBuilder<T>>, IOrderedQueryCommandBuilder<T>
    {        
        private readonly IQueryCompiler _queryCompiler;
        private readonly CompiledQueryDefinition _queryDefinition;
        private readonly IEntityMetadataResolver? _metadataResolver;

        private readonly QueryCommandBuilderContext _context;
        private readonly QueryCommandBuilderComponents _components;
        private protected override QueryCommandBuilderComponents Components => _components;
        
        protected override IQueryCommandBuilder<T> Current => this;

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
        internal QueryCommandBuilder(IQueryCompiler queryCompiler, QuerySourceDefinition querySource, IEntityMetadataResolver? metadataResolver = null)
        {
            ArgumentNullException.ThrowIfNull(queryCompiler);
            ArgumentNullException.ThrowIfNull(querySource);

            _queryCompiler = queryCompiler;
            _metadataResolver = metadataResolver;

            _queryDefinition = new CompiledQueryDefinition
            {
                EntityType = typeof(T),
                TableName = querySource.TableName ?? querySource.TableAlias,
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

            _components = QueryCommandBuilderComponentFactory.Create(_context);

            if (!string.IsNullOrWhiteSpace(querySource.TableAlias))
                _context.AliasRegistry.Register(querySource.TableAlias);
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="QueryCommandBuilder{T}"/> class.
        /// </summary>
        /// <param name="queryCompiler">
        /// Query compiler used to generate provider-specific query output.
        /// </param>
        /// <param name="tableName">
        /// Database table name associated with the query.
        /// </param>
        /// <param name="tableAlias">
        /// Optional table alias used to qualify generated SQL column references.
        /// </param>
        /// <param name="columnMappings">
        /// Optional property-to-column mappings used during SQL generation.
        /// </param>
        /// <param name="metadataResolver">
        /// Optional entity metadata resolver used for metadata-driven joins.
        /// </param>
        internal QueryCommandBuilder(IQueryCompiler queryCompiler, string tableName, string? tableAlias = null,  IReadOnlyDictionary<string, string>? columnMappings = null, IEntityMetadataResolver? metadataResolver = null)
        {
            ArgumentNullException.ThrowIfNull(queryCompiler);
            ArgumentException.ThrowIfNullOrWhiteSpace(tableName);

            if(tableAlias is not null)
                ArgumentException.ThrowIfNullOrWhiteSpace(tableAlias);


            _queryCompiler = queryCompiler;
            _metadataResolver = metadataResolver;

            _queryDefinition = new CompiledQueryDefinition
            {
                TableName = tableName,
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

            _components = QueryCommandBuilderComponentFactory.Create(_context);

            RegisterRootSource(tableName, tableAlias);
        }

        #endregion 
      

      

        #region Ordering Overloads
        /// <summary>
        /// Adds an ascending ordering expression to the query definition.
        /// </summary>      
        /// <param name="keySelector">
        /// Expression that selects the property used for ordering.
        /// </param>
        /// <returns>
        /// Ordered query command builder instance.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="keySelector"/> is null.
        /// </exception>
        public IOrderedQueryCommandBuilder<T> OrderBy(Expression<Func<T, object>> keySelector)
        {
            _components.OrderByClauseBuilder.AddAscending(keySelector);

            return this;
        }

        /// <summary>
        /// Adds an ascending ORDER BY clause for an entity already available in the current query scope.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type associated with the ordered column.
        /// </typeparam>        
        /// <param name="keySelector">
        /// Expression that selects the property used for ordering.
        /// </param>
        /// <returns>
        /// Ordered query command builder instance.
        /// </returns>
        public IOrderedQueryCommandBuilder<T> OrderBy<TEntity>(Expression<Func<TEntity, object>> keySelector)
        {
            _components.OrderByClauseBuilder.AddAscendingForSource(keySelector);

            return this;
        }


        /// <summary>
        /// Adds a descending ordering expression to the query definition.
        /// </summary>   
        /// <param name="keySelector">
        /// Expression that selects the property used for ordering.
        /// </param>
        /// <returns>
        /// Ordered query command builder instance.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="keySelector"/> is null.
        /// </exception>
        public IOrderedQueryCommandBuilder<T> OrderByDescending(Expression<Func<T, object>> keySelector)
        {
            _components.OrderByClauseBuilder.AddDescending(keySelector);

            return this;
        }

        /// <summary>
        /// Adds a descending ORDER BY clause for an entity already available in the current query scope.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type associated with the ordered column.
        /// </typeparam>   
        /// <param name="keySelector">
        /// Expression that selects the property used for ordering.
        /// </param>
        /// <returns>
        /// Ordered query command builder instance.
        /// </returns>
        public IOrderedQueryCommandBuilder<T> OrderByDescending<TEntity>(Expression<Func<TEntity, object>> keySelector)
        {
            _components.OrderByClauseBuilder.AddDescendingForSource(keySelector);

            return this;
        }

        /// <summary>
        /// Adds an additional ascending ordering expression for the root entity.
        /// </summary>       
        /// <param name="keySelector">
        /// Expression that selects the property used for ordering.
        /// </param>
        /// <returns>
        /// Current ordered query command builder instance.
        /// </returns>
        public IOrderedQueryCommandBuilder<T> ThenBy(Expression<Func<T, object>> keySelector)
        {
            _components.OrderByClauseBuilder.AddAscending(keySelector);

            return this;
        }

        /// <summary>
        /// Adds an additional ascending ordering expression for an entity available in the current query scope.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type associated with the ordered column.
        /// </typeparam>       
        /// <param name="keySelector">
        /// Expression that selects the property used for ordering.
        /// </param>
        /// <returns>
        /// Current ordered query command builder instance.
        /// </returns>
        public IOrderedQueryCommandBuilder<T> ThenBy<TEntity>(Expression<Func<TEntity, object>> keySelector)
        {
            _components.OrderByClauseBuilder.AddAscendingForSource(keySelector);

            return this;
        }

        /// <summary>
        /// Adds an additional descending ordering expression for the root entity.
        /// </summary>    
        /// <param name="keySelector">
        /// Expression that selects the property used for ordering.
        /// </param>
        /// <returns>
        /// Current ordered query command builder instance.
        /// </returns>
        public IOrderedQueryCommandBuilder<T> ThenByDescending(Expression<Func<T, object>> keySelector)
        {
            _components.OrderByClauseBuilder.AddDescending(keySelector);

            return this;
        }

        /// <summary>
        /// Adds an additional descending ordering expression for an entity available in the current query scope.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type associated with the ordered column.
        /// </typeparam>       
        /// <param name="keySelector">
        /// Expression that selects the property used for ordering.
        /// </param>
        /// <returns>
        /// Current ordered query command builder instance.
        /// </returns>
        public IOrderedQueryCommandBuilder<T> ThenByDescending<TEntity>(Expression<Func<TEntity, object>> keySelector)
        {
            _components.OrderByClauseBuilder.AddDescendingForSource(keySelector);

            return this;
        }

        #endregion

      
        #region Pagination Methods
        /// <summary>
        /// Sets the number of rows to skip during SQL pagination.
        /// </summary>
        /// <param name="count">
        /// Number of rows to skip.
        /// </param>
        /// <returns>
        /// Current query command builder instance.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="count"/> is negative.
        /// </exception>
        public IQueryCommandBuilder<T> Skip(int count)
        {
            _components.PaginationClauseBuilder.SetSkip(count);

            return this;
        }

        /// <summary>
        /// Sets the maximum number of rows returned during SQL pagination.
        /// </summary>
        /// <param name="count">
        /// Maximum number of rows to return.
        /// </param>
        /// <returns>
        /// Current query command builder instance.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="count"/> is less than or equal to zero.
        /// </exception>
        public IQueryCommandBuilder<T> Take(int count)
        {
            _components.PaginationClauseBuilder.SetTake(count);

            return this;
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
        private void RegisterRootSource(string tableName,string? tableAlias)
        {
            var resolvedAlias = string.IsNullOrWhiteSpace(tableAlias)
                ? QueryAliasGeneratorHelper.Generate(0)
                : tableAlias;

            _queryDefinition.SourceDefinitions[typeof(T)] =
                new QuerySourceDefinition
                {
                    EntityType = typeof(T),
                    TableName = tableName,
                    TableAlias = resolvedAlias,
                    ColumnMappings = _queryDefinition.ColumnMappings
                };

            _queryDefinition.TableAlias = resolvedAlias;
            _context.AliasRegistry.Register(resolvedAlias);
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
