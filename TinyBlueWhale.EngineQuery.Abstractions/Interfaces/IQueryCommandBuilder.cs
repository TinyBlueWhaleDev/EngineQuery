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
        /// Builds the current query definition into SQL command text and parameters.
        /// </summary>
        /// <returns>
        /// Generated SQL query command.
        /// </returns>
        GeneratedSqlQuery Build();
    }
}
