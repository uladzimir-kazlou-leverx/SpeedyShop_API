# Task 12 - No Response Compression

## Scenario

The API intentionally does not enable response compression, making large JSON/text responses more expensive to transfer.

## Endpoints

```http
GET /api/products/{id}
GET /api/orders/customer/{customerId}/history
GET /api/reports/product-quality
```

## Where to look

- `PlaygroundProject/Program.cs`

## Symptoms

- Large responses transfer uncompressed.
- Network time is higher for payload-heavy endpoints.

## How to measure

- Inspect response headers for missing `Content-Encoding`.
- Compare payload transfer sizes before and after compression.
- Use browser/network tools or curl.

## Goal

Enable HTTP response compression for compressible payloads.

## Possible fixes

- Register response compression services.
- Enable Brotli and Gzip providers.
- Call `app.UseResponseCompression()` early in the pipeline.
- Be mindful of already-compressed content types and HTTPS considerations.

## Success criteria

- Responses include `Content-Encoding: br` or `gzip` when requested by clients.
- Transfer size drops for JSON/text endpoints.