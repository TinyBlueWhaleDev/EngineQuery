using System.Linq.Expressions;

namespace TinyBlueWhale.EngineQuery.Abstractions.Interfaces
{

    /// <summary>
    /// Defines operations used to build SQL window function clauses.
    /// </summary>
    public interface IWindowFunctionBuilder
    {
        /// <summary>
        /// Adds a PARTITION BY column to the window function.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type associated with the partitioned column.
        /// </typeparam>
        /// <param name="selector">
        /// Expression that selects the partitioned property.
        /// </param>
        /// <returns>
        /// Current window function builder instance.
        /// </returns>
        IWindowFunctionBuilder PartitionBy<TEntity>(Expression<Func<TEntity, object>> selector);

        /// <summary>
        /// Adds an ascending ORDER BY column to the window function.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type associated with the ordered column.
        /// </typeparam>
        /// <param name="selector">
        /// Expression that selects the ordered property.
        /// </param>
        /// <returns>
        /// Current window function builder instance.
        /// </returns>
        IWindowFunctionBuilder OrderBy<TEntity>(Expression<Func<TEntity, object>> selector);

        /// <summary>
        /// Adds a descending ORDER BY column to the window function.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Entity type associated with the ordered column.
        /// </typeparam>
        /// <param name="selector">
        /// Expression that selects the ordered property.
        /// </param>
        /// <returns>
        /// Current window function builder instance.
        /// </returns>
        IWindowFunctionBuilder OrderByDescending<TEntity>(Expression<Func<TEntity, object>> selector);

    }
}
