using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyBlueWhale.EngineQuery.Tests.Core
{
    public interface IQueryCompilerExpectedSyntax
    {
        string SelectAllSql { get; }
        string SelectProjectionSql { get; }
        string BooleanWhereSql { get; }
        string NegatedBooleanWhereSql { get; }
        string ClosureWhereSql { get; }
        string ContainsWhereSql { get; }
        string StartsWithWhereSql { get; }
        string EndsWithWhereSql { get; }
        string MultipleAndWhereSql { get; }
        string OrWhereSql { get; }
        string OrderBySql { get; }
        string OrderByDescendingSql { get; }
        string ThenBySql { get; }
        string PaginationSql { get; }
        string CompleteQuerySql { get; }

        string SinglePropertySelectSql { get; }
    }
}
