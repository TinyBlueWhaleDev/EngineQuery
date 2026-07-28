# Changelog

All notable changes to EngineQuery will be documented in this file.

The format is inspired by Keep a Changelog, and this project follows Semantic Versioning.

## [1.0.0] - 2026-07-02

### Added

- Initial stable release of EngineQuery.
- Strongly typed SQL query builder API.
- Deterministic SQL generation.
- SQL Server provider.
- PostgreSQL provider.
- MySQL provider.
- Fluent metadata mapping.
- Attribute-based metadata mapping.
- Entity Framework Core metadata integration.
- Dependency Injection integration.
- Multi-provider query engine resolution.
- SELECT, DISTINCT, FROM, WHERE, GROUP BY, HAVING and ORDER BY support.
- INNER JOIN, LEFT JOIN, compound JOIN conditions and table joins.
- Subqueries, derived tables, EXISTS and NOT EXISTS support.
- IN subquery support.
- Common Table Expression support.
- Recursive Common Table Expression support.
- UNION, UNION ALL, INTERSECT and EXCEPT support.
- Computed SELECT expressions.
- Computed WHERE expressions.
- Aggregate projections.
- Computed aggregate expressions.
- Scalar SQL function projections.
- CASE WHEN projections.
- Window function projections.
- APPLY / LATERAL support.
- Pagination support.
- Provider comparison playground.
- Sample applications.
- Snapshot test coverage.
- Benchmark project.

### Notes

- EngineQuery does not execute SQL commands.
- EngineQuery does not manage database connections.
- EngineQuery is not an ORM.
- EngineQuery is designed to work well with Dapper, CQRS read models, reporting systems and SQL-heavy .NET applications.

## [1.0.1] - 2026-07-28

### Added

- Logical predicate composition through the new `QueryLogicalOperator` enum.
- Overloads for `Where` and `WhereIf` supporting explicit logical operators (`AND` / `OR`).
- Automatic SQL grouping for consecutive `OR` predicates during query compilation.

### Changed

- Improved documentation with logical predicate composition examples.
- Expanded the main README with new query composition capabilities.
- Added Lab001 demonstrating dynamic query construction using EngineQuery alongside a handcrafted SQL implementation.
- Updated Labs documentation and usage examples.

### Validation

- Validated SQL generation for SQL Server, PostgreSQL, and MySQL.
- Verified generated SQL for mixed `AND` / `OR` predicate scenarios.
- Validated Lab001 results against equivalent handcrafted SQL queries.
