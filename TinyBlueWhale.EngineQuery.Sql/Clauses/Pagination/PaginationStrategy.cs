using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;
using TinyBlueWhale.EngineQuery.Sql.Compilation;
using TinyBlueWhale.EngineQuery.Sql.Interfaces.ClauseStrategies;

namespace TinyBlueWhale.EngineQuery.Sql.Clauses.Pagination
{
    /// <summary>
    /// Provides the default SQL pagination behavior.
    /// </summary>
    /// <remarks>
    /// The default strategy delegates pagination syntax generation to the
    /// configured database dialect without requiring an ORDER BY clause.
    /// </remarks>
    public class PaginationStrategy : IPaginationStrategy
    {
        /// <inheritdoc />
        public virtual string Build(CompiledQueryDefinition queryDefinition, QueryCompilationContext context)
        {
            ArgumentNullException.ThrowIfNull(queryDefinition);
            ArgumentNullException.ThrowIfNull(context);

            return context.DatabaseDialect.BuildPaginationClause(queryDefinition.Pagination.Skip, queryDefinition.Pagination.Take);
        }
    }
}
