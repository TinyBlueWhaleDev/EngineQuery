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
    /// Provides INSERT identity retrieval operations whose availability depends
    /// on the active database provider profile.
    /// </summary>
    public static class QueryInsertIdentityExtensions
    {
        /// <summary>
        /// Configures the INSERT command to return the generated identity value
        /// using the scalar identity mechanism supported by the current provider.
        /// </summary>
        /// <typeparam name="T">
        /// Entity type associated with the target INSERT table.
        /// </typeparam>
        /// <typeparam name="TProfile">
        /// Database provider profile associated with the INSERT command.
        /// </typeparam>
        /// <param name="queryBuilder">
        /// INSERT VALUES command builder to configure.
        /// </param>
        /// <returns>
        /// Current INSERT VALUES command builder instance.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="queryBuilder"/> is <see langword="null"/>.
        /// </exception>
        public static IInsertValuesCommandBuilder<T, TProfile> ReturnIdentity<T, TProfile>(this IInsertValuesCommandBuilder<T, TProfile> queryBuilder)
            where TProfile : IDatabaseProviderProfile, IInsertScalarIdentityFeature
        {
            ArgumentNullException.ThrowIfNull(queryBuilder);

            return queryBuilder.ApplyReturnIdentity();
        }

        /// <summary>
        /// Configures the INSERT command to return the generated identity value
        /// using the selected target identity column.
        /// </summary>
        /// <typeparam name="T">
        /// Entity type associated with the target INSERT table.
        /// </typeparam>
        /// <typeparam name="TProfile">
        /// Database provider profile associated with the INSERT command.
        /// </typeparam>
        /// <typeparam name="TProperty">
        /// Property type associated with the generated identity.
        /// </typeparam>
        /// <param name="queryBuilder">
        /// INSERT VALUES command builder to configure.
        /// </param>
        /// <param name="identitySelector">
        /// Expression that selects the entity property associated with the generated identity.
        /// </param>
        /// <returns>
        /// Current INSERT VALUES command builder instance.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="queryBuilder"/> or
        /// <paramref name="identitySelector"/> is <see langword="null"/>.
        /// </exception>
        public static IInsertValuesCommandBuilder<T, TProfile> ReturnIdentity<T, TProfile, TProperty>(this IInsertValuesCommandBuilder<T, TProfile> queryBuilder, Expression<Func<T, TProperty>> identitySelector)
            where TProfile : IDatabaseProviderProfile, IInsertReturningIdentityFeature
        {
            ArgumentNullException.ThrowIfNull(queryBuilder);
            ArgumentNullException.ThrowIfNull(identitySelector);

            return queryBuilder.ApplyReturnIdentity(identitySelector);
        }
    }
}
