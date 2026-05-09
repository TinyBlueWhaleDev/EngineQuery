using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Tests.Core;

namespace TinyBlueWhale.EngineQuery.Tests.Sql
{
    public class SqlServerExpectedSqlSyntax : IQueryCompilerExpectedSyntax
    {
        public string SelectAllSql =>
"""
SELECT *
FROM [Users]
""";

        public string SelectProjectionSql =>
    """
SELECT [Id], [Email]
FROM [Users]
""";

        public string BooleanWhereSql =>
    """
SELECT *
FROM [Users]
WHERE ([IsActive] = @p0)
""";

        public string NegatedBooleanWhereSql =>
    """
SELECT *
FROM [Users]
WHERE ([IsDeleted] = @p0)
""";

        public string ClosureWhereSql =>
    """
SELECT *
FROM [Users]
WHERE ([Age] >= @p0)
""";

        public string ContainsWhereSql =>
    """
SELECT *
FROM [Users]
WHERE ([Email] LIKE @p0)
""";

        public string StartsWithWhereSql => ContainsWhereSql;

        public string EndsWithWhereSql => ContainsWhereSql;

        public string MultipleAndWhereSql =>
    """
SELECT *
FROM [Users]
WHERE ((([IsActive] = @p0) AND ([Age] >= @p1)) AND ([Email] LIKE @p2))
""";

        public string OrWhereSql =>
    """
SELECT *
FROM [Users]
WHERE (([Email] LIKE @p0) OR ([Email] LIKE @p1))
""";

        public string OrderBySql =>
    """
SELECT *
FROM [Users]
ORDER BY [Email] ASC
""";

        public string OrderByDescendingSql =>
    """
SELECT *
FROM [Users]
ORDER BY [CreatedAt] DESC
""";

        public string ThenBySql =>
    """
SELECT *
FROM [Users]
ORDER BY [Email] ASC, [CreatedAt] DESC
""";

        public string PaginationSql =>
    """
SELECT *
FROM [Users]
ORDER BY [Id] ASC
OFFSET 20 ROWS FETCH NEXT 10 ROWS ONLY
""";

        public string CompleteQuerySql =>
    """
SELECT [Id], [Email]
FROM [Users]
WHERE (([IsActive] = @p0) AND ([Email] LIKE @p1))
ORDER BY [CreatedAt] DESC
OFFSET 20 ROWS FETCH NEXT 10 ROWS ONLY
""";
        public string SinglePropertySelectSql =>
"""
SELECT [Email]
FROM [Users]
""";
    }
}
