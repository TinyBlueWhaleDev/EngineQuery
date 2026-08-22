using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;
using TinyBlueWhale.EngineQuery.Sql.Clauses;
using TinyBlueWhale.EngineQuery.Sql.Compilation;

namespace TinyBlueWhale.EngineQuery.MySql.Clauses
{
    /// <summary>
    /// Builds MySQL INSERT clauses with optional identity retrieval.
    /// </summary>
    public sealed class MySqlInsertClauseBuilder : InsertClauseBuilder
    {
        /// <summary>
        /// Appends MySQL identity retrieval to the generated INSERT command.
        /// </summary>
        /// <param name="identityDefinition">
        /// Identity retrieval metadata configured by the INSERT VALUES builder.
        /// </param>
        /// <param name="commandText">
        /// Generated INSERT VALUES command text.
        /// </param>
        /// <param name="context">
        /// Current SQL compilation context.
        /// </param>
        /// <returns>
        /// INSERT command followed by a LAST_INSERT_ID query.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="identityDefinition"/> or
        /// <paramref name="context"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="commandText"/> is empty.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Thrown when an identity column selector was configured.
        /// </exception>
        protected override string AppendIdentityRetrieval(QueryInsertIdentityDefinition identityDefinition, string commandText, QueryCompilationContext context)
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
