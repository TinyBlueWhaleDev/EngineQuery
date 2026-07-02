using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;
using TinyBlueWhale.EngineQuery.Sql.Compilation;

namespace TinyBlueWhale.EngineQuery.Sql.Interfaces
{
    /// <summary>
    /// Defines a service capable of building SQL command text from a compiled query definition.
    /// </summary>
    /// <remarks>
    /// Implementations are responsible for orchestrating SQL clause builders but should not format
    /// the final SQL script or create the final generated query object.
    /// </remarks>
    public interface IQueryScriptBuilder
    {
        /// <summary>
        /// Builds SQL command text for the specified query definition.
        /// </summary>
        /// <param name="queryDefinition">
        /// Query definition to compile into SQL command text.
        /// </param>
        /// <param name="context">
        /// Current SQL compilation context.
        /// </param>
        /// <returns>
        /// SQL command text.
        /// </returns>
        string Build(CompiledQueryDefinition queryDefinition, QueryCompilationContext context);
    }
}
