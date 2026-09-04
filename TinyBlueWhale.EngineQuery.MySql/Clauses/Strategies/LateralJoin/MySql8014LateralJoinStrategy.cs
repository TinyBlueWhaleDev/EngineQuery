using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Sql.Interfaces.Strategies;

namespace TinyBlueWhale.EngineQuery.MySql.Clauses.Strategies.LateralJoin
{
    /// <summary>
    /// Provides MySQL-specific LATERAL join rendering behavior.
    /// </summary>
    internal class MySql8014LateralJoinStrategy : ILateralJoinStrategy
    {
        /// <inheritdoc />
        public string GetJoinKeyword(QueryApplyType applyType)
        {
            return applyType switch
            {
                QueryApplyType.Cross => "JOIN LATERAL",
                QueryApplyType.Outer => "LEFT JOIN LATERAL",
                _ => throw new ArgumentOutOfRangeException(nameof(applyType), applyType, null)
            };
        }

        /// <inheritdoc />
        public string GetJoinSuffix()
        {
            return " ON TRUE";
        }
    }
}
