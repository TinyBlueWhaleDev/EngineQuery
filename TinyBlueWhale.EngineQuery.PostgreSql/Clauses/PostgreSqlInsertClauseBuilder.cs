using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;
using TinyBlueWhale.EngineQuery.Sql.Clauses;
using TinyBlueWhale.EngineQuery.Sql.Compilation;

namespace TinyBlueWhale.EngineQuery.PostgreSql.Clauses
{
    /// <summary>
    /// Builds PostgreSQL INSERT clauses with optional identity retrieval.
    /// </summary>
    public sealed class PostgreSqlInsertClauseBuilder : InsertClauseBuilder
    {
        /// <summary>
        /// Appends PostgreSQL identity retrieval to the generated INSERT command.
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
        /// INSERT command with a RETURNING clause.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="identityDefinition"/> or
        /// <paramref name="context"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="commandText"/> is empty.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Thrown when no identity column selector was configured.
        /// </exception>
        protected override string AppendIdentityRetrieval(QueryInsertIdentityDefinition identityDefinition, string commandText, QueryCompilationContext context)
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
