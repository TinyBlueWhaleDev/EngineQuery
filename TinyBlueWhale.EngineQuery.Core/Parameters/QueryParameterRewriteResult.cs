using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.Core.Parameters
{
    /// <summary>
    /// Represents the result of rewriting nested query parameters.
    /// </summary>
    public sealed record QueryParameterRewriteResult
    {
        /// <summary>
        /// Gets the rewritten SQL command text.
        /// </summary>
        public required string CommandText { get; init; }
    }
}
