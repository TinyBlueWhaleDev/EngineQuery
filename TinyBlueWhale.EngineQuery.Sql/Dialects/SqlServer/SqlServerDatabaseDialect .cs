using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Sql.Dialects.Interfaces;

namespace TinyBlueWhale.EngineQuery.Sql.Dialects.SqlServer
{
    public sealed class SqlServerDatabaseDialect : ISqlDatabaseDialect
    {
        public string EscapeIdentifier(string identifier)
        {
            return $"[{identifier}]";
        }

        public string BuildPaginationClause(int? skip,int? take)
        {
            if (!skip.HasValue && !take.HasValue)
                return string.Empty;

            var offset = skip ?? 0;

            if (take.HasValue)
                return $"OFFSET {offset} ROWS FETCH NEXT {take.Value} ROWS ONLY";

            return $"OFFSET {offset} ROWS";
        }
    }
}
