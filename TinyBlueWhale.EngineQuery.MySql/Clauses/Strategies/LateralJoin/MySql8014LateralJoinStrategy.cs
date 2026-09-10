using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Sql.Interfaces.Strategies;

namespace TinyBlueWhale.EngineQuery.MySql.Clauses.Strategies.LateralJoin
{
    /// <summary>
    /// Provides MySQL-specific LATERAL join rendering behavior.
    /// </summary>
    internal class MySql8014LateralJoinStrategy : ILateralJoinStrategy
    {
        /// <summary>
        /// Resolves the MySQL LATERAL join keyword for the specified apply type.
        /// </summary>
        /// <param name="applyType">
        /// Apply type used to determine the LATERAL join syntax.
        /// </param>
        /// <returns>
        /// MySQL LATERAL join keyword.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="applyType"/> is not supported.
        /// </exception>
        public string GetJoinKeyword(QueryApplyType applyType)
        {
            return applyType switch
            {
                QueryApplyType.Cross => "JOIN LATERAL",
                QueryApplyType.Outer => "LEFT JOIN LATERAL",
                _ => throw new ArgumentOutOfRangeException(nameof(applyType), applyType, null)
            };
        }

        /// <summary>
        /// Gets the suffix required by MySQL LATERAL joins.
        /// </summary>
        /// <returns>
        /// The MySQL LATERAL join suffix.
        /// </returns>
        public string GetJoinSuffix()
        {
            return " ON TRUE";
        }
    }
}
