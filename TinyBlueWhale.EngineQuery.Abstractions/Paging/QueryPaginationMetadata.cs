
namespace TinyBlueWhale.EngineQuery.Abstractions.Paging
{

    /// <summary>
    /// Represents pagination metadata for a paginated query result.
    /// </summary>
    public sealed record QueryPaginationMetadata
    {
        /// <summary>
        /// Gets the current page number.
        /// </summary>
        public required int Page { get; init; }

        /// <summary>
        /// Gets the number of items requested per page.
        /// </summary>
        public required int PageSize { get; init; }

        /// <summary>
        /// Gets the total number of records available.
        /// </summary>
        public required long TotalItems { get; init; }

        /// <summary>
        /// Gets the total number of available pages.
        /// </summary>
        public int TotalPages =>PageSize <= 0 ? 0 : (int)Math.Ceiling((double)TotalItems / PageSize);

        /// <summary>
        /// Gets a value indicating whether a previous page exists.
        /// </summary>
        public bool HasPreviousPage => Page > 1;

        /// <summary>
        /// Gets a value indicating whether a next page exists.
        /// </summary>
        public bool HasNextPage => Page < TotalPages;
    }
}
