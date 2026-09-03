using System.Linq.Expressions;
using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Features;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;

namespace TinyBlueWhale.EngineQuery.Abstractions.Interfaces
{
    /// <summary>
    /// Defines reusable SQL query composition capabilities.
    /// </summary>
    /// <typeparam name="T">
    /// Root entity type associated with the query composition.
    /// </typeparam>
    /// <typeparam name="TBuilder">
    /// Fluent builder type returned by query composition operations.
    /// </typeparam>
    public interface IQueryCompositionCommandBuilder<T, TBuilder, TProfile>
        where TProfile : IDatabaseProviderProfile
    {

        /// <summary>
        /// Applies an internal provider feature operation to the current query composition.
        /// </summary>
        /// <param name="operation">
        /// Feature operation to apply.
        /// </param>
        /// <returns>
        /// Current query composition builder instance.
        /// </returns>
        internal TBuilder ApplyFeature(IQueryFeatureOperation operation)
        {
            throw new NotSupportedException($"Query feature operation '{operation.GetType().Name}' is not supported by the current query builder.");
        }

        /// <summary>
        /// Defines a projection for selecting specific properties from the query source.
        /// </summary>
        /// <param name="selector">
        /// Expression used to determine which properties should be included
        /// in the generated SQL SELECT clause.
        /// </param>
        /// <returns>
        /// Current query command builder instance.
        /// </returns>
        TBuilder Select(Expression<Func<T, object>> selector);

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
        TBuilder Select<TEntity>(Expression<Func<TEntity, object>> selector);

        /// <summary>
        /// Applies DISTINCT projection semantics to the query.
        /// </summary>
        /// <returns>
        /// Current query command builder instance.
        /// </returns>
        TBuilder Distinct();

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
        TBuilder SelectAggregate<TEntity>(QueryAggregateFunction function, Expression<Func<TEntity, object>> selector, string alias);

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
        TBuilder SelectScalarFunction<TEntity>(QueryScalarFunction function, Expression<Func<TEntity, object>> selector, string alias);

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
        TBuilder SelectScalarFunction<TEntity>(QueryScalarFunction function, Expression<Func<TEntity, object[]>> argumentsSelector, string alias);

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
        TBuilder SelectComputed<TEntity>(Expression<Func<TEntity, object>> expression, string alias);

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
        TBuilder SelectCaseWhen<TEntity>(Expression<Func<TEntity, bool>> condition, object? whenTrue, object? whenFalse, string alias);

        /// <summary>
        /// Adds a <c>LAG</c> window function projection to the current query.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type containing the target column expression.
        /// </typeparam>
        /// <param name="expression">
        /// Expression that identifies the column used by the <c>LAG</c> function.
        /// </param>
        /// <param name="alias">
        /// Alias assigned to the generated SQL projection.
        /// </param>
        /// <param name="windowBuilder">
        /// Function used to configure the window definition, including
        /// <c>PARTITION BY</c> and <c>ORDER BY</c> clauses.
        /// </param>
        /// <param name="offset">
        /// Number of rows behind the current row to access.
        /// Default value is <c>1</c>.
        /// </param>
        /// <returns>
        /// Current query command builder instance for method chaining.
        /// </returns>
        internal TBuilder ApplyLag<TEntity>(Expression<Func<TEntity, object>> expression, string alias, Func<IWindowFunctionBuilder, IWindowFunctionBuilder> windowBuilder, int offset = 1)
        {
            throw new NotSupportedException("LAG window function projection is not supported by the current query builder.");
        }
        /// <summary>
        /// Adds a <c>LEAD</c> window function projection to the current query.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type containing the target column expression.
        /// </typeparam>
        /// <param name="expression">
        /// Expression that identifies the column used by the <c>LEAD</c> function.
        /// </param>
        /// <param name="alias">
        /// Alias assigned to the generated SQL projection.
        /// </param>
        /// <param name="windowBuilder">
        /// Function used to configure the window definition, including
        /// <c>PARTITION BY</c> and <c>ORDER BY</c> clauses.
        /// </param>
        /// <param name="offset">
        /// Number of rows ahead of the current row to access.
        /// Default value is <c>1</c>.
        /// </param>
        /// <returns>
        /// Current query command builder instance for method chaining.
        /// </returns>      
        internal TBuilder ApplyLead<TEntity>(Expression<Func<TEntity, object>> expression, string alias, Func<IWindowFunctionBuilder, IWindowFunctionBuilder> windowBuilder, int offset = 1)
        {
            throw new NotSupportedException("LEAD window function projection is not supported by the current query builder.");
        }
        /// <summary>
        /// Adds a FIRST_VALUE window function projection to the current query.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type associated with the selected window function value.
        /// </typeparam>
        /// <param name="expression">
        /// Expression that selects the value returned by FIRST_VALUE.
        /// </param>
        /// <param name="alias">
        /// SQL alias assigned to the FIRST_VALUE result.
        /// </param>
        /// <param name="windowBuilder">
        /// Function used to configure the window function clauses.
        /// </param>
        /// <returns>
        /// Current query command builder instance.
        /// </returns>
        internal TBuilder ApplyFirstValue<TEntity>(Expression<Func<TEntity, object>> expression, string alias, Func<IWindowFunctionBuilder, IWindowFunctionBuilder> windowBuilder)
        {
            throw new NotSupportedException("FIRST_VALUE window function projection is not supported by the current query builder.");
        }
        /// <summary>
        /// Adds a LAST_VALUE window function projection to the current query.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type associated with the selected window function value.
        /// </typeparam>
        /// <param name="expression">
        /// Expression that selects the value returned by LAST_VALUE.
        /// </param>
        /// <param name="alias">
        /// SQL alias assigned to the LAST_VALUE result.
        /// </param>
        /// <param name="windowBuilder">
        /// Function used to configure the window function clauses.
        /// </param>
        /// <returns>
        /// Current query command builder instance.
        /// </returns>
        internal TBuilder ApplyLastValue<TEntity>(Expression<Func<TEntity, object>> expression, string alias, Func<IWindowFunctionBuilder, IWindowFunctionBuilder> windowBuilder)
        {
            throw new NotSupportedException("LAST_VALUE window function projection is not supported by the current query builder.");
        }
        /// <summary>
        /// Adds an NTILE window function projection to the current query.
        /// </summary>
        /// <param name="buckets">
        /// Number of ranked groups used by NTILE.
        /// </param>
        /// <param name="alias">
        /// SQL alias assigned to the NTILE result.
        /// </param>
        /// <param name="windowBuilder">
        /// Function used to configure the window function clauses.
        /// </param>
        /// <returns>
        /// Current query command builder instance.
        /// </returns>
        internal TBuilder ApplyNtile(int buckets, string alias, Func<IWindowFunctionBuilder, IWindowFunctionBuilder> windowBuilder)
        {
            throw new NotSupportedException("NTILE window function projection is not supported by the current query builder.");
        }
        /// <summary>
        /// Adds an INNER JOIN using resolved metadata for the joined entity.
        /// </summary>
        /// <typeparam name="TSource">
        /// Source entity type used in the join condition.
        /// </typeparam>
        /// <typeparam name="TJoin">
        /// Joined entity type.
        /// </typeparam>
        /// <param name="alias">
        /// Optional alias assigned to the joined table.
        /// </param>
        /// <param name="on">
        /// Join condition between the source entity and the joined entity.
        /// </param>
        /// <returns>
        /// Current query command builder instance.
        /// </returns>
        TBuilder InnerJoin<TSource, TJoin>(string? alias, Expression<Func<TSource, TJoin, bool>> on);

        /// <summary>
        /// Adds a LEFT JOIN using resolved metadata for the joined entity.
        /// </summary>
        TBuilder LeftJoin<TSource, TJoin>(string? alias, Expression<Func<TSource, TJoin, bool>> on);

        /// <summary>
        /// Adds an INNER JOIN using an explicit joined table name.
        /// </summary>
        TBuilder InnerJoinTable<TSource, TJoin>(string tableName, string? schemaName, string? alias, Expression<Func<TSource, TJoin, bool>> on);

        /// <summary>
        /// Adds a LEFT JOIN using an explicit joined table name.
        /// </summary>
        TBuilder LeftJoinTable<TSource, TJoin>(string tableName, string? schemaName, string? alias, Expression<Func<TSource, TJoin, bool>> on);

        /// <summary>
        /// Adds a CROSS APPLY or provider-equivalent LATERAL subquery join to the current query.
        /// </summary>
        /// <typeparam name="TOuter">
        /// Outer entity type available in the current query scope.
        /// </typeparam>
        /// <typeparam name="TApply">
        /// Root entity type used by the APPLY subquery.
        /// </typeparam>
        /// <param name="alias">
        /// Alias assigned to the APPLY subquery.
        /// </param>
        /// <param name="applyBuilder">
        /// Function used to build the APPLY subquery.
        /// </param>
        /// <returns>
        /// Current query command builder instance.
        /// </returns>
        TBuilder CrossApply<TOuter, TApply>(string alias, Func<IQueryCommandBuilder<TApply, TProfile>, IQueryCommandBuilder<TApply, TProfile>> applyBuilder);

        /// <summary>
        /// Adds an OUTER APPLY or provider-equivalent LEFT LATERAL subquery join to the current query.
        /// </summary>
        /// <typeparam name="TOuter">
        /// Outer entity type available in the current query scope.
        /// </typeparam>
        /// <typeparam name="TApply">
        /// Root entity type used by the APPLY subquery.
        /// </typeparam>
        /// <param name="alias">
        /// Alias assigned to the APPLY subquery.
        /// </param>
        /// <param name="applyBuilder">
        /// Function used to build the APPLY subquery.
        /// </param>
        /// <returns>
        /// Current query command builder instance.
        /// </returns>
        TBuilder OuterApply<TOuter, TApply>(string alias, Func<IQueryCommandBuilder<TApply, TProfile>, IQueryCommandBuilder<TApply, TProfile>> applyBuilder);


        /// <summary>
        /// Adds a filtering condition to the query.
        /// </summary>
        /// <param name="predicate">
        /// Expression used to generate the SQL WHERE clause.
        /// </param>
        /// <returns>
        /// Current query command builder instance.
        /// </returns>
        TBuilder Where(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Adds a filtering condition to the query using the specified
        /// logical operator.
        /// </summary>
        /// <param name="predicate">
        /// Expression used to generate the SQL WHERE clause.
        /// </param>
        /// <param name="logicalOperator">
        /// Logical operator used to connect the WHERE predicate with the preceding predicate.
        /// </param>
        /// <returns>
        /// Current query command builder instance.
        /// </returns>
        TBuilder Where(Expression<Func<T, bool>> predicate, QueryLogicalOperator logicalOperator);

        /// <summary>
        /// Adds a WHERE predicate for an entity already available in the current query scope.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type associated with the filtered columns.
        /// </typeparam>
        /// <param name="predicate">
        /// Predicate expression describing the SQL filter condition.
        /// </param>
        /// <returns>
        /// Current query command builder instance.
        /// </returns>
        TBuilder Where<TEntity>(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Adds a WHERE predicate for an entity already available in the current query scope.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type associated with the filtered columns.
        /// </typeparam>
        /// <param name="predicate">
        /// Predicate expression describing the SQL filter condition.
        /// </param>
        /// <param name="logicalOperator">
        /// Logical operator used to connect the WHERE predicate with the preceding predicate.
        /// </param>
        /// <returns>
        /// Current query command builder instance.
        /// </returns>
        TBuilder Where<TEntity>(Expression<Func<TEntity, bool>> predicate, QueryLogicalOperator logicalOperator);

        /// <summary>
        /// Adds an IN collection condition for the root entity.
        /// </summary>
        /// <typeparam name="TProperty">
        /// Property and collection element type.
        /// </typeparam>
        /// <param name="selector">
        /// Expression that selects the property evaluated by the IN condition.
        /// </param>
        /// <param name="values">
        /// Values evaluated by the IN condition.
        /// </param>
        /// <returns>
        /// Current query command builder instance.
        /// </returns>
        TBuilder WhereIn<TProperty>(Expression<Func<T, TProperty>> selector, IEnumerable<TProperty> values);

        /// <summary>
        /// Adds an IN collection condition for an entity available in the current query scope.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type associated with the selected property.
        /// </typeparam>
        /// <typeparam name="TProperty">
        /// Property and collection element type.
        /// </typeparam>
        /// <param name="selector">
        /// Expression that selects the property evaluated by the IN condition.
        /// </param>
        /// <param name="values">
        /// Values evaluated by the IN condition.
        /// </param>
        /// <returns>
        /// Current query command builder instance.
        /// </returns>
        TBuilder WhereIn<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> selector, IEnumerable<TProperty> values);

        /// <summary>
        /// Adds a NOT IN collection condition for the root entity.
        /// </summary>
        /// <typeparam name="TProperty">
        /// Property and collection element type.
        /// </typeparam>
        /// <param name="selector">
        /// Expression that selects the property evaluated by the NOT IN condition.
        /// </param>
        /// <param name="values">
        /// Values evaluated by the NOT IN condition.
        /// </param>
        /// <returns>
        /// Current query command builder instance.
        /// </returns>
        TBuilder WhereNotIn<TProperty>(Expression<Func<T, TProperty>> selector, IEnumerable<TProperty> values);

        /// <summary>
        /// Adds a NOT IN collection condition for an entity available in the current query scope.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type associated with the selected property.
        /// </typeparam>
        /// <typeparam name="TProperty">
        /// Property and collection element type.
        /// </typeparam>
        /// <param name="selector">
        /// Expression that selects the property evaluated by the NOT IN condition.
        /// </param>
        /// <param name="values">
        /// Values evaluated by the NOT IN condition.
        /// </param>
        /// <returns>
        /// Current query command builder instance.
        /// </returns>
        TBuilder WhereNotIn<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> selector, IEnumerable<TProperty> values);


        /// <summary>
        /// Adds a filtering condition only when the specified condition is true.
        /// </summary>
        /// <param name="condition">
        /// Determines whether the predicate should be applied.
        /// </param>
        /// <param name="predicate">
        /// Expression used to generate the SQL WHERE clause when enabled.
        /// </param>
        /// <returns>
        /// Current query command builder instance.
        /// </returns>
        TBuilder WhereIf(bool condition, Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Adds a filtering condition only when the specified condition is true.
        /// </summary>
        /// <param name="condition">
        /// Determines whether the predicate should be applied.
        /// </param>
        /// <param name="predicate">
        /// Expression used to generate the SQL WHERE clause when enabled.
        /// </param>
        /// <param name="logicalOperator">
        /// Logical operator used to connect the WHERE predicate with the preceding predicate.
        /// </param>
        /// <returns>
        /// Current query command builder instance.
        /// </returns>
        TBuilder WhereIf(bool condition, Expression<Func<T, bool>> predicate, QueryLogicalOperator logicalOperator);

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
        TBuilder WhereIf<TEntity>(bool condition, Expression<Func<TEntity, bool>> predicate);

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
        /// <param name="logicalOperator">
        /// Logical operator used to connect the WHERE predicate with the preceding predicate.
        /// </param>
        /// <returns>
        /// Current query command builder instance.
        /// </returns>
        TBuilder WhereIf<TEntity>(bool condition, Expression<Func<TEntity, bool>> predicate, QueryLogicalOperator logicalOperator);

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
        TBuilder WhereScalarFunction<TEntity>(QueryScalarFunction function, Expression<Func<TEntity, object>> selector, QueryComparisonOperator comparisonOperator, object? value);

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
        TBuilder WhereComputed<TEntity>(Expression<Func<TEntity, bool>> expression);


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
        TBuilder WhereComputed<TLeft, TRight>(Expression<Func<TLeft, TRight, bool>> expression);

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
        TBuilder WhereExists<TSubquery>(Func<IQueryBuilder<TProfile>, IQueryCommandBuilder<TSubquery, TProfile>> subqueryBuilder);

        /// <summary>
        /// Adds a correlated EXISTS subquery condition using an outer entity available in the current query scope.
        /// </summary>
        TBuilder WhereExists<TOuter, TSubquery>(string? alias, Func<IQueryCommandBuilder<TSubquery, TProfile>, IQueryCommandBuilder<TSubquery, TProfile>> subqueryBuilder);

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
        TBuilder WhereIn<TOuter, TSubquery>(Expression<Func<TOuter, object>> outerSelector, string? alias, Func<IQueryCommandBuilder<TSubquery, TProfile>, IQueryCommandBuilder<TSubquery, TProfile>> subqueryBuilder);

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
        TBuilder WhereNotExists<TOuter, TSubquery>(string? alias, Func<IQueryCommandBuilder<TSubquery, TProfile>, IQueryCommandBuilder<TSubquery, TProfile>> subqueryBuilder);

        /// <summary>
        /// Adds a UNION query to the current query.
        /// </summary>
        /// <typeparam name="TSet">
        /// Root entity type used by the set operation query.
        /// </typeparam>
        /// <param name="setBuilder">
        /// Function used to build the UNION query.
        /// </param>
        /// <returns>
        /// Current query command builder instance.
        /// </returns>
        TBuilder Union<TSet>(Func<IQueryBuilder<TProfile>, IQueryCommandBuilder<TSet, TProfile>> setBuilder);

        /// <summary>
        /// Adds a UNION ALL query to the current query.
        /// </summary>
        /// <typeparam name="TSet">
        /// Root entity type used by the set operation query.
        /// </typeparam>
        /// <param name="setBuilder">
        /// Function used to build the UNION ALL query.
        /// </param>
        /// <returns>
        /// Current query command builder instance.
        /// </returns>
        TBuilder UnionAll<TSet>(Func<IQueryBuilder<TProfile>, IQueryCommandBuilder<TSet, TProfile>> setBuilder);

        /// <summary>
        /// Adds an INTERSECT query to the current query.
        /// </summary>
        /// <typeparam name="TSet">
        /// Root entity type used by the set operation query.
        /// </typeparam>
        /// <param name="setBuilder">
        /// Function used to build the INTERSECT query.
        /// </param>
        /// <returns>
        /// Current query command builder instance.
        /// </returns>
        TBuilder Intersect<TSet>(Func<IQueryBuilder<TProfile>, IQueryCommandBuilder<TSet, TProfile>> setBuilder);

        /// <summary>
        /// Adds an EXCEPT query to the current query.
        /// </summary>
        /// <typeparam name="TSet">
        /// Root entity type used by the set operation query.
        /// </typeparam>
        /// <param name="setBuilder">
        /// Function used to build the EXCEPT query.
        /// </param>
        /// <returns>
        /// Current query command builder instance.
        /// </returns>
        TBuilder Except<TSet>(Func<IQueryBuilder<TProfile>, IQueryCommandBuilder<TSet, TProfile>> setBuilder);

        /// <summary>
        /// Adds a GROUP BY clause for the root entity.
        /// </summary>
        TBuilder GroupBy(Expression<Func<T, object>> selector);

        /// <summary>
        /// Adds a GROUP BY clause for an entity available in the current query scope.
        /// </summary>
        TBuilder GroupBy<TEntity>(Expression<Func<TEntity, object>> selector);

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
        TBuilder HavingAggregate<TEntity>(QueryAggregateFunction function, Expression<Func<TEntity, object>> selector, QueryComparisonOperator comparisonOperator, object? value);

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
        internal TBuilder ApplyRowNumber(string alias, Func<IWindowFunctionBuilder, IWindowFunctionBuilder> windowBuilder)
        {
            throw new NotSupportedException("ROW_NUMBER window function projection is not supported by the current query builder.");
        }
        /// <summary>
        /// Adds a RANK window function projection to the current query.
        /// </summary>
        /// <param name="alias">
        /// SQL alias assigned to the RANK result.
        /// </param>
        /// <param name="windowBuilder">
        /// Function used to configure the window function clauses.
        /// </param>
        /// <returns>
        /// Current query command builder instance.
        /// </returns>
        internal TBuilder ApplyRank(string alias, Func<IWindowFunctionBuilder, IWindowFunctionBuilder> windowBuilder)
        {
            throw new NotSupportedException("RANK window function projection is not supported by the current query builder.");
        }

        /// <summary>
        /// Adds a DENSE_RANK window function projection to the current query.
        /// </summary>
        /// <param name="alias">
        /// SQL alias assigned to the DENSE_RANK result.
        /// </param>
        /// <param name="windowBuilder">
        /// Function used to configure the window function clauses.
        /// </param>
        /// <returns>
        /// Current query command builder instance.
        /// </returns>
        internal TBuilder ApplyDenseRank(string alias, Func<IWindowFunctionBuilder, IWindowFunctionBuilder> windowBuilder)
        {
            throw new NotSupportedException("DENSE_RANK window function projection is not supported by the current query builder.");
        }
        /// <summary>
        /// Adds an ascending ordering expression to the query composition.
        /// </summary>
        /// <param name="keySelector">
        /// Expression that selects the property used for ordering.
        /// </param>
        /// <returns>
        /// Current query composition builder instance.
        /// </returns>
        TBuilder OrderBy(Expression<Func<T, object>> keySelector);

        /// <summary>
        /// Adds an ascending ORDER BY clause for an entity already available in the current query scope.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type associated with the ordered column.
        /// </typeparam>
        /// <param name="keySelector">
        /// Expression describing the ordered property.
        /// </param>
        /// <returns>
        /// Current query composition builder instance.
        /// </returns>
        TBuilder OrderBy<TEntity>(Expression<Func<TEntity, object>> keySelector);

        /// <summary>
        /// Adds a descending ordering expression to the query.
        /// </summary>        
        /// <param name="keySelector">
        /// Expression that selects the property used for ordering.
        /// </param>
        /// <returns>
        /// Ordered query command builder instance.
        /// </returns>
        TBuilder OrderByDescending(Expression<Func<T, object>> keySelector);

        /// <summary>
        /// Adds a descending ORDER BY clause for an entity already available in the current query scope.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type associated with the ordered column.
        /// </typeparam>
        /// <param name="selector">
        /// Expression describing the ordered property.
        /// </param>
        /// <returns>
        /// Current query command builder instance.
        /// </returns>
        TBuilder OrderByDescending<TEntity>(Expression<Func<TEntity, object>> selector);

        /// <summary>
        /// Adds an additional ascending ordering expression for the root entity.
        /// </summary>        
        /// <param name="keySelector">
        /// Expression that selects the property used for ordering.
        /// </param>
        /// <returns>
        /// Current ordered query command builder instance.
        /// </returns>
        TBuilder ThenBy(Expression<Func<T, object>> keySelector);

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
        TBuilder ThenBy<TEntity>(Expression<Func<TEntity, object>> keySelector);

        /// <summary>
        /// Adds an additional descending ordering expression for the root entity.
        /// </summary>      
        /// <param name="keySelector">
        /// Expression that selects the property used for ordering.
        /// </param>
        /// <returns>
        /// Current ordered query command builder instance.
        /// </returns>
        TBuilder ThenByDescending(Expression<Func<T, object>> keySelector);

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
        TBuilder ThenByDescending<TEntity>(Expression<Func<TEntity, object>> keySelector);
    }
}
