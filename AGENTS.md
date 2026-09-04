# Contributor Guide

## Purpose

This repository is a learning-oriented .NET 10 sample that builds an online ordering system using Domain-Driven Design (DDD). Keep the business model at the centre of the design: model business behaviour and rules in code instead of exposing mutable data structures.

## Repository layout

- `src/Ordering.Domain` — business concepts, aggregates, value objects, domain rules, and domain events. It must not depend on other solution projects or infrastructure libraries.
- `src/Ordering.Application` — use cases and orchestration. It may depend on `Ordering.Domain` only.
- `src/Ordering.Infrastructure` — persistence and external-service implementations. It may depend on Application and Domain.
- `src/Ordering.Api` — HTTP composition root and delivery concerns. It depends on Application.
- `tests/Ordering.Domain.Tests` and `tests/Ordering.Application.Tests` — xUnit tests for the corresponding layers.

Preserve these dependency directions. Do not introduce references from Domain to Application, Infrastructure, or Api, and do not put persistence or HTTP concerns in Domain.

## Domain-model conventions

- Prefer behaviour-rich aggregates with private setters. Create or transition them through named methods that enforce invariants.
- Validate inputs at the boundary of each public domain operation and throw a meaningful exception when a rule is violated.
- Keep state transitions explicit. Add tests for the valid transition and each invalid transition.
- Use UTC (`DateTime.UtcNow`) for timestamps.
- Keep comments for intent that code alone cannot express; do not add comments that restate an identifier.

## C# conventions

- Target `net10.0`, keep nullable reference types enabled, and use file-scoped namespaces for new files.
- Follow existing naming: PascalCase for types and members; `Async` suffix for asynchronous methods.
- Use XML documentation for public domain APIs when their business meaning, constraints, or exceptions are not self-evident.
- Do not add package dependencies without a concrete need.

## Verification

Run these from the repository root before handing off changes:

```powershell
dotnet build OrderingSystem.slnx
dotnet test OrderingSystem.slnx
```

For an API smoke test, run `dotnet run --project src/Ordering.Api` and call `GET /health`.

## Change checklist

1. Put new code in the correct DDD layer and preserve project-reference direction.
2. Add or update focused xUnit tests for behaviour changes.
3. Build and test the solution.
4. Update `README.md` when a user-facing concept or setup instruction changes.
