using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;
using TinyBlueWhale.EngineQuery.Sql.Compilation;
using TinyBlueWhale.EngineQuery.Sql.Interfaces.Strategies;

namespace TinyBlueWhale.EngineQuery.PostgreSql.Clauses.Strategies.InsertIdentityRetrieval
{
    /// <summary>
    /// Provides PostgreSQL 8.4 identity retrieval behavior for INSERT commands.
    /// </summary>
    public sealed class PostgreSql84InsertIdentityRetrievalStrategy : IInsertIdentityRetrievalStrategy
    {
        /// <inheritdoc />
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
