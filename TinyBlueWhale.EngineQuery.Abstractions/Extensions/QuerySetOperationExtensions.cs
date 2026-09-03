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
    /// Provides feature-gated set operation extensions for query composition builders.
    /// </summary>
    public static class QuerySetOperationExtensions
    {
        /// <summary>
        /// Adds an <c>INTERSECT</c> query to the current query.
        /// </summary>
        /// <typeparam name="T">
        /// Entity type represented by the current query composition.
        /// </typeparam>
        /// <typeparam name="TBuilder">
        /// Concrete builder type returned by the fluent query composition.
        /// </typeparam>
        /// <typeparam name="TProfile">
        /// Database provider profile associated with the query.
        /// </typeparam>
        /// <typeparam name="TSet">
        /// Root entity type used by the intersected query.
        /// </typeparam>
        /// <param name="queryBuilder">
        /// Query composition builder to extend.
        /// </param>
        /// <param name="setBuilder">
        /// Function used to build the query participating in the <c>INTERSECT</c> operation.
        /// </param>
        /// <returns>
        /// Current query builder instance.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="queryBuilder"/> or <paramref name="setBuilder"/> is <see langword="null"/>.
        /// </exception>
        public static TBuilder Intersect<T, TBuilder, TProfile, TSet>(this IQueryCompositionCommandBuilder<T, TBuilder, TProfile> queryBuilder, Func<IQueryBuilder<TProfile>, IQueryCommandBuilder<TSet, TProfile>> setBuilder)
            where TProfile : IDatabaseProviderProfile, IIntersectFeature
        {
            ArgumentNullException.ThrowIfNull(queryBuilder);
            ArgumentNullException.ThrowIfNull(setBuilder);

            return queryBuilder.ApplyIntersect(setBuilder);
        }

        /// <summary>
        /// Adds an <c>EXCEPT</c> query to the current query.
        /// </summary>
        /// <typeparam name="T">
        /// Entity type represented by the current query composition.
        /// </typeparam>
        /// <typeparam name="TBuilder">
        /// Concrete builder type returned by the fluent query composition.
        /// </typeparam>
        /// <typeparam name="TProfile">
        /// Database provider profile associated with the query.
        /// </typeparam>
        /// <typeparam name="TSet">
        /// Root entity type used by the excepted query.
        /// </typeparam>
        /// <param name="queryBuilder">
        /// Query composition builder to extend.
        /// </param>
        /// <param name="setBuilder">
        /// Function used to build the query participating in the <c>EXCEPT</c> operation.
        /// </param>
        /// <returns>
        /// Current query builder instance.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="queryBuilder"/> or <paramref name="setBuilder"/> is <see langword="null"/>.
        /// </exception>
        public static TBuilder Except<T, TBuilder, TProfile, TSet>(this IQueryCompositionCommandBuilder<T, TBuilder, TProfile> queryBuilder, Func<IQueryBuilder<TProfile>, IQueryCommandBuilder<TSet, TProfile>> setBuilder)
            where TProfile : IDatabaseProviderProfile, IExceptFeature
        {
            ArgumentNullException.ThrowIfNull(queryBuilder);
            ArgumentNullException.ThrowIfNull(setBuilder);

            return queryBuilder.ApplyExcept(setBuilder);
        }
    }
}
