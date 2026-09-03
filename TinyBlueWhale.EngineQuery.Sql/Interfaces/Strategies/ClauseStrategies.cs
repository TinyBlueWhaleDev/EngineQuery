using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;
using TinyBlueWhale.EngineQuery.Sql.Compilation;

namespace TinyBlueWhale.EngineQuery.Sql.Interfaces.Strategies
{
    #region Pagination

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

    /// <summary>
    /// Defines a provider profile capable of supplying SQL pagination behavior.
    /// </summary>
    public interface IPaginationStrategyProvider
    {
        /// <summary>
        /// Creates the pagination strategy associated with the provider profile.
        /// </summary>
        /// <returns>
        /// Pagination strategy used to generate provider-specific pagination SQL.
        /// </returns>
        IPaginationStrategy CreatePaginationStrategy();
    }

    #endregion

    #region Cte

    /// <summary>
    /// Defines provider-specific SQL behavior required to build common table expression clauses.
    /// </summary>
    public interface ICTEStrategy
    {
        /// <summary>
        /// Resolves the SQL keyword used when building recursive common table expressions.
        /// </summary>
        /// <returns>
        /// SQL keyword used to start a recursive common table expression clause.
        /// </returns>
        string ResolveRecursiveCteKeyword();
    }

    /// <summary>
    /// Defines a provider profile capable of supplying the common table expression strategy used during SQL compilation.
    /// </summary>
    public interface ICTEStrategyProvider
    {
        /// <summary>
        /// Creates the common table expression strategy associated with the provider profile.
        /// </summary>
        /// <returns>
        /// Common table expression strategy used during SQL compilation.
        /// </returns>
        ICTEStrategy CreateCteStrategy();
    }

    #endregion
}
