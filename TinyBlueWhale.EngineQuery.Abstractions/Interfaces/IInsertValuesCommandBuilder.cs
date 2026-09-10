using System.Linq.Expressions;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Abstractions.Models;

namespace TinyBlueWhale.EngineQuery.Abstractions.Interfaces
{
    /// <summary>
    /// Defines a fluent contract for composing direct INSERT VALUES commands.
    /// </summary>
    /// <typeparam name="T">
    /// Entity type associated with the target INSERT table.
    /// </typeparam>
    /// <typeparam name="TProfile">
    /// Database provider profile associated with the INSERT command.
    /// </typeparam>
    public interface IInsertValuesCommandBuilder<T, TProfile>
         where TProfile : IDatabaseProviderProfile
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
        IInsertValuesCommandBuilder<T, TProfile> Set<TProperty>(Expression<Func<T, TProperty>> selector, TProperty value);

        /// <summary>
        /// Configures provider-specific scalar identity retrieval for the INSERT command.
        /// </summary>
        /// <returns>
        /// Current INSERT VALUES command builder instance.
        /// </returns>
        internal IInsertValuesCommandBuilder<T, TProfile> ApplyReturnIdentity()
        {
            throw new NotSupportedException("Scalar INSERT identity retrieval is not supported by the current INSERT builder.");
        }

        /// <summary>
        /// Configures provider-specific identity retrieval using the selected target column.
        /// </summary>
        /// <typeparam name="TProperty">
        /// Property type associated with the generated identity.
        /// </typeparam>
        /// <param name="identitySelector">
        /// Expression that selects the target identity property.
        /// </param>
        /// <returns>
        /// Current INSERT VALUES command builder instance.
        /// </returns>
        internal IInsertValuesCommandBuilder<T, TProfile> ApplyReturnIdentity<TProperty>(Expression<Func<T, TProperty>> identitySelector)
        {
            throw new NotSupportedException("Column-based INSERT identity retrieval is not supported by the current INSERT builder.");
        }

        /// <summary>
        /// Builds the current INSERT VALUES command into SQL command text and parameters.
        /// </summary>
        /// <returns>
        /// Generated SQL command.
        /// </returns>
        GeneratedSqlQuery Build();
    }
}
