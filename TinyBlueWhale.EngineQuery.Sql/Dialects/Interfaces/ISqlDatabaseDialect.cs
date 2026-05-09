using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.Sql.Dialects.Interfaces
{
    public interface ISqlDatabaseDialect
    {
        string EscapeIdentifier(string identifier);
        string BuildPaginationClause(int? skip,int? take);
    }
}
