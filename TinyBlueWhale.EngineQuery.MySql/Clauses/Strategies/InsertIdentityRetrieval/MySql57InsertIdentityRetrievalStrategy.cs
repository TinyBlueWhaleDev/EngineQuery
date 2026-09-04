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
        /// <inheritdoc />
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
