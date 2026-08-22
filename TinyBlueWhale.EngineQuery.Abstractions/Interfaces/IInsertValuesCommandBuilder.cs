using System.Linq.Expressions;
using TinyBlueWhale.EngineQuery.Abstractions.Models;

namespace TinyBlueWhale.EngineQuery.Abstractions.Interfaces
{
    /// <summary>
    /// Defines a fluent contract for composing direct INSERT VALUES commands.
    /// </summary>
    /// <typeparam name="T">
    /// Entity type associated with the target INSERT table.
    /// </typeparam>
    public interface IInsertValuesCommandBuilder<T>
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
        /// Current INSERT VALUES command builder instance.
        /// </returns>
        IInsertValuesCommandBuilder<T> Set<TProperty>(Expression<Func<T, TProperty>> selector, TProperty value);

        /// <summary>
        /// Configures the INSERT command to return the generated identity value
        /// using the scalar identity function supported by the current provider.
        /// </summary>
        /// <remarks>
        /// SQL Server generates SCOPE_IDENTITY() and MySQL generates LAST_INSERT_ID().
        /// PostgreSQL requires the overload that specifies the identity column.
        /// This operation is available only for direct INSERT value assignments.
        /// </remarks>
        /// <returns>The current INSERT values command builder.</returns>
        IInsertValuesCommandBuilder<T> ReturnIdentity();

        /// <summary>
        /// Configures the INSERT command to return the generated identity column.
        /// </summary>
        /// <typeparam name="TProperty">The identity property type.</typeparam>
        /// <param name="identitySelector">
        /// An expression selecting the entity property mapped to the generated identity column.
        /// </param>
        /// <remarks>
        /// This overload is required by PostgreSQL to generate its RETURNING clause.
        /// The selected property is resolved through the configured entity metadata.
        /// This operation is available only for direct INSERT value assignments.
        /// </remarks>
        /// <returns>The current INSERT values command builder.</returns>
        IInsertValuesCommandBuilder<T> ReturnIdentity<TProperty>(Expression<Func<T, TProperty>> selector);

        /// <summary>
        /// Builds the current INSERT VALUES command into SQL command text and parameters.
        /// </summary>
        /// <returns>
        /// Generated SQL command.
        /// </returns>
        GeneratedSqlQuery Build();
    }
}
