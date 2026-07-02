
namespace TinyBlueWhale.EngineQuery.Core.QueryDefinitions
{
    /// <summary>
    /// Represents pagination settings used to generate SQL paging clauses.
    /// </summary>
    public sealed record QueryPaginationDefinition()
    {
        /// <summary>
        /// Gets the number of rows to skip.
        /// </summary>
        public int? Skip { get; init; }

        /// <summary>
        /// Gets the maximum number of rows to return.
        /// </summary>
        public int? Take { get; init; }

        /// <summary>
        /// Gets a value indicating whether pagination has been configured.
        /// </summary>
        public bool HasPagination =>
            Skip.HasValue || Take.HasValue;
    }
}
