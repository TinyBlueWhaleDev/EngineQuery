using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Sql.Interfaces.Strategies;

namespace TinyBlueWhale.EngineQuery.Sql.Clauses.LateralJoins
{
    /// <summary>
    /// Provides the default APPLY-based lateral join rendering strategy.
    /// </summary>
    public sealed class LateralJoinStrategy : ILateralJoinStrategy
    {
        /// <inheritdoc />
        public string GetJoinKeyword(QueryApplyType applyType)
        {
            return applyType switch
            {
                QueryApplyType.Cross => "CROSS APPLY",
                QueryApplyType.Outer => "OUTER APPLY",
                _ => throw new ArgumentOutOfRangeException(nameof(applyType), applyType, null)
            };
        }

        /// <inheritdoc />
        public string GetJoinSuffix()
        {
            return string.Empty;
        }
    }
}
