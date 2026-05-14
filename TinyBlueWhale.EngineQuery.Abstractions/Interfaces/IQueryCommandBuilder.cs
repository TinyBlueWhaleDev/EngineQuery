using System.Linq.Expressions;
using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Abstractions.Models;

namespace TinyBlueWhale.EngineQuery.Abstractions.Interfaces
{
    /// <summary>
    /// Defines a fluent contract for composing strongly typed SQL query commands.
    /// </summary>
    /// <typeparam name="T">
    /// Entity type used as the source of the query.
    /// </typeparam>
    public interface IQueryCommandBuilder<T>
    {
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
        IQueryCommandBuilder<T> Select(Expression<Func<T, object>> selector);

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
        public IQueryCommandBuilder<T> Select<TEntity>(Expression<Func<TEntity, object>> selector);

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
        IQueryCommandBuilder<T> SelectAggregate<TEntity>(QueryAggregateFunction function, Expression<Func<TEntity, object>> selector, string alias);

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
        IQueryCommandBuilder<T> SelectFunction<TEntity>(QueryScalarFunction function, Expression<Func<TEntity, object>> selector, string alias);

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
        IQueryCommandBuilder<T> SelectFunction<TEntity>(QueryScalarFunction function, Expression<Func<TEntity, object[]>> argumentsSelector, string alias);

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
        IQueryCommandBuilder<T> InnerJoin<TSource, TJoin>(string? alias, Expression<Func<TSource, TJoin, bool>> on);

        /// <summary>
        /// Adds a LEFT JOIN using resolved metadata for the joined entity.
        /// </summary>
        IQueryCommandBuilder<T> LeftJoin<TSource, TJoin>(string? alias, Expression<Func<TSource, TJoin, bool>> on);

        /// <summary>
        /// Adds an INNER JOIN using an explicit joined table name.
        /// </summary>
        IQueryCommandBuilder<T> InnerJoinTable<TSource, TJoin>(string tableName, string? alias, Expression<Func<TSource, TJoin, bool>> on);

        /// <summary>
        /// Adds a LEFT JOIN using an explicit joined table name.
        /// </summary>
        IQueryCommandBuilder<T> LeftJoinTable<TSource, TJoin>(string tableName, string? alias, Expression<Func<TSource, TJoin, bool>> on);

        /// <summary>
        /// Adds a filtering condition to the query.
        /// </summary>
        /// <param name="predicate">
        /// Expression used to generate the SQL WHERE clause.
        /// </param>
        /// <returns>
        /// Current query command builder instance.
        /// </returns>
        IQueryCommandBuilder<T> Where(Expression<Func<T, bool>> predicate);

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
        IQueryCommandBuilder<T> Where<TEntity>(Expression<Func<TEntity, bool>> predicate);

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
        IQueryCommandBuilder<T> WhereIf(bool condition, Expression<Func<T, bool>> predicate);

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
        IQueryCommandBuilder<T> WhereIf<TEntity>(bool condition,Expression<Func<TEntity, bool>> predicate);

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
        IQueryCommandBuilder<T> WhereFunction<TEntity>(QueryScalarFunction function, Expression<Func<TEntity, object>> selector, QueryComparisonOperator comparisonOperator, object? value);

        /// <summary>
        /// Adds an ascending ordering expression to the query.
        /// </summary>        
        /// <param name="keySelector">
        /// Expression that selects the property used for ordering.
        /// </param>
        /// <returns>
        /// Ordered query command builder instance.
        /// </returns>
        IOrderedQueryCommandBuilder<T> OrderBy(Expression<Func<T, object>> keySelector);

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
        /// Current query command builder instance.
        /// </returns>
        IOrderedQueryCommandBuilder<T> OrderBy<TEntity>(Expression<Func<TEntity, object>> keySelector);

        /// <summary>
        /// Adds a descending ordering expression to the query.
        /// </summary>        
        /// <param name="keySelector">
        /// Expression that selects the property used for ordering.
        /// </param>
        /// <returns>
        /// Ordered query command builder instance.
        /// </returns>
        IOrderedQueryCommandBuilder<T> OrderByDescending(Expression<Func<T, object>> keySelector);

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
        IOrderedQueryCommandBuilder<T> OrderByDescending<TEntity>(Expression<Func<TEntity, object>> selector);

        /// <summary>
        /// Skips the specified number of rows when generating paginated SQL.
        /// </summary>
        /// <param name="count">
        /// Number of rows to skip.
        /// </param>
        /// <returns>
        /// Current query command builder instance.
        /// </returns>
        IQueryCommandBuilder<T> Skip(int count);

        /// <summary>
        /// Limits the number of rows returned by the generated SQL query.
        /// </summary>
        /// <param name="count">
        /// Maximum number of rows to return.
        /// </param>
        /// <returns>
        /// Current query command builder instance.
        /// </returns>
        IQueryCommandBuilder<T> Take(int count);

        /// <summary>
        /// Adds a GROUP BY clause for the root entity.
        /// </summary>
        IQueryCommandBuilder<T> GroupBy(Expression<Func<T, object>> selector);

        /// <summary>
        /// Adds a GROUP BY clause for an entity available in the current query scope.
        /// </summary>
        IQueryCommandBuilder<T> GroupBy<TEntity>(Expression<Func<TEntity, object>> selector);

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
        IQueryCommandBuilder<T> HavingAggregate<TEntity>(QueryAggregateFunction function, Expression<Func<TEntity, object>> selector, QueryComparisonOperator comparisonOperator, object? value);

        /// <summary>
        /// Builds the current query definition into SQL command text and parameters.
        /// </summary>
        /// <returns>
        /// Generated SQL query command.
        /// </returns>
        GeneratedSqlQuery Build();
    }
}
