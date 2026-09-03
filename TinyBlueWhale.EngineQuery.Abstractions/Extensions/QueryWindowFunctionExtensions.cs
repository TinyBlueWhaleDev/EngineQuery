using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Features;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;

namespace TinyBlueWhale.EngineQuery.Abstractions.Extensions
{
    /// <summary>
    /// Provides window function query composition operations for provider profiles
    /// that support SQL window functions.
    /// </summary>
    public static class QueryWindowFunctionExtensions
    {
        /// <summary>
        /// Adds a LAG window function projection to the current query.
        /// </summary>
        /// <typeparam name="T">
        /// Root entity type associated with the current query composition.
        /// </typeparam>
        /// <typeparam name="TBuilder">
        /// Query builder type returned by the current fluent composition.
        /// </typeparam>
        /// <typeparam name="TProfile">
        /// Database provider profile type.
        /// </typeparam>
        /// <typeparam name="TEntity">
        /// Entity type containing the target column expression.
        /// </typeparam>
        /// <param name="queryBuilder">
        /// Current query composition builder.
        /// </param>
        /// <param name="expression">
        /// Expression that identifies the column used by the LAG function.
        /// </param>
        /// <param name="alias">
        /// Alias assigned to the generated SQL projection.
        /// </param>
        /// <param name="windowBuilder">
        /// Function used to configure PARTITION BY and ORDER BY clauses.
        /// </param>
        /// <param name="offset">
        /// Number of rows behind the current row to access.
        /// </param>
        /// <returns>
        /// Current query builder instance.
        /// </returns>
        public static TBuilder SelectLag<T, TBuilder, TProfile, TEntity>(this IQueryCompositionCommandBuilder<T, TBuilder, TProfile> queryBuilder, Expression<Func<TEntity, object>> expression, string alias, Func<IWindowFunctionBuilder, IWindowFunctionBuilder> windowBuilder, int offset = 1)
            where TProfile : IDatabaseProviderProfile, IWindowFunctionFeature
        {
            ArgumentNullException.ThrowIfNull(queryBuilder);

            return queryBuilder.ApplyLag(expression, alias, windowBuilder, offset);
        }

        /// <summary>
        /// Adds a LEAD window function projection to the current query.
        /// </summary>
        /// <typeparam name="T">
        /// Root entity type associated with the current query composition.
        /// </typeparam>
        /// <typeparam name="TBuilder">
        /// Query builder type returned by the current fluent composition.
        /// </typeparam>
        /// <typeparam name="TProfile">
        /// Database provider profile type.
        /// </typeparam>
        /// <typeparam name="TEntity">
        /// Entity type containing the target column expression.
        /// </typeparam>
        /// <param name="queryBuilder">
        /// Current query composition builder.
        /// </param>
        /// <param name="expression">
        /// Expression that identifies the column used by the LEAD function.
        /// </param>
        /// <param name="alias">
        /// Alias assigned to the generated SQL projection.
        /// </param>
        /// <param name="windowBuilder">
        /// Function used to configure PARTITION BY and ORDER BY clauses.
        /// </param>
        /// <param name="offset">
        /// Number of rows ahead of the current row to access.
        /// </param>
        /// <returns>
        /// Current query builder instance.
        /// </returns>
        public static TBuilder SelectLead<T, TBuilder, TProfile, TEntity>(this IQueryCompositionCommandBuilder<T, TBuilder, TProfile> queryBuilder, Expression<Func<TEntity, object>> expression, string alias, Func<IWindowFunctionBuilder, IWindowFunctionBuilder> windowBuilder, int offset = 1)
            where TProfile : IDatabaseProviderProfile, IWindowFunctionFeature
        {
            ArgumentNullException.ThrowIfNull(queryBuilder);

            return queryBuilder.ApplyLead(expression, alias, windowBuilder, offset);
        }

        /// <summary>
        /// Adds a FIRST_VALUE window function projection to the current query.
        /// </summary>
        /// <typeparam name="T">
        /// Root entity type associated with the current query composition.
        /// </typeparam>
        /// <typeparam name="TBuilder">
        /// Query builder type returned by the current fluent composition.
        /// </typeparam>
        /// <typeparam name="TProfile">
        /// Database provider profile type.
        /// </typeparam>
        /// <typeparam name="TEntity">
        /// Entity type associated with the selected window function value.
        /// </typeparam>
        /// <param name="queryBuilder">
        /// Current query composition builder.
        /// </param>
        /// <param name="expression">
        /// Expression that selects the value returned by FIRST_VALUE.
        /// </param>
        /// <param name="alias">
        /// SQL alias assigned to the FIRST_VALUE result.
        /// </param>
        /// <param name="windowBuilder">
        /// Function used to configure the window function clauses.
        /// </param>
        /// <returns>
        /// Current query builder instance.
        /// </returns>
        public static TBuilder SelectFirstValue<T, TBuilder, TProfile, TEntity>(this IQueryCompositionCommandBuilder<T, TBuilder, TProfile> queryBuilder, Expression<Func<TEntity, object>> expression, string alias, Func<IWindowFunctionBuilder, IWindowFunctionBuilder> windowBuilder)
            where TProfile : IDatabaseProviderProfile, IWindowFunctionFeature
        {
            ArgumentNullException.ThrowIfNull(queryBuilder);

            return queryBuilder.ApplyFirstValue(expression, alias, windowBuilder);
        }

        /// <summary>
        /// Adds a LAST_VALUE window function projection to the current query.
        /// </summary>
        /// <typeparam name="T">
        /// Root entity type associated with the current query composition.
        /// </typeparam>
        /// <typeparam name="TBuilder">
        /// Query builder type returned by the current fluent composition.
        /// </typeparam>
        /// <typeparam name="TProfile">
        /// Database provider profile type.
        /// </typeparam>
        /// <typeparam name="TEntity">
        /// Entity type associated with the selected window function value.
        /// </typeparam>
        /// <param name="queryBuilder">
        /// Current query composition builder.
        /// </param>
        /// <param name="expression">
        /// Expression that selects the value returned by LAST_VALUE.
        /// </param>
        /// <param name="alias">
        /// SQL alias assigned to the LAST_VALUE result.
        /// </param>
        /// <param name="windowBuilder">
        /// Function used to configure the window function clauses.
        /// </param>
        /// <returns>
        /// Current query builder instance.
        /// </returns>
        public static TBuilder SelectLastValue<T, TBuilder, TProfile, TEntity>(this IQueryCompositionCommandBuilder<T, TBuilder, TProfile> queryBuilder, Expression<Func<TEntity, object>> expression, string alias, Func<IWindowFunctionBuilder, IWindowFunctionBuilder> windowBuilder)
            where TProfile : IDatabaseProviderProfile, IWindowFunctionFeature
        {
            ArgumentNullException.ThrowIfNull(queryBuilder);

            return queryBuilder.ApplyLastValue(expression, alias, windowBuilder);
        }

        /// <summary>
        /// Adds an NTILE window function projection to the current query.
        /// </summary>
        /// <typeparam name="T">
        /// Root entity type associated with the current query composition.
        /// </typeparam>
        /// <typeparam name="TBuilder">
        /// Query builder type returned by the current fluent composition.
        /// </typeparam>
        /// <typeparam name="TProfile">
        /// Database provider profile type.
        /// </typeparam>
        /// <param name="queryBuilder">
        /// Current query composition builder.
        /// </param>
        /// <param name="buckets">
        /// Number of ranked groups used by NTILE.
        /// </param>
        /// <param name="alias">
        /// SQL alias assigned to the NTILE result.
        /// </param>
        /// <param name="windowBuilder">
        /// Function used to configure the window function clauses.
        /// </param>
        /// <returns>
        /// Current query builder instance.
        /// </returns>
        public static TBuilder SelectNtile<T, TBuilder, TProfile>(this IQueryCompositionCommandBuilder<T, TBuilder, TProfile> queryBuilder, int buckets, string alias, Func<IWindowFunctionBuilder, IWindowFunctionBuilder> windowBuilder)
            where TProfile : IDatabaseProviderProfile, IWindowFunctionFeature
        {
            ArgumentNullException.ThrowIfNull(queryBuilder);

            return queryBuilder.ApplyNtile(buckets, alias, windowBuilder);
        }

        /// <summary>
        /// Adds a ROW_NUMBER window function projection to the current query.
        /// </summary>
        /// <typeparam name="T">
        /// Root entity type associated with the current query composition.
        /// </typeparam>
        /// <typeparam name="TBuilder">
        /// Query builder type returned by the current fluent composition.
        /// </typeparam>
        /// <typeparam name="TProfile">
        /// Database provider profile type.
        /// </typeparam>
        /// <param name="queryBuilder">
        /// Current query composition builder.
        /// </param>
        /// <param name="alias">
        /// SQL alias assigned to the ROW_NUMBER result.
        /// </param>
        /// <param name="windowBuilder">
        /// Function used to configure the window function clauses.
        /// </param>
        /// <returns>
        /// Current query builder instance.
        /// </returns>
        public static TBuilder SelectRowNumber<T, TBuilder, TProfile>(this IQueryCompositionCommandBuilder<T, TBuilder, TProfile> queryBuilder, string alias, Func<IWindowFunctionBuilder, IWindowFunctionBuilder> windowBuilder)
            where TProfile : IDatabaseProviderProfile, IWindowFunctionFeature
        {
            ArgumentNullException.ThrowIfNull(queryBuilder);

            return queryBuilder.ApplyRowNumber(alias, windowBuilder);
        }

        /// <summary>
        /// Adds a RANK window function projection to the current query.
        /// </summary>
        /// <typeparam name="T">
        /// Root entity type associated with the current query composition.
        /// </typeparam>
        /// <typeparam name="TBuilder">
        /// Query builder type returned by the current fluent composition.
        /// </typeparam>
        /// <typeparam name="TProfile">
        /// Database provider profile type.
        /// </typeparam>
        /// <param name="queryBuilder">
        /// Current query composition builder.
        /// </param>
        /// <param name="alias">
        /// SQL alias assigned to the RANK result.
        /// </param>
        /// <param name="windowBuilder">
        /// Function used to configure the window function clauses.
        /// </param>
        /// <returns>
        /// Current query builder instance.
        /// </returns>
        public static TBuilder SelectRank<T, TBuilder, TProfile>(this IQueryCompositionCommandBuilder<T, TBuilder, TProfile> queryBuilder, string alias, Func<IWindowFunctionBuilder, IWindowFunctionBuilder> windowBuilder)
            where TProfile : IDatabaseProviderProfile, IWindowFunctionFeature
        {
            ArgumentNullException.ThrowIfNull(queryBuilder);

            return queryBuilder.ApplyRank(alias, windowBuilder);
        }

        /// <summary>
        /// Adds a DENSE_RANK window function projection to the current query.
        /// </summary>
        /// <typeparam name="T">
        /// Root entity type associated with the current query composition.
        /// </typeparam>
        /// <typeparam name="TBuilder">
        /// Query builder type returned by the current fluent composition.
        /// </typeparam>
        /// <typeparam name="TProfile">
        /// Database provider profile type.
        /// </typeparam>
        /// <param name="queryBuilder">
        /// Current query composition builder.
        /// </param>
        /// <param name="alias">
        /// SQL alias assigned to the DENSE_RANK result.
        /// </param>
        /// <param name="windowBuilder">
        /// Function used to configure the window function clauses.
        /// </param>
        /// <returns>
        /// Current query builder instance.
        /// </returns>
        public static TBuilder SelectDenseRank<T, TBuilder, TProfile>(this IQueryCompositionCommandBuilder<T, TBuilder, TProfile> queryBuilder, string alias, Func<IWindowFunctionBuilder, IWindowFunctionBuilder> windowBuilder)
            where TProfile : IDatabaseProviderProfile, IWindowFunctionFeature
        {
            ArgumentNullException.ThrowIfNull(queryBuilder);

            return queryBuilder.ApplyDenseRank(alias, windowBuilder);
        }
    }
}
