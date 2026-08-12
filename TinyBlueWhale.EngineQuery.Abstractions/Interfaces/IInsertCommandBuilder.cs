using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Abstractions.Models;

namespace TinyBlueWhale.EngineQuery.Abstractions.Interfaces
{
    /// <summary>
    /// Defines a fluent contract for composing strongly typed SQL INSERT commands.
    /// </summary>
    /// <typeparam name="T">
    /// Entity type associated with the target INSERT table.
    /// </typeparam>
    public interface IInsertCommandBuilder<T>
    {
        /// <summary>
        /// Adds a value assignment for the selected entity property.
        /// </summary>
        /// <typeparam name="TProperty">
        /// Property type associated with the inserted value.
        /// </typeparam>
        /// <param name="selector">
        /// Expression that selects the target entity property.
        /// </param>
        /// <param name="value">
        /// Value assigned to the selected property.
        /// </param>
        /// <returns>
        /// Current INSERT command builder instance.
        /// </returns>
        IInsertCommandBuilder<T> Set<TProperty>(Expression<Func<T, TProperty>> selector, TProperty value);

        /// <summary>
        /// Builds the current INSERT command into SQL command text and parameters.
        /// </summary>
        /// <returns>
        /// Generated SQL command.
        /// </returns>
        GeneratedSqlQuery Build();
    }
}

