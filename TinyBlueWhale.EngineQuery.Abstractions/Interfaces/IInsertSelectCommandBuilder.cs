using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;
using TinyBlueWhale.EngineQuery.Abstractions.Models;

namespace TinyBlueWhale.EngineQuery.Abstractions.Interfaces
{
    /// <summary>
    /// Defines a fluent contract for composing INSERT SELECT commands.
    /// </summary>
    /// <typeparam name="T">
    /// Entity type associated with the target INSERT table.
    /// </typeparam>
    /// <typeparam name="TProfile">
    /// Database provider profile associated with the command.
    /// </typeparam>
    public interface IInsertSelectCommandBuilder<T, TProfile> :
        IQueryCompositionCommandBuilder<T, IInsertSelectCommandBuilder<T, TProfile>, TProfile>
        where TProfile : IDatabaseProviderProfile
    {
        /// <summary>
        /// Builds the current INSERT SELECT command into SQL command text and parameters.
        /// </summary>
        /// <returns>
        /// Generated SQL command.
        /// </returns>
        GeneratedSqlQuery Build();
    }
}
