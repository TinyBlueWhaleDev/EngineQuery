using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;
using TinyBlueWhale.EngineQuery.Sql.Compilation;

namespace TinyBlueWhale.EngineQuery.Sql.Interfaces
{
    /// <summary>
    /// Defines a SQL clause builder that always produces a required SQL clause.
    /// </summary>
    /// <remarks>
    /// Required clause builders are used for SQL fragments that must exist in every generated query,
    /// such as SELECT and FROM clauses.
    /// </remarks>
    public interface IRequiredSqlClauseBuilder
    {
        /// <summary>
        /// Builds a required SQL clause for the specified query definition.
        /// </summary>
        /// <param name="queryDefinition">
        /// Query definition that contains the metadata needed to build the clause.
        /// </param>
        /// <param name="context">
        /// Current SQL compilation context.
        /// </param>
        /// <returns>
        /// SQL clause text.
        /// </returns>
        string Build(CompiledQueryDefinition queryDefinition, QueryCompilationContext context);
    }
}
