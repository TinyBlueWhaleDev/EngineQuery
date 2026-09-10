using System.Linq.Expressions;
using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Core.Enums;
using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Context;

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
    /// <typeparam name="TProfile">
    /// Database provider profile associated with the query composition.
    /// </typeparam>
    /// <remarks>
    /// This base class captures query composition behavior shared by command builders
    /// without defining command-specific operations such as ordering, pagination or compilation.
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
        /// <typeparam name="TEntity">
        /// Entity type associated with the projected value.
        /// </typeparam>
        /// <param name="expression">
        /// Expression that selects the value evaluated by the LAG window function.
        /// </param>
        /// <param name="alias">
        /// Alias assigned to the LAG projection.
        /// </param>
        /// <param name="windowBuilder">
        /// Delegate used to configure the window partitioning and ordering.
        /// </param>
        /// <param name="offset">
        /// Number of rows preceding the current row used by the LAG function.
        /// </param>
        /// <returns>
        /// Current query composition builder instance.
        /// </returns>
        TBuilder IQueryCompositionCommandBuilder<T, TBuilder, TProfile>.ApplyLag<TEntity>(Expression<Func<TEntity, object>> expression, string alias, Func<IWindowFunctionBuilder, IWindowFunctionBuilder> windowBuilder, int offset)
        {
            Components.WindowFunctionProjectionBuilder.AddLag(expression, alias, windowBuilder, offset);

            return Current;
        }

        /// <summary>
        /// Adds a LEAD window function projection to the current query.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type associated with the projected value.
        /// </typeparam>
        /// <param name="expression">
        /// Expression that selects the value evaluated by the LEAD window function.
        /// </param>
        /// <param name="alias">
        /// Alias assigned to the LEAD projection.
        /// </param>
        /// <param name="windowBuilder">
        /// Delegate used to configure the window partitioning and ordering.
        /// </param>
        /// <param name="offset">
        /// Number of rows following the current row used by the LEAD function.
        /// </param>
        /// <returns>
        /// Current query composition builder instance.
        /// </returns>
        TBuilder IQueryCompositionCommandBuilder<T, TBuilder, TProfile>.ApplyLead<TEntity>(Expression<Func<TEntity, object>> expression, string alias, Func<IWindowFunctionBuilder, IWindowFunctionBuilder> windowBuilder, int offset)
        {
            Components.WindowFunctionProjectionBuilder.AddLead(expression, alias, windowBuilder, offset);

            return Current;
        }

        /// <summary>
        /// Adds a FIRST_VALUE window function projection to the current query.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type associated with the projected value.
        /// </typeparam>
        /// <param name="expression">
        /// Expression that selects the value evaluated by the FIRST_VALUE window function.
        /// </param>
        /// <param name="alias">
        /// Alias assigned to the FIRST_VALUE projection.
        /// </param>
        /// <param name="windowBuilder">
        /// Delegate used to configure the window partitioning and ordering.
        /// </param>
        /// <returns>
        /// Current query composition builder instance.
        /// </returns>
        TBuilder IQueryCompositionCommandBuilder<T, TBuilder, TProfile>.ApplyFirstValue<TEntity>(Expression<Func<TEntity, object>> expression, string alias, Func<IWindowFunctionBuilder, IWindowFunctionBuilder> windowBuilder)
        {
            Components.WindowFunctionProjectionBuilder.AddFirstValue(expression, alias, windowBuilder);

            return Current;
        }

        /// <summary>
        /// Adds a LAST_VALUE window function projection to the current query.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type associated with the projected value.
        /// </typeparam>
        /// <param name="expression">
        /// Expression that selects the value evaluated by the LAST_VALUE window function.
        /// </param>
        /// <param name="alias">
        /// Alias assigned to the LAST_VALUE projection.
        /// </param>
        /// <param name="windowBuilder">
        /// Delegate used to configure the window partitioning and ordering.
        /// </param>
        /// <returns>
        /// Current query composition builder instance.
        /// </returns>
        TBuilder IQueryCompositionCommandBuilder<T, TBuilder, TProfile>.ApplyLastValue<TEntity>(Expression<Func<TEntity, object>> expression, string alias, Func<IWindowFunctionBuilder, IWindowFunctionBuilder> windowBuilder)
        {
            Components.WindowFunctionProjectionBuilder.AddLastValue(expression, alias, windowBuilder);

            return Current;
        }

        /// <summary>
        /// Adds an NTILE window function projection to the current query.
        /// </summary>
        /// <param name="buckets">
        /// Number of buckets used to distribute rows.
        /// </param>
        /// <param name="alias">
        /// Alias assigned to the NTILE projection.
        /// </param>
        /// <param name="windowBuilder">
        /// Delegate used to configure the window partitioning and ordering.
        /// </param>
        /// <returns>
        /// Current query composition builder instance.
        /// </returns>
        TBuilder IQueryCompositionCommandBuilder<T, TBuilder, TProfile>.ApplyNtile(int buckets, string alias, Func<IWindowFunctionBuilder, IWindowFunctionBuilder> windowBuilder)
        {
            Components.WindowFunctionProjectionBuilder.AddNtile(buckets, alias, windowBuilder);

            return Current;
        }

        /// <summary>
        /// Adds a ROW_NUMBER window function projection to the current query.
        /// </summary>
        /// <param name="alias">
        /// Alias assigned to the ROW_NUMBER projection.
        /// </param>
        /// <param name="windowBuilder">
        /// Delegate used to configure the window partitioning and ordering.
        /// </param>
        /// <returns>
        /// Current query composition builder instance.
        /// </returns>
        TBuilder IQueryCompositionCommandBuilder<T, TBuilder, TProfile>.ApplyRowNumber(string alias, Func<IWindowFunctionBuilder, IWindowFunctionBuilder> windowBuilder)
        {
            return AddRankingFunction(QueryWindowFunction.RowNumber, alias, windowBuilder);
        }

        /// <summary>
        /// Adds a RANK window function projection to the current query.
        /// </summary>
        /// <param name="alias">
        /// Alias assigned to the RANK projection.
        /// </param>
        /// <param name="windowBuilder">
        /// Delegate used to configure the window partitioning and ordering.
        /// </param>
        /// <returns>
        /// Current query composition builder instance.
        /// </returns>
        TBuilder IQueryCompositionCommandBuilder<T, TBuilder, TProfile>.ApplyRank(string alias, Func<IWindowFunctionBuilder, IWindowFunctionBuilder> windowBuilder)
        {
            return AddRankingFunction(QueryWindowFunction.Rank, alias, windowBuilder);
        }

        /// <summary>
        /// Adds a DENSE_RANK window function projection to the current query.
        /// </summary>
        /// <param name="alias">
        /// Alias assigned to the DENSE_RANK projection.
        /// </param>
        /// <param name="windowBuilder">
        /// Delegate used to configure the window partitioning and ordering.
        /// </param>
        /// <returns>
        /// Current query composition builder instance.
        /// </returns>
        TBuilder IQueryCompositionCommandBuilder<T, TBuilder, TProfile>.ApplyDenseRank(string alias, Func<IWindowFunctionBuilder, IWindowFunctionBuilder> windowBuilder)
        {
            return AddRankingFunction(QueryWindowFunction.DenseRank, alias, windowBuilder);
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
        /// Expression used to generate the computed projection.
        /// </param>
        /// <param name="alias">
        /// Alias assigned to the computed projection.
        /// </param>
        /// <returns>
        /// Current query composition builder instance.
        /// </returns>
        public TBuilder SelectComputed<TEntity>(Expression<Func<TEntity, object>> expression, string alias)
        {
            Components.ComputedProjectionBuilder.Add(expression, alias);

            return Current;
        }

        #endregion

        #region Projection Overloads

        /// <summary>
        /// Adds an aggregate SELECT expression for an entity available in the current query scope.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type associated with the aggregate expression.
        /// </typeparam>
        /// <param name="function">
        /// Aggregate function applied to the selected expression.
        /// </param>
        /// <param name="selector">
        /// Expression evaluated by the aggregate function.
        /// </param>
        /// <param name="alias">
        /// Alias assigned to the aggregate projection.
        /// </param>
        /// <returns>
        /// Current query composition builder instance.
        /// </returns>
        public TBuilder SelectAggregate<TEntity>(QueryAggregateFunction function, Expression<Func<TEntity, object>> selector, string alias)
        {
            Components.AggregateProjectionBuilder.Add(function, selector, alias);

            return Current;
        }

        /// <summary>
        /// Adds a scalar SQL function projection for an entity available in the current query scope.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type associated with the scalar function projection.
        /// </typeparam>
        /// <param name="function">
        /// Scalar SQL function applied to the selected expression.
        /// </param>
        /// <param name="selector">
        /// Expression that selects the function input value.
        /// </param>
        /// <param name="alias">
        /// Alias assigned to the scalar function projection.
        /// </param>
        /// <returns>
        /// Current query composition builder instance.
        /// </returns>
        public TBuilder SelectScalarFunction<TEntity>(QueryScalarFunction function, Expression<Func<TEntity, object>> selector, string alias)
        {
            Components.ScalarFunctionProjectionBuilder.Add(function, selector, alias);

            return Current;
        }

        /// <summary>
        /// Adds a scalar SQL function projection using multiple function arguments for an entity available in the current query scope.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type associated with the scalar function projection.
        /// </typeparam>
        /// <param name="function">
        /// Scalar SQL function applied to the supplied arguments.
        /// </param>
        /// <param name="argumentsSelector">
        /// Expression that supplies the scalar function arguments.
        /// </param>
        /// <param name="alias">
        /// Alias assigned to the scalar function projection.
        /// </param>
        /// <returns>
        /// Current query composition builder instance.
        /// </returns>
        public TBuilder SelectScalarFunction<TEntity>(QueryScalarFunction function, Expression<Func<TEntity, object[]>> argumentsSelector, string alias)
        {
            Components.ScalarFunctionProjectionBuilder.Add(function, argumentsSelector, alias);

            return Current;
        }

        /// <summary>
        /// Adds a CASE WHEN SELECT expression for an entity available in the current query scope.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type associated with the CASE WHEN condition.
        /// </typeparam>
        /// <param name="condition">
        /// Predicate expression evaluated by the CASE WHEN expression.
        /// </param>
        /// <param name="whenTrue">
        /// Value returned when the condition evaluates to true.
        /// </param>
        /// <param name="whenFalse">
        /// Value returned when the condition evaluates to false.
        /// </param>
        /// <param name="alias">
        /// Alias assigned to the CASE WHEN projection.
        /// </param>
        /// <returns>
        /// Current query composition builder instance.
        /// </returns>
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
        /// <typeparam name="TSubquery">
        /// Entity type associated with the subquery source.
        /// </typeparam>
        /// <param name="subqueryBuilder">
        /// Delegate used to configure the EXISTS subquery.
        /// </param>
        /// <returns>
        /// Current query composition builder instance.
        /// </returns>
        public TBuilder WhereExists<TSubquery>(Func<IQueryBuilder<TProfile>, IQueryCommandBuilder<TSubquery, TProfile>> subqueryBuilder)
        {
            Components.ExistsClauseBuilder.Add(subqueryBuilder);

            return Current;
        }

        /// <summary>
        /// Adds a correlated EXISTS subquery condition using an outer entity available in the current query scope.
        /// </summary>
        /// <typeparam name="TOuter">
        /// Entity type associated with the outer query source.
        /// </typeparam>
        /// <typeparam name="TSubquery">
        /// Entity type associated with the subquery source.
        /// </typeparam>
        /// <param name="alias">
        /// Optional alias assigned to the subquery source.
        /// </param>
        /// <param name="subqueryBuilder">
        /// Delegate used to configure the correlated EXISTS subquery.
        /// </param>
        /// <returns>
        /// Current query composition builder instance.
        /// </returns>
        public TBuilder WhereExists<TOuter, TSubquery>(string? alias, Func<IQueryCommandBuilder<TSubquery, TProfile>, IQueryCommandBuilder<TSubquery, TProfile>> subqueryBuilder)
        {
            Components.ExistsClauseBuilder.AddCorrelated<TOuter, TSubquery>(alias, subqueryBuilder, isNegated: false);

            return Current;
        }

        /// <summary>
        /// Adds an IN subquery condition for an entity available in the current query scope.
        /// </summary>
        /// <typeparam name="TOuter">
        /// Entity type associated with the outer query source.
        /// </typeparam>
        /// <typeparam name="TSubquery">
        /// Entity type associated with the subquery source.
        /// </typeparam>
        /// <param name="outerSelector">
        /// Expression selecting the outer value compared against the subquery.
        /// </param>
        /// <param name="alias">
        /// Optional alias assigned to the subquery source.
        /// </param>
        /// <param name="subqueryBuilder">
        /// Delegate used to configure the IN subquery.
        /// </param>
        /// <returns>
        /// Current query composition builder instance.
        /// </returns>
        public TBuilder WhereIn<TOuter, TSubquery>(Expression<Func<TOuter, object>> outerSelector, string? alias, Func<IQueryCommandBuilder<TSubquery, TProfile>, IQueryCommandBuilder<TSubquery, TProfile>> subqueryBuilder)
        {
            Components.InSubqueryClauseBuilder.Add(outerSelector, alias, subqueryBuilder);

            return Current;
        }

        /// <summary>
        /// Adds a correlated NOT EXISTS subquery condition using an outer entity available in the current query scope.
        /// </summary>
        /// <typeparam name="TOuter">
        /// Entity type associated with the outer query source.
        /// </typeparam>
        /// <typeparam name="TSubquery">
        /// Entity type associated with the subquery source.
        /// </typeparam>
        /// <param name="alias">
        /// Optional alias assigned to the subquery source.
        /// </param>
        /// <param name="subqueryBuilder">
        /// Delegate used to configure the correlated NOT EXISTS subquery.
        /// </param>
        /// <returns>
        /// Current query composition builder instance.
        /// </returns>
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
        /// <typeparam name="TSource">
        /// Entity type associated with the existing query source.
        /// </typeparam>
        /// <typeparam name="TJoin">
        /// Entity type associated with the joined source.
        /// </typeparam>
        /// <param name="alias">
        /// Optional alias assigned to the joined source.
        /// </param>
        /// <param name="on">
        /// Expression used to define the JOIN predicate.
        /// </param>
        /// <returns>
        /// Current query composition builder instance.
        /// </returns>
        public TBuilder InnerJoin<TSource, TJoin>(string? alias, Expression<Func<TSource, TJoin, bool>> on)
        {
            Components.JoinClauseBuilder.Add(QueryJoinType.Inner, alias, on);

            return Current;
        }

        /// <summary>
        /// Adds a LEFT JOIN using resolved metadata for the joined entity.
        /// </summary>
        /// <typeparam name="TSource">
        /// Entity type associated with the existing query source.
        /// </typeparam>
        /// <typeparam name="TJoin">
        /// Entity type associated with the joined source.
        /// </typeparam>
        /// <param name="alias">
        /// Optional alias assigned to the joined source.
        /// </param>
        /// <param name="on">
        /// Expression used to define the JOIN predicate.
        /// </param>
        /// <returns>
        /// Current query composition builder instance.
        /// </returns>
        public TBuilder LeftJoin<TSource, TJoin>(string? alias, Expression<Func<TSource, TJoin, bool>> on)
        {
            Components.JoinClauseBuilder.Add(QueryJoinType.Left, alias, on);

            return Current;
        }

        /// <summary>
        /// Adds an INNER JOIN using an explicit joined table name.
        /// </summary>
        /// <typeparam name="TSource">
        /// Entity type associated with the existing query source.
        /// </typeparam>
        /// <typeparam name="TJoin">
        /// Entity type associated with the joined table.
        /// </typeparam>
        /// <param name="tableName">
        /// Physical table name associated with the joined source.
        /// </param>
        /// <param name="schemaName">
        /// Optional database schema associated with the joined table.
        /// </param>
        /// <param name="alias">
        /// Optional alias assigned to the joined table.
        /// </param>
        /// <param name="on">
        /// Expression used to define the JOIN predicate.
        /// </param>
        /// <returns>
        /// Current query composition builder instance.
        /// </returns>
        public TBuilder InnerJoinTable<TSource, TJoin>(string tableName, string? schemaName, string? alias, Expression<Func<TSource, TJoin, bool>> on)
        {
            Components.JoinClauseBuilder.AddTable(QueryJoinType.Inner, tableName, schemaName, alias, on);

            return Current;
        }

        /// <summary>
        /// Adds a LEFT JOIN using an explicit joined table name.
        /// </summary>
        /// <typeparam name="TSource">
        /// Entity type associated with the existing query source.
        /// </typeparam>
        /// <typeparam name="TJoin">
        /// Entity type associated with the joined table.
        /// </typeparam>
        /// <param name="tableName">
        /// Physical table name associated with the joined source.
        /// </param>
        /// <param name="schemaName">
        /// Optional database schema associated with the joined table.
        /// </param>
        /// <param name="alias">
        /// Optional alias assigned to the joined table.
        /// </param>
        /// <param name="on">
        /// Expression used to define the JOIN predicate.
        /// </param>
        /// <returns>
        /// Current query composition builder instance.
        /// </returns>
        public TBuilder LeftJoinTable<TSource, TJoin>(string tableName, string? schemaName, string? alias, Expression<Func<TSource, TJoin, bool>> on)
        {
            Components.JoinClauseBuilder.AddTable(QueryJoinType.Left, tableName, schemaName, alias, on);

            return Current;
        }

        /// <summary>
        /// Adds a CROSS APPLY or provider-equivalent LATERAL subquery join to the current query.
        /// </summary>
        /// <typeparam name="TApply">
        /// Entity type associated with the APPLY subquery source.
        /// </typeparam>
        /// <param name="alias">
        /// Alias assigned to the APPLY result.
        /// </param>
        /// <param name="applyBuilder">
        /// Delegate used to configure the APPLY subquery.
        /// </param>
        /// <returns>
        /// Current query composition builder instance.
        /// </returns>
        TBuilder IQueryCompositionCommandBuilder<T, TBuilder, TProfile>.ApplyCrossApply<TApply>(string alias, Func<IQueryCommandBuilder<TApply, TProfile>, IQueryCommandBuilder<TApply, TProfile>> applyBuilder)
        {
            Components.ApplyClauseBuilder.Add<T, TApply>(QueryApplyType.Cross, alias, applyBuilder);
            return Current;
        }

        /// <summary>
        /// Adds an OUTER APPLY or provider-equivalent LEFT LATERAL subquery join to the current query.
        /// </summary>
        /// <typeparam name="TApply">
        /// Entity type associated with the APPLY subquery source.
        /// </typeparam>
        /// <param name="alias">
        /// Alias assigned to the APPLY result.
        /// </param>
        /// <param name="applyBuilder">
        /// Delegate used to configure the APPLY subquery.
        /// </param>
        /// <returns>
        /// Current query composition builder instance.
        /// </returns>
        TBuilder IQueryCompositionCommandBuilder<T, TBuilder, TProfile>.ApplyOuterApply<TApply>(string alias, Func<IQueryCommandBuilder<TApply, TProfile>, IQueryCommandBuilder<TApply, TProfile>> applyBuilder)
        {
            Components.ApplyClauseBuilder.Add<T, TApply>(QueryApplyType.Outer, alias, applyBuilder);
            return Current;
        }
        #endregion

        #region Where Overloads

        /// <summary>
        /// Adds a WHERE predicate for the root entity.
        /// </summary>
        /// <param name="predicate">
        /// Predicate expression added to the WHERE clause.
        /// </param>
        /// <returns>
        /// Current query composition builder instance.
        /// </returns>
        public TBuilder Where(Expression<Func<T, bool>> predicate)
        {
            Components.WhereClauseBuilder.Add(predicate);

            return Current;
        }

        /// <summary>
        /// Adds a WHERE predicate for the root entity using the specified logical operator.
        /// </summary>
        /// <param name="predicate">
        /// Predicate expression added to the WHERE clause.
        /// </param>
        /// <param name="logicalOperator">
        /// Logical operator used to combine the predicate with the previous condition.
        /// </param>
        /// <returns>
        /// Current query composition builder instance.
        /// </returns>
        public TBuilder Where(Expression<Func<T, bool>> predicate, QueryLogicalOperator logicalOperator)
        {
            Components.WhereClauseBuilder.Add(predicate, logicalOperator);

            return Current;
        }

        /// <summary>
        /// Adds a WHERE predicate for an entity already available in the current query scope.
        /// </summary>
        /// <typeparam name="TSource">
        /// Entity type associated with the predicate.
        /// </typeparam>
        /// <param name="predicate">
        /// Predicate expression added to the WHERE clause.
        /// </param>
        /// <returns>
        /// Current query composition builder instance.
        /// </returns>
        public TBuilder Where<TSource>(Expression<Func<TSource, bool>> predicate)
        {
            Components.WhereClauseBuilder.Add(predicate);

            return Current;
        }

        /// <summary>
        /// Adds a WHERE predicate for an entity already available in the current query scope using the specified logical operator.
        /// </summary>
        /// <typeparam name="TSource">
        /// Entity type associated with the predicate.
        /// </typeparam>
        /// <param name="predicate">
        /// Predicate expression added to the WHERE clause.
        /// </param>
        /// <param name="logicalOperator">
        /// Logical operator used to combine the predicate with the previous condition.
        /// </param>
        /// <returns>
        /// Current query composition builder instance.
        /// </returns>
        public TBuilder Where<TSource>(Expression<Func<TSource, bool>> predicate, QueryLogicalOperator logicalOperator)
        {
            Components.WhereClauseBuilder.Add(predicate, logicalOperator);

            return Current;
        }

        /// <summary>
        /// Adds an IN collection condition for the root entity.
        /// </summary>
        /// <typeparam name="TProperty">
        /// Type of the selected property and collection elements.
        /// </typeparam>
        /// <param name="selector">
        /// Expression that selects the property evaluated by the IN condition.
        /// </param>
        /// <param name="values">
        /// Values evaluated by the IN condition.
        /// </param>
        /// <returns>
        /// Current query composition builder instance.
        /// </returns>
        public TBuilder WhereIn<TProperty>(Expression<Func<T, TProperty>> selector, IEnumerable<TProperty> values)
        {
            Components.WhereClauseBuilder.AddCollection(selector, values, isNegated: false);

            return Current;
        }

        /// <summary>
        /// Adds an IN collection condition for an entity available in the current query scope.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type associated with the selected property.
        /// </typeparam>
        /// <typeparam name="TProperty">
        /// Type of the selected property and collection elements.
        /// </typeparam>
        /// <param name="selector">
        /// Expression that selects the property evaluated by the IN condition.
        /// </param>
        /// <param name="values">
        /// Values evaluated by the IN condition.
        /// </param>
        /// <returns>
        /// Current query composition builder instance.
        /// </returns>
        public TBuilder WhereIn<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> selector, IEnumerable<TProperty> values)
        {
            Components.WhereClauseBuilder.AddCollection(selector, values, isNegated: false);

            return Current;
        }

        /// <summary>
        /// Adds a NOT IN collection condition for the root entity.
        /// </summary>
        /// <typeparam name="TProperty">
        /// Type of the selected property and collection elements.
        /// </typeparam>
        /// <param name="selector">
        /// Expression that selects the property evaluated by the NOT IN condition.
        /// </param>
        /// <param name="values">
        /// Values evaluated by the NOT IN condition.
        /// </param>
        /// <returns>
        /// Current query composition builder instance.
        /// </returns>
        public TBuilder WhereNotIn<TProperty>(Expression<Func<T, TProperty>> selector, IEnumerable<TProperty> values)
        {
            Components.WhereClauseBuilder.AddCollection(selector, values, isNegated: true);

            return Current;
        }

        /// <summary>
        /// Adds a NOT IN collection condition for an entity available in the current query scope.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type associated with the selected property.
        /// </typeparam>
        /// <typeparam name="TProperty">
        /// Type of the selected property and collection elements.
        /// </typeparam>
        /// <param name="selector">
        /// Expression that selects the property evaluated by the NOT IN condition.
        /// </param>
        /// <param name="values">
        /// Values evaluated by the NOT IN condition.
        /// </param>
        /// <returns>
        /// Current query composition builder instance.
        /// </returns>
        public TBuilder WhereNotIn<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> selector, IEnumerable<TProperty> values)
        {
            Components.WhereClauseBuilder.AddCollection(selector, values, isNegated: true);

            return Current;
        }

        /// <summary>
        /// Adds a filtering expression only when the specified condition is true.
        /// </summary>
        /// <param name="condition">
        /// Indicates whether the predicate should be added.
        /// </param>
        /// <param name="predicate">
        /// Predicate expression added when the condition is true.
        /// </param>
        /// <returns>
        /// Current query composition builder instance.
        /// </returns>
        public TBuilder WhereIf(bool condition, Expression<Func<T, bool>> predicate)
        {
            Components.WhereClauseBuilder.AddIf(condition, predicate);

            return Current;
        }

        /// <summary>
        /// Adds a conditional WHERE predicate for the root entity using the specified logical operator.
        /// </summary>
        /// <param name="condition">
        /// Indicates whether the predicate should be added.
        /// </param>
        /// <param name="predicate">
        /// Predicate expression added when the condition is true.
        /// </param>
        /// <param name="logicalOperator">
        /// Logical operator used to combine the predicate with the previous condition.
        /// </param>
        /// <returns>
        /// Current query composition builder instance.
        /// </returns>
        public TBuilder WhereIf(bool condition, Expression<Func<T, bool>> predicate, QueryLogicalOperator logicalOperator)
        {
            Components.WhereClauseBuilder.AddIf(condition, predicate, logicalOperator);

            return Current;
        }

        /// <summary>
        /// Adds a WHERE predicate for an entity available in the current query scope only when the specified condition is true.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type associated with the predicate.
        /// </typeparam>
        /// <param name="condition">
        /// Indicates whether the predicate should be added.
        /// </param>
        /// <param name="predicate">
        /// Predicate expression added when the condition is true.
        /// </param>
        /// <returns>
        /// Current query composition builder instance.
        /// </returns>
        public TBuilder WhereIf<TEntity>(bool condition, Expression<Func<TEntity, bool>> predicate)
        {
            Components.WhereClauseBuilder.AddIfForSource(condition, predicate);

            return Current;
        }

        /// <summary>
        /// Adds a conditional WHERE predicate for an entity available in the current query scope using the specified logical operator.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type associated with the predicate.
        /// </typeparam>
        /// <param name="condition">
        /// Indicates whether the predicate should be added.
        /// </param>
        /// <param name="predicate">
        /// Predicate expression added when the condition is true.
        /// </param>
        /// <param name="logicalOperator">
        /// Logical operator used to combine the predicate with the previous condition.
        /// </param>
        /// <returns>
        /// Current query composition builder instance.
        /// </returns>
        public TBuilder WhereIf<TEntity>(bool condition, Expression<Func<TEntity, bool>> predicate, QueryLogicalOperator logicalOperator)
        {
            Components.WhereClauseBuilder.AddIfForSource(condition, predicate, logicalOperator);

            return Current;
        }

        /// <summary>
        /// Adds a WHERE condition based on a scalar SQL function for an entity available in the current query scope.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type associated with the selected column.
        /// </typeparam>
        /// <param name="function">
        /// Scalar SQL function applied to the selected column.
        /// </param>
        /// <param name="selector">
        /// Expression that selects the target entity property.
        /// </param>
        /// <param name="comparisonOperator">
        /// Comparison operator applied to the scalar function result.
        /// </param>
        /// <param name="value">
        /// Value compared against the scalar function result.
        /// </param>
        /// <returns>
        /// Current query composition builder instance.
        /// </returns>
        public TBuilder WhereScalarFunction<TEntity>(QueryScalarFunction function, Expression<Func<TEntity, object>> selector, QueryComparisonOperator comparisonOperator, object? value)
        {
            Components.WhereClauseBuilder.AddFunction(function, selector, comparisonOperator, value);

            return Current;
        }

        /// <summary>
        /// Adds a WHERE condition based on a computed expression for an entity available in the current query scope.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type associated with the computed expression.
        /// </typeparam>
        /// <param name="expression">
        /// Computed predicate expression added to the query.
        /// </param>
        /// <returns>
        /// Current query composition builder instance.
        /// </returns>
        public TBuilder WhereComputed<TEntity>(Expression<Func<TEntity, bool>> expression)
        {
            Components.WhereClauseBuilder.AddComputed(expression);

            return Current;
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
        /// Computed predicate expression involving both entity sources.
        /// </param>
        /// <returns>
        /// Current query composition builder instance.
        /// </returns>
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
        /// <param name="selector">
        /// Expression that selects the columns included in the GROUP BY clause.
        /// </param>
        /// <returns>
        /// Current query composition builder instance.
        /// </returns>
        public TBuilder GroupBy(Expression<Func<T, object>> selector)
        {
            Components.GroupByClauseBuilder.Add(selector);

            return Current;
        }

        /// <summary>
        /// Adds a GROUP BY clause for an entity available in the current query scope.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type associated with the grouped columns.
        /// </typeparam>
        /// <param name="selector">
        /// Expression that selects the columns included in the GROUP BY clause.
        /// </param>
        /// <returns>
        /// Current query composition builder instance.
        /// </returns>
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
        /// <typeparam name="TSet">
        /// Entity type associated with the query on the right side of the UNION operation.
        /// </typeparam>
        /// <param name="setBuilder">
        /// Delegate used to configure the query on the right side of the UNION operation.
        /// </param>
        /// <returns>
        /// Current query composition builder instance.
        /// </returns>
        public TBuilder Union<TSet>(Func<IQueryBuilder<TProfile>, IQueryCommandBuilder<TSet, TProfile>> setBuilder)
        {
            return AddSetOperation(QuerySetOperation.Union, setBuilder);
        }

        /// <summary>
        /// Adds a UNION ALL query to the current query.
        /// </summary>
        /// <typeparam name="TSet">
        /// Entity type associated with the query on the right side of the UNION ALL operation.
        /// </typeparam>
        /// <param name="setBuilder">
        /// Delegate used to configure the query on the right side of the UNION ALL operation.
        /// </param>
        /// <returns>
        /// Current query composition builder instance.
        /// </returns>
        public TBuilder UnionAll<TSet>(Func<IQueryBuilder<TProfile>, IQueryCommandBuilder<TSet, TProfile>> setBuilder)
        {
            return AddSetOperation(QuerySetOperation.UnionAll, setBuilder);
        }

        /// <summary>
        /// Adds an INTERSECT query to the current query.
        /// </summary>
        /// <typeparam name="TSet">
        /// Entity type associated with the query on the right side of the INTERSECT operation.
        /// </typeparam>
        /// <param name="setBuilder">
        /// Delegate used to configure the query on the right side of the INTERSECT operation.
        /// </param>
        /// <returns>
        /// Current query composition builder instance.
        /// </returns>
        TBuilder IQueryCompositionCommandBuilder<T, TBuilder, TProfile>.ApplyIntersect<TSet>(Func<IQueryBuilder<TProfile>, IQueryCommandBuilder<TSet, TProfile>> setBuilder)
        {
            return AddSetOperation(QuerySetOperation.Intersect, setBuilder);
        }

        /// <summary>
        /// Adds an EXCEPT query to the current query.
        /// </summary>
        /// <typeparam name="TSet">
        /// Entity type associated with the query on the right side of the EXCEPT operation.
        /// </typeparam>
        /// <param name="setBuilder">
        /// Delegate used to configure the query on the right side of the EXCEPT operation.
        /// </param>
        /// <returns>
        /// Current query composition builder instance.
        /// </returns>
        TBuilder IQueryCompositionCommandBuilder<T, TBuilder, TProfile>.ApplyExcept<TSet>(Func<IQueryBuilder<TProfile>, IQueryCommandBuilder<TSet, TProfile>> setBuilder)
        {
            return AddSetOperation(QuerySetOperation.Except, setBuilder);
        }

        #endregion

        #region Having Overloads

        /// <summary>
        /// Adds a HAVING condition based on an aggregate expression for an entity available in the current query scope.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type associated with the aggregate expression.
        /// </typeparam>
        /// <param name="function">
        /// Aggregate function applied to the selected column.
        /// </param>
        /// <param name="selector">
        /// Expression that selects the column used by the aggregate function.
        /// </param>
        /// <param name="comparisonOperator">
        /// Comparison operator applied to the aggregate result.
        /// </param>
        /// <param name="value">
        /// Value compared against the aggregate result.
        /// </param>
        /// <returns>
        /// Current query composition builder instance.
        /// </returns>
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
        /// Current query composition builder instance.
        /// </returns>
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
        /// Current query composition builder instance.
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
        /// Current query composition builder instance.
        /// </returns>
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
        /// Current query composition builder instance.
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
        /// Current query composition builder instance.
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
        /// Current query composition builder instance.
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
        /// Current query composition builder instance.
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
        /// Current query composition builder instance.
        /// </returns>
        public TBuilder ThenByDescending<TEntity>(Expression<Func<TEntity, object>> keySelector)
        {
            Components.OrderByClauseBuilder.AddDescendingForSource(keySelector);

            return Current;
        }

        #endregion

        #region Pagination

        /// <summary>
        /// Applies a row offset to the current query.
        /// </summary>
        /// <param name="count">
        /// Number of rows skipped before returning query results.
        /// </param>
        /// <returns>
        /// Current query composition builder instance.
        /// </returns>
        TBuilder IQueryCompositionCommandBuilder<T, TBuilder, TProfile>.ApplySkip(int count)
        {
            Components.PaginationClauseBuilder.SetSkip(count);
            return Current;
        }

        /// <summary>
        /// Applies a row limit to the current query.
        /// </summary>
        /// <param name="count">
        /// Maximum number of rows returned by the query.
        /// </param>
        /// <returns>
        /// Current query composition builder instance.
        /// </returns>
        TBuilder IQueryCompositionCommandBuilder<T, TBuilder, TProfile>.ApplyTake(int count)
        {
            Components.PaginationClauseBuilder.SetTake(count);
            return Current;
        }

        #endregion


        // Adds a set operation to the current query and returns the fluent builder.
        private TBuilder AddSetOperation<TSet>(QuerySetOperation operation, Func<IQueryBuilder<TProfile>, IQueryCommandBuilder<TSet, TProfile>> setBuilder)
        {
            Components.SetOperationClauseBuilder.Add(operation, setBuilder);

            return Current;
        }

        // Adds a ranking window function projection and returns the fluent builder.
        private TBuilder AddRankingFunction(QueryWindowFunction function, string alias, Func<IWindowFunctionBuilder, IWindowFunctionBuilder> windowBuilder)
        {
            Components.WindowFunctionProjectionBuilder.AddRankingFunction(function, alias, windowBuilder);

            return Current;
        }
    }
}
