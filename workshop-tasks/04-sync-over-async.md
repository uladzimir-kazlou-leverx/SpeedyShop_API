# Task 04 - Sync-over-Async

## Scenario

The product listing blocks on an asynchronous operation using `.Result` inside request processing.

## Endpoint

```http
GET /api/products?take=50
```

## Where to look

- `PlaygroundProject/Services/ProductWorkshopService.cs`
- `FakeExternalCatalogClient.GetSupplierStatusAsync(...)`

## Symptoms

- Thread pool starvation under load.
- Poor throughput even when the external operation is simulated with async delay.
- Latency increases disproportionately as concurrency increases.

## How to measure

- Run the Node load test with increasing `CONCURRENCY`.
- Watch runtime/thread pool metrics.
- Compare throughput before and after using async all the way.

## Goal

Remove blocking waits and make the request path asynchronous.

## Possible fixes

- Change service/controller signatures to async.
- Replace `.Result` with `await`.
- Combine independent operations where appropriate.

## Success criteria

- No `.Result` or `.Wait()` in request processing.
- Better throughput under concurrent load.
- Thread pool metrics are healthier.