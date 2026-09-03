using System.Linq.Expressions;
using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Features;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Core.Enums;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Context;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Features;

namespace TinyBlueWhale.EngineQuery.Core.QueryBuilding
{
    /// <summary>
    /// Provides reusable query composition behavior for strongly typed SQL command builders.
    /// </summary>
    /// <typeparam name="T">
    /// Root entity type associated with the query composition.
    /// </typeparam>
    /// <typeparam name="TBuilder">
    /// Fluent builder type returned by query composition operations.
    /// </typeparam>
    /// <remarks>
    /// This base class captures query composition behavior shared by command builders
    /// without defining command-specific operations such as ordering, pagination or compilation.
    /// </remarks>
    /// <remarks>
    /// Initializes a new instance of the <see cref="QueryCompositionCommandBuilderBase{T, TBuilder}"/> class.
    /// </remarks>        
    public abstract class QueryCompositionCommandBuilderBase<T, TBuilder, TProfile> :
        IQueryCompositionCommandBuilder<T, TBuilder, TProfile>
        where TProfile : IDatabaseProviderProfile
    {

        /// <summary>
        /// Gets the internal query composition components.
        /// </summary>
        private protected abstract QueryCommandBuilderComponents<TProfile> Components { get; }

        /// <summary>
        /// Gets the fluent builder instance returned by composition operations.
        /// </summary>
        protected abstract TBuilder Current { get; }

        #region Distinct Overloads

        /// <summary>
        /// Applies DISTINCT projection semantics to the query.
        /// </summary>
        /// <returns>
        /// Current query composition builder instance.
        /// </returns>
        public TBuilder Distinct()
        {
            Components.SelectProjectionBuilder.ApplyDistinct();

            return Current;
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
        /// Current query composition builder instance.
        /// </returns>
        public TBuilder Select(Expression<Func<T, object>> selector)
        {
            Components.SelectProjectionBuilder.Add(selector);

            return Current;
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
        /// Current query composition builder instance.
        /// </returns>
        public TBuilder Select<TEntity>(Expression<Func<TEntity, object>> selector)
        {
            Components.SelectProjectionBuilder.AddForSource(selector);

            return Current;
        }

        /// <summary>
        /// Adds a LAG window function projection to the current query.
        /// </summary>
        TBuilder IQueryCompositionCommandBuilder<T, TBuilder, TProfile>.ApplyLag<TEntity>(Expression<Func<TEntity, object>> expression, string alias, Func<IWindowFunctionBuilder, IWindowFunctionBuilder> windowBuilder, int offset)
        {
            Components.WindowFunctionProjectionBuilder.AddLag(expression, alias, windowBuilder, offset);

            return Current;
        }

        /// <summary>
        /// Adds a LEAD window function projection to the current query.
        /// </summary>
        TBuilder IQueryCompositionCommandBuilder<T, TBuilder, TProfile>.ApplyLead<TEntity>(Expression<Func<TEntity, object>> expression, string alias, Func<IWindowFunctionBuilder, IWindowFunctionBuilder> windowBuilder, int offset)
        {
            Components.WindowFunctionProjectionBuilder.AddLead(expression, alias, windowBuilder, offset);

            return Current;
        }

        /// <summary>
        /// Adds a FIRST_VALUE window function projection to the current query.
        /// </summary>
        TBuilder IQueryCompositionCommandBuilder<T, TBuilder, TProfile>.ApplyFirstValue<TEntity>(Expression<Func<TEntity, object>> expression, string alias, Func<IWindowFunctionBuilder, IWindowFunctionBuilder> windowBuilder)
        {
            Components.WindowFunctionProjectionBuilder.AddFirstValue(expression, alias, windowBuilder);

            return Current;
        }

        /// <summary>
        /// Adds a LAST_VALUE window function projection to the current query.
        /// </summary>
        TBuilder IQueryCompositionCommandBuilder<T, TBuilder, TProfile>.ApplyLastValue<TEntity>(Expression<Func<TEntity, object>> expression, string alias, Func<IWindowFunctionBuilder, IWindowFunctionBuilder> windowBuilder)
        {
            Components.WindowFunctionProjectionBuilder.AddLastValue(expression, alias, windowBuilder);

            return Current;
        }

        /// <summary>
        /// Adds an NTILE window function projection to the current query.
        /// </summary>
        TBuilder IQueryCompositionCommandBuilder<T, TBuilder, TProfile>.ApplyNtile(int buckets, string alias, Func<IWindowFunctionBuilder, IWindowFunctionBuilder> windowBuilder)
        {
            Components.WindowFunctionProjectionBuilder.AddNtile(buckets, alias, windowBuilder);

            return Current;
        }

        /// <summary>
        /// Adds a ROW_NUMBER window function projection to the current query.
        /// </summary>
        TBuilder IQueryCompositionCommandBuilder<T, TBuilder, TProfile>.ApplyRowNumber(string alias, Func<IWindowFunctionBuilder, IWindowFunctionBuilder> windowBuilder)
        {
            Components.WindowFunctionProjectionBuilder.AddRankingFunction(QueryWindowFunction.RowNumber, alias, windowBuilder);

            return Current;
        }

        /// <summary>
        /// Adds a RANK window function projection to the current query.
        /// </summary>
        TBuilder IQueryCompositionCommandBuilder<T, TBuilder, TProfile>.ApplyRank(string alias, Func<IWindowFunctionBuilder, IWindowFunctionBuilder> windowBuilder)
        {
            Components.WindowFunctionProjectionBuilder.AddRankingFunction(QueryWindowFunction.Rank, alias, windowBuilder);

            return Current;
        }

        /// <summary>
        /// Adds a DENSE_RANK window function projection to the current query.
        /// </summary>
        TBuilder IQueryCompositionCommandBuilder<T, TBuilder, TProfile>.ApplyDenseRank(string alias, Func<IWindowFunctionBuilder, IWindowFunctionBuilder> windowBuilder)
        {
            Components.WindowFunctionProjectionBuilder.AddRankingFunction(QueryWindowFunction.DenseRank, alias, windowBuilder);

            return Current;
        }

        #endregion

        #region Computed Expression Overloads

        /// <summary>
        /// Adds a computed SELECT expression for an entity available in the current query scope.
        /// </summary>
        public TBuilder SelectComputed<TEntity>(Expression<Func<TEntity, object>> expression, string alias)
        {
            Components.ComputedProjectionBuilder.Add(expression, alias);

            return Current;
        }

        #endregion

        #region Aggregate Overloads

        /// <summary>
        /// Adds an aggregate SELECT expression for an entity available in the current query scope.
        /// </summary>
        public TBuilder SelectAggregate<TEntity>(QueryAggregateFunction function, Expression<Func<TEntity, object>> selector, string alias)
        {
            Components.AggregateProjectionBuilder.Add(function, selector, alias);

            return Current;
        }

        /// <summary>
        /// Adds a scalar SQL function projection for an entity available in the current query scope.
        /// </summary>
        public TBuilder SelectScalarFunction<TEntity>(QueryScalarFunction function, Expression<Func<TEntity, object>> selector, string alias)
        {
            Components.ScalarFunctionProjectionBuilder.Add(function, selector, alias);

            return Current;
        }

        /// <summary>
        /// Adds a scalar SQL function projection using multiple function arguments for an entity available in the current query scope.
        /// </summary>
        public TBuilder SelectScalarFunction<TEntity>(QueryScalarFunction function, Expression<Func<TEntity, object[]>> argumentsSelector, string alias)
        {
            Components.ScalarFunctionProjectionBuilder.Add(function, argumentsSelector, alias);

            return Current;
        }

        /// <summary>
        /// Adds a CASE WHEN SELECT expression for an entity available in the current query scope.
        /// </summary>
        public TBuilder SelectCaseWhen<TEntity>(Expression<Func<TEntity, bool>> condition, object? whenTrue, object? whenFalse, string alias)
        {
            Components.CaseWhenProjectionBuilder.Add(condition, whenTrue, whenFalse, alias);

            return Current;
        }

        #endregion

        #region Subquery Filtering Overloads

        /// <summary>
        /// Adds an EXISTS subquery condition.
        /// </summary>
        public TBuilder WhereExists<TSubquery>(Func<IQueryBuilder<TProfile>, IQueryCommandBuilder<TSubquery, TProfile>> subqueryBuilder)
        {
            Components.ExistsClauseBuilder.Add(subqueryBuilder);

            return Current;
        }

        /// <summary>
        /// Adds a correlated EXISTS subquery condition using an outer entity available in the current query scope.
        /// </summary>
        public TBuilder WhereExists<TOuter, TSubquery>(string? alias, Func<IQueryCommandBuilder<TSubquery, TProfile>, IQueryCommandBuilder<TSubquery, TProfile>> subqueryBuilder)
        {
            Components.ExistsClauseBuilder.AddCorrelated<TOuter, TSubquery>(alias, subqueryBuilder, isNegated: false);

            return Current;
        }

        /// <summary>
        /// Adds an IN subquery condition for an entity available in the current query scope.
        /// </summary>
        public TBuilder WhereIn<TOuter, TSubquery>(Expression<Func<TOuter, object>> outerSelector, string? alias, Func<IQueryCommandBuilder<TSubquery, TProfile>, IQueryCommandBuilder<TSubquery, TProfile>> subqueryBuilder)
        {
            Components.InSubqueryClauseBuilder.Add(outerSelector, alias, subqueryBuilder);

            return Current;
        }

        /// <summary>
        /// Adds a correlated NOT EXISTS subquery condition using an outer entity available in the current query scope.
        /// </summary>
        public TBuilder WhereNotExists<TOuter, TSubquery>(string? alias, Func<IQueryCommandBuilder<TSubquery, TProfile>, IQueryCommandBuilder<TSubquery, TProfile>> subqueryBuilder)
        {
            Components.ExistsClauseBuilder.AddCorrelated<TOuter, TSubquery>(alias, subqueryBuilder, isNegated: true);

            return Current;
        }

        #endregion

        #region Join Overloads

        /// <summary>
        /// Adds an INNER JOIN using resolved metadata for the joined entity.
        /// </summary>
        public TBuilder InnerJoin<TSource, TJoin>(string? alias, Expression<Func<TSource, TJoin, bool>> on)
        {
            Components.JoinClauseBuilder.Add(QueryJoinType.Inner, alias, on);

            return Current;
        }

        /// <summary>
        /// Adds a LEFT JOIN using resolved metadata for the joined entity.
        /// </summary>
        public TBuilder LeftJoin<TSource, TJoin>(string? alias, Expression<Func<TSource, TJoin, bool>> on)
        {
            Components.JoinClauseBuilder.Add(QueryJoinType.Left, alias, on);

            return Current;
        }

        /// <summary>
        /// Adds an INNER JOIN using an explicit joined table name.
        /// </summary>
        public TBuilder InnerJoinTable<TSource, TJoin>(string tableName, string? schemaName, string? alias, Expression<Func<TSource, TJoin, bool>> on)
        {
            Components.JoinClauseBuilder.AddTable(QueryJoinType.Inner, tableName, schemaName, alias, on);

            return Current;
        }

        /// <summary>
        /// Adds a LEFT JOIN using an explicit joined table name.
        /// </summary>
        public TBuilder LeftJoinTable<TSource, TJoin>(string tableName, string? schemaName, string? alias, Expression<Func<TSource, TJoin, bool>> on)
        {
            Components.JoinClauseBuilder.AddTable(QueryJoinType.Left, tableName, schemaName, alias, on);

            return Current;
        }

        /// <summary>
        /// Adds a CROSS APPLY or provider-equivalent LATERAL subquery join to the current query.
        /// </summary>
        public TBuilder CrossApply<TOuter, TApply>(string alias, Func<IQueryCommandBuilder<TApply, TProfile>, IQueryCommandBuilder<TApply, TProfile>> applyBuilder)
        {
            Components.ApplyClauseBuilder.Add<TOuter, TApply>(QueryApplyType.Cross, alias, applyBuilder);

            return Current;
        }

        /// <summary>
        /// Adds an OUTER APPLY or provider-equivalent LEFT LATERAL subquery join to the current query.
        /// </summary>
        public TBuilder OuterApply<TOuter, TApply>(string alias, Func<IQueryCommandBuilder<TApply, TProfile>, IQueryCommandBuilder<TApply, TProfile>> applyBuilder)
        {
            Components.ApplyClauseBuilder.Add<TOuter, TApply>(QueryApplyType.Outer, alias, applyBuilder);

            return Current;
        }

        #endregion

        #region Where Overloads

        /// <summary>
        /// Adds a WHERE predicate for the root entity.
        /// </summary>
        public TBuilder Where(Expression<Func<T, bool>> predicate)
        {
            Components.WhereClauseBuilder.Add(predicate);

            return Current;
        }

        /// <summary>
        /// Adds a WHERE predicate for the root entity using the specified logical operator.
        /// </summary>
        public TBuilder Where(Expression<Func<T, bool>> predicate, QueryLogicalOperator logicalOperator)
        {
            Components.WhereClauseBuilder.Add(predicate, logicalOperator);

            return Current;
        }

        /// <summary>
        /// Adds a WHERE predicate for an entity already available in the current query scope.
        /// </summary>
        public TBuilder Where<TSource>(Expression<Func<TSource, bool>> predicate)
        {
            Components.WhereClauseBuilder.Add(predicate);

            return Current;
        }

        /// <summary>
        /// Adds a WHERE predicate for an entity already available in the current query scope using the specified logical operator.
        /// </summary>
        public TBuilder Where<TSource>(Expression<Func<TSource, bool>> predicate, QueryLogicalOperator logicalOperator)
        {
            Components.WhereClauseBuilder.Add(predicate, logicalOperator);

            return Current;
        }

        /// <summary>
        /// Adds an IN collection condition for the root entity.
        /// </summary>
        public TBuilder WhereIn<TProperty>(Expression<Func<T, TProperty>> selector, IEnumerable<TProperty> values)
        {
            Components.WhereClauseBuilder.AddCollection(selector, values, isNegated: false);

            return Current;
        }

        /// <summary>
        /// Adds an IN collection condition for an entity available in the current query scope.
        /// </summary>
        public TBuilder WhereIn<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> selector, IEnumerable<TProperty> values)
        {
            Components.WhereClauseBuilder.AddCollection(selector, values, isNegated: false);

            return Current;
        }

        /// <summary>
        /// Adds a NOT IN collection condition for the root entity.
        /// </summary>
        public TBuilder WhereNotIn<TProperty>(Expression<Func<T, TProperty>> selector, IEnumerable<TProperty> values)
        {
            Components.WhereClauseBuilder.AddCollection(selector, values, isNegated: true);

            return Current;
        }

        /// <summary>
        /// Adds a NOT IN collection condition for an entity available in the current query scope.
        /// </summary>
        public TBuilder WhereNotIn<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> selector, IEnumerable<TProperty> values)
        {
            Components.WhereClauseBuilder.AddCollection(selector, values, isNegated: true);

            return Current;
        }

        /// <summary>
        /// Adds a filtering expression only when the specified condition is true.
        /// </summary>
        public TBuilder WhereIf(bool condition, Expression<Func<T, bool>> predicate)
        {
            Components.WhereClauseBuilder.AddIf(condition, predicate);

            return Current;
        }

        /// <summary>
        /// Adds a conditional WHERE predicate for the root entity using the specified logical operator.
        /// </summary>
        public TBuilder WhereIf(bool condition, Expression<Func<T, bool>> predicate, QueryLogicalOperator logicalOperator)
        {
            Components.WhereClauseBuilder.AddIf(condition, predicate, logicalOperator);

            return Current;
        }

        /// <summary>
        /// Adds a WHERE predicate for an entity available in the current query scope only when the specified condition is true.
        /// </summary>
        public TBuilder WhereIf<TEntity>(bool condition, Expression<Func<TEntity, bool>> predicate)
        {
            Components.WhereClauseBuilder.AddIfForSource(condition, predicate);

            return Current;
        }

        /// <summary>
        /// Adds a conditional WHERE predicate for an entity available in the current query scope using the specified logical operator.
        /// </summary>
        public TBuilder WhereIf<TEntity>(bool condition, Expression<Func<TEntity, bool>> predicate, QueryLogicalOperator logicalOperator)
        {
            Components.WhereClauseBuilder.AddIfForSource(condition, predicate, logicalOperator);

            return Current;
        }

        /// <summary>
        /// Adds a WHERE condition based on a scalar SQL function for an entity available in the current query scope.
        /// </summary>
        public TBuilder WhereScalarFunction<TEntity>(QueryScalarFunction function, Expression<Func<TEntity, object>> selector, QueryComparisonOperator comparisonOperator, object? value)
        {
            Components.WhereClauseBuilder.AddFunction(function, selector, comparisonOperator, value);

            return Current;
        }

        /// <summary>
        /// Adds a WHERE condition based on a computed expression for an entity available in the current query scope.
        /// </summary>
        public TBuilder WhereComputed<TEntity>(Expression<Func<TEntity, bool>> expression)
        {
            Components.WhereClauseBuilder.AddComputed(expression);

            return Current;
        }

        /// <summary>
        /// Adds a WHERE condition based on a computed expression involving two entities available in the current query scope.
        /// </summary>
        public TBuilder WhereComputed<TLeft, TRight>(Expression<Func<TLeft, TRight, bool>> expression)
        {
            Components.WhereClauseBuilder.AddComputed(expression);

            return Current;
        }

        #endregion

        #region GroupBy Overloads

        /// <summary>
        /// Adds a GROUP BY clause for the root entity.
        /// </summary>
        public TBuilder GroupBy(Expression<Func<T, object>> selector)
        {
            Components.GroupByClauseBuilder.Add(selector);

            return Current;
        }

        /// <summary>
        /// Adds a GROUP BY clause for an entity available in the current query scope.
        /// </summary>
        public TBuilder GroupBy<TEntity>(Expression<Func<TEntity, object>> selector)
        {
            Components.GroupByClauseBuilder.Add(selector);

            return Current;
        }

        #endregion

        #region Set Operation Overloads

        /// <summary>
        /// Adds a UNION query to the current query.
        /// </summary>
        public TBuilder Union<TSet>(Func<IQueryBuilder<TProfile>, IQueryCommandBuilder<TSet, TProfile>> setBuilder)
        {
            Components.SetOperationClauseBuilder.Add(QuerySetOperation.Union, setBuilder);

            return Current;
        }

        /// <summary>
        /// Adds a UNION ALL query to the current query.
        /// </summary>
        public TBuilder UnionAll<TSet>(Func<IQueryBuilder<TProfile>, IQueryCommandBuilder<TSet, TProfile>> setBuilder)
        {
            Components.SetOperationClauseBuilder.Add(QuerySetOperation.UnionAll, setBuilder);

            return Current;
        }

        /// <summary>
        /// Adds an INTERSECT query to the current query.
        /// </summary>        
        TBuilder IQueryCompositionCommandBuilder<T, TBuilder, TProfile>.ApplyIntersect<TSet>(Func<IQueryBuilder<TProfile>, IQueryCommandBuilder<TSet, TProfile>> setBuilder)
        {
            Components.SetOperationClauseBuilder.Add(QuerySetOperation.Intersect, setBuilder);
            return Current;
        }

        /// <summary>
        /// Adds an EXCEPT query to the current query.
        /// </summary>
        TBuilder IQueryCompositionCommandBuilder<T, TBuilder, TProfile>.ApplyExcept<TSet>(Func<IQueryBuilder<TProfile>, IQueryCommandBuilder<TSet, TProfile>> setBuilder)
        {
            Components.SetOperationClauseBuilder.Add(QuerySetOperation.Except, setBuilder);
            return Current;
        }

        #endregion

        #region Having Overloads

        /// <summary>
        /// Adds a HAVING condition based on an aggregate expression for an entity available in the current query scope.
        /// </summary>
        public TBuilder HavingAggregate<TEntity>(QueryAggregateFunction function, Expression<Func<TEntity, object>> selector, QueryComparisonOperator comparisonOperator, object? value)
        {
            Components.HavingClauseBuilder.AddAggregate(function, selector, comparisonOperator, value);

            return Current;
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
        public TBuilder OrderBy(Expression<Func<T, object>> keySelector)
        {
            Components.OrderByClauseBuilder.AddAscending(keySelector);

            return Current;
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
        public TBuilder OrderBy<TEntity>(Expression<Func<TEntity, object>> keySelector)
        {
            Components.OrderByClauseBuilder.AddAscendingForSource(keySelector);

            return Current;
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
        public TBuilder OrderByDescending(Expression<Func<T, object>> keySelector)
        {
            Components.OrderByClauseBuilder.AddDescending(keySelector);

            return Current;
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
        public TBuilder OrderByDescending<TEntity>(Expression<Func<TEntity, object>> keySelector)
        {
            Components.OrderByClauseBuilder.AddDescendingForSource(keySelector);

            return Current;
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
        public TBuilder ThenBy(Expression<Func<T, object>> keySelector)
        {
            Components.OrderByClauseBuilder.AddAscending(keySelector);

            return Current;
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
        public TBuilder ThenBy<TEntity>(Expression<Func<TEntity, object>> keySelector)
        {
            Components.OrderByClauseBuilder.AddAscendingForSource(keySelector);

            return Current;
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
        public TBuilder ThenByDescending(Expression<Func<T, object>> keySelector)
        {
            Components.OrderByClauseBuilder.AddDescending(keySelector);

            return Current;
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
        public TBuilder ThenByDescending<TEntity>(Expression<Func<TEntity, object>> keySelector)
        {
            Components.OrderByClauseBuilder.AddDescendingForSource(keySelector);

            return Current;
        }

        #endregion


        /// <inheritdoc />
        TBuilder IQueryCompositionCommandBuilder<T, TBuilder, TProfile>.ApplyFeature(IQueryFeatureOperation operation)
        {
            QueryFeatureOperationDispatcher.Apply(Components, operation);
            return Current;
        }

    }
}
