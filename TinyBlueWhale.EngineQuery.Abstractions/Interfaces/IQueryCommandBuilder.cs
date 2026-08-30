using System.Linq.Expressions;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Abstractions.Models;

namespace TinyBlueWhale.EngineQuery.Abstractions.Interfaces
{
    /// <summary>
    /// Defines a fluent contract for composing strongly typed SQL query commands.
    /// </summary>
    /// <typeparam name="T">
    /// Entity type used as the source of the query.
    /// </typeparam>
    public interface IQueryCommandBuilder<T, TProfile> :
        IQueryCompositionCommandBuilder<T, IQueryCommandBuilder<T, TProfile>, TProfile>
        where TProfile : IDatabaseProviderProfile
    {

        /// <summary>
        /// Adds an ascending ordering expression to the query.
        /// </summary>        
        /// <param name="keySelector">
        /// Expression that selects the property used for ordering.
        /// </param>
        /// <returns>
        /// Ordered query command builder instance.
        /// </returns>
        IOrderedQueryCommandBuilder<T, TProfile> OrderBy(Expression<Func<T, object>> keySelector);

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
        IOrderedQueryCommandBuilder<T, TProfile> OrderBy<TEntity>(Expression<Func<TEntity, object>> keySelector);

        /// <summary>
        /// Adds a descending ordering expression to the query.
        /// </summary>        
        /// <param name="keySelector">
        /// Expression that selects the property used for ordering.
        /// </param>
        /// <returns>
        /// Ordered query command builder instance.
        /// </returns>
        IOrderedQueryCommandBuilder<T, TProfile> OrderByDescending(Expression<Func<T, object>> keySelector);

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
        IOrderedQueryCommandBuilder<T, TProfile> OrderByDescending<TEntity>(Expression<Func<TEntity, object>> selector);

        /// <summary>
        /// Builds the current query definition into SQL command text and parameters.
        /// </summary>
        /// <returns>
        /// Generated SQL query command.
        /// </returns>
        GeneratedSqlQuery Build();
    }
}
