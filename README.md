# TinyBlueWhale.EngineQuery

Provider-agnostic high-performance SQL query builder for .NET.

EngineQuery generates deterministic SQL for SQL Server, PostgreSQL and MySQL using strongly typed expressions, advanced SQL capabilities and provider-specific dialect compilation.

Built for real-world Dapper, CQRS and SQL-heavy enterprise applications.

[![NuGet](https://img.shields.io/nuget/v/TinyBlueWhale.EngineQuery.DependencyInjection.svg)](https://www.nuget.org/packages/TinyBlueWhale.EngineQuery.DependencyInjection)
[![Downloads](https://img.shields.io/nuget/dt/TinyBlueWhale.EngineQuery.DependencyInjection.svg)](https://www.nuget.org/packages/TinyBlueWhale.EngineQuery.DependencyInjection)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

---

# Why EngineQuery?

EngineQuery was created to solve a common problem in modern .NET applications:

Generating maintainable, provider-specific and deterministic SQL without the overhead and limitations of a full ORM.

Most applications eventually reach scenarios where:

* LINQ becomes difficult to maintain
* ORM-generated SQL becomes unpredictable
* Reporting queries become too complex
* Provider-specific SQL behavior matters
* Performance tuning becomes critical
* Dapper requires excessive manual SQL maintenance

EngineQuery bridges the gap between handwritten SQL and full ORM abstractions.

---

# Features

* Strongly typed query builder
* SQL Server support
* PostgreSQL support
* MySQL support
* Provider-specific SQL dialect generation
* Deterministic SQL output
* Window functions
* Recursive CTEs
* EXISTS / NOT EXISTS
* APPLY / LATERAL
* CASE WHEN
* GROUP BY / HAVING
* Aggregate functions
* Compound JOIN conditions
* Scalar functions
* Computed expressions
* UNION / INTERSECT / EXCEPT
* Fluent metadata
* Attribute metadata
* Entity Framework metadata integration
* Dependency injection support
* Multi-provider architecture
* Minimal allocations
* Lightweight infrastructure

---

# Supported Frameworks

* .NET 8
* .NET 9

---

# Installation

## Recommended Setup

```bash id="x1d0rn"
dotnet add package TinyBlueWhale.EngineQuery.DependencyInjection
```

---

# Package Structure

| Package                                            | Purpose                        |
| -------------------------------------------------- | ------------------------------ |
| TinyBlueWhale.EngineQuery.DependencyInjection      | Plug-and-play setup            |
| TinyBlueWhale.EngineQuery.Core                     | Query builder core             |
| TinyBlueWhale.EngineQuery.SqlServer                | SQL Server provider            |
| TinyBlueWhale.EngineQuery.MySql                    | MySQL provider                 |
| TinyBlueWhale.EngineQuery.PostgreSql               | PostgreSQL provider            |
| TinyBlueWhale.EngineQuery.Metadata                 | Metadata mapping               |
| TinyBlueWhale.EngineQuery.Metadata.EntityFramework | EF Core metadata integration   |
| TinyBlueWhale.EngineQuery.Abstractions             | Shared contracts               |
| TinyBlueWhale.EngineQuery.Sql                      | SQL compilation infrastructure |

---

# Choosing Packages

| Scenario                       | Recommended Package      |
| ------------------------------ | ------------------------ |
| Plug-and-play setup            | DependencyInjection      |
| Manual setup                   | Core                     |
| SQL Server only                | SqlServer                |
| PostgreSQL only                | PostgreSql               |
| MySQL only                     | MySql                    |
| EF Core metadata reuse         | Metadata.EntityFramework |
| Custom provider implementation | Sql + Abstractions       |

---

# Quick Start

## Dependency Injection

```csharp id="l1j2ov"
services.AddEngineQuery(options =>
{
    options.Add(QueryEngineProvider.SqlServer, metadata =>
    {
        metadata.UseFluentMetadata(BuildMetadataResolver.Create);
    });
});
```

---

# Building Queries

```csharp id="f0w97i"
public sealed class UserReportService
{
    private readonly IQueryEngine _queryEngine;

    public UserReportService(IQueryEngine queryEngine)
    {
        _queryEngine = queryEngine;
    }

    public GeneratedSqlQuery Build()
    {
        return _queryEngine
            .From<User>(alias: "u")
            .Select<User>(user => new
            {
                user.Id,
                user.Email
            })
            .Where<User>(user => user.IsActive)
            .OrderBy<User>(user => user.Id)
            .Build();
    }
}
```

---

# Generated SQL

```sql id="q0g0u0"
SELECT [u].[Id],
       [u].[Email]
FROM [Users] AS [u]
WHERE [u].[IsActive] = 1
ORDER BY [u].[Id] ASC
```

---

# Multi-Provider Setup

```csharp id="m74qg0"
services.AddEngineQuery(options =>
{
    options.Add(QueryEngineProvider.SqlServer, metadata =>
    {
        metadata.UseFluentMetadata(BuildSqlServerMetadata.Create);
    });

    options.Add(QueryEngineProvider.MySql, metadata =>
    {
        metadata.UseAttributeMetadata();
    });

    options.Add(QueryEngineProvider.PostgreSql, metadata =>
    {
        metadata.UseFluentMetadata(BuildPostgreSqlMetadata.Create);
    });
});
```

---

# Selecting Providers Dynamically

```csharp id="r3dyct"
public sealed class ReportService
{
    private readonly IQueryEngineFactory _factory;

    public ReportService(IQueryEngineFactory factory)
    {
        _factory = factory;
    }

    public GeneratedSqlQuery Build()
    {
        var queryEngine = _factory.Create(QueryEngineProvider.SqlServer);

        return queryEngine
            .From<User>(alias: "u")
            .Build();
    }
}
```

---

# Metadata Strategies

EngineQuery supports multiple metadata resolution strategies.

---

## Fluent Metadata

```csharp id="h5tkxv"
registry.Entity<User>()
    .ToTable("Users")
    .Property(user => user.Id)
    .HasColumnName("UserId");
```

---

## Attribute Metadata

```csharp id="bd1n5l"
[Table("Users")]
public sealed class User
{
    [Column("UserId")]
    public int Id { get; set; }
}
```

---

## Entity Framework Metadata

```csharp id="bspx3y"
services.AddEngineQuery(options =>
{
    options.Add(QueryEngineProvider.SqlServer, metadata =>
    {
        metadata.UseEntityFrameworkMetadata<ApplicationDbContext>();
    });
});
```

---

# Advanced Features

## Window Functions

```csharp id="9twgx0"
queryBuilder
    .From<Order>(alias: "o")
    .SelectRowNumber(
        "RowNumber",
        window => window
            .PartitionBy<Order>(order => order.CustomerId)
            .OrderByDescending<Order>(order => order.CreatedAt))
    .Build();
```

---

## Recursive Common Table Expressions

```csharp id="7jlwm5"
queryBuilder
    .WithRecursive<CategoryNode>(
        "CategoryTree",
        anchor => anchor,
        recursive => recursive)
    .Build();
```

---

## EXISTS

```csharp id="6z8s0f"
queryBuilder
    .From<User>(alias: "u")
    .WhereExists<Order>(
        subquery => subquery
            .From<Order>(alias: "o")
            .Where<Order>(order => order.UserId == 1))
    .Build();
```

---

## APPLY / LATERAL

```csharp id="l69dfx"
queryBuilder
    .From<User>(alias: "u")
    .CrossApply<Order>(
        "orders",
        apply => apply
            .From<Order>(alias: "o"))
    .Build();
```

---

## Aggregate Functions

```csharp id="3grc50"
queryBuilder
    .From<Order>(alias: "o")
    .SelectSum<Order>(
        order => order.Total,
        "TotalSales")
    .GroupBy<Order>(order => order.CustomerId)
    .Build();
```

---

## CASE WHEN

```csharp id="jjlwm3"
queryBuilder
    .From<Order>(alias: "o")
    .SelectCase(
        "Status",
        builder => builder
            .When<Order>(order => order.Total > 1000, "VIP")
            .Else("Standard"))
    .Build();
```

---

# Designed For

* Dapper applications
* CQRS read models
* Reporting systems
* High-performance APIs
* SQL-heavy applications
* Enterprise reporting
* Legacy SQL migrations
* Multi-provider architectures
* Provider-specific SQL optimization

---

# Not Intended For

EngineQuery is not an ORM.

It does not provide:

* Change tracking
* Lazy loading
* Entity persistence
* IQueryable providers
* Automatic migrations
* LINQ execution providers

---

# Architecture

EngineQuery uses a provider-agnostic query compilation pipeline.

The architecture is composed of:

* Query builders
* Query definitions
* SQL clause builders
* SQL dialects
* Provider capabilities
* Metadata resolvers
* Query compilers

This allows deterministic SQL generation while maintaining provider-specific behavior.

---

# Benchmarks

Benchmarks are available in the repository benchmark project.

Current benchmark scenarios include:

* Basic select queries
* JOIN aggregation
* EXISTS subqueries
* Derived tables
* Common table expressions
* Window functions
* Set operations

---

# Samples

Sample applications are available in the repository:

* Playground
* Sample applications
* Provider comparison validators
* Benchmark project

---

# Repository

https://github.com/TinyBlueWhaleDev/EngineQuery

---

# License

MIT
