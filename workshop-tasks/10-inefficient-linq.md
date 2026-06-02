# Task 10 - Inefficient LINQ

## Scenario

The search endpoint materializes products before filtering, uses `Count() > 0`, and performs avoidable in-memory filtering.

## Endpoint

```http
GET /api/products/search?term=Product
```

## Where to look

- `PlaygroundProject/Services/ProductWorkshopService.cs`

## Symptoms

- Too much data loaded from the database.
- Filtering happens in application memory.
- CPU and allocation costs grow with product count.

## How to measure

- Inspect generated SQL and database spans.
- Compare memory usage with larger product seed counts.
- Profile CPU during repeated search calls.

## Goal

Push filtering to the database and avoid unnecessary enumerations.

## Possible fixes

- Move `Where(...)` before `ToListAsync()`.
- Use `Any()` instead of `Count() > 0`.
- Use projections and server-side filtering where possible.

## Success criteria

- SQL includes the search predicate.
- Less data is transferred from DB to app.
- Search endpoint allocates less memory and responds faster.