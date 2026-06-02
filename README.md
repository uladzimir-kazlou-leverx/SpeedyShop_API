# SpeedyShop API - Making Your APIs Faster Workshop

SpeedyShop API is an intentionally slow but functional ASP.NET Core 8 Web API for performance workshops. Participants use OpenTelemetry, profiling tools, caching, query tuning, and API best practices to discover and fix bottlenecks.

## Stack

- ASP.NET Core 8 Web API
- Entity Framework Core 8
- SQLite by default for no-Docker debug runs
- SQL Server 2022 available through Docker Compose for workshop parity
- OpenTelemetry tracing and metrics
- Swagger UI
- In-memory cache available for caching exercises
- Aspire Dashboard via Docker Compose
- Node.js load testing script

## Run locally

The default configuration uses **SQLite** (`PlaygroundProject/App_Data/speedyshop.db`) and creates/seeds the database automatically on startup. Docker is not required for normal debug runs.

```bash
dotnet run --project PlaygroundProject/PlaygroundProject.csproj --urls http://localhost:5000
```

Swagger: http://localhost:5000/swagger

Development seed sizes are intentionally smaller so F5/debug startup is fast. The full workshop seed sizes remain in `appsettings.json`.

## Optional Docker services

Use Docker when you want SQL Server and Aspire Dashboard:

```bash
docker compose up -d
set Database__Provider=SqlServer
set OpenTelemetry__OtlpEndpoint=http://localhost:4317
dotnet run --project PlaygroundProject/PlaygroundProject.csproj --urls http://localhost:5000
```

Aspire Dashboard: http://localhost:18888

To seed the full workshop dataset with SQLite or SQL Server, override the development seed values:

```bash
set Seed__RunOnStartup=true
set Seed__Products=50000
set Seed__Reviews=500000
set Seed__Orders=100000
set Seed__Customers=20000
dotnet run --project PlaygroundProject/PlaygroundProject.csproj --urls http://localhost:5000
```

The default seed sizes are intentionally large:

- 50,000 products
- 500,000 reviews
- 100,000 orders
- 20,000 customers

For quick smoke tests, temporarily override counts, for example `Seed__Products=1000`.

## Intentional performance issues

All intentional bottlenecks are marked with `// WORKSHOP: Performance Issue` comments.

Separate participant task descriptions are available in [`workshop-tasks/`](workshop-tasks/README.md).

| Issue | Where | Discovery signal | Typical fix |
|---|---|---|---|
| N+1 DB queries | `GET /api/products` in `ProductWorkshopService` | Many EF/SQL spans per request | `Include`, projections, batched query |
| Missing indexes | `Orders`, `Products`, `Reviews`; queries by `CustomerId`, `ProductId`, `CreatedAt` | Slow DB spans/table scans | Add indexes and compare execution plans |
| Over-fetching | Product endpoints return full entities | Huge response bytes/serialization cost | DTO projections |
| Sync-over-async | `.Result` in product listing | Thread pool starvation under load | Async all the way |
| Blocking calls | `Thread.Sleep(200)` in product listing | Fixed latency floor | `await Task.Delay` only for real async waiting, or remove |
| Sequential external requests | `GET /api/dashboard` | Waterfall spans | `Task.WhenAll` |
| Missing caching | `GET /api/products/popular` | Repeated expensive aggregate | `IMemoryCache` |
| Chatty DB access | `GET /api/orders/{id}` | Many DB roundtrips and `SaveChanges` calls | Single projection, one save if needed |
| Excessive allocations | `GET /api/reports/product-quality` | High GC/allocation profile | `StringBuilder`, streaming, reduce lists |
| Inefficient LINQ | `GET /api/products/search` | CPU/memory and client-side filtering | Server-side `Where`, `Any`, one enumeration |
| Large payloads | Product details/order history | High network and JSON serialization time | Smaller DTOs, pagination |
| No response compression | `Program.cs` | Large payload transfer time | Add Brotli/Gzip response compression |

## Workshop flow for instructors

1. For the simplest setup, run the API directly and let SQLite be created automatically.
2. For SQL Server/Aspire, start services with `docker compose up -d` and set `Database__Provider=SqlServer`.
3. Seed data. Full seed can take several minutes; reduce counts for shorter sessions.
4. Run the app and open Swagger.
5. Run `node loadtest/speedyshop-load.js`.
6. Ask participants to inspect traces and metrics:
   - request duration (`http.server.request.duration`)
   - EF Core database spans and SQL text
   - runtime metrics such as GC and thread pool behavior
7. Fix one issue at a time and rerun the same load test.

Target improvements:

| Metric | Before | After |
|---|---:|---:|
| P95 latency | ~1500ms+ | <200ms |
| Requests/sec | Low | Much higher |
| SQL queries/request | 100+ | <5 |
| DB time | High | Low |
| Memory allocations | High | Lower |

## Notes

- OpenTelemetry is intentionally partial: request tracing, EF database tracing, HTTP client tracing, and runtime/request metrics are enabled, but custom business metrics and full production tuning are left as exercises.
- `IMemoryCache` is registered but deliberately unused by the expensive popular-products endpoint.
- Response compression is deliberately not enabled.