# Task 09 - Excessive Memory Allocations

## Scenario

The report endpoint repeatedly concatenates strings and creates unnecessary collections while building a large report.

## Endpoint

```http
GET /api/reports/product-quality
```

## Where to look

- `PlaygroundProject/Services/ReportWorkshopService.cs`

## Symptoms

- High allocation rate.
- More frequent garbage collections.
- CPU time spent building strings and temporary lists.

## How to measure

- Use a profiler or `dotnet-counters` to watch GC/allocation metrics.
- Call the report endpoint repeatedly.
- Compare memory allocations before and after refactoring.

## Goal

Reduce unnecessary allocations while preserving report output.

## Possible fixes

- Use `StringBuilder`.
- Avoid repeated `.ToList()` where enumeration is enough.
- Pre-group reviews by product.
- Stream large responses if appropriate.

## Success criteria

- Allocation rate decreases.
- Report endpoint latency improves.
- GC pressure is lower under load.