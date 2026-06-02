# Task 03 - Over-fetching Data

## Scenario

Product endpoints return full product entities containing large descriptions, metadata JSON, internal notes, and audit fields even when clients only need summaries.

## Endpoints

```http
GET /api/products?take=50
GET /api/products/{id}
GET /api/products/popular
```

## Where to look

- `PlaygroundProject/Models/Product.cs`
- `PlaygroundProject/Services/ProductWorkshopService.cs`

## Symptoms

- Large JSON responses.
- High serialization time.
- More memory allocation per request.
- Network transfer becomes part of latency.

## How to measure

- Inspect response size in browser/dev tools or curl.
- Compare endpoint latency before and after DTO projections.
- Profile allocations while calling product endpoints.

## Goal

Return only fields required by the client use case.

## Possible fixes

- Create DTOs such as `ProductSummaryDto` and `ProductDetailsDto`.
- Use EF Core `Select()` projections.
- Avoid exposing internal/audit fields from public API responses.

## Success criteria

- Response payload size is much smaller.
- Serialization and allocation costs drop.
- API contract is clearer and client-focused.