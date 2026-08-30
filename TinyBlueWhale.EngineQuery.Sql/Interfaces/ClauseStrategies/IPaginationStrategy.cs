using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;
using TinyBlueWhale.EngineQuery.Sql.Compilation;

namespace TinyBlueWhale.EngineQuery.Sql.Interfaces.ClauseStrategies
{
    /// <summary>
    /// Defines provider-specific SQL pagination behavior.
    /// </summary>
    public interface IPaginationStrategy
    {
        /// <summary>
        /// Builds the SQL pagination clause for the specified query definition.
        /// </summary>
        /// <param name="queryDefinition">
        /// Query definition containing pagination metadata.
        /// </param>
        /// <param name="context">
        /// Current SQL compilation context.
        /// </param>
        /// <returns>
        /// Provider-specific SQL pagination clause.
        /// </returns>
        string Build(CompiledQueryDefinition queryDefinition, QueryCompilationContext context);
    }
}
