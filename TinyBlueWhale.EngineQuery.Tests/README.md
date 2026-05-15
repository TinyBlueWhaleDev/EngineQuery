# TinyBlueWhale.EngineQuery.Tests

Provider-specific NUnit snapshot test suite for TinyBlueWhale.EngineQuery.

This project validates deterministic SQL generation behavior across supported database providers using snapshot-based regression testing.

---

# Purpose

The test suite ensures that all query generation features produce stable, provider-correct SQL output.

The suite validates:

- SQL Server generation
- PostgreSQL generation
- MySQL generation
- Provider capability validation
- Edge-case SQL generation
- Negative validation behavior
- Deterministic query compilation

---

# Project Structure

```txt
TinyBlueWhale.EngineQuery.Tests
├── Infrastructure
├── Models
├── Providers
└── Snapshots