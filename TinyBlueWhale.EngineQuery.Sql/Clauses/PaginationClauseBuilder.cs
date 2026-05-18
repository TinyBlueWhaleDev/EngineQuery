using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;
using TinyBlueWhale.EngineQuery.Sql.Compilation;
using TinyBlueWhale.EngineQuery.Sql.Interfaces;

namespace TinyBlueWhale.EngineQuery.Sql.Clauses
{
    /// <summary>
    /// Builds provider-specific SQL pagination clauses.
    /// </summary>
    /// <remarks>
    /// This builder emits pagination syntax only when the query definition contains skip or take values.
    /// Pagination requires at least one ORDER BY clause to guarantee deterministic results.
    /// </remarks>
    public sealed class PaginationClauseBuilder : IOptionalSqlClauseBuilder
    {
        /// <summary>
        /// Determines whether a pagination clause should be built.
        /// </summary>
        /// <param name="queryDefinition">
        /// Query definition to inspect.
        /// </param>
        /// <returns>
        /// <see langword="true"/> when pagination is configured; otherwise, <see langword="false"/>.
        /// </returns>
        public bool CanBuild(CompiledQueryDefinition queryDefinition)
        {
            ArgumentNullException.ThrowIfNull(queryDefinition);

            return queryDefinition.Pagination.HasPagination;
        }

        /// <summary>
        /// Builds the provider-specific SQL pagination clause.
        /// </summary>
        /// <param name="queryDefinition">
        /// Query definition that contains pagination metadata.
        /// </param>
        /// <param name="context">
        /// Current SQL compilation context.
        /// </param>
        /// <returns>
        /// Provider-specific SQL pagination clause.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when pagination is configured without any ORDER BY clause.
        /// </exception>
        public string Build(CompiledQueryDefinition queryDefinition, QueryCompilationContext context)
        {
            ArgumentNullException.ThrowIfNull(queryDefinition);
            ArgumentNullException.ThrowIfNull(context);

            if (queryDefinition.OrderingDefinitions.Count == 0)
                throw new InvalidOperationException("Pagination requires at least one ORDER BY clause.");

            return context.DatabaseDialect.BuildPaginationClause(
                queryDefinition.Pagination.Skip,
                queryDefinition.Pagination.Take);
        }
    }
}
