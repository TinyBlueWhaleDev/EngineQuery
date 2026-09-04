using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Features;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;

namespace TinyBlueWhale.EngineQuery.Abstractions.Extensions
{
    /// <summary>
    /// Provides pagination operations for query compositions whose provider profile supports pagination.
    /// </summary>
    public static class QueryPaginationExtensions
    {
        /// <summary>
        /// Skips the specified number of rows before returning query results.
        /// </summary>
        /// <typeparam name="T">
        /// Entity type associated with the current query composition.
        /// </typeparam>
        /// <typeparam name="TBuilder">
        /// Fluent query builder type returned by the current composition.
        /// </typeparam>
        /// <typeparam name="TProfile">
        /// Database provider profile used to configure query features.
        /// </typeparam>
        /// <param name="queryBuilder">
        /// Query composition builder to configure.
        /// </param>
        /// <param name="count">
        /// Number of rows to skip.
        /// </param>
        /// <returns>
        /// Current query composition builder instance.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="queryBuilder"/> is <see langword="null"/>.
        /// </exception>
        public static TBuilder Skip<T, TBuilder, TProfile>(
            this IQueryCompositionCommandBuilder<T, TBuilder, TProfile> queryBuilder,
            int count)
            where TProfile : IDatabaseProviderProfile, IPaginationFeature
        {
            ArgumentNullException.ThrowIfNull(queryBuilder);

            return queryBuilder.ApplySkip(count);
        }

        /// <summary>
        /// Limits the maximum number of rows returned by the query.
        /// </summary>
        /// <typeparam name="T">
        /// Entity type associated with the current query composition.
        /// </typeparam>
        /// <typeparam name="TBuilder">
        /// Fluent query builder type returned by the current composition.
        /// </typeparam>
        /// <typeparam name="TProfile">
        /// Database provider profile used to configure query features.
        /// </typeparam>
        /// <param name="queryBuilder">
        /// Query composition builder to configure.
        /// </param>
        /// <param name="count">
        /// Maximum number of rows to return.
        /// </param>
        /// <returns>
        /// Current query composition builder instance.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="queryBuilder"/> is <see langword="null"/>.
        /// </exception>
        public static TBuilder Take<T, TBuilder, TProfile>(
            this IQueryCompositionCommandBuilder<T, TBuilder, TProfile> queryBuilder,
            int count)
            where TProfile : IDatabaseProviderProfile, IPaginationFeature
        {
            ArgumentNullException.ThrowIfNull(queryBuilder);

            return queryBuilder.ApplyTake(count);
        }
    }
}
