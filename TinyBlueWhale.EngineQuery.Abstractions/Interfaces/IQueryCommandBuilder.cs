using System.Linq.Expressions;
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
        /// Adds an ascending ordering expression to the query.
        /// </summary>
        /// <typeparam name="TKey">
        /// Type of the selected ordering property.
        /// </typeparam>
        /// <param name="keySelector">
        /// Expression that selects the property used for ordering.
        /// </param>
        /// <returns>
        /// Ordered query command builder instance.
        /// </returns>
        IOrderedQueryCommandBuilder<T> OrderBy<TKey>(Expression<Func<T, TKey>> keySelector);

        /// <summary>
        /// Adds a descending ordering expression to the query.
        /// </summary>
        /// <typeparam name="TKey">
        /// Type of the selected ordering property.
        /// </typeparam>
        /// <param name="keySelector">
        /// Expression that selects the property used for ordering.
        /// </param>
        /// <returns>
        /// Ordered query command builder instance.
        /// </returns>
        IOrderedQueryCommandBuilder<T> OrderByDescending<TKey>(Expression<Func<T, TKey>> keySelector);

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
        /// Builds the current query definition into SQL command text and parameters.
        /// </summary>
        /// <returns>
        /// Generated SQL query command.
        /// </returns>
        GeneratedSqlQuery Build();
    }
}
