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
    /// Defines a fluent contract for composing strongly typed SQL DELETE commands.
    /// </summary>
    /// <typeparam name="T">
    /// Entity type associated with the target DELETE table.
    /// </typeparam>
    public interface IDeleteCommandBuilder<T>
    {
        /// <summary>
        /// Adds a WHERE predicate for the target entity.
        /// </summary>
        /// <param name="predicate">
        /// Predicate expression describing the SQL filter condition.
        /// </param>
        /// <returns>
        /// Current DELETE command builder instance.
        /// </returns>
        IDeleteCommandBuilder<T> Where(Expression<Func<T, bool>> predicate);

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
        /// Current DELETE command builder instance.
        /// </returns>
        IDeleteCommandBuilder<T> Where(Expression<Func<T, bool>> predicate, QueryLogicalOperator logicalOperator);

        /// <summary>
        /// Adds an IN collection condition for the target entity.
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
        /// Current DELETE command builder instance.
        /// </returns>
        IDeleteCommandBuilder<T> WhereIn<TProperty>(Expression<Func<T, TProperty>> selector, IEnumerable<TProperty> values);

        /// <summary>
        /// Adds a NOT IN collection condition for the target entity.
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
        /// Current DELETE command builder instance.
        /// </returns>
        IDeleteCommandBuilder<T> WhereNotIn<TProperty>(Expression<Func<T, TProperty>> selector, IEnumerable<TProperty> values);

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
        /// Current DELETE command builder instance.
        /// </returns>
        IDeleteCommandBuilder<T> WhereIf(bool condition, Expression<Func<T, bool>> predicate);

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
        /// Current DELETE command builder instance.
        /// </returns>
        IDeleteCommandBuilder<T> WhereIf(bool condition, Expression<Func<T, bool>> predicate, QueryLogicalOperator logicalOperator);

        /// <summary>
        /// Builds the current DELETE command into SQL command text and parameters.
        /// </summary>
        /// <returns>
        /// Generated SQL command.
        /// </returns>
        GeneratedSqlQuery Build();
    }
}
