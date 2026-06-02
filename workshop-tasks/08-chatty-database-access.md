# Task 08 - Chatty Database Access

## Scenario

The order details endpoint performs many separate queries and multiple `SaveChangesAsync()` calls in a loop.

## Endpoint

```http
GET /api/orders/{id}
```

## Where to look

- `PlaygroundProject/Services/OrderWorkshopService.cs`

## Symptoms

- Many database spans for one order.
- Additional product lookup per order item.
- Multiple save roundtrips during a read-style request.

## How to measure

- Inspect trace spans for one order request.
- Count SQL commands per request.
- Compare orders with different item counts.

## Goal

Reduce database roundtrips and avoid unnecessary writes.

## Possible fixes

- Use projection to load the full response in one query.
- Use `Include()` carefully if returning entities.
- Remove writes from read endpoints or batch them into one save.

## Success criteria

- Query count is small and stable.
- No looped `SaveChangesAsync()` calls.
- Endpoint latency improves.