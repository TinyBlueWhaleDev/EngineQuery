using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Features;
using TinyBlueWhale.EngineQuery.Abstractions.Interfaces.Providers;

namespace TinyBlueWhale.EngineQuery.Abstractions.Interfaces
{
    /// <summary>
    /// Defines APPLY and LATERAL join operations available to query command builders
    /// whose provider profile supports lateral joins.
    /// </summary>
    /// <typeparam name="T">
    /// Entity type represented by the current query command.
    /// </typeparam>
    /// <typeparam name="TProfile">
    /// Database provider profile associated with the query.
    /// </typeparam>
    public interface ILateralJoinQueryCommandBuilder<T, TProfile> : IQueryCommandBuilder<T, TProfile>
        where TProfile : IDatabaseProviderProfile, ILateralJoinFeature
    {
        /// <summary>
        /// Adds a CROSS APPLY or provider-equivalent LATERAL join to the current query.
        /// </summary>
        /// <typeparam name="TApply">
        /// Root entity type used by the APPLY subquery.
        /// </typeparam>
        /// <param name="alias">
        /// Alias assigned to the APPLY subquery.
        /// </param>
        /// <param name="applyBuilder">
        /// Function used to build the APPLY subquery.
        /// </param>
        /// <returns>
        /// Current query command builder instance.
        /// </returns>
        IQueryCommandBuilder<T, TProfile> CrossApply<TApply>(string alias, Func<IQueryCommandBuilder<TApply, TProfile>, IQueryCommandBuilder<TApply, TProfile>> applyBuilder);

        /// <summary>
        /// Adds an OUTER APPLY or provider-equivalent LEFT LATERAL join to the current query.
        /// </summary>
        /// <typeparam name="TApply">
        /// Root entity type used by the APPLY subquery.
        /// </typeparam>
        /// <param name="alias">
        /// Alias assigned to the APPLY subquery.
        /// </param>
        /// <param name="applyBuilder">
        /// Function used to build the APPLY subquery.
        /// </param>
        /// <returns>
        /// Current query command builder instance.
        /// </returns>
        IQueryCommandBuilder<T, TProfile> OuterApply<TApply>(string alias, Func<IQueryCommandBuilder<TApply, TProfile>, IQueryCommandBuilder<TApply, TProfile>> applyBuilder);
    }
}
