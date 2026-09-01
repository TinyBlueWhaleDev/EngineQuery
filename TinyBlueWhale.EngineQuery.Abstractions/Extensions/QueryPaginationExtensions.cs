using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        /// Defines the number of rows to skip from the query result.
        /// </summary>
        /// <typeparam name="T">
        /// Entity type used as the source of the query.
        /// </typeparam>
        /// <typeparam name="TBuilder">
        /// Fluent builder type associated with the current query composition.
        /// </typeparam>
        /// <typeparam name="TProfile">
        /// Database provider profile type.
        /// </typeparam>
        /// <param name="queryBuilder">
        /// Current query composition builder.
        /// </param>
        /// <param name="count">
        /// Number of rows to skip.
        /// </param>
        /// <returns>
        /// Current query composition builder instance.
        /// </returns>
        public static TBuilder Skip<T, TBuilder, TProfile>(this IQueryCompositionCommandBuilder<T, TBuilder, TProfile> queryBuilder, int count)
            where TProfile : IDatabaseProviderProfile, IPaginationFeature
        {
            ArgumentNullException.ThrowIfNull(queryBuilder);

            return queryBuilder.ApplyFeature(new PaginationSkipOperation(count));
        }

        /// <summary>
        /// Defines the maximum number of rows returned by the query.
        /// </summary>
        /// <typeparam name="T">
        /// Entity type used as the source of the query.
        /// </typeparam>
        /// <typeparam name="TBuilder">
        /// Fluent builder type associated with the current query composition.
        /// </typeparam>
        /// <typeparam name="TProfile">
        /// Database provider profile type.
        /// </typeparam>
        /// <param name="queryBuilder">
        /// Current query composition builder.
        /// </param>
        /// <param name="count">
        /// Maximum number of rows to return.
        /// </param>
        /// <returns>
        /// Current query composition builder instance.
        /// </returns>
        public static TBuilder Take<T, TBuilder, TProfile>(this IQueryCompositionCommandBuilder<T, TBuilder, TProfile> queryBuilder, int count)
            where TProfile : IDatabaseProviderProfile, IPaginationFeature
        {
            ArgumentNullException.ThrowIfNull(queryBuilder);

            return queryBuilder.ApplyFeature(new PaginationTakeOperation(count));
        }
    }
}
