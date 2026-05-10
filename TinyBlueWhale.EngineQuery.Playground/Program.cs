using TinyBlueWhale.EngineQuery.Core.QueryBuilding;
using TinyBlueWhale.EngineQuery.SqlServer.Compilation;
using TinyBlueWhale.EngineQuery.SqlServer.Dialects;

var engine = new QueryEngine(new QuerySqlServerCompiler(new SqlServerDatabaseDialect()));

var sql = engine.Query<User>("Users")
    .Select(x => new { x.Id, x.Email })
    .Where(x => x.IsActive && x.Email.Contains("@gmail.com"))
    .OrderByDescending(x => x.CreatedAt)
    .Skip(20)
    .Take(10)
    .ToSql();

Console.WriteLine(sql.CommandText);

Console.WriteLine();

foreach (var parameter in sql.Parameters)
{
    Console.WriteLine($"{parameter.Name} = {parameter.Value}");
}

public sealed class User
{
    public int Id { get; set; }

    public string Email { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }
}