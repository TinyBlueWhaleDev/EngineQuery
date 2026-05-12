using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Core.Interfaces;

namespace TinyBlueWhale.EngineQuery.MySql.Dialects
{
    public sealed class MySqlDatabaseDialect : ISqlDatabaseDialect
    {
        public string EscapeIdentifier(string identifier) => $"`{identifier}`";

        public string BuildPaginationClause(int? skip, int? take)
        {
            if (!skip.HasValue && !take.HasValue)
                return string.Empty;

            if (take.HasValue && skip.HasValue)
                return $"LIMIT {take.Value} OFFSET {skip.Value}";


            if (take.HasValue)
                return $"LIMIT {take.Value}";

            return $"OFFSET {skip!.Value}";
        }
    }
}
