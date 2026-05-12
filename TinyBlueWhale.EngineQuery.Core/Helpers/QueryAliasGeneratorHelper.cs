using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.Core.Helpers
{    
    /// <summary>
    /// Generates deterministic SQL table aliases for query sources.
    /// </summary>
    public static class QueryAliasGeneratorHelper
    {
        /// <summary>
        /// Generates a SQL alias using the specified index.
        /// </summary>
        /// <param name="index">
        /// Alias index.
        /// </param>
        /// <returns>
        /// Generated SQL alias.
        /// </returns>
        public static string Generate(int index)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(index);

            return $"t{index}";
        }
    }
}
