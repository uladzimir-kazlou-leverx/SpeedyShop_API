# Task 06 - Sequential External Requests

## Scenario

The dashboard endpoint calls inventory, pricing, and recommendation services one after another even though they are independent.

## Endpoint

```http
GET /api/dashboard
```

## Where to look

- `PlaygroundProject/Controllers/DashboardController.cs`
- `PlaygroundProject/Services/FakeExternalCatalogClient.cs`

## Symptoms

- Trace waterfall shows external calls running sequentially.
- Total endpoint latency is close to the sum of all external delays.

## How to measure

- Call `/api/dashboard` and inspect duration.
- Compare with the longest individual downstream call.

## Goal

Run independent external calls concurrently.

## Possible fixes

- Start all tasks first.
- Await them together with `Task.WhenAll(...)`.
- Preserve error handling and cancellation behavior.

## Success criteria

- Dashboard latency approaches the slowest downstream call rather than the sum of all calls.