using TinyBlueWhale.EngineQuery.Core.QueryBuilding.Context;

namespace TinyBlueWhale.EngineQuery.Core.QueryBuilding.Ordering
{

    /// <summary>
    /// Builds SQL pagination definitions.
    /// </summary>
    internal sealed class PaginationClauseBuilder(QueryCommandBuilderContext context)
    {
        private readonly QueryCommandBuilderContext _context = context;

        /// <summary>
        /// Sets the number of rows to skip.
        /// </summary>
        /// <param name="count">
        /// Number of rows skipped before returning query results.
        /// </param>
        public void SetSkip(int count)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(count);

            _context.QueryDefinition.Pagination = _context.QueryDefinition.Pagination with
            {
                Skip = count
            };
        }

        /// <summary>
        /// Sets the number of rows to take.
        /// </summary>
        /// <param name="count">
        /// Maximum number of rows returned by the query.
        /// </param>
        public void SetTake(int count)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(count);

            _context.QueryDefinition.Pagination = _context.QueryDefinition.Pagination with
            {
                Take = count
            };
        }
    }
}
