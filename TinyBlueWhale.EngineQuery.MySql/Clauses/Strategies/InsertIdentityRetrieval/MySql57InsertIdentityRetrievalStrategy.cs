using TinyBlueWhale.EngineQuery.Core.QueryDefinitions.Commands;
using TinyBlueWhale.EngineQuery.Sql.Compilation;
using TinyBlueWhale.EngineQuery.Sql.Interfaces.Strategies;

namespace TinyBlueWhale.EngineQuery.MySql.Clauses.Strategies.InsertIdentityRetrieval
{
    /// <summary>
    /// Provides MySQL 5.7 identity retrieval behavior for INSERT commands.
    /// </summary>
    public sealed class MySql57InsertIdentityRetrievalStrategy : IInsertIdentityRetrievalStrategy
    {
        /// <summary>
        /// Appends MySQL identity retrieval SQL to the generated INSERT command.
        /// </summary>
        /// <param name="identityDefinition">
        /// Identity retrieval definition associated with the INSERT command.
        /// </param>
        /// <param name="commandText">
        /// Generated INSERT command text.
        /// </param>
        /// <param name="context">
        /// Current SQL compilation context.
        /// </param>
        /// <returns>
        /// INSERT command followed by a query that retrieves the generated identity value.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="identityDefinition"/> or
        /// <paramref name="context"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="commandText"/> is null, empty or consists only of white-space characters.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Thrown when an identity column selector is specified.
        /// </exception>
        public string AppendIdentityRetrieval(QueryInsertIdentityDefinition identityDefinition, string commandText, QueryCompilationContext context)
        {
            ArgumentNullException.ThrowIfNull(identityDefinition);
            ArgumentException.ThrowIfNullOrWhiteSpace(commandText);
            ArgumentNullException.ThrowIfNull(context);

            if (identityDefinition.ColumnName is not null)
                throw new NotSupportedException("MySQL identity retrieval does not require an identity column selector. Use ReturnIdentity().");

            return $"{commandText};{Environment.NewLine}" +
                "SELECT LAST_INSERT_ID();";
        }
    }
}
