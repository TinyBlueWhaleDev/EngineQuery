using TinyBlueWhale.EngineQuery.Core.QueryDefinitions.Commands;
using TinyBlueWhale.EngineQuery.Sql.Compilation;
using TinyBlueWhale.EngineQuery.Sql.Interfaces.Strategies;

namespace TinyBlueWhale.EngineQuery.SqlServer.Clauses.Strategies.InsertIdentityRetrieval
{
    /// <summary>
    /// Provides SQL Server 2008 identity retrieval behavior for INSERT commands.
    /// </summary>
    public sealed class SqlServer2008InsertIdentityRetrievalStrategy : IInsertIdentityRetrievalStrategy
    {
        /// <inheritdoc />
        public string AppendIdentityRetrieval(QueryInsertIdentityDefinition identityDefinition, string commandText, QueryCompilationContext context)
        {
            ArgumentNullException.ThrowIfNull(identityDefinition);
            ArgumentException.ThrowIfNullOrWhiteSpace(commandText);
            ArgumentNullException.ThrowIfNull(context);

            if (identityDefinition.ColumnName is not null)
                throw new NotSupportedException("SQL Server identity retrieval does not require an identity column selector. Use ReturnIdentity().");

            return $"{commandText};{Environment.NewLine}" +
                "SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";
        }
    }
}
