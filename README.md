# TinyBlueWhale.EngineQuery

Typed provider-agnostic SQL script generation engine for .NET.

EngineQuery generates deterministic SQL command text and parameters from strongly typed C# expressions.

It is designed for .NET applications that need SQL control without spreading hardcoded SQL strings across the codebase.

---

## Why EngineQuery Exists

A common architecture in .NET systems is:

```txt
EF Core for writes
Dapper for reads
```

This works well.

EF Core handles change tracking, transactions, migrations and write-side consistency.

Dapper gives full SQL control and high-performance read models.

But as the read side grows, teams often end up with:

* large SQL strings embedded in code
* duplicated queries
* fragile aliases
* hardcoded table names
* hardcoded column names
* runtime-only SQL errors
* provider-specific incompatibilities
* painful database migrations
* difficult refactors

EngineQuery was created to solve that specific problem.

The goal is simple:

```txt
Generate provider-compatible SQL scripts using typed expressions instead of magic strings.
```

---

## What EngineQuery Is

EngineQuery is a SQL script generation engine.

It generates:

* SQL command text
* SQL parameters

The generated output can be passed to:

* Dapper
* ADO.NET
* EF Core raw SQL
* custom execution pipelines

---

## What EngineQuery Is Not

EngineQuery is not an ORM.

EngineQuery does not:

* execute SQL
* open connections
* manage transactions
* track entities
* materialize objects
* replace EF Core
* replace Dapper
* implement repositories
* implement Unit of Work
* run migrations

EngineQuery only generates SQL scripts.

---

## Current V1 Scope

V1 is focused only on:

```txt
Typed provider-agnostic SELECT query generation.
```

Write-side generation is intentionally not part of V1.

Out of scope for V1:

* INSERT generation
* UPDATE generation
* DELETE generation
* MERGE generation
* UPSERT generation
* query execution
* ORM behavior

---

## Supported Providers

| Provider   | Status    |
| ---------- | --------- |
| SQL Server | Supported |
| PostgreSQL | Supported |
| MySQL      | Supported |

---

## Supported .NET Versions

| Target | Status    |
| ------ | --------- |
| .NET 8 | Supported |
| .NET 9 | Supported |

---

## Package Ecosystem

EngineQuery is split into focused packages.

| Package                                  | Purpose                                        |
| ---------------------------------------- | ---------------------------------------------- |
| `TinyBlueWhale.EngineQuery.Abstractions` | Public contracts, models and shared enums      |
| `TinyBlueWhale.EngineQuery.Core`         | Query builders, query definitions and metadata |
| `TinyBlueWhale.EngineQuery.Sql`          | Shared SQL compilation infrastructure          |
| `TinyBlueWhale.EngineQuery.SqlServer`    | SQL Server dialect, compiler and capabilities  |
| `TinyBlueWhale.EngineQuery.PostgreSql`   | PostgreSQL dialect, compiler and capabilities  |
| `TinyBlueWhale.EngineQuery.MySql`        | MySQL dialect, compiler and capabilities       |

---

## Installation

### Core Packages

```bash
dotnet add package TinyBlueWhale.EngineQuery.Abstractions
dotnet add package TinyBlueWhale.EngineQuery.Core
dotnet add package TinyBlueWhale.EngineQuery.Sql
```

### SQL Server

```bash
dotnet add package TinyBlueWhale.EngineQuery.SqlServer
```

### PostgreSQL

```bash
dotnet add package TinyBlueWhale.EngineQuery.PostgreSql
```

### MySQL

```bash
dotnet add package TinyBlueWhale.EngineQuery.MySql
```

---

## Feature Matrix

| Feature                    |   SQL Server |   PostgreSQL |         MySQL |
| -------------------------- | -----------: | -----------: | ------------: |
| SELECT                     |          Yes |          Yes |           Yes |
| WHERE                      |          Yes |          Yes |           Yes |
| JOIN                       |          Yes |          Yes |           Yes |
| GROUP BY                   |          Yes |          Yes |           Yes |
| HAVING                     |          Yes |          Yes |           Yes |
| DISTINCT                   |          Yes |          Yes |           Yes |
| Pagination                 | OFFSET/FETCH | LIMIT/OFFSET |  LIMIT/OFFSET |
| Aggregate Functions        |          Yes |          Yes |           Yes |
| Scalar Functions           |          Yes |          Yes |           Yes |
| Computed Expressions       |          Yes |          Yes |           Yes |
| CASE WHEN                  |          Yes |          Yes |           Yes |
| EXISTS / NOT EXISTS        |          Yes |          Yes |           Yes |
| IN Subqueries              |          Yes |          Yes |           Yes |
| Derived Tables             |          Yes |          Yes |           Yes |
| CTE                        |          Yes |          Yes |      MySQL 8+ |
| Recursive CTE              |          Yes |          Yes |      MySQL 8+ |
| UNION                      |          Yes |          Yes |           Yes |
| UNION ALL                  |          Yes |          Yes |           Yes |
| INTERSECT                  |          Yes |          Yes | MySQL 8.0.31+ |
| EXCEPT                     |          Yes |          Yes | MySQL 8.0.31+ |
| CROSS APPLY / LATERAL      |        APPLY |      LATERAL | MySQL 8.0.14+ |
| OUTER APPLY / LEFT LATERAL |        APPLY |      LATERAL | MySQL 8.0.14+ |
| Window Functions           |          Yes |          Yes |      MySQL 8+ |
| Version Capabilities       |          Yes |          Yes |           Yes |

---

## Query Features

### Core Query Features

* SELECT projections
* WHERE expressions
* ORDER BY
* THEN BY
* Pagination
* DISTINCT
* INNER JOIN
* LEFT JOIN
* GROUP BY
* HAVING

### Advanced Query Features

* Aggregate functions
* Scalar functions
* Computed expressions
* CASE WHEN expressions
* EXISTS
* NOT EXISTS
* IN subqueries
* Correlated subqueries
* Derived tables
* Common Table Expressions
* Recursive Common Table Expressions
* UNION
* UNION ALL
* INTERSECT
* EXCEPT
* CROSS APPLY
* OUTER APPLY
* LATERAL joins
* Window functions

### Window Functions

* ROW_NUMBER
* RANK
* DENSE_RANK
* LAG
* LEAD
* FIRST_VALUE
* LAST_VALUE
* NTILE

---

## Quick Start

### Entity

```csharp
public sealed class User
{
    public int Id { get; set; }

    public string Email { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }
}
```

### Metadata Configuration

EngineQuery uses metadata to map C# properties to SQL tables and columns.

```csharp
var registry = new EntityMetadataRegistry();

registry.Entity<User>()
    .ToTable("users")
    .Property(x => x.Id).HasColumnName("user_id")
    .Property(x => x.Email).HasColumnName("email")
    .Property(x => x.IsActive).HasColumnName("is_active")
    .Property(x => x.CreatedAt).HasColumnName("created_at");

var metadataResolver = new FluentEntityMetadataResolver(registry);
```

### SQL Server Query Builder

```csharp
var queryBuilder = new QueryBuilder(
    new SqlServerQueryCompiler(
        new SqlServerDatabaseDialect(),
        new SqlServerProviderCapabilities()),
    metadataResolver);
```

---

## Basic SELECT Example

```csharp
var generatedQuery = queryBuilder
    .From<User>(alias: "u")
    .Select<User>(u => new
    {
        UserId = u.Id,
        u.Email
    })
    .Where<User>(u => u.IsActive)
    .OrderByDescending<User>(u => u.CreatedAt)
    .Skip(20)
    .Take(10)
    .Build();
```

Generated SQL Server output:

```sql
SELECT [u].[user_id] AS [UserId], [u].[email]
FROM [users] AS [u]
WHERE ([u].[is_active] = @p0)
ORDER BY [u].[created_at] DESC
OFFSET 20 ROWS FETCH NEXT 10 ROWS ONLY
```

Parameters:

```txt
@p0 = True
```

Generated PostgreSQL output:

```sql
SELECT "u"."user_id" AS "UserId", "u"."email"
FROM "users" AS "u"
WHERE ("u"."is_active" = @p0)
ORDER BY "u"."created_at" DESC
LIMIT 10 OFFSET 20
```

Generated MySQL output:

```sql
SELECT `u`.`user_id` AS `UserId`, `u`.`email`
FROM `users` AS `u`
WHERE (`u`.`is_active` = @p0)
ORDER BY `u`.`created_at` DESC
LIMIT 10 OFFSET 20
```

---

## Why Not Just Dapper?

Dapper is excellent for execution.

EngineQuery is not trying to replace Dapper.

EngineQuery helps before execution by generating SQL and parameters from typed expressions.

A typical usage is:

```txt
EngineQuery generates SQL
Dapper executes SQL
```

---

## Why Not Just EF Core?

EF Core is excellent for write-side workflows, tracking, migrations and transactional consistency.

EngineQuery is useful when you want explicit SQL control for read-side queries without embedding raw SQL strings everywhere.

A typical architecture is:

```txt
EF Core for writes
EngineQuery + Dapper for reads
```

---

## Testing

EngineQuery uses provider-specific snapshot testing.

The test suite validates:

* SQL Server output
* PostgreSQL output
* MySQL output
* deterministic SQL generation
* provider capabilities
* provider version gates
* edge cases
* negative validations

Run tests:

```bash
dotnet test
```

Regenerate snapshots intentionally:

```powershell
$env:ENGINEQUERY_UPDATE_SNAPSHOTS="true"
dotnet test
$env:ENGINEQUERY_UPDATE_SNAPSHOTS=$null
```

---

## Current Limitations

V1 limitations:

* SELECT generation only
* no SQL execution
* no INSERT generation
* no UPDATE generation
* no DELETE generation
* no MERGE / UPSERT generation
* no LINQ provider implementation
* no query materialization
* no entity tracking
* no transaction management

---

## Roadmap

### V1

Status: feature frozen.

* Typed SELECT query generation
* Multi-provider SQL generation
* Advanced SELECT features
* Provider capabilities
* Version capabilities
* Deterministic parameter generation
* Deterministic SQL formatting
* Snapshot validation

### V2

Planned:

* INSERT script generation
* UPDATE script generation
* DELETE script generation
* write-side provider capabilities
* write-side snapshots

### V3

Planned:

* UPSERT / MERGE script generation
* custom SQL function registry
* controlled raw SQL escape hatches
* advanced provider extensibility

---

## Contributing

Feedback, issues, ideas and constructive criticism are welcome.

If you find an unsupported query scenario or provider-specific SQL issue, please open an issue with:

* provider
* provider version
* generated SQL
* expected SQL
* minimal C# query example
* package version

---

## License

MIT
