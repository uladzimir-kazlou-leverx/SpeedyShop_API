# Task 05 - Blocking Calls

## Scenario

The frequently used product listing endpoint contains a blocking `Thread.Sleep(200)` call.

## Endpoint

```http
GET /api/products?take=50
```

## Where to look

- `PlaygroundProject/Services/ProductWorkshopService.cs`

## Symptoms

- Every product-listing request has at least ~200ms extra latency.
- Threads are blocked instead of being returned to the pool.
- Throughput suffers under concurrency.

## How to measure

- Call the endpoint repeatedly and inspect request duration.
- Run load tests with different concurrency values.
- Look for fixed latency floor in traces.

## Goal

Remove blocking work from the request path.

## Possible fixes

- Delete the artificial blocking wait.
- If waiting for real I/O, use asynchronous APIs.
- Avoid CPU/blocking work on request threads.

## Success criteria

- Product listing latency drops by roughly the sleep duration.
- Throughput improves under load.