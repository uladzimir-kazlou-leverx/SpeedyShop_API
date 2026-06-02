# Task 11 - Large Payload Responses

## Scenario

Product details and order history endpoints return large object graphs with fields clients do not need.

## Endpoints

```http
GET /api/products/{id}
GET /api/orders/customer/{customerId}/history
```

## Where to look

- `PlaygroundProject/Services/ProductWorkshopService.cs`
- `PlaygroundProject/Services/OrderWorkshopService.cs`
- `PlaygroundProject/Models/Product.cs`
- `PlaygroundProject/Models/Order.cs`

## Symptoms

- Large response bodies.
- High JSON serialization time.
- More network transfer and client parsing work.

## How to measure

- Compare response sizes.
- Inspect request duration and serialization overhead.
- Use browser/network tools or curl to observe payload size.

## Goal

Return smaller, purpose-built API responses.

## Possible fixes

- Add DTOs for product details and order history.
- Add pagination for history endpoints.
- Remove internal notes and audit fields from public responses.

## Success criteria

- Response payloads are significantly smaller.
- Endpoint latency improves, especially under load.