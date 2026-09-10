using TinyBlueWhale.EngineQuery.Sql.Clauses.Cte;

namespace TinyBlueWhale.EngineQuery.SqlServer.Clauses.Strategies.Cte
{
    /// <summary>
    /// Provides SQL Server 2008 common table expression behavior.
    /// </summary>
    public sealed class SqlServer2008CteStrategy : CteStrategy
    {
        /// <inheritdoc />
        public override string ResolveRecursiveCteKeyword()
        {
            return "WITH";
        }
    }
}
