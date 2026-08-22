using System.Linq.Expressions;

namespace TinyBlueWhale.EngineQuery.Abstractions.Interfaces
{
    /// <summary>
    /// Represents a query command builder with ordering support.
    /// </summary>
    /// <typeparam name="T">
    /// Root entity type associated with the query.
    /// </typeparam>
    public interface IOrderedQueryCommandBuilder<T> : IQueryCommandBuilder<T>
    {
        /// <summary>
        /// Adds an additional ascending ordering expression for the root entity.
        /// </summary>        
        /// <param name="keySelector">
        /// Expression that selects the property used for ordering.
        /// </param>
        /// <returns>
        /// Current ordered query command builder instance.
        /// </returns>
        IOrderedQueryCommandBuilder<T> ThenBy(Expression<Func<T, object>> keySelector);

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
        IOrderedQueryCommandBuilder<T> ThenBy<TEntity>(Expression<Func<TEntity, object>> keySelector);

        /// <summary>
        /// Adds an additional descending ordering expression for the root entity.
        /// </summary>      
        /// <param name="keySelector">
        /// Expression that selects the property used for ordering.
        /// </param>
        /// <returns>
        /// Current ordered query command builder instance.
        /// </returns>
        IOrderedQueryCommandBuilder<T> ThenByDescending(Expression<Func<T, object>> keySelector);

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
        IOrderedQueryCommandBuilder<T> ThenByDescending<TEntity>(Expression<Func<TEntity, object>> keySelector);
    }
}
