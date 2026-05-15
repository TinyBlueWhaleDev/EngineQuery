using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Core.Interfaces;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;
using TinyBlueWhale.EngineQuery.Sql.Compilation;

namespace TinyBlueWhale.EngineQuery.MySql.Compilation
{
    /// <summary>
    /// Compiles query definitions into MySQL command text.
    /// </summary>
    public sealed class MySqlQueryCompiler(ISqlDatabaseDialect databaseDialect) : QueryCompilerBase(databaseDialect)
    {
        // Builds a MySQL LATERAL join clause for APPLY definitions.
        protected override string BuildApplyClause(QueryApplyDefinition applyDefinition, string commandText)
        {
            var applyKeyword = applyDefinition.ApplyType == QueryApplyType.Cross
                ? "JOIN LATERAL"
                : "LEFT JOIN LATERAL";

            return $"{applyKeyword} ({commandText}) AS {_databaseDialect.EscapeIdentifier(applyDefinition.Alias)} ON TRUE";
        }
    }
}
