using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Core.Interfaces;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;
using TinyBlueWhale.EngineQuery.PostgreSqlServer.Capabilities;
using TinyBlueWhale.EngineQuery.Sql.Compilation;

namespace TinyBlueWhale.EngineQuery.PostgreSql.Compilation
{
    /// <summary>
    /// Compiles query definitions into PostgreSQL command text.
    /// </summary>
    public sealed class PostgreSqlQueryCompiler(ISqlDatabaseDialect databaseDialect, PostgreSqlProviderCapabilities providerCapabilities) : QueryCompilerBase(databaseDialect, providerCapabilities)
    {
        // Builds a PostgreSQL LATERAL join clause for APPLY definitions.
        protected override string BuildApplyClause(QueryApplyDefinition applyDefinition, string commandText)
        {
            var applyKeyword = applyDefinition.ApplyType == QueryApplyType.Cross
                ? "JOIN LATERAL"
                : "LEFT JOIN LATERAL";

            return $"{applyKeyword} ({commandText}) AS {_databaseDialect.EscapeIdentifier(applyDefinition.Alias)} ON TRUE";
        }
    }
}
