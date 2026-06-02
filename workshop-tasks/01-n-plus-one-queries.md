# Task 01 - N+1 Database Queries

## Scenario

The product listing endpoint loads a page of products, then loads each product's category and reviews separately.

## Endpoint

```http
GET /api/products?take=50
```

## Where to look

- `PlaygroundProject/Services/ProductWorkshopService.cs`
- Comments marked `// WORKSHOP: Performance Issue`

## Symptoms

- One request produces many SQL/database spans.
- Latency grows as `take` increases.
- Database duration dominates request duration.

## How to measure

- Open Swagger and call the endpoint with `take=10`, `take=50`, and `take=100`.
- Run `node loadtest/speedyshop-load.js` and watch request latency.
- Inspect OpenTelemetry traces for repeated review/category queries.

## Goal

Reduce SQL queries per request from dozens/hundreds to a small fixed number.

## Possible fixes

- Use `Include()` / `ThenInclude()` carefully.
- Prefer DTO projections with `Select()`.
- Batch review/category loading.
- Avoid returning full EF entities from read endpoints.

## Success criteria

- SQL query count per request is below 5.
- P95 latency improves noticeably.
- Response shape remains functionally useful.