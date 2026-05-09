using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.Abstractions.Interfaces
{
    /// <summary>
    /// Defines a fluent contract for composing ordered SQL query commands.
    /// </summary>
    /// <typeparam name="T">
    /// Entity type used as the source of the query.
    /// </typeparam>
    public interface IOrderedQueryCommandBuilder<T> : IQueryCommandBuilder<T>
    {
        /// <summary>
        /// Adds an additional ascending ordering expression to the query.
        /// </summary>
        /// <typeparam name="TKey">
        /// Type of the selected ordering property.
        /// </typeparam>
        /// <param name="keySelector">
        /// Expression that selects the property used for ordering.
        /// </param>
        /// <returns>
        /// Current ordered query command builder instance.
        /// </returns>
        IOrderedQueryCommandBuilder<T> ThenBy<TKey>(Expression<Func<T, TKey>> keySelector);

        /// <summary>
        /// Adds an additional descending ordering expression to the query.
        /// </summary>
        /// <typeparam name="TKey">
        /// Type of the selected ordering property.
        /// </typeparam>
        /// <param name="keySelector">
        /// Expression that selects the property used for ordering.
        /// </param>
        /// <returns>
        /// Current ordered query command builder instance.
        /// </returns>
        IOrderedQueryCommandBuilder<T> ThenByDescending<TKey>(Expression<Func<T, TKey>> keySelector);
    }
}
