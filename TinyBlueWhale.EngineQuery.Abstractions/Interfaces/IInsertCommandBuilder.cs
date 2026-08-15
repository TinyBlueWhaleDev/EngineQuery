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
    public interface IInsertCommandBuilder<T> : IQueryCompositionCommandBuilder<T, IInsertCommandBuilder<T>>
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
        /// Defines the target columns associated with the INSERT command.
        /// </summary>
        /// <param name="selector">
        /// Expression used to determine which target entity properties should be included in the generated SQL INSERT clause.
        /// </param>
        /// <returns>
        /// Current INSERT command builder instance.
        /// </returns>
        IInsertCommandBuilder<T> Columns(Expression<Func<T, object>> selector);

        /// <summary>
        /// Configures the INSERT command to include identity columns.
        /// </summary>
        /// <returns>
        /// Current INSERT command builder instance.
        /// </returns>
        IInsertCommandBuilder<T> IncludeIdentityColumns();

        /// <summary>
        /// Configures an INSERT SELECT source using an explicit table name.
        /// </summary>
        /// <typeparam name="TSource">
        /// Entity type used as the source of the INSERT SELECT command.
        /// </typeparam>
        /// <param name="tableName">
        /// Database table name associated with the INSERT SELECT source.
        /// </param>
        /// <param name="alias">
        /// Optional table alias used to qualify generated SQL column references.
        /// </param>
        /// <returns>
        /// Current INSERT command builder instance.
        /// </returns>
        IInsertCommandBuilder<T> From<TSource>(string tableName, string? alias = null);

        /// <summary>
        /// Configures an INSERT SELECT source using resolved entity metadata.
        /// </summary>
        /// <typeparam name="TSource">
        /// Entity type used as the source of the INSERT SELECT command.
        /// </typeparam>
        /// <param name="alias">
        /// Optional table alias used to qualify generated SQL column references.
        /// </param>
        /// <returns>
        /// Current INSERT command builder instance.
        /// </returns>
        IInsertCommandBuilder<T> From<TSource>(string? alias = null);

        /// <summary>
        /// Builds the current INSERT command into SQL command text and parameters.
        /// </summary>
        /// <returns>
        /// Generated SQL command.
        /// </returns>
        GeneratedSqlQuery Build();
    }
}

