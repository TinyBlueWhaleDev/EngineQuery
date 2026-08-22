using TinyBlueWhale.EngineQuery.Abstractions.Models;

namespace TinyBlueWhale.EngineQuery.Abstractions.Interfaces
{
    /// <summary>
    /// Defines a fluent contract for composing INSERT SELECT commands.
    /// </summary>
    /// <typeparam name="T">
    /// Entity type associated with the target INSERT table.
    /// </typeparam>
    public interface IInsertSelectCommandBuilder<T> : IQueryCompositionCommandBuilder<T, IInsertSelectCommandBuilder<T>>
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
