using System.Linq.Expressions;
using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Models;
using TinyBlueWhale.EngineQuery.Core.Enums;
using TinyBlueWhale.EngineQuery.Core.ExpressionsParsing;
using TinyBlueWhale.EngineQuery.Core.Helpers;
using TinyBlueWhale.EngineQuery.Core.Interfaces;
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
        private readonly HashSet<string> _registeredAliases = [];

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

            if (!string.IsNullOrWhiteSpace(querySource.TableAlias))
                _registeredAliases.Add(querySource.TableAlias);
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
            _queryDefinition.IsDistinct = true;

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
            ArgumentNullException.ThrowIfNull(selector);

            var selectedProperties = SelectedPropertyExpressionExtractor.ExtractSelectedProperties(selector);

            _queryDefinition.SelectDefinitions.AddRange(selectedProperties);

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
            ArgumentNullException.ThrowIfNull(selector);

            var sourceDefinition = ResolveQuerySource<TEntity>();
            var selectedColumns = SelectedPropertyExpressionExtractor.ExtractSelectedProperties(selector);

            foreach (var selectedColumn in selectedColumns)
            {
                _queryDefinition.SelectDefinitions.Add(
                    selectedColumn with
                    {
                        Source = sourceDefinition
                    });
            }

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
            ArgumentNullException.ThrowIfNull(expression);
            ArgumentException.ThrowIfNullOrWhiteSpace(alias);

            var sourceDefinition = ResolveQuerySource<TEntity>();

            _queryDefinition.ComputedExpressionDefinitions.Add(
                new QueryComputedExpressionDefinition
                {
                    Expression = expression,
                    Alias = alias,
                    Source = sourceDefinition
                });

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
            ArgumentNullException.ThrowIfNull(selector);
            ArgumentException.ThrowIfNullOrWhiteSpace(alias);

            var sourceDefinition = ResolveQuerySource<TEntity>();
            var propertyName = QueryColumnExpressionExtractor.ExtractColumns(selector).Single().PropertyName;

            _queryDefinition.AggregateDefinitions.Add(
                new QueryAggregateDefinition
                {
                    Function = function,
                    PropertyName = propertyName,
                    Alias = alias,
                    Source = sourceDefinition
                });

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
            ArgumentNullException.ThrowIfNull(selector);
            ArgumentException.ThrowIfNullOrWhiteSpace(alias);

            var sourceDefinition = ResolveQuerySource<TEntity>();

            var propertyName = QueryColumnExpressionExtractor
                .ExtractColumns(selector)
                .Single()
                .PropertyName;

            _queryDefinition.ScalarFunctionDefinitions.Add(
                new QueryScalarFunctionDefinition
                {
                    Function = function,
                    PropertyName = propertyName,
                    Alias = alias,
                    Source = sourceDefinition
                });

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
            ArgumentNullException.ThrowIfNull(argumentsSelector);
            ArgumentException.ThrowIfNullOrWhiteSpace(alias);

            var sourceDefinition = ResolveQuerySource<TEntity>();

            _queryDefinition.ScalarFunctionDefinitions.Add(
                new QueryScalarFunctionDefinition
                {
                    Function = function,
                    Arguments = ExtractScalarFunctionArguments(argumentsSelector),
                    Alias = alias,
                    Source = sourceDefinition
                });

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
            ArgumentNullException.ThrowIfNull(condition);
            ArgumentException.ThrowIfNullOrWhiteSpace(alias);

            var sourceDefinition = ResolveQuerySource<TEntity>();

            _queryDefinition.CaseWhenDefinitions.Add(
                new QueryCaseWhenDefinition
                {
                    ConditionExpression = condition,
                    WhenTrueValue = whenTrue,
                    WhenFalseValue = whenFalse,
                    Alias = alias,
                    Source = sourceDefinition
                });

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
            ArgumentNullException.ThrowIfNull(subqueryBuilder);

            var nestedQueryBuilder = new QueryBuilder(_queryCompiler, _metadataResolver);

            var nestedCommandBuilder = subqueryBuilder(nestedQueryBuilder);

            if (nestedCommandBuilder is not QueryCommandBuilder<TSubquery> concreteNestedCommandBuilder)
                throw new InvalidOperationException("The EXISTS subquery builder returned an unsupported query command builder instance.");

            var subqueryDefinition = concreteNestedCommandBuilder.BuildDefinition();
            subqueryDefinition.UseConstantSelectProjection = true;

            _queryDefinition.ExistsDefinitions.Add(
                new QueryExistsDefinition
                {
                    Subquery = subqueryDefinition
                });

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
            ArgumentNullException.ThrowIfNull(subqueryBuilder);

            var outerSource = ResolveQuerySource<TOuter>();

            if (_metadataResolver is null)
                throw new InvalidOperationException("No entity metadata resolver is configured.");

            if (!_metadataResolver.TryResolve<TSubquery>(out var subqueryMetadata))
                throw new InvalidOperationException($"Metadata for entity type '{typeof(TSubquery).Name}' could not be resolved.");

            var columnMappings = subqueryMetadata!.Properties.ToDictionary(
                property => property.Key,
                property => property.Value.ColumnName);

            var nestedCommandBuilder = new QueryCommandBuilder<TSubquery>(
                _queryCompiler,
                subqueryMetadata.TableName,
                alias,
                columnMappings,
                _metadataResolver);

            nestedCommandBuilder.RegisterOuterSources(
                new Dictionary<Type, QuerySourceDefinition>
                {
                    [typeof(TOuter)] = outerSource
                });

            var configuredNestedCommandBuilder = subqueryBuilder(nestedCommandBuilder);

            if (configuredNestedCommandBuilder is not QueryCommandBuilder<TSubquery> concreteNestedCommandBuilder)
                throw new InvalidOperationException("The EXISTS subquery builder returned an unsupported query command builder instance.");

            var subqueryDefinition = concreteNestedCommandBuilder.BuildDefinition();

            subqueryDefinition.UseConstantSelectProjection = true;

            _queryDefinition.ExistsDefinitions.Add(
                new QueryExistsDefinition
                {
                    Subquery = subqueryDefinition
                });

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
            ArgumentNullException.ThrowIfNull(outerSelector);
            ArgumentNullException.ThrowIfNull(subqueryBuilder);

            var outerSource = ResolveQuerySource<TOuter>();

            if (_metadataResolver is null)
                throw new InvalidOperationException("No entity metadata resolver is configured.");

            if (!_metadataResolver.TryResolve<TSubquery>(out var subqueryMetadata))
                throw new InvalidOperationException($"Metadata for entity type '{typeof(TSubquery).Name}' could not be resolved.");

            var columnMappings = subqueryMetadata!.Properties.ToDictionary(
                property => property.Key,
                property => property.Value.ColumnName);

            var nestedCommandBuilder = new QueryCommandBuilder<TSubquery>(
                _queryCompiler,
                subqueryMetadata.TableName,
                alias,
                columnMappings,
                _metadataResolver);

            nestedCommandBuilder.RegisterOuterSources(
                new Dictionary<Type, QuerySourceDefinition>
                {
                    [typeof(TOuter)] = outerSource
                });

            var configuredNestedCommandBuilder = subqueryBuilder(nestedCommandBuilder);

            if (configuredNestedCommandBuilder is not QueryCommandBuilder<TSubquery> concreteNestedCommandBuilder)
                throw new InvalidOperationException("The IN subquery builder returned an unsupported query command builder instance.");

            _queryDefinition.InSubqueryDefinitions.Add(
                new QueryInSubqueryDefinition
                {
                    OuterSelector = outerSelector,
                    OuterSource = outerSource,
                    Subquery = concreteNestedCommandBuilder.BuildDefinition()
                });

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
            ArgumentNullException.ThrowIfNull(subqueryBuilder);

            var outerSource = ResolveQuerySource<TOuter>();

            if (_metadataResolver is null)
                throw new InvalidOperationException("No entity metadata resolver is configured.");

            if (!_metadataResolver.TryResolve<TSubquery>(out var subqueryMetadata))
                throw new InvalidOperationException($"Metadata for entity type '{typeof(TSubquery).Name}' could not be resolved.");

            var columnMappings = subqueryMetadata!.Properties.ToDictionary(
                property => property.Key,
                property => property.Value.ColumnName);

            var nestedCommandBuilder = new QueryCommandBuilder<TSubquery>(
                _queryCompiler,
                subqueryMetadata.TableName,
                alias,
                columnMappings,
                _metadataResolver);

            nestedCommandBuilder.RegisterOuterSources(
                new Dictionary<Type, QuerySourceDefinition>
                {
                    [typeof(TOuter)] = outerSource
                });

            var configuredNestedCommandBuilder = subqueryBuilder(nestedCommandBuilder);

            if (configuredNestedCommandBuilder is not QueryCommandBuilder<TSubquery> concreteNestedCommandBuilder)
                throw new InvalidOperationException("The NOT EXISTS subquery builder returned an unsupported query command builder instance.");

            var subqueryDefinition = concreteNestedCommandBuilder.BuildDefinition();

            subqueryDefinition.UseConstantSelectProjection = true;

            _queryDefinition.ExistsDefinitions.Add(
                new QueryExistsDefinition
                {
                    Subquery = subqueryDefinition,
                    IsNegated = true
                });

            return this;
        }

        // Extracts scalar SQL function arguments from an array expression.
        private static List<QueryScalarFunctionArgumentDefinition> ExtractScalarFunctionArguments<TEntity>(Expression<Func<TEntity, object[]>> expression)
        {
            return expression.Body switch
            {
                NewArrayExpression newArrayExpression => [.. newArrayExpression.Expressions.Select(CreateScalarFunctionArgument)],

                _ => throw new NotSupportedException(
                    $"Expression '{expression}' is not supported as a scalar function argument selector.")
            };
        }

        // Creates a scalar SQL function argument definition from an expression.
        private static QueryScalarFunctionArgumentDefinition CreateScalarFunctionArgument(Expression expression)
        {
            if (expression is UnaryExpression unaryExpression)
                expression = unaryExpression.Operand;

            if (expression is MemberExpression memberExpression)
                return new QueryScalarFunctionArgumentDefinition
                {
                    PropertyName = memberExpression.Member.Name
                };
            

            if (expression is ConstantExpression constantExpression)
                return new QueryScalarFunctionArgumentDefinition
                {
                    ConstantValue = constantExpression.Value
                };
            

            throw new NotSupportedException($"Scalar function argument expression '{expression}' is not supported.");
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
            return AddJoin(QueryJoinType.Inner, alias, on);
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
            return AddJoin(QueryJoinType.Left, alias, on);
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
            return AddJoin(QueryJoinType.Inner, tableName, alias, on);
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
            return AddJoin(QueryJoinType.Left, tableName, alias, on);
        }

        // Adds a metadata-driven join definition.
        private QueryCommandBuilder<T> AddJoin<TSource, TJoin>(QueryJoinType joinType, string? alias, Expression<Func<TSource, TJoin, bool>> on)
        {
            ArgumentNullException.ThrowIfNull(on);

            if (_metadataResolver is null)
                throw new InvalidOperationException("No entity metadata resolver is configured.");

            if (!_metadataResolver.TryResolve<TJoin>(out var joinMetadata))
                throw new InvalidOperationException($"Metadata for entity type '{typeof(TJoin).Name}' could not be resolved.");

            return AddJoin(joinType, joinMetadata!.TableName, alias, on);
        }

        // Adds an explicit table join definition.
        private QueryCommandBuilder<T> AddJoin<TSource, TJoin>(QueryJoinType joinType, string tableName, string? alias, Expression<Func<TSource, TJoin, bool>> on)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(tableName);
            ArgumentNullException.ThrowIfNull(on);

            var joinAlias = ResolveJoinAlias(alias);
            var sourceAlias = ResolveSourceAlias<TSource>();

            var sourceColumnMappings = ResolveColumnMappings<TSource>();
            var joinColumnMappings = ResolveColumnMappings<TJoin>();

            _queryDefinition.JoinDefinitions.Add(
                new QueryJoinDefinition
                {
                    JoinType = joinType,
                    TableName = tableName,
                    TableAlias = joinAlias,
                    SourceType = typeof(TSource),
                    SourceAlias = sourceAlias,
                    SourceColumnMappings = sourceColumnMappings,
                    JoinTypeEntity = typeof(TJoin),
                    JoinColumnMappings = joinColumnMappings,
                    JoinExpression = on
                });

            RegisterJoinSource<TJoin>(tableName, joinAlias, joinColumnMappings);

            return this;
        }


        // Registers a joined query source in the current query scope.
        private void RegisterJoinSource<TEntity>(string tableName, string tableAlias, IReadOnlyDictionary<string, string> columnMappings)
        {
            _queryDefinition.SourceDefinitions[typeof(TEntity)] =
                new QuerySourceDefinition
                {
                    EntityType = typeof(TEntity),
                    TableName = tableName,
                    TableAlias = tableAlias,
                    ColumnMappings = columnMappings
                };
        }

        // Resolves column mappings for an entity participating in the query.
        private IReadOnlyDictionary<string, string> ResolveColumnMappings<TEntity>()
        {
            if (typeof(TEntity) == typeof(T))
                return _queryDefinition.ColumnMappings;

            if (_metadataResolver is not null && _metadataResolver.TryResolve<TEntity>(out var metadata))
            {
                return metadata!.Properties.ToDictionary(
                    property => property.Key,
                    property => property.Value.ColumnName);
            }

            return new Dictionary<string, string>();
        }

        // Resolves or generates a SQL alias for joined tables.
        private string ResolveJoinAlias(string? alias)
        {
            var resolvedAlias = string.IsNullOrWhiteSpace(alias)
                ? QueryAliasGeneratorHelper.Generate(_registeredAliases.Count)
                : alias;

            if (!_registeredAliases.Add(resolvedAlias))
                throw new InvalidOperationException($"Alias '{resolvedAlias}' is already registered.");

            return resolvedAlias;
        }


        // Resolves the alias associated with a previously registered query entity.
        private string ResolveSourceAlias<TSource>()
        {
            if (typeof(TSource) == typeof(T))
                return EnsureRootAlias();

            var joinDefinition = _queryDefinition.JoinDefinitions
                .LastOrDefault(join => join.JoinTypeEntity == typeof(TSource));

            return joinDefinition is null
                ? throw new InvalidOperationException($"Entity type '{typeof(TSource).Name}' is not available in the current query scope.")
                : joinDefinition.TableAlias;
        }

        // Ensures the root query source has a deterministic alias when joins exist.
        private string EnsureRootAlias()
        {
            if (!string.IsNullOrWhiteSpace(_queryDefinition.TableAlias))
                return _queryDefinition.TableAlias;

            var alias = QueryAliasGeneratorHelper.Generate(0);

            _queryDefinition.TableAlias = alias;

            _registeredAliases.Add(alias);

            return alias;
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
            return AddWhere(predicate);
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
            return AddWhere(predicate);
        }

        // Adds a WHERE definition using the metadata of the specified query source.
        private QueryCommandBuilder<T> AddWhere<TEntity>(Expression<Func<TEntity, bool>> predicate)
        {
            ArgumentNullException.ThrowIfNull(predicate);

            var sourceDefinition = ResolveQuerySource<TEntity>();

            _queryDefinition.WhereDefinitions.Add(
                new QueryWhereDefinition
                {
                    PredicateExpression = predicate,
                    Source = sourceDefinition
                });

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
            ArgumentNullException.ThrowIfNull(predicate);

            return condition ? AddWhere(predicate) : this;
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
            return condition ? AddWhere(predicate) : this;
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
            ArgumentNullException.ThrowIfNull(selector);

            var sourceDefinition = ResolveQuerySource<TEntity>();
            var propertyName = QueryColumnExpressionExtractor.ExtractColumns(selector).Single().PropertyName;

            _queryDefinition.WhereScalarFunctionDefinitions.Add(
                new QueryWhereScalarFunctionDefinition
                {
                    Function = function,
                    PropertyName = propertyName,
                    ComparisonOperator = comparisonOperator,
                    Value = value,
                    Source = sourceDefinition
                });

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
            ArgumentNullException.ThrowIfNull(expression);

            var sourceDefinition = ResolveQuerySource<TEntity>();

            _queryDefinition.WhereComputedExpressionDefinitions.Add(
                new QueryWhereComputedExpressionDefinition
                {
                    Expression = expression,
                    Sources = new Dictionary<ParameterExpression, QuerySourceDefinition>
                    {
                        [expression.Parameters.Single()] = sourceDefinition
                    }
                });

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
            ArgumentNullException.ThrowIfNull(expression);

            var leftSource = ResolveQuerySource<TLeft>();
            var rightSource = ResolveQuerySource<TRight>();

            _queryDefinition.WhereComputedExpressionDefinitions.Add(
                new QueryWhereComputedExpressionDefinition
                {
                    Expression = expression,
                    Sources = new Dictionary<ParameterExpression, QuerySourceDefinition>
                    {
                        [expression.Parameters[0]] = leftSource,
                        [expression.Parameters[1]] = rightSource
                    }
                });

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
            return AddOrderingDefinition<T>(keySelector, QueryOrderingDirection.Ascending);
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
            return AddOrderingDefinition<TEntity>(keySelector, QueryOrderingDirection.Ascending);
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
            return AddOrderingDefinition<T>(keySelector, QueryOrderingDirection.Descending);
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
            return AddOrderingDefinition<TEntity>(keySelector, QueryOrderingDirection.Descending);
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
            return AddOrderingDefinition<T>(keySelector, QueryOrderingDirection.Ascending);
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
            return AddOrderingDefinition<TEntity>(keySelector, QueryOrderingDirection.Ascending);
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
            return AddOrderingDefinition<T>(keySelector, QueryOrderingDirection.Descending);
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
            return AddOrderingDefinition<TEntity>(keySelector, QueryOrderingDirection.Descending);
        }

        // Adds an ORDER BY definition using the metadata of the specified query source.
        private QueryCommandBuilder<T> AddOrderingDefinition<TEntity>(Expression<Func<TEntity, object>> keySelector, QueryOrderingDirection orderingDirection)
        {
            ArgumentNullException.ThrowIfNull(keySelector);

            var sourceDefinition = ResolveQuerySource<TEntity>();
            var orderingColumns = QueryColumnExpressionExtractor.ExtractColumns(keySelector);

            _queryDefinition.OrderingDefinitions.Add(
                new QueryOrderingDefinition
                {
                    Columns = orderingColumns,
                    Direction = orderingDirection,
                    Source = sourceDefinition
                });

            return this;
        }     

        #endregion

        #region GroupBy Overloads

        /// <summary>
        /// Adds a GROUP BY clause for the root entity.
        /// </summary>
        public IQueryCommandBuilder<T> GroupBy(Expression<Func<T, object>> selector)
        {
            return AddGroupByDefinition(selector);
        }

        /// <summary>
        /// Adds a GROUP BY clause for an entity available in the current query scope.
        /// </summary>
        public IQueryCommandBuilder<T> GroupBy<TEntity>(Expression<Func<TEntity, object>> selector)
        {
            return AddGroupByDefinition(selector);
        }

        // Adds a GROUP BY definition using the metadata of the specified query source.
        private QueryCommandBuilder<T> AddGroupByDefinition<TEntity>(Expression<Func<TEntity, object>> selector)
        {
            ArgumentNullException.ThrowIfNull(selector);

            var sourceDefinition = ResolveQuerySource<TEntity>();
            var groupByColumns = QueryColumnExpressionExtractor.ExtractColumns(selector);

            _queryDefinition.GroupByDefinitions.Add(
                new QueryGroupByDefinition
                {
                    Columns = groupByColumns,
                    Source = sourceDefinition
                });

            return this;
        }

        #endregion

        #region Union Overloads

        /// <summary>
        /// Adds a UNION query to the current query.
        /// </summary>
        /// <typeparam name="TUnion">
        /// Root entity type used by the UNION query.
        /// </typeparam>
        /// <param name="unionBuilder">
        /// Function used to build the UNION query.
        /// </param>
        /// <returns>
        /// Current query command builder instance.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="unionBuilder"/> is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the UNION builder returns an unsupported query command builder instance.
        /// </exception>
        public IQueryCommandBuilder<T> Union<TUnion>(Func<IQueryBuilder, IQueryCommandBuilder<TUnion>> unionBuilder)
        {
            return AddUnion(unionBuilder, includeDuplicates: false);
        }

        /// <summary>
        /// Adds a UNION ALL query to the current query.
        /// </summary>
        /// <typeparam name="TUnion">
        /// Root entity type used by the UNION ALL query.
        /// </typeparam>
        /// <param name="unionBuilder">
        /// Function used to build the UNION ALL query.
        /// </param>
        /// <returns>
        /// Current query command builder instance.
        /// </returns>
        public IQueryCommandBuilder<T> UnionAll<TUnion>(Func<IQueryBuilder, IQueryCommandBuilder<TUnion>> unionBuilder)
        {
            return AddUnion(unionBuilder, includeDuplicates: true);
        }

        // Adds a UNION or UNION ALL query to the current query.
        private QueryCommandBuilder<T> AddUnion<TUnion>(Func<IQueryBuilder, IQueryCommandBuilder<TUnion>> unionBuilder, bool includeDuplicates)
        {
            ArgumentNullException.ThrowIfNull(unionBuilder);

            var nestedQueryBuilder = new QueryBuilder(
                _queryCompiler,
                _metadataResolver);

            var unionCommandBuilder = unionBuilder(nestedQueryBuilder);

            if (unionCommandBuilder is not QueryCommandBuilder<TUnion> concreteUnionCommandBuilder)
                throw new InvalidOperationException("The UNION builder returned an unsupported query command builder instance.");

            var unionQueryDefinition = concreteUnionCommandBuilder.BuildDefinition();

            unionQueryDefinition.ForceSelectAliases = true;

            _queryDefinition.ForceSelectAliases = true;

            _queryDefinition.UnionDefinitions.Add(
                new QueryUnionDefinition
                {
                    Query = unionQueryDefinition,
                    IncludeDuplicates = includeDuplicates
                });

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
            ArgumentNullException.ThrowIfNull(selector);

            var sourceDefinition = ResolveQuerySource<TEntity>();
            var propertyName = QueryColumnExpressionExtractor.ExtractColumns(selector).Single().PropertyName;

            _queryDefinition.HavingAggregateDefinitions.Add(
                new QueryHavingAggregateDefinition
                {
                    Function = function,
                    PropertyName = propertyName,
                    ComparisonOperator = comparisonOperator,
                    Value = value,
                    Source = sourceDefinition
                });

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
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Skip count cannot be negative.");

            _queryDefinition.Pagination =
                _queryDefinition.Pagination with
                {
                    Skip = count
                };

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
            if (count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Take count must be greater than zero.");

            _queryDefinition.Pagination =
                _queryDefinition.Pagination with
                {
                    Take = count
                };

            return this;
        }

        #endregion

        /// <summary>
        /// Adds a ROW_NUMBER window function projection to the current query.
        /// </summary>
        /// <param name="alias">
        /// SQL alias assigned to the ROW_NUMBER result.
        /// </param>
        /// <param name="windowBuilder">
        /// Function used to configure the window function clauses.
        /// </param>
        /// <returns>
        /// Current query command builder instance.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="alias"/> is null, empty or whitespace.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="windowBuilder"/> is null.
        /// </exception>
        public IQueryCommandBuilder<T> SelectRowNumber(
            string alias,
            Func<IWindowFunctionBuilder, IWindowFunctionBuilder> windowBuilder)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(alias);
            ArgumentNullException.ThrowIfNull(windowBuilder);

            var builder = new WindowFunctionBuilder(ResolveQuerySource);

            windowBuilder(builder);

            _queryDefinition.RowNumberDefinitions.Add(
                builder.BuildRowNumberDefinition(alias));

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
            _registeredAliases.Add(resolvedAlias);
        }

        // Resolves a query source previously registered in the current query scope.
        private QuerySourceDefinition ResolveQuerySource<TEntity>()
        {
            var type = typeof(TEntity);
            return ResolveQuerySource(type);            
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
