# Pre-Task - OpenTelemetry Service Tracing

## Scenario

Before starting the performance tasks, participants should be able to see not only HTTP and database spans, but also spans for internal application services.

The goal is to make calls inside `ProductWorkshopService`, `OrderWorkshopService`, `ReportWorkshopService`, and external client integrations visible in traces.

## Why this matters

Several workshop tasks ask participants to inspect traces and understand where time is spent.

Without service-level spans, traces show only:
- the incoming HTTP request
- EF Core database spans
- outgoing HTTP spans if any exist

With service-level spans, participants can also see:
- which service handled the work
- how long each service method took
- which logical step produced slow behavior
- better parent/child relationships inside one request trace

## Where to look

- `PlaygroundProject/Program.cs`
- `PlaygroundProject/Services/ProductWorkshopService.cs`
- `PlaygroundProject/Services/OrderWorkshopService.cs`
- `PlaygroundProject/Services/ReportWorkshopService.cs`
- `PlaygroundProject/Services/FakeExternalCatalogClient.cs`

## Goal

Add OpenTelemetry tracing for internal services so each important service method produces its own span.

## Suggested implementation

1. Register custom `ActivitySource` names in `Program.cs` using `.AddSource(...)`.
2. Create one `ActivitySource` per service or component.
3. Start an activity inside important public methods.
4. Add useful tags such as:
   - `order.id`
   - `customer.id`
   - `product.id`
   - `products.take`
   - `report.range`
5. Keep the existing ASP.NET Core, EF Core, and HttpClient instrumentation.

## Example directions

- Add sources such as:
  - `SpeedyShop.Api.ProductService`
  - `SpeedyShop.Api.OrderService`
  - `SpeedyShop.Api.ReportService`
  - `SpeedyShop.Api.ExternalCatalog`
- Wrap important service methods with `StartActivity("MethodName")`.
- Verify that the new spans appear nested under the incoming request span.

## How to measure

- Run the API and open Swagger.
- Call a few endpoints such as:
  - `GET /api/products/popular?take=25`
  - `GET /api/orders/1`
  - `GET /api/reports/...`
- Inspect traces in the console exporter or OTLP backend.
- Confirm that request traces now contain service-level spans in addition to database spans.

## Success criteria

- Each main service produces spans for important operations.
- Spans are visible under the same request trace.
- Service spans include useful tags for debugging.
- Participants can identify which service method is slow before starting the main tasks.

## Advanced option

- *For advanced users:* implement service tracing via a DI proxy/decorator instead of adding `ActivitySource` calls manually in every service. The proxy can intercept interface calls such as `IProductWorkshopService`, `IOrderWorkshopService`, and `IReportWorkshopService`, create spans automatically, and attach tags like method name, arguments, and execution status.