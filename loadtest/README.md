# SpeedyShop API load testing

This folder contains a dependency-free Node.js script that drives enough traffic to expose the intentional bottlenecks.

## Run

Start the API first:

```bash
dotnet run --project PlaygroundProject/PlaygroundProject.csproj --urls http://localhost:5000
```

Then run the load test:

```bash
node loadtest/speedyshop-load.js
```

Optional settings:

```bash
set BASE_URL=http://localhost:5000
set DURATION_SECONDS=120
set CONCURRENCY=25
set THINK_TIME_MS=250
node loadtest/speedyshop-load.js
```

Watch the console telemetry or Aspire Dashboard at http://localhost:18888 when OTLP is enabled for request duration, database spans, and runtime metrics.