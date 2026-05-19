using System.Linq.Expressions;
using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Core.Enums;
using TinyBlueWhale.EngineQuery.Core.ExpressionsParsing;
using TinyBlueWhale.EngineQuery.Core.Helpers;
using TinyBlueWhale.EngineQuery.Core.Interfaces;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Context;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Filtering;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Grouping;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Joining;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Ordering;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Projections;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.SetOperations;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Sources;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Subqueries;
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
    public sealed class QueryCommandBuilder<T> : IOrderedQueryCommandBuilder<T>
    {
        private readonly IQueryCompiler _queryCompiler;
        private readonly CompiledQueryDefinition _queryDefinition;
        private readonly IEntityMetadataResolver? _metadataResolver;

        private readonly QueryCommandBuilderContext _context;
        private readonly QueryCommandBuilderComponents _components;

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
        public QueryCommandBuilder(IQueryCompiler queryCompiler, string tableName, string? tableAlias = null,  IReadOnlyDictionary<string, string>? columnMappings = null, IEntityMetadataResolver? metadataResolver = null)
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

        #region Distinct Overloads
        /// <summary>
        /// Applies DISTINCT projection semantics to the query.
        /// </summary>
        /// <returns>
        /// Current query command builder instance.
        /// </returns>
        public IQueryCommandBuilder<T> Distinct()
        {
            _components.SelectProjectionBuilder.ApplyDistinct();

            return this;
        }
        #endregion

        #region Select Overloads
        /// <summary>
        /// Adds selected entity properties to the query projection definition.
        /// </summary>
        /// <param name="selector">
        /// Projection expression that determines which properties are included in the SQL SELECT clause.
        /// </param>
        /// <returns>
        /// Current query command builder instance.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="selector"/> is null.
        /// </exception>
        public IQueryCommandBuilder<T> Select(Expression<Func<T, object>> selector)
        {
            _components.SelectProjectionBuilder.Add(selector);

            return this;
        }

        /// <summary>
        /// Adds selected columns for an entity already available in the current query scope.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type associated with the selected columns.
        /// </typeparam>
        /// <param name="selector">
        /// Projection expression describing the selected columns for the entity.
        /// </param>
        /// <returns>
        /// Current query command builder instance.
        /// </returns>
        public IQueryCommandBuilder<T> Select<TEntity>(Expression<Func<TEntity, object>> selector)
        {
            _components.SelectProjectionBuilder.AddForSource(selector);

            return this;
        }

        /// <summary>
        /// Adds a LAG window function projection to the current query.
        /// </summary>
        public IQueryCommandBuilder<T> SelectLag<TEntity>(Expression<Func<TEntity, object>> expression, string alias, Func<IWindowFunctionBuilder, IWindowFunctionBuilder> windowBuilder, int offset = 1)
        {
            _components.WindowFunctionProjectionBuilder.AddLag(expression, alias, windowBuilder, offset);

            return this;
        }

        /// <summary>
        /// Adds a LEAD window function projection to the current query.
        /// </summary>
        public IQueryCommandBuilder<T> SelectLead<TEntity>(Expression<Func<TEntity, object>> expression, string alias, Func<IWindowFunctionBuilder, IWindowFunctionBuilder> windowBuilder, int offset = 1)
        {
            _components.WindowFunctionProjectionBuilder.AddLead(expression, alias, windowBuilder, offset);

            return this;
        }

        /// <summary>
        /// Adds a FIRST_VALUE window function projection to the current query.
        /// </summary>
        public IQueryCommandBuilder<T> SelectFirstValue<TEntity>(Expression<Func<TEntity, object>> expression, string alias, Func<IWindowFunctionBuilder, IWindowFunctionBuilder> windowBuilder)
        {
            _components.WindowFunctionProjectionBuilder.AddFirstValue(expression, alias, windowBuilder);

            return this;
        }

        /// <summary>
        /// Adds a LAST_VALUE window function projection to the current query.
        /// </summary>
        public IQueryCommandBuilder<T> SelectLastValue<TEntity>(Expression<Func<TEntity, object>> expression, string alias, Func<IWindowFunctionBuilder, IWindowFunctionBuilder> windowBuilder)
        {
            _components.WindowFunctionProjectionBuilder.AddLastValue( expression, alias, windowBuilder);

            return this;
        }

        /// <summary>
        /// Adds an NTILE window function projection to the current query.
        /// </summary>
        public IQueryCommandBuilder<T> SelectNtile(int buckets, string alias, Func<IWindowFunctionBuilder, IWindowFunctionBuilder> windowBuilder)
        {
            _components.WindowFunctionProjectionBuilder.AddNtile(buckets, alias, windowBuilder);

            return this;
        }

       
        #endregion

        #region Computed Expression Overloads
        /// <summary>
        /// Adds a computed SELECT expression for an entity available in the current query scope.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type associated with the computed expression.
        /// </typeparam>
        /// <param name="expression">
        /// Expression used to generate the computed SQL expression.
        /// </param>
        /// <param name="alias">
        /// SQL alias assigned to the computed expression result.
        /// </param>
        /// <returns>
        /// Current query command builder instance.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="expression"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="alias"/> is null, empty or whitespace.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when <typeparamref name="TEntity"/> is not available in the current query scope.
        /// </exception>
        public IQueryCommandBuilder<T> SelectComputed<TEntity>(Expression<Func<TEntity, object>> expression, string alias)
        {
            _components.ComputedProjectionBuilder.Add(expression, alias);

            return this;
        }
        #endregion

        #region Aggregate Overloads
        /// <summary>
        /// Adds an aggregate SELECT expression for an entity available in the current query scope.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type associated with the aggregated column.
        /// </typeparam>
        /// <param name="function">
        /// Aggregate function applied to the selected column.
        /// </param>
        /// <param name="selector">
        /// Expression that selects the aggregated property.
        /// </param>
        /// <param name="alias">
        /// SQL alias assigned to the aggregate result.
        /// </param>
        /// <returns>
        /// Current query command builder instance.
        /// </returns>
        public IQueryCommandBuilder<T> SelectAggregate<TEntity>(QueryAggregateFunction function, Expression<Func<TEntity, object>> selector, string alias)
        {
            _components.AggregateProjectionBuilder.Add(function, selector, alias);

            return this;
        }

        /// <summary>
        /// Adds a scalar SQL function projection for an entity available in the current query scope.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type associated with the selected column.
        /// </typeparam>
        /// <param name="function">
        /// Scalar SQL function applied to the selected column.
        /// </param>
        /// <param name="selector">
        /// Expression that selects the entity property used by the scalar function.
        /// </param>
        /// <param name="alias">
        /// SQL alias assigned to the scalar function result.
        /// </param>
        /// <returns>
        /// Current query command builder instance.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="selector"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="alias"/> is null, empty or whitespace.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when <typeparamref name="TEntity"/> is not available in the current query scope.
        /// </exception>
        public IQueryCommandBuilder<T> SelectFunction<TEntity>(QueryScalarFunction function, Expression<Func<TEntity, object>> selector, string alias)
        {
            _components.ScalarFunctionProjectionBuilder.Add(function, selector, alias);

            return this;
        }

        /// <summary>
        /// Adds a scalar SQL function projection using multiple function arguments for an entity available in the current query scope.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type associated with the function arguments.
        /// </typeparam>
        /// <param name="function">
        /// Scalar SQL function applied to the selected arguments.
        /// </param>
        /// <param name="argumentsSelector">
        /// Expression that selects the scalar function arguments.
        /// </param>
        /// <param name="alias">
        /// SQL alias assigned to the scalar function result.
        /// </param>
        /// <returns>
        /// Current query command builder instance.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="argumentsSelector"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="alias"/> is null, empty or whitespace.
        /// </exception>
        public IQueryCommandBuilder<T> SelectFunction<TEntity>(QueryScalarFunction function, Expression<Func<TEntity, object[]>> argumentsSelector, string alias)
        {
            _components.ScalarFunctionProjectionBuilder.Add(function, argumentsSelector, alias);

            return this;
        }

        /// <summary>
        /// Adds a CASE WHEN SELECT expression for an entity available in the current query scope.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type associated with the CASE WHEN condition.
        /// </typeparam>
        /// <param name="condition">
        /// Boolean expression evaluated by the CASE WHEN expression.
        /// </param>
        /// <param name="whenTrue">
        /// Value returned when the condition is true.
        /// </param>
        /// <param name="whenFalse">
        /// Value returned when the condition is false.
        /// </param>
        /// <param name="alias">
        /// SQL alias assigned to the CASE WHEN expression result.
        /// </param>
        /// <returns>
        /// Current query command builder instance.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="condition"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="alias"/> is null, empty or whitespace.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when <typeparamref name="TEntity"/> is not available in the current query scope.
        /// </exception>
        public IQueryCommandBuilder<T> SelectCase<TEntity>(Expression<Func<TEntity, bool>> condition, object? whenTrue, object? whenFalse, string alias)
        {
            _components.CaseWhenProjectionBuilder.Add(condition, whenTrue, whenFalse, alias);

            return this;
        }

        /// <summary>
        /// Adds an EXISTS subquery condition.
        /// </summary>
        /// <typeparam name="TSubquery">
        /// Root entity type of the EXISTS subquery.
        /// </typeparam>
        /// <param name="subqueryBuilder">
        /// Function used to build the EXISTS subquery.
        /// </param>
        /// <returns>
        /// Current query command builder instance.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="subqueryBuilder"/> is null.
        /// </exception>
        public IQueryCommandBuilder<T> WhereExists<TSubquery>(Func<IQueryBuilder, IQueryCommandBuilder<TSubquery>> subqueryBuilder)
        {
            _components.ExistsClauseBuilder.Add(subqueryBuilder);

            return this;
        }

        /// <summary>
        /// Adds a correlated EXISTS subquery condition using an outer entity available in the current query scope.
        /// </summary>
        /// <typeparam name="TOuter">
        /// Outer entity type available in the current query scope.
        /// </typeparam>
        /// <typeparam name="TSubquery">
        /// Root entity type of the EXISTS subquery.
        /// </typeparam>
        /// <param name="alias">
        /// Optional alias assigned to the EXISTS subquery root table.
        /// </param>
        /// <param name="subqueryBuilder">
        /// Function used to build the correlated EXISTS subquery.
        /// </param>
        /// <returns>
        /// Current query command builder instance.
        /// </returns>
        public IQueryCommandBuilder<T> WhereExists<TOuter, TSubquery>(string? alias, Func<IQueryCommandBuilder<TSubquery>, IQueryCommandBuilder<TSubquery>> subqueryBuilder)
        {
            _components.ExistsClauseBuilder.AddCorrelated<TOuter, TSubquery>(alias, subqueryBuilder, isNegated: false);

            return this;
        }

        /// <summary>
        /// Adds an IN subquery condition for an entity available in the current query scope.
        /// </summary>
        /// <typeparam name="TOuter">
        /// Outer entity type associated with the selected column.
        /// </typeparam>
        /// <typeparam name="TSubquery">
        /// Root entity type of the IN subquery.
        /// </typeparam>
        /// <param name="outerSelector">
        /// Expression that selects the outer column evaluated by the IN condition.
        /// </param>
        /// <param name="alias">
        /// Optional alias assigned to the IN subquery root table.
        /// </param>
        /// <param name="subqueryBuilder">
        /// Function used to build the IN subquery.
        /// </param>
        /// <returns>
        /// Current query command builder instance.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="outerSelector"/> or <paramref name="subqueryBuilder"/> is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when metadata for <typeparamref name="TSubquery"/> cannot be resolved.
        /// </exception>
        public IQueryCommandBuilder<T> WhereIn<TOuter, TSubquery>(Expression<Func<TOuter, object>> outerSelector, string? alias, Func<IQueryCommandBuilder<TSubquery>, IQueryCommandBuilder<TSubquery>> subqueryBuilder)
        {
            _components.InSubqueryClauseBuilder.Add(outerSelector, alias, subqueryBuilder);

            return this;
        }

        /// <summary>
        /// Adds a correlated NOT EXISTS subquery condition using an outer entity available in the current query scope.
        /// </summary>
        /// <typeparam name="TOuter">
        /// Outer entity type available in the current query scope.
        /// </typeparam>
        /// <typeparam name="TSubquery">
        /// Root entity type of the NOT EXISTS subquery.
        /// </typeparam>
        /// <param name="alias">
        /// Optional alias assigned to the NOT EXISTS subquery root table.
        /// </param>
        /// <param name="subqueryBuilder">
        /// Function used to build the correlated NOT EXISTS subquery.
        /// </param>
        /// <returns>
        /// Current query command builder instance.
        /// </returns>
        public IQueryCommandBuilder<T> WhereNotExists<TOuter, TSubquery>(string? alias, Func<IQueryCommandBuilder<TSubquery>, IQueryCommandBuilder<TSubquery>> subqueryBuilder)
        {
            _components.ExistsClauseBuilder.AddCorrelated<TOuter, TSubquery>(alias, subqueryBuilder, isNegated: true);

            return this;
        }              

        #endregion

        #region Join Overloads
        /// <summary>
        /// Adds an INNER JOIN using resolved metadata for the joined entity.
        /// </summary>
        /// <typeparam name="TSource">
        /// Source entity type participating in the join condition.
        /// </typeparam>
        /// <typeparam name="TJoin">
        /// Joined entity type.
        /// </typeparam>
        /// <param name="alias">
        /// Optional alias assigned to the joined table.
        /// </param>
        /// <param name="on">
        /// Join condition expression.
        /// </param>
        /// <returns>
        /// Current query command builder instance.
        /// </returns>
        public IQueryCommandBuilder<T> InnerJoin<TSource, TJoin>(string? alias, Expression<Func<TSource, TJoin, bool>> on)
        {
            _components.JoinClauseBuilder.Add(QueryJoinType.Inner, alias, on);

            return this;
        }

        /// <summary>
        /// Adds a LEFT JOIN using resolved metadata for the joined entity.
        /// </summary>
        /// <typeparam name="TSource">
        /// Source entity type participating in the join condition.
        /// </typeparam>
        /// <typeparam name="TJoin">
        /// Joined entity type.
        /// </typeparam>
        /// <param name="alias">
        /// Optional alias assigned to the joined table.
        /// </param>
        /// <param name="on">
        /// Join condition expression.
        /// </param>
        /// <returns>
        /// Current query command builder instance.
        /// </returns>
        public IQueryCommandBuilder<T> LeftJoin<TSource, TJoin>(string? alias, Expression<Func<TSource, TJoin, bool>> on)
        {
            _components.JoinClauseBuilder.Add(QueryJoinType.Left, alias, on);

            return this;
        }

        /// <summary>
        /// Adds an INNER JOIN using an explicit joined table name.
        /// </summary>
        /// <typeparam name="TSource">
        /// Source entity type participating in the join condition.
        /// </typeparam>
        /// <typeparam name="TJoin">
        /// Joined entity type.
        /// </typeparam>
        /// <param name="tableName">
        /// Explicit database table name associated with the joined entity.
        /// </param>
        /// <param name="alias">
        /// Optional alias assigned to the joined table.
        /// </param>
        /// <param name="on">
        /// Join condition expression.
        /// </param>
        /// <returns>
        /// Current query command builder instance.
        /// </returns>
        public IQueryCommandBuilder<T> InnerJoinTable<TSource, TJoin>(string tableName, string? alias, Expression<Func<TSource, TJoin, bool>> on)
        {
            _components.JoinClauseBuilder.AddTable(QueryJoinType.Inner, tableName, alias, on);

            return this;
        }

        /// <summary>
        /// Adds a LEFT JOIN using an explicit joined table name.
        /// </summary>
        /// <typeparam name="TSource">
        /// Source entity type participating in the join condition.
        /// </typeparam>
        /// <typeparam name="TJoin">
        /// Joined entity type.
        /// </typeparam>
        /// <param name="tableName">
        /// Explicit database table name associated with the joined entity.
        /// </param>
        /// <param name="alias">
        /// Optional alias assigned to the joined table.
        /// </param>
        /// <param name="on">
        /// Join condition expression.
        /// </param>
        /// <returns>
        /// Current query command builder instance.
        /// </returns>
        public IQueryCommandBuilder<T> LeftJoinTable<TSource, TJoin>(string tableName, string? alias, Expression<Func<TSource, TJoin, bool>> on)
        {
            _components.JoinClauseBuilder.AddTable(QueryJoinType.Left, tableName, alias, on);

            return this;    
        }

        /// <summary>
        /// Adds a CROSS APPLY or provider-equivalent LATERAL subquery join to the current query.
        /// </summary>
        public IQueryCommandBuilder<T> CrossApply<TOuter, TApply>(string alias, Func<IQueryCommandBuilder<TApply>, IQueryCommandBuilder<TApply>> applyBuilder)
        {
            _components.ApplyClauseBuilder.Add<TOuter, TApply>(QueryApplyType.Cross, alias, applyBuilder);

            return this;
        }

        /// <summary>
        /// Adds an OUTER APPLY or provider-equivalent LEFT LATERAL subquery join to the current query.
        /// </summary>
        public IQueryCommandBuilder<T> OuterApply<TOuter, TApply>(string alias, Func<IQueryCommandBuilder<TApply>, IQueryCommandBuilder<TApply>> applyBuilder)
        {
            _components.ApplyClauseBuilder.Add<TOuter, TApply>(QueryApplyType.Outer, alias, applyBuilder);

            return this;
        }

        #endregion

        #region Where Overloads
        /// <summary>
        /// Adds a WHERE predicate for the root entity.
        /// </summary>
        /// <param name="predicate">
        /// Predicate expression describing the SQL filter condition.
        /// </param>
        /// <returns>
        /// Current query command builder instance.
        /// </returns>
        public IQueryCommandBuilder<T> Where(Expression<Func<T, bool>> predicate)
        {
            _components.WhereClauseBuilder.Add(predicate);
            return this;
        }

        /// <summary>
        /// Adds a WHERE predicate for an entity already available in the current query scope.
        /// </summary>
        /// <typeparam name="TSource">
        /// Entity type associated with the filtered columns.
        /// </typeparam>
        /// <param name="predicate">
        /// Predicate expression describing the SQL filter condition.
        /// </param>
        /// <returns>
        /// Current query command builder instance.
        /// </returns>
        public IQueryCommandBuilder<T> Where<TSource>(Expression<Func<TSource, bool>> predicate)
        {
            _components.WhereClauseBuilder.Add(predicate);
            return this;
        }

        /// <summary>
        /// Adds a filtering expression only when the specified condition is true.
        /// </summary>
        /// <param name="condition">
        /// Determines whether the predicate should be added to the query definition.
        /// </param>
        /// <param name="predicate">
        /// Predicate expression used later to generate the SQL WHERE clause.
        /// </param>
        /// <returns>
        /// Current query command builder instance.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="predicate"/> is null.
        /// </exception>
        public IQueryCommandBuilder<T> WhereIf(bool condition, Expression<Func<T, bool>> predicate)
        {
            _components.WhereClauseBuilder.AddIf(condition, predicate);
            return this;
        }


        /// <summary>
        /// Adds a WHERE predicate for an entity available in the current query scope only when the specified condition is true.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type associated with the filtered columns.
        /// </typeparam>
        /// <param name="condition">
        /// Condition that determines whether the predicate is added.
        /// </param>
        /// <param name="predicate">
        /// Predicate expression describing the SQL filter condition.
        /// </param>
        /// <returns>
        /// Current query command builder instance.
        /// </returns>
        public IQueryCommandBuilder<T> WhereIf<TEntity>(bool condition,Expression<Func<TEntity, bool>> predicate)
        {
            _components.WhereClauseBuilder.AddIfForSource(condition, predicate);

            return this;
        }

        /// <summary>
        /// Adds a WHERE condition based on a scalar SQL function for an entity available in the current query scope.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type associated with the function column.
        /// </typeparam>
        /// <param name="function">
        /// Scalar SQL function evaluated by the WHERE condition.
        /// </param>
        /// <param name="selector">
        /// Expression that selects the entity property used by the scalar function.
        /// </param>
        /// <param name="comparisonOperator">
        /// Comparison operator applied to the scalar function result.
        /// </param>
        /// <param name="value">
        /// Comparison value used by the WHERE condition.
        /// </param>
        /// <returns>
        /// Current query command builder instance.
        /// </returns>
        public IQueryCommandBuilder<T> WhereFunction<TEntity>(QueryScalarFunction function, Expression<Func<TEntity, object>> selector, QueryComparisonOperator comparisonOperator, object? value)
        {
            _components.WhereClauseBuilder.AddFunction(function, selector, comparisonOperator, value);

            return this;
        }

        /// <summary>
        /// Adds a WHERE condition based on a computed expression for an entity available in the current query scope.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type associated with the computed expression.
        /// </typeparam>
        /// <param name="expression">
        /// Computed boolean expression used to generate the SQL WHERE condition.
        /// </param>
        /// <returns>
        /// Current query command builder instance.
        /// </returns>
        public IQueryCommandBuilder<T> WhereComputed<TEntity>(Expression<Func<TEntity, bool>> expression)
        {
            _components.WhereClauseBuilder.AddComputed(expression);

            return this;
        }

        /// <summary>
        /// Adds a WHERE condition based on a computed expression involving two entities available in the current query scope.
        /// </summary>
        /// <typeparam name="TLeft">
        /// Left entity type associated with the computed expression.
        /// </typeparam>
        /// <typeparam name="TRight">
        /// Right entity type associated with the computed expression.
        /// </typeparam>
        /// <param name="expression">
        /// Computed boolean expression used to generate the SQL WHERE condition.
        /// </param>
        /// <returns>
        /// Current query command builder instance.
        /// </returns>
        public IQueryCommandBuilder<T> WhereComputed<TLeft, TRight>(Expression<Func<TLeft, TRight, bool>> expression)
        {
            _components.WhereClauseBuilder.AddComputed(expression);
            return this;
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

        #region GroupBy Overloads

        /// <summary>
        /// Adds a GROUP BY clause for the root entity.
        /// </summary>
        public IQueryCommandBuilder<T> GroupBy(Expression<Func<T, object>> selector)
        {
            _components.GroupByClauseBuilder.Add(selector);

            return this;
        }

        /// <summary>
        /// Adds a GROUP BY clause for an entity available in the current query scope.
        /// </summary>
        public IQueryCommandBuilder<T> GroupBy<TEntity>(Expression<Func<TEntity, object>> selector)
        {
            _components.GroupByClauseBuilder.Add(selector);

            return this;
        }
        #endregion

        #region Union Overloads

        /// <summary>
        /// Adds a UNION query to the current query.
        /// </summary>
        public IQueryCommandBuilder<T> Union<TSet>(Func<IQueryBuilder, IQueryCommandBuilder<TSet>> setBuilder)
        {
            _components.SetOperationClauseBuilder.Add(QuerySetOperation.Union, setBuilder);

            return this;
        }

        /// <summary>
        /// Adds a UNION ALL query to the current query.
        /// </summary>
        public IQueryCommandBuilder<T> UnionAll<TSet>(Func<IQueryBuilder, IQueryCommandBuilder<TSet>> setBuilder)
        {
            _components.SetOperationClauseBuilder.Add(QuerySetOperation.UnionAll, setBuilder);

            return this;
        }

        /// <summary>
        /// Adds an INTERSECT query to the current query.
        /// </summary>
        public IQueryCommandBuilder<T> Intersect<TSet>(Func<IQueryBuilder, IQueryCommandBuilder<TSet>> setBuilder)
        {
            _components.SetOperationClauseBuilder.Add(QuerySetOperation.Intersect, setBuilder);

            return this;
        }

        /// <summary>
        /// Adds an EXCEPT query to the current query.
        /// </summary>
        public IQueryCommandBuilder<T> Except<TSet>(Func<IQueryBuilder, IQueryCommandBuilder<TSet>> setBuilder)
        {
            _components.SetOperationClauseBuilder.Add(QuerySetOperation.Except, setBuilder);

            return this;
        }

        #endregion

        #region Having Overloads
        /// <summary>
        /// Adds a HAVING condition based on an aggregate expression for an entity available in the current query scope.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type associated with the aggregated column.
        /// </typeparam>
        /// <param name="function">
        /// Aggregate function evaluated by the HAVING condition.
        /// </param>
        /// <param name="selector">
        /// Expression that selects the aggregated property.
        /// </param>
        /// <param name="comparisonOperator">
        /// Comparison operator applied to the aggregate result.
        /// </param>
        /// <param name="value">
        /// Comparison value used by the HAVING condition.
        /// </param>
        /// <returns>
        /// Current query command builder instance.
        /// </returns>
        public IQueryCommandBuilder<T> HavingAggregate<TEntity>(QueryAggregateFunction function, Expression<Func<TEntity, object>> selector, QueryComparisonOperator comparisonOperator, object? value)
        {
            _components.HavingClauseBuilder.AddAggregate(function, selector, comparisonOperator, value);

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
        /// Adds a ROW_NUMBER window function projection to the current query.
        /// </summary>
        public IQueryCommandBuilder<T> SelectRowNumber(string alias, Func<IWindowFunctionBuilder, IWindowFunctionBuilder> windowBuilder)
        {
            _components.WindowFunctionProjectionBuilder.AddRankingFunction(QueryWindowFunction.RowNumber, alias, windowBuilder);

            return this;
        }

        /// <summary>
        /// Adds a RANK window function projection to the current query.
        /// </summary>
        public IQueryCommandBuilder<T> SelectRank(string alias, Func<IWindowFunctionBuilder, IWindowFunctionBuilder> windowBuilder)
        {
            _components.WindowFunctionProjectionBuilder.AddRankingFunction(QueryWindowFunction.Rank, alias, windowBuilder);

            return this;
        }

        /// <summary>
        /// Adds a DENSE_RANK window function projection to the current query.
        /// </summary>
        public IQueryCommandBuilder<T> SelectDenseRank(string alias, Func<IWindowFunctionBuilder, IWindowFunctionBuilder> windowBuilder)
        {
            _components.WindowFunctionProjectionBuilder.AddRankingFunction(QueryWindowFunction.DenseRank, alias, windowBuilder);

            return this;
        }

        /// <summary>
        /// Compiles the current query definition into SQL command text and parameters.
        /// </summary>
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


        // Resolves a query source using a runtime entity type.
        private QuerySourceDefinition ResolveQuerySource(Type entityType)
        {
            ArgumentNullException.ThrowIfNull(entityType);

            if (_queryDefinition.SourceDefinitions.TryGetValue(entityType, out var sourceDefinition))
                return sourceDefinition;

            if (_queryDefinition.OuterSourceDefinitions.TryGetValue(entityType, out var outerSourceDefinition))
                return outerSourceDefinition;

            throw new InvalidOperationException($"Entity type '{entityType.Name}' is not available in the current query scope.");
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
