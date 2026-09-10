using TinyBlueWhale.EngineQuery.Core.QueryDefinitions.Commands;
using TinyBlueWhale.EngineQuery.Sql.Compilation;
using TinyBlueWhale.EngineQuery.Sql.Interfaces.Strategies;

namespace TinyBlueWhale.EngineQuery.PostgreSql.Clauses.Strategies.InsertIdentityRetrieval
{
    /// <summary>
    /// Provides PostgreSQL 8.4 identity retrieval behavior for INSERT commands.
    /// </summary>
    public sealed class PostgreSql84InsertIdentityRetrievalStrategy : IInsertIdentityRetrievalStrategy
    {
        /// <summary>
        /// Appends PostgreSQL identity retrieval SQL to the generated INSERT command.
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
        /// INSERT command followed by a RETURNING clause for the selected identity column.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="identityDefinition"/> or
        /// <paramref name="context"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="commandText"/> is null, empty or consists only of white-space characters.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Thrown when an identity column selector is not specified.
        /// </exception>
        public string AppendIdentityRetrieval(QueryInsertIdentityDefinition identityDefinition, string commandText, QueryCompilationContext context)
        {
            ArgumentNullException.ThrowIfNull(identityDefinition);
            ArgumentException.ThrowIfNullOrWhiteSpace(commandText);
            ArgumentNullException.ThrowIfNull(context);

            if (identityDefinition.ColumnName is null)
                throw new NotSupportedException("PostgreSQL identity retrieval requires an identity column selector. Use ReturnIdentity(entity => entity.Id).");

            var columnName = context.DatabaseDialect.EscapeIdentifier(identityDefinition.ColumnName);

            return $"{commandText}{Environment.NewLine}" +
                $"RETURNING {columnName};";
        }
    }
}
