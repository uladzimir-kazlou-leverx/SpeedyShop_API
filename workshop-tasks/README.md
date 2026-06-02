# SpeedyShop Workshop Tasks

Each file in this folder describes one intentional performance problem in SpeedyShop API. Use them as participant handouts or instructor checkpoints.

Recommended workflow for every task:

1. Reproduce the problem with Swagger or `node loadtest/speedyshop-load.js`.
2. Observe traces, database spans, logs, and runtime metrics.
3. Form a hypothesis.
4. Implement the smallest fix.
5. Rerun the same measurement and compare before/after numbers.

## Tasks

1. [N+1 Database Queries](01-n-plus-one-queries.md)
2. [Missing Database Indexes](02-missing-database-indexes.md)
3. [Over-fetching Data](03-over-fetching-data.md)
4. [Sync-over-Async](04-sync-over-async.md)
5. [Blocking Calls](05-blocking-calls.md)
6. [Sequential External Requests](06-sequential-external-requests.md)
7. [Missing Caching](07-missing-caching.md)
8. [Chatty Database Access](08-chatty-database-access.md)
9. [Excessive Memory Allocations](09-excessive-memory-allocations.md)
10. [Inefficient LINQ](10-inefficient-linq.md)
11. [Large Payload Responses](11-large-payload-responses.md)
12. [No Response Compression](12-no-response-compression.md)