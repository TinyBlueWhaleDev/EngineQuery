using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;
using TinyBlueWhale.EngineQuery.Sql.Compilation;

namespace TinyBlueWhale.EngineQuery.Sql.Interfaces
{
    /// <summary>
    /// Defines a SQL clause builder that can conditionally produce a SQL clause.
    /// </summary>
    /// <remarks>
    /// Optional clause builders are used for SQL fragments that should only be emitted when
    /// the query definition contains the corresponding metadata.
    /// </remarks>
    public interface IOptionalSqlClauseBuilder
    {
        /// <summary>
        /// Determines whether the SQL clause should be built for the specified query definition.
        /// </summary>
        /// <param name="queryDefinition">
        /// Query definition to inspect.
        /// </param>
        /// <returns>
        /// <see langword="true"/> when the clause should be generated; otherwise, <see langword="false"/>.
        /// </returns>
        bool CanBuild(CompiledQueryDefinition queryDefinition);

        /// <summary>
        /// Builds an optional SQL clause for the specified query definition.
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
