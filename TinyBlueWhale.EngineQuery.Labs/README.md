# Lab 001 — Dynamic Order Search

## Objective

Compare two different approaches for building dynamic SQL queries for the same use case:

1. Handwritten Raw SQL.
2. SQL generated with **TinyBlueWhale.EngineQuery** using a strongly typed fluent API.

This lab does not attempt to declare a winner. Each approach has different strengths:

- **Raw SQL** provides complete transparency and full control over the generated statement.
- **EngineQuery** reduces manual SQL string manipulation while keeping the generated SQL fully visible before execution.

In both implementations, **Dapper** remains responsible for executing the generated SQL.

> **TinyBlueWhale.EngineQuery is not an ORM.** It does not manage database connections, execute commands, track entities, or replace Dapper. Its responsibility is generating deterministic SQL from strongly typed expressions.

---

# Comparison

| Feature | Raw SQL | EngineQuery |
|---|---|---|
| FROM / JOIN | Manual SQL strings | `From<T>` and `InnerJoin<TLeft,TRight>` |
| Dynamic filters | `if`, `StringBuilder`, manual synchronization | `WhereIf` with strongly typed expressions |
| Logical predicates | Manual `AND` / `OR` | `QueryLogicalOperator` |
| Parameters | Manual `DynamicParameters` | Automatically generated |
| Text search | Manual `LIKE` clauses | `Contains` expressions |
| Sorting | SQL column whitelist | `OrderBy` / `ThenBy` |
| Pagination | Manual `OFFSET/FETCH` | `Skip` / `Take` |
| Count query | Independent SQL | Independent typed query |
| Generated SQL | Source code | Available after `Build()` |
| Execution | Dapper | Dapper |
| Incidental code | Higher | Lower |

Both implementations produce exactly the same functional result:

- identical filtering;
- identical ordering;
- identical pagination;
- identical total count;
- identical response model.

The only difference is how the SQL statement is constructed.

---

# EngineQuery Features Used

This lab uses only the public EngineQuery API.

The following features are demonstrated:

- `From<T>`
- `InnerJoin<TLeft, TRight>`
- `Select<T>`
- `SelectAggregate<T>`
- `Where`
- `WhereIf`
- `Where<TEntity>`
- `WhereIf<TEntity>`
- `QueryLogicalOperator`
- `OrderBy`
- `OrderByDescending`
- `ThenBy`
- `ThenByDescending`
- `Skip`
- `Take`
- `Build`

The lab also demonstrates logical predicate composition using `QueryLogicalOperator.Or`.

Example:

```csharp
.WhereIf(
    hasSearch,
    order => order.OrderNumber.Contains(search),
    QueryLogicalOperator.Or)
.WhereIf<Customer>(
    hasSearch,
    customer =>
        customer.FirstName.Contains(search) ||
        customer.LastName.Contains(search) ||
        customer.Email.Contains(search),
    QueryLogicalOperator.Or)
```

Which generates SQL equivalent to:

```sql
WHERE
    ...
AND
(
    OrderNumber LIKE @p0
    OR FirstName LIKE @p1
    OR LastName LIKE @p2
    OR Email LIKE @p3
)
```

Consecutive `OR` predicates are automatically grouped while preserving deterministic parameter ordering.

---

# Database

Execute the scripts in the following order:

```powershell
sqlcmd -S "(localdb)\MSSQLLocalDB" -E -b -i Infrastructure/Persistence/Database/Schema/CreateDatabase.sql
sqlcmd -S "(localdb)\MSSQLLocalDB" -E -b -i Infrastructure/Persistence/Database/Schema/CreateSchema.sql
sqlcmd -S "(localdb)\MSSQLLocalDB" -E -b -i Infrastructure/Persistence/Database/Seed/SeedData.sql
```

Configure the SQL Server connection string:

```json
{
  "SqlServer": {
    "ConnectionString": "Server=(localdb)\\MSSQLLocalDB;Database=TinyBlueWhaleEngineQueryLabs;Integrated Security=true;TrustServerCertificate=true"
  }
}
```

---

# API

The lab exposes two endpoints:

- `POST /api/labs/001/orders/raw`
- `POST /api/labs/001/orders/engine-query`

Both endpoints receive exactly the same request model.

Example request body:

```json
{
  "search": "03",
  "customerId": 3,
  "status": 3,
  "createdFromUtc": "2024-01-01T00:00:00Z",
  "createdToUtc": "2024-12-31T23:59:59Z",
  "minimumTotal": 100.00,
  "maximumTotal": 5000.00,
  "page": 1,
  "pageSize": 10,
  "sortBy": "TotalAmount",
  "sortDirection": "desc"
}
```

Available filters:

- `search`
- `customerId`
- `status`
- `createdFromUtc`
- `createdToUtc`
- `minimumTotal`
- `maximumTotal`
- `page`
- `pageSize`
- `sortBy`
- `sortDirection`

Example using `curl`:

```bash
curl -X POST "http://localhost:5214/api/labs/001/orders/raw" \
-H "Content-Type: application/json" \
-d @request.json
```

```bash
curl -X POST "http://localhost:5214/api/labs/001/orders/engine-query" \
-H "Content-Type: application/json" \
-d @request.json
```

---

# Generated SQL

EngineQuery generates fully parameterized SQL.

Example:

```sql
SELECT
    [o].[Id] AS [OrderId],
    [o].[OrderNumber],
    [o].[OrderDateUtc],
    [o].[Status],
    [o].[TotalAmount],
    [o].[CustomerId],
    [c].[FirstName] AS [CustomerFirstName],
    [c].[LastName] AS [CustomerLastName],
    [c].[Email] AS [CustomerEmail]
FROM [Orders] AS [o]
INNER JOIN [Customers] AS [c]
    ON ([o].[CustomerId] = [c].[Id])
WHERE
    ([o].[CustomerId] = @p0)
AND
    ([o].[Status] = @p1)
AND
(
    ([o].[OrderNumber] LIKE @p2)
    OR
    (
        ([c].[FirstName] LIKE @p3)
        OR
        ([c].[LastName] LIKE @p4)
        OR
        ([c].[Email] LIKE @p5)
    )
)
ORDER BY
    [o].[TotalAmount] DESC,
    [o].[Id] ASC
OFFSET 0 ROWS
FETCH NEXT 10 ROWS ONLY;
```

---

# Logging

Both implementations log the generated SQL and parameter list when the application is running with the **Debug** log level.

The generated SQL is never returned as part of the HTTP response.

---

# Design Notes

For educational purposes, the lab keeps the data query and count query independent.

In a production application, the common filter construction could be extracted into a reusable component or specification to avoid duplication between both queries.

---

# What This Lab Demonstrates

- Dynamic SQL generation.
- Strongly typed joins.
- Optional filters.
- Dynamic text search.
- Logical predicate composition (`AND` / `OR`).
- Dynamic sorting.
- Pagination.
- Independent count queries.
- Parameterized SQL generation.
- Dapper integration.
- SQL generation without manual string concatenation.
