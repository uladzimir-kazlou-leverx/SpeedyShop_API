# Task 07 - Missing Caching

## Scenario

The popular products endpoint runs an expensive aggregation on every request. `IMemoryCache` is registered, but this endpoint intentionally does not use it.

## Endpoint

```http
GET /api/products/popular?take=25
```

## Where to look

- `PlaygroundProject/Program.cs`
- `PlaygroundProject/Services/ProductWorkshopService.cs`

## Symptoms

- Repeated identical requests hit the database every time.
- Expensive group-by/order-by query appears in traces repeatedly.
- Latency does not improve for hot data.

## How to measure

- Call the endpoint several times with the same `take` value.
- Inspect database spans for every request.
- Run the Node load test and watch DB duration.

## Goal

Cache expensive, frequently requested aggregate results.

## Possible fixes

- Inject `IMemoryCache` into `ProductWorkshopService`.
- Cache by key such as `popular-products:{take}`.
- Use a sensible absolute/sliding expiration.
- Add cache invalidation discussion for real systems.

## Success criteria

- First request may be slow; repeated requests are much faster.
- Database spans disappear or decrease for cache hits.