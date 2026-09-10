using TinyBlueWhale.EngineQuery.Core.QueryDefinitions;
using TinyBlueWhale.EngineQuery.Sql.Compilation;
using TinyBlueWhale.EngineQuery.Sql.Interfaces;
using TinyBlueWhale.EngineQuery.Sql.Interfaces.Strategies;

namespace TinyBlueWhale.EngineQuery.Sql.Clauses.Pagination
{
    /// <summary>
    /// Builds provider-specific SQL pagination clauses.
    /// </summary>
    /// <remarks>
    /// This builder emits pagination syntax when the query definition contains skip or take values.
    /// </remarks>
    /// <param name="paginationStrategy">
    /// Provider-specific pagination strategy.
    /// </param>
    public sealed class PaginationClauseBuilder(IPaginationStrategy paginationStrategy) : IOptionalSqlClauseBuilder
    {

        private readonly IPaginationStrategy _paginationStrategy = paginationStrategy ?? throw new ArgumentNullException(nameof(paginationStrategy));

        /// <summary>
        /// Determines whether a pagination clause should be built.
        /// </summary>
        /// <param name="queryDefinition">
        /// Query definition to inspect.
        /// </param>
        /// <returns>
        /// <see langword="true"/> when pagination is configured; otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="queryDefinition"/> is null.
        /// </exception>
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
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="queryDefinition"/> or
        /// <paramref name="context"/> is null.
        /// </exception>
        public string Build(CompiledQueryDefinition queryDefinition, QueryCompilationContext context)
        {
            ArgumentNullException.ThrowIfNull(queryDefinition);
            ArgumentNullException.ThrowIfNull(context);

            return _paginationStrategy.Build(queryDefinition, context);
        }
    }
}
