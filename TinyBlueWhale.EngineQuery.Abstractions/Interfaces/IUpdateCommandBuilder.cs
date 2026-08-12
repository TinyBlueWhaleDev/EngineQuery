using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Abstractions.Models;

namespace TinyBlueWhale.EngineQuery.Abstractions.Interfaces
{
    /// <summary>
    /// Defines a fluent contract for composing strongly typed SQL UPDATE commands.
    /// </summary>
    /// <typeparam name="T">
    /// Entity type associated with the target UPDATE table.
    /// </typeparam>
    public interface IUpdateCommandBuilder<T>
    {
        /// <summary>
        /// Adds a value assignment for the selected entity property.
        /// </summary>
        /// <typeparam name="TProperty">
        /// Property type associated with the assigned value.
        /// </typeparam>
        /// <param name="selector">
        /// Expression that selects the target entity property.
        /// </param>
        /// <param name="value">
        /// Value assigned to the selected property.
        /// </param>
        /// <returns>
        /// Current UPDATE command builder instance.
        /// </returns>
        IUpdateCommandBuilder<T> Set<TProperty>(Expression<Func<T, TProperty>> selector, TProperty value);

        /// <summary>
        /// Adds a WHERE predicate for the target entity.
        /// </summary>
        /// <param name="predicate">
        /// Predicate expression describing the SQL filter condition.
        /// </param>
        /// <returns>
        /// Current UPDATE command builder instance.
        /// </returns>
        IUpdateCommandBuilder<T> Where(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Adds a WHERE predicate for the target entity using the specified logical operator.
        /// </summary>
        /// <param name="predicate">
        /// Predicate expression describing the SQL filter condition.
        /// </param>
        /// <param name="logicalOperator">
        /// Logical operator used to connect the predicate with the preceding WHERE predicate.
        /// </param>
        /// <returns>
        /// Current UPDATE command builder instance.
        /// </returns>
        IUpdateCommandBuilder<T> Where(Expression<Func<T, bool>> predicate, QueryLogicalOperator logicalOperator);

        /// <summary>
        /// Adds a filtering expression only when the specified condition is true.
        /// </summary>
        /// <param name="condition">
        /// Determines whether the predicate should be added.
        /// </param>
        /// <param name="predicate">
        /// Predicate expression describing the SQL filter condition.
        /// </param>
        /// <returns>
        /// Current UPDATE command builder instance.
        /// </returns>
        IUpdateCommandBuilder<T> WhereIf(bool condition, Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Adds a conditional WHERE predicate using the specified logical operator.
        /// </summary>
        /// <param name="condition">
        /// Determines whether the predicate should be added.
        /// </param>
        /// <param name="predicate">
        /// Predicate expression describing the SQL filter condition.
        /// </param>
        /// <param name="logicalOperator">
        /// Logical operator used to connect the predicate with the preceding WHERE predicate.
        /// </param>
        /// <returns>
        /// Current UPDATE command builder instance.
        /// </returns>
        IUpdateCommandBuilder<T> WhereIf(bool condition, Expression<Func<T, bool>> predicate, QueryLogicalOperator logicalOperator);

        /// <summary>
        /// Builds the current UPDATE command into SQL command text and parameters.
        /// </summary>
        /// <returns>
        /// Generated SQL command.
        /// </returns>
        GeneratedSqlQuery Build();
    }
}
